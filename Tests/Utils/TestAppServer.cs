
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ApplicationTests.Utils
{
    public class TestAppServer : WebApplicationFactory<Program>
    {
        private readonly ServiceDescriptor[] _overrides;

        public TestAppServer(params ServiceDescriptor[]? overrides)
        {
            _overrides = overrides ?? Array.Empty<ServiceDescriptor>();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                foreach (var service in _overrides)
                {
                    services.Replace(service);
                }
            });

            return base.CreateHost(builder);
        }
    }
}