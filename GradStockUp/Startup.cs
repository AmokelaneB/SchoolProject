using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GradStockUp.Startup))]
namespace GradStockUp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
