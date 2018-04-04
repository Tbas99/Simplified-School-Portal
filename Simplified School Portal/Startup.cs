using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Simplified_School_Portal.Startup))]
namespace Simplified_School_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
