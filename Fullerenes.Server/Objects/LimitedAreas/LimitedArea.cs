using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public abstract class LimitedArea(float x, float y, float z, (string name, float value)[] parameters, int series, Random random, Gamma gamma)
    {
        public int Series => series;
        public Vector3 Center => new(x, y, z);
        public abstract float OuterRadius { get; }
        public required IOctree Octree { get; init; }
        public IEnumerable<Fullerene>? Fullerenes { get; set; }
        public (string name, float value)[] Params => parameters;
        public required Func<float, float, float, float, float, float, float, Fullerene>  ProduceFullerene { get; init; }

        public readonly Gamma Gamma = gamma;
        public readonly Random Random = random;
        public static readonly int RetryCountMax = 100;

        public abstract bool Contains(Fullerene fullerene);

        public void StartGeneration(int fullerenesNumber, EulerAngles RotationAngles, (float min, float max) FullereneSize) 
            => Fullerenes = GenerateFullerenes(RotationAngles, FullereneSize).Take(fullerenesNumber);

        protected abstract IEnumerable<Fullerene> GenerateFullerenes(EulerAngles RotationAngles, (float min, float max) FullereneSize);
    }
}
