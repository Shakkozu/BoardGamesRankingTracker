using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BoardGamesRankingTracker.Startup))]
[assembly: OwinStartup(typeof(BoardGamesRankingTracker.Startup))]
namespace BoardGamesRankingTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

           // app.MapSignalR();
        }
    }
}
