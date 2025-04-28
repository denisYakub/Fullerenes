using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.Fullerenes;
using System.Drawing;
using System.Numerics;

namespace Fullerenes.Server.Objects.CustomStructures.Octrees.Regions
{
    public class Cube : IRegion, IEquatable<Cube>
    {
        public required Vector3 Center { get; init; }
        public required float Height { get; init; }
        public required float Width { get; init; }
        public required float Length { get; init; }

        public IRegion[] Split8Parts()
        {
            var halfWidth = Width / 2;
            var halfHeight = Height / 2;
            var halfLength = Length / 2;

            var dX = Width / 4;
            var dY = Height / 4;
            var dZ = Length / 4;

            return [
                new Cube {
                    Center = new Vector3(Center.X - dX, Center.Y - dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Cube {
                    Center = new Vector3(Center.X - dX, Center.Y - dY, Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Cube {
                    Center = new Vector3(Center.X - dX, Center.Y + dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Cube {
                    Center = new Vector3(Center.X - dX, Center.Y + dY, Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Cube {
                    Center = new Vector3(Center.X + dX, Center.Y - dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Cube {
                    Center = new Vector3(Center.X + dX, Center.Y - dY, Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Cube {
                    Center = new Vector3(Center.X + dX, Center.Y + dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Cube {
                    Center = new Vector3(Center.X + dX, Center.Y + dY, Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
            ];

        }

        public bool CreateCondition(float maxFigureSize)
        {
            return Width > 3 * maxFigureSize;
        }

        public int MaxDepth(float maxSize)
        {
            int count = 0;
            float a = Width;

            while (a > maxSize)
            {
                a /= 2;
                count++;
            }

            return count;   
        }

        public bool Contains(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            var fullereneR = fullerene.OuterSphereRadius;

            return
                (Center.X - Length / 2) + fullereneR <= fullerene.Center.X &&
                fullerene.Center.X <= (Center.X + Length / 2) - fullereneR &&
                (Center.Y - Length / 2) + fullereneR <= fullerene.Center.Y &&
                fullerene.Center.Y <= (Center.Y + Length / 2) - fullereneR &&
                (Center.Z - Length / 2) + fullereneR <= fullerene.Center.Z &&
                fullerene.Center.Z <= (Center.Z + Length / 2) - fullereneR;
        }

        public bool ContainsPart(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            var centerF = fullerene.Center;

            if (!FiguresCollision.Intersects(this, in centerF, fullerene.OuterSphereRadius))
                return false;

            if (FiguresCollision.Intersects(this, in centerF, fullerene.InnerSphereRadius))
                return true;

            return
                fullerene.Vertices
                .Any(vertex => FiguresCollision.Pointinside(this, in vertex));
        }

        public override string ToString()
        {
            return '{' + $"Center: {Center}, Height: {Height}, Width: {Width}, Length: {Length}" + '}';
        }

        public override bool Equals(object? obj)
        {
            return obj is Cube second
                && Center.Equals(second.Center)
                && Math.Abs(Height - second.Height) < 0.00001
                && Math.Abs(Width - second.Width) < 0.00001
                && Math.Abs(Length - second.Length) < 0.00001;
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ Height.GetHashCode() ^ Width.GetHashCode() ^ Length.GetHashCode();
        }

        public static bool operator ==(Cube left, Cube right)
        {
            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Cube left, Cube right)
        {
            return !(left == right);
        }

        public bool Equals(Cube? other)
        {
            if (other is null)
                return false;

            return Center.Equals(other.Center)
                && Math.Abs(Height - other.Height) < 0.00001
                && Math.Abs(Width - other.Width) < 0.00001
                && Math.Abs(Length - other.Length) < 0.00001;
        }
    }
}