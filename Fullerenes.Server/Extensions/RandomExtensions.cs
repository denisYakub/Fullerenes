using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Extensions
{
    public static class RandomExtensions
    {
        public static IEnumerable<float> GetEvenlyRandoms(this Random random, float min, float max)
        {
            ArgumentNullException.ThrowIfNull(random);

            while (true)
            {
                float randValue = (float)random.NextDouble();

                yield return min + (max - min) * randValue;
            }
        }
        public static IEnumerable<float> GetGammaRandoms(this Gamma gamma, float min, float max)
        {
            ArgumentNullException.ThrowIfNull(gamma);

            float xMax = (float)gamma.InverseCumulativeDistribution(0.99);

            while (true)
            {
                float x = (float)gamma.Sample();

                float randValue = min + (max - min) * (x / xMax);

                yield return Math.Max(min, Math.Min(max, randValue));
            }
        }
    }
}
