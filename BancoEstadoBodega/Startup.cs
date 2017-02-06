using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BancoEstadoBodega.Startup))]
namespace BancoEstadoBodega
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
