using Fullerenes.Server.Objects.Dtos;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class SystemAbstractFactoryCreator
    {
        public abstract SystemAbstractFactory CreateSystemFactory(CreateFullerenesAndLimitedAreaRequest request, int fullereneNumber);
    }
}
