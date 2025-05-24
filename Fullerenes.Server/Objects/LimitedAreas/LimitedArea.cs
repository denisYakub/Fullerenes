using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public abstract class LimitedArea
    {
        public int Series { get; init; }
        public Vector3 Center { get; init; }
        public abstract float OuterRadius { get; }
        public required IOctree Octree { get; init; }
        public ICollection<Fullerene> Fullerenes { get; set; }
        public (string name, float value)[] Params { get; set; }
        public required Func<float, float, float, float, float, float, float, Fullerene>  ProduceFullerene { get; init; }

        public required Gamma Gamma { get; init; }
        public required Random Random { get; init; }

        public static readonly int RetryCountMax = 1000;

        protected LimitedArea (float x, float y, float z, (string name, float value)[] parameters, int series)
        {
            Series = series;
            Center = new(x, y, z);
            Params = parameters;
        }

        public abstract bool Contains(Fullerene fullerene);

        public void StartGeneration(int fullerenesNumber, EulerAngles RotationAngles, (float min, float max) FullereneSize)
        {
            Fullerenes = GenerateFullerenes(RotationAngles, FullereneSize).Take(fullerenesNumber).ToArray();
        }

        protected abstract IEnumerable<Fullerene> GenerateFullerenes(EulerAngles RotationAngles, (float min, float max) FullereneSize);
    }
}
