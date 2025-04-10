using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures;

namespace Fullerenes.Server.Objects.Dtos
{
    public record struct CreateFullerenesAndLimitedAreaRequest(
        float AreaX, float AreaY, float AreaZ, AreaAdditionalParamsRequest AreaAdditionalParams,
        int NumberOfF, float MinSizeF, float MaxSizeF, float MaxAlphaF, float MaxBetaF, float MaxGammaF,
        float Shape, float Scale, int NumberOfSeries) : IRequest
    {
        public bool IsCorrectRequest()
        {
            return true;
        }
    }
    public record struct AreaAdditionalParamsRequest(float[]? AreaParams, float? Nc) : IRequest
    {
        public bool IsCorrectRequest()
        {
            return true;
        }
    }
    public record struct LimitedAreaWithFullerenesRequest(
        Vector3 Center,
        float[] Parameters, IReadOnlyCollection<FullereneRequest> Fullerenes) : IRequest
    {
        public bool IsCorrectRequest()
        {
            return true;
        }
    }

    public record struct FullereneRequest(
        Vector3 Center, EulerAngles EulerAngles, float Size) : IRequest
    {
        public bool IsCorrectRequest()
        {
            return true;
        }
    }

}