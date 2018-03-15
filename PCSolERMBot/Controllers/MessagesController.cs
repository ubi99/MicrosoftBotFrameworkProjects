namespace DemoHelpDeskBot
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System.Collections.Generic;
    using System;
    using DemoHelpDeskBot.Util;
    using Microsoft.Bot.Builder.FormFlow;
    using Autofac;
    using System.Linq;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else
            {
                await this.HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                //IConversationUpdateActivity update = message;
                //var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                //var reply = message.CreateReply();
                //reply.Text = $"Welcome @{message.From.Name}";
                //await client.Conversations.ReplyToActivityAsync(reply);


                IConversationUpdateActivity update = message;
                using (var scope = Microsoft.Bot.Builder.Dialogs.Internals.DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (update.MembersAdded.Any())
                    {
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != message.Recipient.Id)
                            {
                                var reply = message.CreateReply();
                                reply.Text = $"Welcome {newMember.Name}!";
                                await client.Conversations.ReplyToActivityAsync(reply);

                                reply = message.CreateReply("You can ask me questions related to PSPL payroll like Attendance, Leaves, Salary Slip and Salary Deductions:");
                                reply.Type = ActivityTypes.Message;
                                reply.TextFormat = TextFormatTypes.Plain;

                                //reply.SuggestedActions = new SuggestedActions()
                                //{
                                //    Actions = new List<CardAction>()
                                //    {
                                //        new CardAction(){ Title = "Attendance", Type=ActionTypes.ImBack, Value="Show me my attendance for this month" },
                                //        new CardAction(){ Title = "Leave", Type=ActionTypes.ImBack, Value="What is my leave balance?" },
                                //        new CardAction(){ Title = "Deductions", Type=ActionTypes.ImBack, Value="Deductions last month" }
                                //    }
                                //};
                                await client.Conversations.ReplyToActivityAsync(reply);
                            }
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}