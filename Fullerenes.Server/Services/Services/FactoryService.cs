using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Services.IServices;

namespace Fullerenes.Server.Services.Services
{
    public class FactoryService : IFactoryService
    {
        public FullereneAndLimitedAreaFactory GetFactory(AreaTypes areaType, FullereneTypes fullereneType, CreateFullerenesAndLimitedAreaRequest request)
        {
            return (typeLA: areaType, typeF: fullereneType) switch
            {
                (AreaTypes.Sphere, FullereneTypes.Icosahedron) => new IcosahedronFullereneAndSphereLimitedAreaFactory(request),
                _ => throw new NotImplementedException("We are not working with this type of limited area and fullerenes!")
            };
        }
    }
}
