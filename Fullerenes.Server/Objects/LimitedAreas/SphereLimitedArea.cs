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
    public class SphereLimitedArea(float x, float y, float z, float r, int series) 
        : LimitedArea(x, y, z, [("Radius", r)], series)
    {
        public SphereLimitedArea() : this(0, 0, 0, 0, 0) { }

        public override float OuterRadius => Params[0].value;

        public override bool Contains(Fullerene fullerene)
        {
            return FiguresCollision.SpheresInside(fullerene.Center, fullerene.OuterSphereRadius, Center, OuterRadius);
        }

        protected override IEnumerable<Fullerene> GenerateFullerenes(EulerAngles RotationAngles, (float min, float max) FullereneSize)
        {
            var retryCount = 0;

            var xS = Random
                .GetEvenlyRandoms(Center.X - Params[0].value, Center.X + Params[0].value)
                .GetEnumerator();
            var yS = Random
                .GetEvenlyRandoms(Center.Y - Params[0].value, Center.Y + Params[0].value)
                .GetEnumerator();
            var zS = Random
                .GetEvenlyRandoms(Center.Z - Params[0].value, Center.Z + Params[0].value)
                .GetEnumerator();

            var praecessioAngleS = Random
                .GetEvenlyRandoms(-RotationAngles.PraecessioAngle, RotationAngles.PraecessioAngle)
                .GetEnumerator();
            var nutatioAngleS = Random
                .GetEvenlyRandoms(-RotationAngles.NutatioAngle, RotationAngles.NutatioAngle)
                .GetEnumerator();
            var properRotationAngleS = Random
                .GetEvenlyRandoms(-RotationAngles.ProperRotationAngle, RotationAngles.ProperRotationAngle)
                .GetEnumerator();

            var sizeS = Gamma
                .GetGammaRandoms(FullereneSize.min, FullereneSize.max)
                .GetEnumerator();

            try
            {
                while (true)
                {
                    if (xS.MoveNext() && yS.MoveNext() && zS.MoveNext() &&
                        praecessioAngleS.MoveNext() && nutatioAngleS.MoveNext() && properRotationAngleS.MoveNext() &&
                        sizeS.MoveNext())
                    {
                        if (retryCount == RetryCountMax)
                            yield break;

                        if(ProduceFullerene is null)
                            yield break;

                        Fullerene fullerene = ProduceFullerene.Invoke(
                            xS.Current, yS.Current, zS.Current,
                            praecessioAngleS.Current, nutatioAngleS.Current, properRotationAngleS.Current,
                            sizeS.Current);

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
                xS.Dispose();
                yS.Dispose();
                zS.Dispose();
                praecessioAngleS.Dispose();
                nutatioAngleS.Dispose();
                properRotationAngleS.Dispose();
                sizeS.Dispose();
            }
        }
    }
}