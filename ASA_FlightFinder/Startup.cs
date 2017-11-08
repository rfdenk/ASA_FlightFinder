using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASA_FlightFinder.Startup))]
namespace ASA_FlightFinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
