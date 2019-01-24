using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Social_Media_Exchanger.Startup))]
namespace Social_Media_Exchanger
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
