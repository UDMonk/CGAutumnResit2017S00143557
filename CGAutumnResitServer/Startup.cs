using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CGAutumnResitServer.Startup))]

namespace CGAutumnResitServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {   
            app.MapSignalR();
        }
    }
}
