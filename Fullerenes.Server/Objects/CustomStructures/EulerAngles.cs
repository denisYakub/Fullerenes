namespace Fullerenes.Server.Objects.CustomStructures
{
    public readonly struct EulerAngles
    {
        public required float PraecessioAngle { get; init; }
        public required float NutatioAngle { get; init; }
        public required float ProperRotationAngle { get; init; }
        public override string ToString()
        {
            return $"<{PraecessioAngle} {NutatioAngle} {ProperRotationAngle}>";
        }
    }
}
