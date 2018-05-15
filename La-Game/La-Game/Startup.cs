using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(La_Game.Startup))]
namespace La_Game
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
