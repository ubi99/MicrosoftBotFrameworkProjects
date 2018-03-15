using Autofac;
using Microsoft.Bot.Builder.Autofac.Base;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoHelpDeskBot
{
    public class DefaultExceptionMessageOverrideModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterKeyedType<PostUnhandledExceptionToUserCustom, IPostToBot>().InstancePerLifetimeScope();

            builder.RegisterAdapterChain<IPostToBot>(
                typeof(EventLoopDialogTask),
                typeof(SetAmbientThreadCulture),
                typeof(QueueDrainingDialogTask),
                typeof(PersistentDialogTask),
                typeof(ExceptionTranslationDialogTask),
                typeof(SerializeByConversation),
                typeof(PostUnhandledExceptionToUserCustom),
                typeof(LogPostToBot)
                ).InstancePerLifetimeScope();
        }
    }
}