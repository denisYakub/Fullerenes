using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures;

namespace Fullerenes.Server.Objects.Dtos
{
    public record struct CreateFullerenesAndLimitedAreaRequest(
        float AreaX, float AreaY, float AreaZ, AreaAdditionalParamsRequest AreaAdditionalParams,
        float MinSizeF, float MaxSizeF, float MaxAlphaF, float MaxBetaF, float MaxGammaF,
        float Shape, float Scale) : IRequest
    {
        public bool IsCorrectRequest()
        {
            return
                AreaAdditionalParams.IsCorrectRequest()
                && MinSizeF <= MaxSizeF
                && MaxAlphaF <= 360 && MaxAlphaF >= 0
                && MaxBetaF <= 360 && MaxBetaF >= 0
                && MaxGammaF <= 360 && MaxGammaF >= 0;
        }
    }
    public record struct AreaAdditionalParamsRequest(float[]? AreaParams, float? Nc) : IRequest
    {
        public bool IsCorrectRequest()
        {
            return Nc.HasValue || (AreaParams?.Any() ?? false);
        }
    }
}