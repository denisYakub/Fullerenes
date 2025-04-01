using System.Numerics;
using Fullerenes.Server.CustomLogger;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Services.IServices;

namespace Fullerenes.Server.Services.Services
{
    public class TestService : ITestService
    {
        public async Task<bool> CheckFullerenesIntersectionAsync(IReadOnlyCollection<Fullerene> fullerenes, Vector3 areaCenter, float areaRadius)
        {
            Task<bool>[] tests = [
                Task.Run(() => IntersectIByLinq(fullerenes)),
                Task.Run(() => IntersectByMonticello(fullerenes, areaCenter, areaRadius))
            ];

            var testResults = await Task.WhenAll(tests).ConfigureAwait(false);

            return testResults.All(test => test);
        }

        public bool IntersectIByLinq(IReadOnlyCollection<Fullerene> fullerenes)
        {
            return fullerenes
               .AsParallel()
               .SelectMany((fullerene, i) => fullerenes.Skip(i + 1), (fullerene1, fullerene2) => new { Fullerene1 = fullerene1, Fullerene2 = fullerene2 })
               .Any(pair => pair.Fullerene1.Intersect(pair.Fullerene2));
        }

        public bool IntersectByMonticello(IReadOnlyCollection<Fullerene> fullerenes, Vector3 sphereCenter, float sphereRadius)
        {
            ArgumentNullException.ThrowIfNull(fullerenes);

            Fullerene avgFullerene = GenerateAvgSize(fullerenes);

            double avgFullereneVolume = GenerateVolume(avgFullerene);

            double sphereVolume = 4 * MathF.PI * MathF.Pow(sphereRadius, 3) / 3;

            int numberOfDotsToGenerate = GenerateDotsNumber(avgFullereneVolume, fullerenes.Count, sphereVolume);

            GenerateDots(sphereCenter, sphereRadius, numberOfDotsToGenerate, out var dotsX, out var dotsY, out var dotsZ);

            Print.PrintToConsole($"Number of dots is: {numberOfDotsToGenerate}");

            for (int i = 0; i < numberOfDotsToGenerate; i++)
            {
                var dot = new Vector3(dotsX[i], dotsY[i], dotsZ[i]);
                int countInsideFullerene = 0;

                foreach (var fullerene in fullerenes)
                {
                    if (fullerene.Contains(dot))
                    {
                        if (++countInsideFullerene == 2)
                        {
                            Print.PrintToConsole($"Current dot is: {dot}");

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static int GenerateDotsNumber(double avgVolume, int count, double areaVolume)
        {
            const int baseNumberOfDots = 10000;
            double density = avgVolume * count / areaVolume;

            return (int)(baseNumberOfDots * density);
        }
        private static void GenerateDots(Vector3 sphereCenter, float sphereRadius, int numberOfDotsToGenerate, out float[] x, out float[] y, out float[] z)
        {
            x = new Random().GetEvenlyRandoms(sphereCenter.X - sphereRadius, sphereCenter.X + sphereRadius).Take(numberOfDotsToGenerate).ToArray();
            y = new Random().GetEvenlyRandoms(sphereCenter.Y - sphereRadius, sphereCenter.Y + sphereRadius).Take(numberOfDotsToGenerate).ToArray();
            z = new Random().GetEvenlyRandoms(sphereCenter.Z - sphereRadius, sphereCenter.Z + sphereRadius).Take(numberOfDotsToGenerate).ToArray();
        }
        private static Fullerene GenerateAvgSize(IReadOnlyCollection<Fullerene> fullerenes)
        {
            var avgSize = fullerenes.Sum(fullerene => fullerene.Size) / fullerenes.Count;

            return fullerenes.First() switch
            {
                IcosahedronFullerene => new IcosahedronFullerene(0, 0, 0, 0, 0, 0, avgSize),
                _ => throw new NotImplementedException("We dont work with this kind of fullerene")
            };
        }
        private static float GenerateVolume(Fullerene fullerene)
        {
            int numberOfDotsInsideFullerene = 0;
            const int numberOfDots = 1_000_000;

            var dots = new Random().GetEvenlyRandoms(-fullerene.Size, fullerene.Size).Take(numberOfDots * 3).ToArray();

            for (int i = 0; i < numberOfDots * 3; i += 3)
            {
                if (fullerene.Contains(new Vector3(dots[i], dots[i + 1], dots[i + 2])))
                    numberOfDotsInsideFullerene++;
            }

            return MathF.Pow(2 * fullerene.Size, 3) * numberOfDotsInsideFullerene / numberOfDots;
        }
    }
}
