using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LeagueManager.Startup))]
namespace LeagueManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
