using Fullerenes.Server.CustomLogger;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;
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

        protected override IEnumerable<Fullerene> GenerateFullerenes(EulerAngles RotationAngles, (float min, float max) FullereneSize)
        {
            var retryCount = 0;

            var xS = Random
                .GetEvenlyRandoms(Center.X - Params[0].value, Center.X + Params[0].value);
            var yS = Random
                .GetEvenlyRandoms(Center.Y - Params[0].value, Center.Y + Params[0].value);
            var zS = Random
                .GetEvenlyRandoms(Center.Z - Params[0].value, Center.Z + Params[0].value);

            var praecessioAngleS = Random
                .GetEvenlyRandoms(-RotationAngles.PraecessioAngle, RotationAngles.PraecessioAngle);
            var nutatioAngleS = Random
                .GetEvenlyRandoms(-RotationAngles.NutatioAngle, RotationAngles.NutatioAngle);
            var properRotationAngleS = Random
                .GetEvenlyRandoms(-RotationAngles.ProperRotationAngle, RotationAngles.ProperRotationAngle);

            var sizeS = Gamma
                .GetGammaRandoms(FullereneSize.min, FullereneSize.max);

            try
            {
                while (true)
                {
                    using var x = xS.GetEnumerator();
                    using var y = yS.GetEnumerator();
                    using var z = zS.GetEnumerator();
                    using var praecessioAngle = praecessioAngleS.GetEnumerator();
                    using var nutatioAngle = nutatioAngleS.GetEnumerator();
                    using var properRotationAngle = properRotationAngleS.GetEnumerator();
                    using var size = sizeS.GetEnumerator();

                    while (x.MoveNext() && y.MoveNext() && z.MoveNext() &&
                        praecessioAngle.MoveNext() && nutatioAngle.MoveNext() && properRotationAngle.MoveNext() &&
                        size.MoveNext())
                    {
                        if (retryCount == RetryCountMax)
                            yield break;

                        if(ProduceFullerene is null)
                            yield break;

                        Fullerene fullerene = ProduceFullerene.Invoke(
                            x.Current, y.Current, z.Current,
                            praecessioAngle.Current, nutatioAngle.Current, properRotationAngle.Current,
                            size.Current);

                        if (!Contains(fullerene) || !Octree.Add(fullerene, Series))
                        {
                            retryCount++;
                        }
                        else
                        {
                            retryCount = 0;

                            yield return fullerene;
                        }
                    }
                }
            }
            finally
            {
                Print.PrintToConsole($"Generation_{Series} DONE!");
            }
        }
    }
}