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
            var outerR = GenerateOuterRadius();

            var xs = Random
                .GetEvenlyRandoms(Center.X- outerR, Center.X + outerR)
                .Take(fullerenesNumber);

            var ys = Random
               .GetEvenlyRandoms(Center.Y - outerR, Center.Y + outerR)
               .Take(fullerenesNumber);

            var zs = Random
               .GetEvenlyRandoms(Center.Z - outerR, Center.Z + outerR)
               .Take(fullerenesNumber);

            var praecessioAngles = Random
                        .GetEvenlyRandoms(-RotationAngles.PraecessioAngle, RotationAngles.PraecessioAngle)
                        .Take(fullerenesNumber);

            var nutatioAngles = Random
                .GetEvenlyRandoms(-RotationAngles.NutatioAngle, RotationAngles.NutatioAngle)
                .Take(fullerenesNumber);

            var properRotationAngles = Random
                .GetEvenlyRandoms(-RotationAngles.ProperRotationAngle, RotationAngles.ProperRotationAngle)
                .Take(fullerenesNumber);

            var sizes = Gamma
                .GetGammaRandoms(FullereneSize.min, FullereneSize.max)
                .Take(fullerenesNumber);

            Fullerenes = GenerateFullerenes(xs, ys, zs, praecessioAngles, nutatioAngles, properRotationAngles, sizes)
                .Take(fullerenesNumber);
        }

        protected virtual IEnumerable<Fullerene> GenerateFullerenes(
            IEnumerable<float> xs,
            IEnumerable<float> ys,
            IEnumerable<float> zs,
            IEnumerable<float> praecessioAngles,
            IEnumerable<float> nutationAngles,
            IEnumerable<float> properRotationAngles,
            IEnumerable<float> sizes)
        {
            ArgumentNullException.ThrowIfNull(Octree);
            ArgumentNullException.ThrowIfNull(praecessioAngles);
            ArgumentNullException.ThrowIfNull(nutationAngles);
            ArgumentNullException.ThrowIfNull(properRotationAngles);
            ArgumentNullException.ThrowIfNull(sizes);

            var x = xs.GetEnumerator();
            var y = ys.GetEnumerator();
            var z = zs.GetEnumerator();

            var praecessioAngle = praecessioAngles.GetEnumerator();
            var nutationAngle = nutationAngles.GetEnumerator();
            var properRotationAngle = properRotationAngles.GetEnumerator();

            var size = sizes.GetEnumerator();

            try
            {
                int reTryCount = 0;

                while (true)
                {
                    if (reTryCount == RetryCountMax)
                        yield break;

                    if (!x.MoveNext() && !y.MoveNext() && !z.MoveNext() && 
                        !praecessioAngle.MoveNext() && !nutationAngle.MoveNext() && !properRotationAngle.MoveNext() && 
                        !size.MoveNext())
                        yield break;

                    var fullerene = TryToGenerateFullerene(
                        x.Current, y.Current, z.Current,
                        praecessioAngle.Current,
                        nutationAngle.Current,
                        properRotationAngle.Current,
                        size.Current);

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
        protected virtual Fullerene? TryToGenerateFullerene(
            float x, float y, float z,
            float praecessioAngle, 
            float nutatioAngle, 
            float properRotationAngle, 
            float size)
        {
            var fullerene = ProduceFullerene?
                .Invoke(
                    x, y, z,
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
