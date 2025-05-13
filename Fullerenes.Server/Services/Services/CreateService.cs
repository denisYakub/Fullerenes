using System;
using System.Numerics;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;

namespace Fullerenes.Server.Services.Services
{
    public class CreateService(IDataBaseService dataBaseService, IFileService fileService) : ICreateService
    {

        public (long id, List<long> superIds) GenerateArea(SystemAbstractFactory factory)
        {
            long GenId = dataBaseService.GetGenerationId();

            List<long> superIds = new List<long>(factory.ThreadNumber);

            IOctree octree = factory.GenerateOctree();

            Parallel.For(0, factory.ThreadNumber, async (thread, state) =>
            {
                LimitedArea limitedArea = factory.GenerateLimitedArea(thread, octree);

                string filePath = fileService.Write([limitedArea], $"Series_{limitedArea.Series}", $"Gen_{GenId}");

                SpData data = new(filePath);
                dataBaseService.SaveData(data);

                SpGen gen = new(factory.AreaType, factory.FullereneType, thread, GenId, data.Id)
                {
                    Phi = GeneratePhis(filePath).Result.Average()
                };
                dataBaseService.SaveGen(gen);

                superIds.Add(data.Id);
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

                float phi = ((float)dotsInFullerene) / ((float)dotsInLayer);

                phis.Add(phi);
            }
            
            return phis;
        }

        public void GenerateIntensOpt(long genId)
        {
            LimitedArea[] areas = fileService.GetAreas(genId);

            for (int i = 0; i < areas.Length; i++)
            {

            }
        }

        private static (
            IEnumerable<float> localVolume, 
            IEnumerable<float> globalVolume, 
            IEnumerable<float> tmpConst
            ) Eval(LimitedArea area, IEnumerable<float> q)
        {
            float vConst = 4 / 3 * MathF.PI;

            float globalVolume = vConst * MathF.Pow(area.OuterRadius, 3);

            var localVolume = area.Fullerenes
                .Select(fullerene => vConst * MathF.Pow(fullerene.OuterSphereRadius, 3));

            var spGlobal = q.Select(qI => globalVolume * Factor(area.OuterRadius * qI));

            var tmpConst = area.Fullerenes
                .Select(fullerene => fullerene.Center.Length());

            return (localVolume, spGlobal, tmpConst);
        }

        private static (IEnumerable<float> q, IEnumerable<float> I) IntenceOpt(
            IEnumerable<float> qs, 
            IEnumerable<float> spGlobal, 
            IEnumerable<float> tmpConst,
            IEnumerable<float> localVolume,
            LimitedArea area,
            float nC)
        {
            int qNum = qs.Count();
            int realN = area.Fullerenes.Count;

            float[] spLocalVolumeSqr = localVolume.Select(v => v * v).ToArray();
            var qr = new float[qNum, realN];
            var spFactorConst = new float[qNum, realN];
            var spFactorSqr = new float[qNum, realN];

            for (int i = 0; i < qNum; i++)
                for (int j = 0; j < realN; j++)
                {
                    qr[i, j] = qs.ElementAt(i) * area.Fullerenes.ElementAt(j).OuterSphereRadius;
                    spFactorConst[i, j] = Factor(qr[i, j]);
                    spFactorSqr[i, j] = spFactorConst[i, j] * spFactorConst[i, j];
                }

            float[] s2 = new float[qNum];
            float[] spFirstSummand = new float[qNum];

            for (int i = 0; i < qNum; i++)
                for (int j = 0; j < realN; j++)
                {
                    s2[i] += spFactorSqr[i, j] * spLocalVolumeSqr[j];
                    spFirstSummand[i] += localVolume.ElementAt(j) * spFactorConst[i, j] * Sinc(qs.ElementAt(i) * tmpConst.ElementAt(j));
                }

            float[] spFactors = new float[qNum];

            for (int i = 0; i < realN; i++)
                for (int j = i + 1; j < realN; j++)
                {
                    float dist = Vector3.Distance(area.Fullerenes.ElementAt(i).Center, area.Fullerenes.ElementAt(j).Center);
                    float vol = localVolume.ElementAt(j) * localVolume.ElementAt(i);

                    for (int k = 0; k < qNum; k++) 
                        spFactors[k] += spFactorConst[k, i] * spFactorConst[k, j] * Sinc(qs.ElementAt(k) * dist) * vol;
                }

            float[] spGlobalArray = spGlobal.ToArray(); // Преобразуем в массив один раз

            float[] I = new float[qNum];

            for (int k = 0; k < qNum; k++)
            {
                float spG = spGlobalArray[k];
                I[k] = s2[k]
                     + 2 * spFactors[k]
                     + nC * nC * spG * spG
                     - 2 * nC * spFirstSummand[k] * spG;
            }

            return (qs, I);
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
