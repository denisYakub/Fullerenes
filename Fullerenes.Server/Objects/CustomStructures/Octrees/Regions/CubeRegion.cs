using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.Fullerenes;
using System.Drawing;
using System.Numerics;

namespace Fullerenes.Server.Objects.CustomStructures.Octrees.Regions
{
    public readonly struct CubeRegion
    {
        public required Vector3 Center { get; init; }
        public required float Edge { get; init; }

        public CubeRegion[] Split8Parts()
        {
            var halfEdge = Edge / 2;
            var dCoord = Edge / 4;

            return new CubeRegion[8] {
                new CubeRegion {
                    Center = new Vector3(Center.X - dCoord, Center.Y - dCoord, Center.Z - dCoord),
                    Edge = halfEdge,
                },
                new CubeRegion {
                    Center = new Vector3(Center.X - dCoord, Center.Y - dCoord, Center.Z + dCoord),
                    Edge = halfEdge,
                },
                new CubeRegion {
                    Center = new Vector3(Center.X - dCoord, Center.Y + dCoord, Center.Z - dCoord),
                    Edge = halfEdge,
                },
                new CubeRegion {
                    Center = new Vector3(Center.X - dCoord, Center.Y + dCoord, Center.Z + dCoord),
                    Edge = halfEdge,
                },
                new CubeRegion {
                    Center = new Vector3(Center.X + dCoord, Center.Y - dCoord, Center.Z - dCoord),
                    Edge = halfEdge,
                },
                new CubeRegion {
                    Center = new Vector3(Center.X + dCoord, Center.Y - dCoord, Center.Z + dCoord),
                    Edge = halfEdge,
                },
                new CubeRegion {
                    Center = new Vector3(Center.X + dCoord, Center.Y + dCoord, Center.Z - dCoord),
                    Edge = halfEdge,
                },
                new CubeRegion {
                    Center = new Vector3(Center.X + dCoord, Center.Y + dCoord, Center.Z + dCoord),
                    Edge = halfEdge,
                },
            };

        }

        public int MaxDepth(float maxSize)
        {
            int count = 0;
            float a = Edge;

            while (a > maxSize)
            {
                a /= 2;
                count++;
            }

            return count;   
        }

        public bool Contains(Fullerene fullerene)
        {
            var fullereneR = fullerene.OuterSphereRadius;

            return
                (Center.X - Edge / 2) + fullereneR <= fullerene.Center.X &&
                fullerene.Center.X <= (Center.X + Edge / 2) - fullereneR &&
                (Center.Y - Edge / 2) + fullereneR <= fullerene.Center.Y &&
                fullerene.Center.Y <= (Center.Y + Edge / 2) - fullereneR &&
                (Center.Z - Edge / 2) + fullereneR <= fullerene.Center.Z &&
                fullerene.Center.Z <= (Center.Z + Edge / 2) - fullereneR;
        }
    }
}