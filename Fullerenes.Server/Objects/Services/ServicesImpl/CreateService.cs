using System;
using System.Numerics;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Objects.Services.ServicesImpl
{
    public class CreateService(IDataBaseService dataBaseService, IFileService fileService) : ICreateService
    {
        public (long id, List<long> superIds) GenerateArea(SystemAbstractFactory factory, int series)
        {
            var GenId = dataBaseService.GetGenerationId();

            var superIds = new List<long>(series);

            IOctree octree = factory.GenerateOctree();

            Parallel.For(0, series, async (thread, state) =>
            {
                var limitedArea = factory.GenerateLimitedArea(thread);

                var filePath = fileService.Write([limitedArea], $"Series_{limitedArea.Series}", $"Gen_{GenId}");

                var spData = new SpData(filePath);
                dataBaseService.SaveData(spData);

                var spGen = new SpGen(factory.AreaType, factory.FullereneType, thread, GenId, spData.Id)
                {
                    Phi = GeneratePhis(filePath).Result.Average()
                };
                dataBaseService.SaveGen(spGen);

                superIds.Add(spData.Id);
            });

            return (GenId, superIds);
        }
        
        public async Task<List<float>> GeneratePhis(string dataPath, int numberOfLayers = 5, int numberOfPoints = 100_000)
        {
            var data = fileService.GetArea(dataPath);

            var radii = GenerateRadii(data.OuterRadius, numberOfLayers);

            var points = GenerateDots(data.Center, data.OuterRadius).Take(numberOfPoints);

            var tasks = new Task<(int dotsInFullerene, int dotsInLayer)>[numberOfLayers];

            for (int i = 0; i < numberOfLayers; i++)
            {
                float rMin = radii[i];
                float rMax = radii[i + 1];

                tasks[i] = new Task<(int dotsInFullerene, int dotsInLayer)>(
                    () => CountDotsHit(points, data.Fullerenes ?? [], data.Center, rMin, rMax));

                tasks[i].Start();
            }

            var phis = new List<float>(numberOfLayers);

            foreach (var task in tasks)
            {
                var (dotsInFullerene, dotsInLayer) = task.Result;

                float phi = dotsInFullerene / (float)dotsInLayer;

                phis.Add(phi);
            }
            
            return phis;
        }

        public (IReadOnlyCollection<float> q, IReadOnlyCollection<float> I) GenerateIntensOpt(string dataPath, float qMin, float qMax, int qNum)
        {
            var data = fileService.GetArea(dataPath);

            var qs = new Random().GetEvenlyRandoms(qMin, qMax).Take(qNum);

            var eval = Eval(data, qs);

            var nC = data.Fullerenes.Count;

            var (q, I) = IntenceOpt(qs, eval.globalVolume, eval.tmpConst, eval.localVolume, data/*, nC*/);

            return (q.ToArray(), I.ToArray());
        }

        private static (
            IEnumerable<float> localVolume, 
            IEnumerable<float> globalVolume, 
            IEnumerable<float> tmpConst
            ) Eval(LimitedArea area, IEnumerable<float> q)
        {
            var vConst = 4 / 3 * MathF.PI;

            var outerSphereVolume = vConst * MathF.Pow(area.OuterRadius, 3);

            var localVolume = area.Fullerenes
                .Select(fullerene => vConst * MathF.Pow(fullerene.OuterSphereRadius, 3));

            var globalVolume = q.Select(qI => outerSphereVolume * Factor(area.OuterRadius * qI));

            var tmpConst = area.Fullerenes
                .Select(fullerene => fullerene.Center.Length());

            return (localVolume, globalVolume, tmpConst);
        }
        
        private static (IEnumerable<float> q, IEnumerable<float> I) IntenceOpt(
            IEnumerable<float> qs, 
            IEnumerable<float> globalVolume, 
            IEnumerable<float> tmpConst,
            IEnumerable<float> localVolume,
            LimitedArea area)
        {
            int qNum = qs.Count();
            int fullereneNum = area.Fullerenes.Count;

            Span<float> localVolumeSqr = localVolume.Select(v => MathF.Pow(v, 2)).ToArray();

            var qR = new FloatMatrix(new float[qNum * fullereneNum], fullereneNum);
            var factorConst = new FloatMatrix(new float[qNum * fullereneNum], fullereneNum);
            var factorSqr = new FloatMatrix(new float[qNum * fullereneNum], fullereneNum);

            for (int i = 0; i < qNum; i++)
                for (int j = 0; j < fullereneNum; j++)
                {
                    qR[i, j] = qs.ElementAt(i) * area.Fullerenes.ElementAt(j).OuterSphereRadius;
                    factorConst[i, j] = Factor(qR[i, j]);
                    factorSqr[i, j] = factorConst[i, j] * factorConst[i, j];
                }

            Span<float> s2 = new float[qNum];
            Span<float> spFirstSummand = new float[qNum];

            for (int i = 0; i < qNum; i++)
                for (int j = 0; j < fullereneNum; j++)
                {
                    s2[i] += factorSqr[i, j] * localVolumeSqr[j];
                    spFirstSummand[i] += localVolume.ElementAt(j) * factorConst[i, j] * Sinc(qs.ElementAt(i) * tmpConst.ElementAt(j));
                }

            Span<float> spFactors = new float[qNum];

            for (int i = 0; i < fullereneNum; i++)
                for (int j = i + 1; j < fullereneNum; j++)
                {
                    var dist = Vector3.Distance(area.Fullerenes.ElementAt(i).Center, area.Fullerenes.ElementAt(j).Center);
                    var vol = localVolume.ElementAt(j) * localVolume.ElementAt(i);

                    for (int k = 0; k < qNum; k++) 
                        spFactors[k] += factorConst[k, i] * factorConst[k, j] * Sinc(qs.ElementAt(k) * dist) * vol;
                }

            Span<float> spGlobalArray = globalVolume.ToArray(); // Преобразуем в массив один раз

            var I = new float[qNum];

            for (int k = 0; k < qNum; k++)
            {
                var spG = spGlobalArray[k];
                I[k] = s2[k]
                     + 2 * spFactors[k]
                     + fullereneNum * fullereneNum * spG * spG
                     - 2 * fullereneNum * spFirstSummand[k] * spG;
            }

            return (qs, I);
        }

        private ref struct FloatMatrix
        {
            private readonly Span<float> _data;
            private readonly int _cols;

            public FloatMatrix(Span<float> data, int cols)
            {
                _data = data;
                _cols = cols;
            }

            public ref float this[int row, int col]
                => ref _data[row * _cols + col];

            public int Rows => _data.Length / _cols;
            public int Columns => _cols;
        }

        private static float Factor(float x)
            => 3 * (MathF.Sin(x) - x * MathF.Cos(x)) / MathF.Pow(x, 3);

        private static float Sinc(float x)
            => MathF.Sin(x) / x;

        private static float[] GenerateRadii(float lastR, int numberOfR)
        {
            float[] radii = new float[numberOfR + 1];

            for (int i = 0; i < numberOfR; i++)
            {
                radii[i] = (float) Math.Pow(i * (Math.Pow(lastR, 3) / numberOfR), 1.0f / 3);
            }

            radii[numberOfR] = lastR;

            return radii;
        }

        private static (int dotsInFullerene, int dotsInLayer) CountDotsHit(IEnumerable<Vector3> dots, IEnumerable<Fullerene> fullerenes, Vector3 sphereCenter, float radiusMin, float radiusMax)
        {
            int dotsInLayerCount = 0, dotsInFullerenesAndLayerCount = 0;

            foreach (var dot in dots)
            {
                var distance = Vector3.Distance(sphereCenter, dot);

                if (distance >= radiusMin && distance <= radiusMax)
                {
                    dotsInLayerCount++;

                    foreach (var fullerene in fullerenes)
                    {
                        if (fullerene.Contains(dot))
                            dotsInFullerenesAndLayerCount++;
                    }
                }
            }

            return (dotsInFullerene: dotsInFullerenesAndLayerCount, dotsInLayer: dotsInLayerCount);
        }

        private static IEnumerable<Vector3> GenerateDots(Vector3 center, float radius)
        {
            while (true)
            {
                var x = new Random().GetEvenlyRandoms(-radius, radius).First();
                var y = new Random().GetEvenlyRandoms(-radius, radius).First();
                var z = new Random().GetEvenlyRandoms(-radius, radius).First();

                var randomDot = new Vector3(x, y, z);

                if (randomDot.LengthSquared() <= radius * radius)
                    yield return center + randomDot;
            }
        }
        private static bool CrossesRegion(Vector3 centerRegion, float startRegion, float endRegion, Fullerene fullerene)
        {
            foreach (var vertex in fullerene.Vertices)
            {
                var distance = Vector3.Distance(centerRegion, vertex);

                if (distance > startRegion && distance < endRegion)
                    return true;
            }
            return false;
        }
    }
}
