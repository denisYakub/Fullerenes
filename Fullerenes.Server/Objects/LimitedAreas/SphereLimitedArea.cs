using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using MathNet.Numerics.Distributions;
using MessagePack;
using System.Numerics;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public class SphereLimitedArea(float x, float y, float z, float r, int series, Random random, Gamma gamma) 
        : LimitedArea(x, y, z, [("Radius", r)], series, random, gamma)
    {
        public SphereLimitedArea() : this(0, 0, 0, 0, 0, null, null) { }

        public override bool Contains(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            return
                FiguresCollision.SpheresInside(
                    fullerene.Center, fullerene.GenerateOuterSphereRadius(),
                    Center, Params[0].value);
        }

        public override float GenerateOuterRadius() => Params[0].value;

        public override string ToString()
        {
            return
                "Center: " + Center + ", " +
                "Parameters: " + string.Join(", ", Params.Select(val => val.name + ": " + val.value));
        }

        protected override Vector3 GetRandomCenter()
        {
            return new( 
                Random.GetEvenlyRandoms(Center.X - Params[0].value, Center.X + Params[0].value).First(),
                Random.GetEvenlyRandoms(Center.Y - Params[0].value, Center.Y + Params[0].value).First(),
                Random.GetEvenlyRandoms(Center.Z - Params[0].value, Center.Z + Params[0].value).First()
                );
        }
    }
}