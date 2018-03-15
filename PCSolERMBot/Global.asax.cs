using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DemoHelpDeskBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Conversation.UpdateContainer(builder =>
            //{
            //    builder.RegisterType<DebugActivityLogger>().AsImplementedInterfaces().InstancePerDependency();
            //});
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DefaultExceptionMessageOverrideModule());
            builder.RegisterType<DebugActivityLogger>().AsImplementedInterfaces().InstancePerDependency();
            builder.Update(Conversation.Container);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
