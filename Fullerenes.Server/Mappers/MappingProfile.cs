using AutoMapper;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FullereneRequest, Fullerene>()
                 .ConvertUsing(src => new IcosahedronFullerene(src.Center.X, src.Center.Y, src.Center.Z,
                 src.EulerAngles.PraecessioAngle, src.EulerAngles.NutatioAngle, src.EulerAngles.ProperRotationAngle, src.Size));
        }
    }
}