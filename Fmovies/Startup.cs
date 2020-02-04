using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Fmovies.Startup))]
namespace Fmovies
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
