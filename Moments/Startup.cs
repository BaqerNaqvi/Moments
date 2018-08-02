using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Moments.Startup))]
namespace Moments
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
