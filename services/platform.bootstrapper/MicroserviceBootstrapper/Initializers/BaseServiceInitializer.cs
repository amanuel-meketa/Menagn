using MicroserviceBootstrapper.Interfaces;
using MicroserviceBootstrapper.Utils;

namespace MicroserviceBootstrapper.Initializers
{
    public abstract class BaseServiceInitializer : IServiceInitializer
    {
        protected readonly Logger _logger;

        protected BaseServiceInitializer(Logger logger)
        {
            _logger = logger;
        }

        public abstract Task InitializeAsync();
    }
}
