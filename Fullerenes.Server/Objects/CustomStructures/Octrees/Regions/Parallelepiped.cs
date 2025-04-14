using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.Fullerenes;
using System.Drawing;
using System.Numerics;

namespace Fullerenes.Server.Objects.CustomStructures.Octrees.Regions
{
    public class Parallelepiped : IRegion, IEquatable<Parallelepiped>
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
                new Parallelepiped {
                    Center = new Vector3(Center.X - dX, Center.Y - dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(Center.X - dX, Center.Y - dY, Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(Center.X - dX, Center.Y + dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(Center.X - dX, Center.Y + dY, Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(Center.X + dX, Center.Y - dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(Center.X + dX, Center.Y - dY, Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(Center.X + dX, Center.Y + dY, Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
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

        public bool Contains(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            var outerSphereRadius = fullerene.GenerateOuterSphereRadius();

            return Center.X - Length / 2 <= fullerene.Center.X - outerSphereRadius &&
                fullerene.Center.X + outerSphereRadius <= Center.X + Length / 2 &&
                Center.Y - Height / 2 <= fullerene.Center.Y - outerSphereRadius &&
                fullerene.Center.Y + outerSphereRadius <= Center.Y + Height / 2 &&
                Center.Z - Width / 2 <= fullerene.Center.Z - outerSphereRadius &&
                fullerene.Center.Z + outerSphereRadius <= Center.Z + Width / 2;
        }

        public bool ContainsPart(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            if (!FiguresCollision.Intersects(this, fullerene.Center, fullerene.GenerateOuterSphereRadius()))
                return false;

            if (FiguresCollision.Intersects(this, fullerene.Center, fullerene.GenerateInnerSphereRadius()))
                return true;

            return
                fullerene.Vertices
                .AddMidPoints(fullerene.Faces)
                .Any(vertex => FiguresCollision.Pointinside(this, vertex));
        }

        public override string ToString()
        {
            return '{' + $"Center: {Center}, Height: {Height}, Width: {Width}, Length: {Length}" + '}';
        }

        public override bool Equals(object? obj)
        {
            return obj is Parallelepiped second
                && Center.Equals(second.Center)
                && Math.Abs(Height - second.Height) < 0.00001
                && Math.Abs(Width - second.Width) < 0.00001
                && Math.Abs(Length - second.Length) < 0.00001;
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ Height.GetHashCode() ^ Width.GetHashCode() ^ Length.GetHashCode();
        }

        public static bool operator ==(Parallelepiped left, Parallelepiped right)
        {
            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Parallelepiped left, Parallelepiped right)
        {
            return !(left == right);
        }

        public bool Equals(Parallelepiped? other)
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