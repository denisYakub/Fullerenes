using MessagePack;

namespace Fullerenes.Server.Objects.CustomStructures
{
    [MessagePackObject]
    public readonly struct EulerAngles : IEquatable<EulerAngles>
    {
        [Key(0)]
        public float PraecessioAngle { get; init; }
        [Key(1)]
        public float NutatioAngle { get; init; }
        [Key(2)]
        public float ProperRotationAngle { get; init; }
        public EulerAngles(float praecessioAngle, float nutatioAngle, float properRotationAngle)
        {
            PraecessioAngle = praecessioAngle;
            NutatioAngle = nutatioAngle;
            ProperRotationAngle = properRotationAngle;
        }
        public EulerAngles(string anglesStr)
        {
            ArgumentNullException.ThrowIfNull(anglesStr);

            float[] coordinates = anglesStr.Split(' ').Select(float.Parse).ToArray();

            if (coordinates.Length != 3) return;

            PraecessioAngle = coordinates[0];
            NutatioAngle = coordinates[1];
            ProperRotationAngle = coordinates[2];
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            return obj is EulerAngles other && Equals(other);
        }

        public override int GetHashCode()
        {
            return PraecessioAngle.GetHashCode() ^ NutatioAngle.GetHashCode() ^ ProperRotationAngle.GetHashCode();
        }

        public static bool operator ==(EulerAngles left, EulerAngles right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EulerAngles left, EulerAngles right)
        {
            return !(left == right);
        }

        public bool Equals(EulerAngles other)
        {
            return Math.Abs(PraecessioAngle - other.PraecessioAngle) < 0.0001 &&
                    Math.Abs(NutatioAngle - other.NutatioAngle) < 0.0001 &&
                    Math.Abs(ProperRotationAngle - other.ProperRotationAngle) < 0.0001;
        }
    }
}
