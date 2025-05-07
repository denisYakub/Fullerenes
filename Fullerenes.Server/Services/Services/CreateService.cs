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
            long generationId = dataBaseService.GetGenerationId();

            List<long> superIds = new List<long>(factory.ThreadNumber);

            IOctree octree = factory.GenerateOctree();

            Parallel.For(0, factory.ThreadNumber, async (thread, state) =>
            {
                LimitedArea limitedArea = factory.GenerateLimitedArea(thread, octree);

                string filePath = fileService.Write([limitedArea], $"Series_{limitedArea.Series}", $"Gen_{generationId}");

                SpData data = new(filePath);
                dataBaseService.SaveData(data);

                SpGen gen = new(factory.AreaType, factory.FullereneType, thread, generationId, data.Id)
                {
                    Phi = GeneratePhis(filePath).Result.Average()
                };
                dataBaseService.SaveGen(gen);

                superIds.Add(data.Id);
            });

            return (generationId, superIds);
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

        public void GenerateIntensOpt(string dataPath)
        {
            var data = fileService.GetArea(dataPath);


        }

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
