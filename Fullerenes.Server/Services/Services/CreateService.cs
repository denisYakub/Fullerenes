using System.Numerics;
using Fullerenes.Server.CustomLogger;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using StackExchange.Profiling;

namespace Fullerenes.Server.Services.Services
{
    public class CreateService(IDataBaseService dataBaseService) : ICreateService
    {
        public async Task<int> GenerateAreaAsync(FullereneAndLimitedAreaFactory factory)
        {
            LimitedArea limitedArea = factory.CreateLimitedArea();

            int savedAreaId = 0 /*= await dataBaseService.SaveAreaAsync(limitedArea).ConfigureAwait(false)*/;

            ArgumentNullException.ThrowIfNull(factory);

            Octree<Parallelepiped, Fullerene> octree = GenerateOctree(limitedArea, factory.NumberOfSeries);
            octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * factory.FullereneSizeRange.MaxSizeFullerenes);

            Parallel.For(0, factory.NumberOfSeries, (i, state) =>
            {
                Thread.CurrentThread.Name = $"Thread-{i}";

                var fullerenes = limitedArea.GenerateFullerenes(i, octree).Take(factory.NumberOfFullerenes).ToList();

                //dataBaseService.SaveFullerenes(fullerenes);
            });

            return savedAreaId;
        }
        
        public async Task<(float[], float)> GenerateDensityAsync(int areaId, int seriesFs, int numberOfLayers, int numberOfPoints)
        {
            var limitedArea = await dataBaseService.GetAreaWithFullerenesAsync(areaId, seriesFs).ConfigureAwait(false);

            var limitedAreaR = limitedArea.GenerateOuterRadius();

            var radii = GenerateRadii(limitedAreaR, numberOfLayers).ToArray();
            var dots = GenerateDots(limitedArea.Center, limitedAreaR).Take(numberOfPoints);

            var tasks = new Task<(int dotsInFullerene, int dotsInLayer)>[numberOfLayers];

            for (int i = 0; i < numberOfLayers; i++)
            {
                float rMin = radii[i], rMax = radii[i + 1];

                var task = new Task<(int dotsInFullerene, int dotsInLayer)>(() => CountDotsHit(dots, limitedArea.Fullerenes ?? [], limitedArea.Center, rMin, rMax));
                tasks[i] = task;
                task.Start();
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            var densityResult = new float[numberOfLayers];
            float excessResult = 0;

            for (int i = 0; i < numberOfLayers; i++)
            {
                densityResult[i] = results[i].dotsInFullerene / results[i].dotsInLayer;

                if (i < 5)
                    excessResult = results[i].dotsInFullerene / results[i].dotsInLayer;
            }

            excessResult /= numberOfLayers < 5 ? numberOfLayers : 5;

            return (densityResult, excessResult);
        }

        private static Octree<Parallelepiped, Fullerene> GenerateOctree(LimitedArea limitedArea, int threadsNumber)
        {
            (float height, float width, float length) = limitedArea switch
            {
                SphereLimitedArea sphere => (2 * sphere.Radius, 2 * sphere.Radius, 2 * sphere.Radius),
                _ => throw new NotImplementedException("Cannot get (w, h, l) of limited area"),
            };

            return new Octree<Parallelepiped, Fullerene>(
                new Parallelepiped
                {
                    Center = limitedArea.Center,
                    Height = height,
                    Width = width,
                    Length = length,
                },
                threadsNumber
            );
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
