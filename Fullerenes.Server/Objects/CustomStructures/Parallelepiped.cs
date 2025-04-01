using System.Numerics;

namespace Fullerenes.Server.Objects.CustomStructures
{
    public class Parallelepiped : IEquatable<Parallelepiped>
    {
        public required Vector3 Center { get; init; }
        public required float Height { get; init; }
        public required float Width { get; init; }
        public required float Length { get; init; }

        public static Parallelepiped[] Split8Parts(Parallelepiped parent)
        {
            ArgumentNullException.ThrowIfNull(parent);

            var halfWidth = parent.Width / 2;
            var halfHeight = parent.Height / 2;
            var halfLength = parent.Length / 2;

            var dX = parent.Width / 4;
            var dY = parent.Height / 4;
            var dZ = parent.Length / 4;

            return [
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X - dX, parent.Center.Y - dY, parent.Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X - dX, parent.Center.Y - dY, parent.Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X - dX, parent.Center.Y + dY, parent.Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X - dX, parent.Center.Y + dY, parent.Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X + dX, parent.Center.Y - dY, parent.Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X + dX, parent.Center.Y - dY, parent.Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X + dX, parent.Center.Y + dY, parent.Center.Z - dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
                new Parallelepiped {
                    Center = new Vector3(parent.Center.X + dX, parent.Center.Y + dY, parent.Center.Z + dZ),
                    Width = halfWidth,
                    Height = halfHeight,
                    Length = halfLength
                },
            ];

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