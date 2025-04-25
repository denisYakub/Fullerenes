using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public abstract class LimitedArea(
        float x, float y, float z, 
        (string name, float value)[] parameters, 
        int series,
        Random random, Gamma gamma)
    {
        protected readonly Random Random = random;
        protected readonly Gamma Gamma = gamma;
        public Vector3 Center { get; } = new(x, y, z);
        public (string name, float value)[] Params { get; } = parameters;
        public int Series { get; } = series;
        public IEnumerable<Fullerene>? Fullerenes { get; set; }
        public required Func<float, float, float, float, float, float, float, Fullerene>  ProduceFullerene { get; init; }
        public required IOctree<Fullerene> Octree { get; init; }

        protected static bool ClearOctreeCollection { get; set; }
        protected static readonly int RetryCountMax = 100;

        public abstract bool Contains(Fullerene fullerene);
        public abstract float GenerateOuterRadius();

        public void StartGeneration(int fullerenesNumber,
            EulerAngles RotationAngles,
            (float min, float max) FullereneSize)
        {
            int reTryCount = 0;

            List<Fullerene> fullerenes = new List<Fullerene>(fullerenesNumber);

            while (true)
            {
                if (reTryCount == RetryCountMax || fullerenes.Count == fullerenesNumber)
                    break;

                var center = GetRandomCenter();

                var fullerene = ProduceFullerene?
                    .Invoke(
                        center.X, center.Y, center.Z,
                        Random.GetEvenlyRandoms(-RotationAngles.PraecessioAngle, RotationAngles.PraecessioAngle).First(),
                        Random.GetEvenlyRandoms(-RotationAngles.NutatioAngle, RotationAngles.NutatioAngle).First(),
                        Random.GetEvenlyRandoms(-RotationAngles.ProperRotationAngle, RotationAngles.ProperRotationAngle).First(),
                        Gamma.GetGammaRandoms(FullereneSize.min, FullereneSize.max).First()) ?? null;

                if (fullerene == null || !Contains(fullerene) || fullerenes.AsParallel().Any(fullerene.Intersect))
                {
                    ++reTryCount;
                }
                else
                {
                    fullerenes.Add(fullerene);
                    reTryCount = 0;
                }
            }

            Fullerenes = fullerenes;
            //Fullerenes = GenerateFullerenes(RotationAngles, FullereneSize)
                //.Take(fullerenesNumber);
        }

        protected virtual IEnumerable<Fullerene> GenerateFullerenes(
            EulerAngles RotationAngles,
            (float min, float max) FullereneSize)
        {
            ArgumentNullException.ThrowIfNull(Octree);
            try
            {
                int reTryCount = 0;

                while (true)
                {
                    if (reTryCount == RetryCountMax)
                        yield break;

                    var fullerene = TryToGenerateFullerene(
                        GetRandomCenter(),
                        Random.GetEvenlyRandoms(-RotationAngles.PraecessioAngle, RotationAngles.PraecessioAngle).First(),
                        Random.GetEvenlyRandoms(-RotationAngles.NutatioAngle, RotationAngles.NutatioAngle).First(),
                        Random.GetEvenlyRandoms(-RotationAngles.ProperRotationAngle, RotationAngles.ProperRotationAngle).First(),
                        Gamma.GetGammaRandoms(FullereneSize.min, FullereneSize.max).First());

                    if (fullerene != null)
                    {
                        reTryCount = 0;

                        yield return fullerene;

                    }
                    else
                    {
                        reTryCount++;
                    }
                }
            }
            finally
            {
                if (ClearOctreeCollection)
                    Octree.ClearThreadCollection(Series);
            }
        }
        protected abstract Vector3 GetRandomCenter();
        protected virtual Fullerene? TryToGenerateFullerene(
            Vector3 center,
            float praecessioAngle, 
            float nutatioAngle, 
            float properRotationAngle, 
            float size)
        {
            var fullerene = ProduceFullerene?
                .Invoke(
                    center.X, center.Y, center.Z,
                    praecessioAngle, 
                    nutatioAngle, 
                    properRotationAngle,
                    size) ?? null;

            return
                fullerene is not null &&
                Contains(fullerene) &&
                Octree.AddData(fullerene, Series, fullerene.Intersect)
                ? fullerene
                : null;
        }
    }
}
