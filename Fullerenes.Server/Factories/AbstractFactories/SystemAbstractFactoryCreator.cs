using Fullerenes.Server.Objects.Adapters;
using Fullerenes.Server.Objects.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class SystemAbstractFactoryCreator
    {
        public abstract SystemAbstractFactory CreateSystemFactory(CreateFullerenesAndLimitedAreaRequest request, int series, int fullereneNumber);
    }
}
