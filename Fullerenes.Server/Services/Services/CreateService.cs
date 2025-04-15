using System.Numerics;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using static MessagePack.GeneratedMessagePackResolver.Fullerenes.Server.Objects;

namespace Fullerenes.Server.Services.Services
{
    public class CreateService(IDataBaseService dataBaseService) : ICreateService
    {
        public long GenerateArea(SystemAbstractFactory factory, int series, int fullereneNumber)
        {
            ArgumentNullException.ThrowIfNull(factory);

            long generationId = dataBaseService.GetGenerationId();

            IOctree<Fullerene> octree = factory.GenerateOctree();

            Parallel.For(0, series, (i, state) =>
            {
                LimitedArea limitedArea = factory.GenerateLimitedArea(i, octree);

                //limitedArea.StartGeneration(fullereneNumber);

                ArgumentNullException.ThrowIfNull(limitedArea.Fullerenes);

                SpData data = new(limitedArea, limitedArea.Fullerenes.ToArray(), i, generationId);
                dataBaseService.SaveData(data);

                SpGen gen = new(factory.AreaType, factory.FullereneType, i, generationId, data.Id);
                dataBaseService.SaveGen(gen);
            });

            return generationId;
        }
        
        public async Task<float[]> GeneratePhis(long superId, int numberOfLayers = 5, int numberOfPoints = 1_000_000)
        {
            string? genData = dataBaseService.GetDataPath(superId);

            ArgumentNullException.ThrowIfNull(genData);

            var data = SpData.GetDataFormFileBin(genData);

            var radii = GenerateRadii(data.GenerateOuterRadius(), numberOfLayers);

            var points = GenerateDots(data.Center, data.GenerateOuterRadius()).Take(numberOfPoints);

            var tasks = new Task<(int dotsInFullerene, int dotsInLayer)>[numberOfLayers];

            for (int i = 0; i < numberOfLayers; i++)
            {
                float rMin = radii.ElementAt(i), rMax = radii.ElementAt(i + 1);

                var task = new Task<(int dotsInFullerene, int dotsInLayer)>(() => CountDotsHit(points, data.Fullerenes ?? [], data.Center, rMin, rMax));
                tasks[i] = task;
                task.Start();
            }

            var taskResults = await Task.WhenAll(tasks).ConfigureAwait(false);

            var phis = new float[numberOfLayers];

            for (int i = 0; i < numberOfLayers; i++)
            {
                phis[i] = taskResults[i].dotsInFullerene / taskResults[i].dotsInLayer;
            }
            
            return phis;
        }

        private static IEnumerable<float> GenerateRadii(float mainRadius, int numberOfLayers)
        {
            for (int i = 0; i < numberOfLayers; i++)
            {
                yield return (float)Math.Pow(i * (Math.Pow(mainRadius, 3) / numberOfLayers), 1.0f / 3);
            }

            yield return mainRadius;
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

                    ArgumentNullException.ThrowIfNull(fullerenes);

                    dotsInFullerenesAndLayerCount += fullerenes.Count(
                        fullerene =>
                        CrossesRegion(sphereCenter, radiusMin, radiusMax, fullerene)
                        && fullerene.Contains(dot)
                    );
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
