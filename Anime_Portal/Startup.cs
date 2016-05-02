using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Anime_Portal.Startup))]
namespace Anime_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
