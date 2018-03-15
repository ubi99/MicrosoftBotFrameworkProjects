using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Threading.Tasks;
using System.Resources;
using System.Diagnostics;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs;

namespace DemoHelpDeskBot
{
    public class PostUnhandledExceptionToUserCustom : IPostToBot
    {
        private readonly ResourceManager resources;
        private readonly IPostToBot inner;
        private readonly IBotToUser botToUser;
        private readonly TraceListener trace;

        public PostUnhandledExceptionToUserCustom(IPostToBot inner, IBotToUser botToUser, ResourceManager resources, TraceListener trace)
        {
            SetField.NotNull(out this.inner, nameof(inner), inner);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
            SetField.NotNull(out this.resources, nameof(resources), resources);
            SetField.NotNull(out this.trace, nameof(trace), trace);
        }

        public async Task PostAsync(IActivity activity, CancellationToken token)
        {
            try
            {
                await inner.PostAsync(activity, token);
            }
            catch(Exception ex)
            {
                try
                {
                    await botToUser.PostAsync("Sorry that I'm unable to answer you in current context. Please try again later...", cancellationToken: token);
                }
                catch(Exception innerex)
                {
                    trace.WriteLine(inner);
                }
                throw;
            }
        }
    }
}