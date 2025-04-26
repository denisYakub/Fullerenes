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
        public required IOctree Octree { get; init; }

        protected static bool ClearOctreeCollection { get; set; }
        protected static readonly int RetryCountMax = 100;

        public abstract bool Contains(Fullerene fullerene);
        public abstract float GenerateOuterRadius();

        public void StartGeneration(int fullerenesNumber, EulerAngles RotationAngles, (float min, float max) FullereneSize) 
            => Fullerenes = GenerateFullerenes(RotationAngles, FullereneSize).Take(fullerenesNumber);

        protected abstract IEnumerable<Fullerene> GenerateFullerenes(EulerAngles RotationAngles, (float min, float max) FullereneSize);
    }
}
