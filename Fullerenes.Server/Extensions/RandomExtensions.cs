using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Extensions
{
    public static class RandomExtensions
    {
        public static IEnumerable<float> GetEvenlyRandoms(this Random random, float min, float max)
        {
            while (true)
            {
                float randValue = (float)random.NextDouble();

                yield return min + (max - min) * randValue;
            }
        }
        public static IEnumerable<float> GetGammaRandoms(this Gamma gamma, float min, float max)
        {
            while (true)
            {
                float randValue = (float)gamma.Sample();

                while (randValue <= min || randValue >= max)
                {
                    randValue = (float)gamma.Sample();
                }

                yield return randValue;
            }
        }
    }
}
