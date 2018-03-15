namespace DemoHelpDeskBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Util;
    using Newtonsoft.Json;
    using System.Text.RegularExpressions;
    using System.Linq;
    using System.Web;
    using System.IO;
    using System.Net.Http;
    using System.Data;
    using System.Configuration;
    using System.Net;

    [Serializable]
    //[LuisModel("6cf52af8-72ed-44f5-a511-2b3d1ebe971a", "48528a74184d4b53ac59407d73b2c1b9")]
    [LuisModel("c8411640-95b3-4820-8646-d23113f6696d", "1609ef8fbefb4354b48da48bb9859501")]
    public class RootDialog : LuisDialog<object>
    {
        Queries qry = null;
        string query = "";
        string subquery = "";
        string queryDescription = "";
        string EmpId = "";
        QnAMakerResult response;


        /// *********************************************************
        /// ******************** START OF INTENTS *******************
        /// *********************************************************

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string test = null;
            test.Split('"');
            context.Done<object>(null);
            return;
            EmpId = context.Activity.From.Id.ToString();
            //EmpId = "e6518";
            EntityRecommendation greetingEntityRecommendation;
            result.TryFindEntity("Greeting", out greetingEntityRecommendation);
            Newtonsoft.Json.Linq.JArray jarr = null;
            string greetingSynonym = "";
            try
            {
                jarr = Newtonsoft.Json.Linq.JArray.Parse(JsonConvert.SerializeObject(greetingEntityRecommendation?.Resolution["values"]));
                greetingSynonym = jarr[0].ToString();
            }
            catch (Exception ex)
            {
            }
            if (greetingSynonym == "Bad Salutation")
            {
                Random randomNoGen = new Random();
                double randomNo = randomNoGen.NextDouble();
                string text = null;
                if (randomNo < 0.33)
                    text = $"It seems you are trying to greet me. It's nice to have you here. \n\nHow may I be of your assistance?";
                else if (randomNo < 0.66)
                    text = $"Looks like you are trying to greet me. \n\nHello there. How may I help you?";
                else if (randomNo < 1.0)
                    text = $"Hello there. It is nice to have someone to talk to. \n\nHow may I be of your assistance?";
                await context.PostAsync(text);
                context.Done<object>(null);
            }
            else if (greetingSynonym == "Salutation")
            {
                //string welcomeNote = "";
                //Random randomNoGen = new Random();
                //double randomNo = randomNoGen.NextDouble();
                //if (randomNo < 0.2)
                //    welcomeNote = "Hi. How may I help you?";
                //else if (randomNo < 0.4)
                //    welcomeNote = "Hello. How may I assist you today?";
                //else if (randomNo < 0.7)
                //    welcomeNote = "Hey There. I'm PSPL dude. How may I assist you today?";
                //else
                //    welcomeNote = "Hi. How may I be at your service?";
                //await context.PostAsync(welcomeNote);
                string text = "";
                if(DateTime.Now.Hour > 16)
                {
                    text = "Hey There. I'm PSPL dude. How may I be of your assistance today?";
                }
                else if (DateTime.Now.Hour >= 12)
                {
                    text = "Good Afternoon. I'm PSPL dude. How may I help you today?";
                }
                else
                {
                    text = "Good Morning. I'm PSPL dude. How may I assist you today?";
                }
                await context.PostAsync(text);
                context.Done<object>(null);
            }
            else
            {
                await context.PostAsync("I'm sorry. I didn't understand that. Ask me anything realated to your Daily Office Attendance, Salary, Leaves, etc.");
                context.Done<object>(null);
            }
        }
        //done//

        [LuisIntent("QnA")]
        public async Task QnA(IDialogContext context, LuisResult result)
        {
            string query = result.Query;
            try
            {
                string responseString = string.Empty;
                string knowledgebaseId = ConfigurationManager.AppSettings["QnaKnowledgebaseId"].ToString();
                string qnamakerSubscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionKey"].ToString();

                //Build the URI
                Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");
                var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

                //Add the question as part of the body
                var postBody = $"{{\"question\": \"{query}\"}}";

                //Send the POST request
                using (WebClient client = new WebClient())
                {
                    //Set the encoding to UTF8
                    client.Encoding = System.Text.Encoding.UTF8;

                    //Add the subscription key header
                    client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                    client.Headers.Add("Content-Type", "application/json");
                    responseString = client.UploadString(builder.Uri, postBody);
                }
                int flag = 0;
                try
                {
                    response = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
                }
                catch
                {
                    //throw new Exception("Unable to deserialize QnA Maker response string.");
                    response = null;
                }

                if (response == null)
                {
                    await context.PostAsync("Currently I'm unable to send you an answer due to some technical issue. Please try again later.");
                    context.Done<object>(null);
                }
                else
                {
                    if (response.Answers[0].Questions.Length > 0)
                    {
                        if(response.Answers[0].Score < 70)
                        {
                            string text = $"Did you mean '{response.Answers[0].Questions[0]}?'";
                            PromptDialog.Confirm(context, questionConfirmed, text);
                            return;
                        }
                        if (response.Answers[0].Questions[0] == "Do you know Devender Taneja")
                        {
                            await context.PostAsync(response.Answers[0].Answer);
                            context.Done<object>(null);
                        }
                        else if (response.Answers[0].Questions[0] == "Ms. Punita Taneja")
                        {
                            await context.PostAsync(response.Answers[0].Answer);
                            context.Done<object>(null);
                        }
                        else if (response.Answers[0].Questions[0] == "Mr. D Chandrashekar")
                        {
                            await context.PostAsync(response.Answers[0].Answer);
                            context.Done<object>(null);
                        }
                        else if (response.Answers[0].Questions[0] == "Mr. Deepak Chawla")
                        {
                            await context.PostAsync(response.Answers[0].Answer);
                            context.Done<object>(null);
                        }
                        else if (response.Answers[0].Questions[0] == "Mr. Giriraj Singh Bhamu")
                        {
                            await context.PostAsync(response.Answers[0].Answer);
                            context.Done<object>(null);
                        }
                        else if (response.Answers[0].Questions[0] == "Mr. Yawar Aubaid Mir")
                        {
                            await context.PostAsync(response.Answers[0].Answer);
                            context.Done<object>(null);
                        }
                        else if (response.Answers[0].Questions[0] == "Mr. Jitender Kumar Garhwal")
                        {
                            await context.PostAsync(response.Answers[0].Answer);
                            context.Done<object>(null);
                        }
                    }
                    else
                    {
                        await context.PostAsync("Could not find an answer in the knowledge base.");
                        context.Done<object>(null);
                    }
                }
            }
            catch(Exception ex)
            {
                await context.PostAsync("Something went wrong and I couldn't find an answer to your question. Please try again later");
                context.Done<object>(null);
            }
        }
        //done

        [LuisIntent("SalaryRelated")]
        public async Task SalaryRelated(IDialogContext context, LuisResult result)
        {
            //EmpId = context.Activity.From.Id.ToString();
            string ecode = context.Activity.From.Id;
            if(ecode.ToLower().Contains("cons"))
            {
                await context.PostAsync("I'm sorry, these features are not available for the currently logged in user.");
                context.Done<object>(null);
                return;
            }
            DateTime currentdate = DateTime.Now;
            var obj = new GetDetailsFromERM();
            currentdate = currentdate.AddMonths(-1);
            string resultdata = obj.getTotalAndPaidDays(ecode, currentdate);
            if (string.IsNullOrEmpty(resultdata))
            {
                await context.PostAsync("I'm sorry that I could not find your salary detail for last month. Please try again later.");
                context.Done<object>(null);
            }
            else
            {
                try
                {
                    resultdata = resultdata.Trim('"');
                    int totalDays = Convert.ToInt32(Convert.ToDouble(resultdata.Split('-')[0]));
                    int paidDays = Convert.ToInt32(Convert.ToDouble(resultdata.Split('-')[1]));
                    int totaldaysinmonth = DateTime.DaysInMonth(currentdate.Year, currentdate.Month);
                    //match total days with paid days and inform accordingly
                    if (totaldaysinmonth == totalDays)
                    {
                        if(currentdate.Day > 6)
                        {
                            await context.PostAsync($"As I can see your salary was approved for whole {totalDays} Days last month. In case you received salary with a few days' decuction, the rest amount will be credited in next Payment Cycle.");
                            PromptDialog.Confirm(context, showDeductionsConfirmed, "I can show you deductions for the last month if you want:");
                        }
                        else
                        {
                            await context.PostAsync($"As I can see your salary was approved for whole {totalDays} Days last month, so complete salary will be credited to your account. No worries :-)");
                            context.Done<object>(null);

                        }
                    }
                    else
                    {
                        await LessPaidDaysDialog(context);
                        await DeductionsDialog(context, currentdate);
                        context.Done<object>(null);
                    }
                }
                catch(Exception ex)
                {
                    await context.PostAsync("Something went wrong while I was trying to retrieve your salary details. Please try again later.");
                    context.Done<object>(null);
                }
            }
        }
        //done

        [LuisIntent("DeductionsRelated")]
        public async Task DeductionsRelated(IDialogContext context, LuisResult result)
        {
            if (context.Activity.From.Id.ToLower().Contains("cons"))
            {
                await context.PostAsync("I'm sorry to say that these features are not available for the currently logged in user.");
                context.Done<object>(null);
                return;
            }
            EntityRecommendation dateRangeEntityRecommendation;
            List<DateTime> datelist = new List<DateTime>();
            result.TryFindEntity("builtin.datetimeV2.daterange", out dateRangeEntityRecommendation);
            if(dateRangeEntityRecommendation!=null)
            {
                try
                {
                    Dictionary<string, object> val = (Dictionary<string, object>)((List<object>)dateRangeEntityRecommendation.Resolution["values"]).FirstOrDefault();
                    var start = val["start"].ToString();
                    DateTime startdate = Convert.ToDateTime(start);
                    await DeductionsDialog(context, startdate);
                    context.Done<object>(null);
                }
                catch (Exception ex)
                {
                    await context.PostAsync("Something went wrong while I was retrieving your salary deductions. Please try again later.");
                    context.Done<object>(null);
                }
            }
            else
            {
                DateTime currentdate = DateTime.Now;
                if (currentdate.Day > 6)
                {
                    currentdate = currentdate.AddMonths(-1);
                }
                else
                {
                    currentdate = currentdate.AddMonths(-2);
                }
                await DeductionsDialog(context, currentdate);
                context.Done<object>(null);
            }
        }
        //done

        [LuisIntent("NoSalarySlip")]
        public async Task NoSalarySlip(IDialogContext context, LuisResult result)
        {
            string[] steps = { "Login to your ERM account.", "Click on the human-like icon found in top right corner.", "Click on salary slip from the available menu located on the left of the screen", "All the salary slips will be visible in a table on the opened page." };
            string text = "You can check for salary slip in ERM portal. Follow these steps to find the salary slip:";
            var card = CreateListCard(steps, text);
            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
            await context.PostAsync(message);
            PromptDialog.Confirm(context, SalarySlipFoundMessageReceived, "Were you able to locate the Salary Slip after following the above instructions?");

        }
        //done

        [LuisIntent("SalarySlipRelated")]
        public async Task SalarySlipRelated(IDialogContext context, LuisResult result)
        {
            if(context.Activity.From.Id.ToLower().Contains("cons"))
            {
                await context.PostAsync("Apologies. These features are not available for the currently logged in user.");
                context.Done<object>(null);
                return;
            }
            //EmpId = context.Activity.From.Id.ToString();
            string[] options = { "Less Paid Days", "Deduction", "Both" };
            string text = "Does Salary Slip show less paid days or is there any deduction?";
            PromptDialog.Choice(context, DeductionReasonMessageReceived, options, text);
        }
        //done

        [LuisIntent("SalaryDueDate")]
        public async Task SalaryDueDate(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("The Salary gets credited by 7th of every month. \n\nYour salary will be credited in comming payment cycle.");
            context.Done<object>(null);
        }
        //done

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            string[] questionExamples = { "My salary slip is not visible in ERM", "What is my leave balance?" };
            await context.PostAsync("You can ask me any PSPL HR or Payroll related questions. Here are a few examples:");
            //PromptDialog.Choice(context, null, questionExamples, "");
            context.Done<object>(null);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string text = "";
            Random rnd = new Random();
            double randomNo = rnd.NextDouble();
            if (randomNo < 0.3)
                text = $"Hello { context.Activity.From.Name}. I'm doing good. Thanks for asking.";
            else if (randomNo < 0.6)
                text = $"Hi {context.Activity.From.Name}. I'm fine. Thanks for asking.";
            else
                text = $"Nice that you asked. I'm great.";
            await context.PostAsync(text);
            context.Done<object>(null);
        }
        //done

        [LuisIntent("BotIntro")]
        public async Task BotIntro(IDialogContext context, LuisResult result)
        {
            EmpId = context.Activity.From.Id.ToString();
            await context.PostAsync("I am PSPL dude. I can help you with your basic queries regarding PSPL Payroll like attendance, salary, leaves, etc.");
            context.Done<object>(null);
        }

        [LuisIntent("Angry")]
        public async Task Angry(IDialogContext context, LuisResult result)
        {
            string text = "";
            Random rnd = new Random();
            double randomNo = rnd.NextDouble();
            if (randomNo < 0.3)
                text = $"I'm sorry if I did anything wrong.";
            else if (randomNo < 0.6)
                text = $"I'm sorry. I'll try to be better in future.";
            else
                text = $"It seems you are angry with me. Sorry that I made you feel bad.";
            await context.PostAsync(text);
            context.Done<object>(null);
        }
        //done

        [LuisIntent("AttendanceRelated")]
        public async Task AttendanceRelated(IDialogContext context, LuisResult result)
        {
            if (context.Activity.From.Id.ToLower().Contains("cons"))
            {
                await context.PostAsync("I'm sorry to say that these features are not available for the currently logged in user.");
                context.Done<object>(null);
                return;
            }
            EmpId = context.Activity.From.Id.ToString();
            EntityRecommendation weekOffCategoryEntityRecommendation, dateRangeEntityRecommendation;
            List<DateTime> datelist = new List<DateTime>();
            result.TryFindEntity( "WeekOff", out weekOffCategoryEntityRecommendation);
            foreach(EntityRecommendation e in result.Entities)
            {
                try
                {
                    Dictionary<string, object> val = (Dictionary<string, object>)((List<object>)e.Resolution["values"]).FirstOrDefault();
                    var value = val["value"].ToString();
                    datelist.Add(Convert.ToDateTime(value));
                }
                catch (Exception ex)
                { }
            }
            result.TryFindEntity("builtin.datetimeV2.daterange", out dateRangeEntityRecommendation);
            if (weekOffCategoryEntityRecommendation != null)
            {
                //find weekoffs and check attendance for those days
                List<DateTime> weekofflist = new List<DateTime>();
                DateTime firstday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                for(int i=1; i< DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
                {
                    if (firstday.DayOfWeek == 0)
                        weekofflist.Add(firstday);
                    else if (firstday.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (((i/7 == 1 || i / 7 == 3) &&  (i%7 != 0)) || ((i/7 == 2 || i/7 == 4) && (i%7 == 0)))
                            weekofflist.Add(firstday);
                    }
                    firstday = firstday.AddDays(1);
                }
                GetDetailsFromERM obj = new GetDetailsFromERM();
                List<string> attendanceDetailList = new List<string>();
                string data = "";
                bool isWeekoffAbsent = false;
                bool isWeekoffUnvalidated = false;
                try
                {
                    data = obj.getAttendanceDetails(context.Activity.From.Id.ToString(), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
                    AttendanceInfo[] attendanceInfo = JsonConvert.DeserializeObject<AttendanceInfo[]>(data);
                    //flag for weekoffs marked absent or unvalidated yet
                    
                    foreach (DateTime date in weekofflist)
                    {
                        bool isFound = false;
                        for (int i = 0; i < attendanceInfo.Length; i++)
                        {
                            DateTime tempdate = Convert.ToDateTime(attendanceInfo[i].Date);
                            if (date.ToShortDateString() == tempdate.ToShortDateString())
                            {
                                if (attendanceInfo[i].AttendanceType == "Absent")
                                {
                                    isWeekoffAbsent = true;
                                    attendanceDetailList.Add(tempdate.ToString("dd/MM/yyyy") + " : Absent");
                                }
                                else if (attendanceInfo[i].Status == "U")
                                {
                                    isWeekoffUnvalidated = true;
                                    attendanceDetailList.Add(tempdate.ToString("dd/MM/yyyy") + " : Not Validated");
                                }
                                else
                                {
                                    attendanceDetailList.Add(tempdate.ToString("dd/MM/yyyy") + "Approved");
                                }
                                isFound = true;
                                break;
                            }
                        }
                        if (!isFound)
                        {
                            attendanceDetailList.Add(date.ToString("dd/MM/yyyy") + "Approved");
                        }

                    }
                }
                catch(Exception ex)
                {
                }
                
                if(isWeekoffAbsent && isWeekoffUnvalidated)
                {
                    string[] dateList = attendanceDetailList.ToArray();
                    await context.PostAsync("I see your weekoffs are marked absent and also there are some unvalidated entries. Contact your reporting manager to get the unvalidated entries approved soon.");
                    var card = CreateListCard(dateList, "Find the weekoff attendance entries below:");
                    var message = context.MakeMessage();
                    message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
                    await context.PostAsync(message);
                    await context.PostAsync("Currently I'm unable to drop a mail regarding this on your behalf so I suggest you contact Accounts Deptt. for this.");
                }
                else if(isWeekoffUnvalidated)
                {
                    string[] dateList = attendanceDetailList.ToArray();
                    await context.PostAsync("I found out that your weekoffs are yet to be validated. Contact your reporting manager to get the unvalidated entries approved soon.");
                    var card = CreateListCard(dateList, "Find the weekoff attendance entries below:");
                    var message = context.MakeMessage();
                    message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
                    await context.PostAsync(message);
                    await context.PostAsync("How may I assist you further.");
                }
                else if(isWeekoffAbsent)
                {
                    string[] dateList = attendanceDetailList.ToArray();
                    await context.PostAsync("You're right. Some of your weekoffs are marked absent. Currently I'm unable to drop a mail regarding this on your behalf so I suggest you contact Accounts Deptt. for this.");
                    var card = CreateListCard(dateList, "Find the weekoff attendance entries below to pin point the Absent dates:");
                    var message = context.MakeMessage();
                    message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
                    await context.PostAsync(message);
                }
                else
                {
                    string[] dateList = attendanceDetailList.ToArray();
                    string text = "I cross verified your attendance entries and your attendance entries seem fine. Find the weekoff attendance entries below for reference:";
                    var card = CreateListCard(dateList, text);
                    var message = context.MakeMessage();
                    message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
                    await context.PostAsync(message);
                }
            }
            //done
            else if (datelist != null && datelist.Count>0)
            {
                //check attendance details for these dates
                GetDetailsFromERM obj = new GetDetailsFromERM();
                List<string> attendanceDetailList = new List<string>();
                string data = obj.getAttendanceDetails(context.Activity.From.Id.ToString(),new DateTime(datelist[0].Year, datelist[0].Month, 1), new DateTime(datelist[0].Year, datelist[0].Month, DateTime.DaysInMonth(datelist[0].Year, datelist[0].Month)));
                AttendanceInfo[] attendanceInfo;
                if (!string.IsNullOrEmpty(data))
                {
                    data = data.Trim('"');
                }
                try
                {
                    attendanceInfo = JsonConvert.DeserializeObject<AttendanceInfo[]>(data);
                }
                catch(Exception ex)
                {
                    attendanceInfo = null;
                }
                
                foreach (DateTime date in datelist)
                {
                    bool isFound = false;
                    if(attendanceInfo==null)
                    {
                        attendanceDetailList.Add(date.ToString("dd/MM/yyyy") + " : Not Found");
                        continue;
                    }
                    for(int i=0; i<attendanceInfo.Length; i++)
                    {
                        DateTime tempdate = Convert.ToDateTime(attendanceInfo[i].Date);
                        if (date.ToShortDateString() == tempdate.ToShortDateString())
                        {
                            if (attendanceInfo[i].AttendanceType == "Absent")
                            {
                                attendanceDetailList.Add(tempdate.ToString("dd/MM/yyyy") + " : Absent");
                            }
                            else if(attendanceInfo[i].Status == "U")
                            {
                                attendanceDetailList.Add(tempdate.ToString("dd/MM/yyyy") + " : Not Validated");
                            }
                            else
                            {
                                attendanceDetailList.Add(tempdate.ToString("dd/MM/yyyy") + " : Approved");
                            }
                            isFound = true;
                            break;
                        }
                    }
                    if(!isFound)
                    {
                        attendanceDetailList.Add(date.ToString("dd/MM/yyyy") + " : Not Found");
                    }
                }
                if(attendanceDetailList.Count > 0)
                {
                    string[] dateList = attendanceDetailList.ToArray();
                    var card = CreateListCard(dateList, "Find the attendance details below:");
                    var message = context.MakeMessage();
                    message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
                    await context.PostAsync(message);
                    await context.PostAsync("In case of unvalidated entries contact your manager and ask him/her to approve the attendance.");
                }
                else
                {
                    await context.PostAsync("I'm sorry, I couldn't find any attendance entries for the dates you provided.");
                }
            }
            //done
            else if(dateRangeEntityRecommendation != null)
            {
                //find dates from result and check attendance details for those dates
                //await AttendanceDialog(context);
                try
                {
                    Dictionary<string, object> val = (Dictionary<string, object>)((List<object>)dateRangeEntityRecommendation.Resolution["values"]).FirstOrDefault();
                    var start = val["start"].ToString();
                    var end = val["end"].ToString();
                    DateTime startdate = Convert.ToDateTime(start);
                    DateTime enddate = Convert.ToDateTime(end);
                    enddate = enddate.AddDays(-1);
                    if(startdate.Month != enddate.Month)
                    {
                        await context.PostAsync("I can provide the attendance information only for a date range that lies between the start and end of a same month. Please try again.");
                        context.Done<object>(null);
                        return;
                    }
                    await AttendanceDialog(context, startdate, enddate);
                }
                catch (Exception ex)
                {
                    await context.PostAsync("Something went wrong while I was retrieving your attendance details. Please try again later.");
                    context.Done<object>(null);
                }
            }
            else
            {
                //find attendance details for whole month
                try
                {
                    await AttendanceDialog(context, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
                }
                catch(Exception ex)
                {
                    await context.PostAsync("Something went wrong while I was retrieving your attendance details. Please try again later.");
                    context.Done<object>(null);
                }
            }
            //done
            context.Done<object>(null);
        }

        [LuisIntent("LeaveRelated")]
        public async Task LeaveRelated(IDialogContext context, LuisResult result)
        {
            if (context.Activity.From.Id.ToLower().Contains("cons"))
            {
                await context.PostAsync("My apologies. I do not have access to your leave related data.");
                context.Done<object>(null);
                return;
            }
            GetDetailsFromERM obj = new GetDetailsFromERM();
            DateTime currentTime = DateTime.Now;
            DateTime currentdate;
            if(currentTime.Day < 5)
            {
                currentdate = new DateTime(currentTime.AddMonths(-2).Year, currentTime.AddMonths(-1).Month, 1);
            }
            else
            {
                currentdate = new DateTime(currentTime.AddMonths(-1).Year, currentTime.AddMonths(-1).Month, 1);
            }
            string leaveDetails = obj.getLeaveInfo(context.Activity.From.Id.ToString(), currentdate);
            //string message = "";
            LeaveInfo[] leaveInfo = null;
            try
            {
                leaveInfo = JsonConvert.DeserializeObject<LeaveInfo[]>(leaveDetails);
                //message = leaveDetails;
            }
            catch(Exception ex)
            {
                //message = ex.StackTrace;
                //message = leaveDetails;
            }
            if (leaveDetails == null)
            {
                //await context.PostAsync(message);
                await context.PostAsync("Something went wrong while I was retreiving your Leave details. Please try again later.");
                context.Done<object>(null);
            }
            else
            {
                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = new List<Attachment>()
                {
                    new HeroCard()
                    {
                        Title="Casual Leaves (CL's)",
                        Subtitle=$"Balance Available: {leaveInfo[0].BalanceAvailable}",
                        Text=$"Previoius Balance: {leaveInfo[0].OpeningBalance}\n\nLeaves Credited: {leaveInfo[0].LeaveCredited}\n\nLeaves Availed: {leaveInfo[0].LeaveAvailed}",
                    }.ToAttachment(),
                    new HeroCard()
                    {
                        Title="Earned Leaves (EL's)",
                        Subtitle=$"Balance Available: {leaveInfo[2].BalanceAvailable}",
                        Text=$"Previoius Balance: {leaveInfo[2].OpeningBalance}\n\nLeaves Credited: {leaveInfo[2].LeaveCredited}\n\nLeaves Availed: {leaveInfo[2].LeaveAvailed}",
                    }.ToAttachment(),
                    new HeroCard()
                    {
                        Title="Comp Offs (CO's)",
                        Subtitle=$"Balance Available: {leaveInfo[1].BalanceAvailable}",
                        Text=$"Previoius Balance: {leaveInfo[1].OpeningBalance}\n\nLeaves Credited: {leaveInfo[1].LeaveCredited}\n\nLeaves Availed: {leaveInfo[1].LeaveAvailed}",
                    }.ToAttachment(),
                };
         
                //await context.PostAsync($"Here is the leave info I found: \n\n\n\n**CLs**: \n\nPrevious CL Balance: {leaveInfo[0].OpeningBalance} \n\nCL's Credited: {leaveInfo[0].LeaveCredited} \n\nCL's Availed: {leaveInfo[0].LeaveAvailed} \n\nBalance: {leaveInfo[0].BalanceAvailable} \n\n\n\n**EL's:** \n\nPrevious EL Balance: {leaveInfo[2].OpeningBalance} \n\nEL's Credited: {leaveInfo[2].LeaveCredited} \n\nEL's Availed: {leaveInfo[2].LeaveAvailed} \n\nBalance: {leaveInfo[2].BalanceAvailable} \n\n\n\n**CompOff's:** \n\nPrevious CO Balance: {leaveInfo[1].OpeningBalance} \n\nCO's Credited: {leaveInfo[1].LeaveCredited} \n\nCO's Availed: {leaveInfo[1].LeaveAvailed} \n\nBalance: {leaveInfo[1].BalanceAvailable}");
                await context.PostAsync(reply);
                await context.PostAsync("Please note that the leave details get updated 5th of every month.");
                await context.PostAsync("How may I be of further assistance?");
                context.Done<object>(null);
            }
        }
        //done
        
        /// *********************************************************
        /// ******************* END OF INTENTS **********************
        /// *********************************************************

        private async Task questionConfirmed(IDialogContext context, IAwaitable<bool> argument)
        {
            bool answer = await argument;

            if(answer)
            {
                if (response.Answers[0].Questions[0] == "Do you know Devender Taneja")
                {
                    await context.PostAsync(response.Answers[0].Answer);
                    context.Done<object>(null);
                }
                else if (response.Answers[0].Questions[0] == "Ms. Punita Taneja")
                {
                    await context.PostAsync(response.Answers[0].Answer);
                    context.Done<object>(null);
                }
                else if (response.Answers[0].Questions[0] == "Mr. D Chandrashekar")
                {
                    await context.PostAsync(response.Answers[0].Answer);
                    context.Done<object>(null);
                }
                else if (response.Answers[0].Questions[0] == "Mr. Deepak Chawla")
                {
                    await context.PostAsync(response.Answers[0].Answer);
                    context.Done<object>(null);
                }
                else if (response.Answers[0].Questions[0] == "Mr. Giriraj Singh Bhamu")
                {
                    await context.PostAsync(response.Answers[0].Answer);
                    context.Done<object>(null);
                }
                else if (response.Answers[0].Questions[0] == "Mr. Yawar Aubaid Mir")
                {
                    await context.PostAsync(response.Answers[0].Answer);
                    context.Done<object>(null);
                }
                else if (response.Answers[0].Questions[0] == "Mr. Jitender Kumar Garhwal")
                {
                    await context.PostAsync(response.Answers[0].Answer);
                    context.Done<object>(null);
                }
            }
            else
            {
                await context.PostAsync("I have no idea who else you are talking about. Ask me some other question.");
                context.Done<object>(null);
            }
        }

        private async Task EnsureSalaryRelatedQuery(IDialogContext context)
        {

        }

        public async Task SalarySlipFoundMessageReceived(IDialogContext context, IAwaitable<bool> argument)
        {
            bool response = await argument;
            if(response)
            {
                await context.PostAsync("I'm glad that I could help you out. \n\nHow may I further help you?");
                context.Done<object>(null);
            }
            else
            {
                await context.PostAsync("Alright, I'm going to send you the salary slip in mail. Check your mail in a while. \n\nHow may I help you further?");
                context.Done<object>(null);
            }
        }

        public async Task DeductionReasonMessageReceived(IDialogContext context, IAwaitable<string> argument)
        {
            var response = await argument;
            if (response == "Less Paid Days")
            {
                await LessPaidDaysDialog(context);
                await context.PostAsync("How may I help you further?");
                context.Done<object>(null);
            }
            else if(response == "Deduction")
            {
                await DeductionsDialog(context, DateTime.Now.AddMonths(-1));
                await context.PostAsync("How may I help you further?");
                context.Done<object>(null);
            }
            else if(response == "Both")
            {
                await LessPaidDaysDialog(context);
                await DeductionsDialog(context, DateTime.Now.AddMonths(-1));
                await context.PostAsync("How may I help you further?");
                context.Done<object>(null);
            }
            else
            {
                await context.PostAsync("unknown response");
            }
        }

        public async Task showDeductionsConfirmed(IDialogContext context, IAwaitable<bool> argument)
        {
            bool response = await argument;
            if (response)
            {
                await DeductionsDialog(context, DateTime.Now.AddMonths(-1));
            }
            else
            {
                await context.PostAsync("Alright. How may I help you further?");
                context.Done<object>(null);
            }
        }

        public async Task LessPaidDaysDialog(IDialogContext context)
        {
            string ecode = context.Activity.From.Id;
            DateTime currentdate = DateTime.Now;
            var obj = new GetDetailsFromERM();
            currentdate = currentdate.AddMonths(-1);
            //if(currentdate.Day>5)
            //{
            //    currentdate = currentdate.AddMonths(-1);
            //}
            //else
            //{
            //    currentdate = currentdate.AddMonths(-2);
            //}
            //find paid days and inform user
            string result = obj.getTotalAndPaidDays(ecode, currentdate);
            if(string.IsNullOrEmpty(result))
            {
                return;
            }
            result = result.Trim('"');
            int totalDays = Convert.ToInt32(Convert.ToDouble(result.Split('-')[0]));
            int paidDays = Convert.ToInt32(Convert.ToDouble(result.Split('-')[1]));
            int totalDaysInMonth = DateTime.DaysInMonth(currentdate.Year, currentdate.Month);
            if (totalDays == totalDaysInMonth)
            {
                //paid days equal to total days -- data will be updated in final payslip
                if(currentdate.Day>6)
                    await context.PostAsync($"As I found out that your salary was approved for {totalDays} Days for the month of {currentdate.ToString("MMMM")} {currentdate.Year}, which is correct. You will receive complete salary for this month. In case you already received the salary and that was incomplete, the rest amount will be credited in next Payment cycle.");
                else
                    await context.PostAsync($"I can see that your Total Paid Days are equal to Total Days for the month of {currentdate.Month} {currentdate.Year}. The updated data will be reflected in Final PaySlip. No worries :-)");
                return;
                //context.Done<object>(null);
            }
            else
            {
                //paid days not equal to total days - find undervalidated, blank or absent days and inform user
                await context.PostAsync($"As I can see your salary is approved for only {totalDays} Days for the month of {currentdate.ToString("MMMM")} {currentdate.Year}. There is a deduction of {totalDaysInMonth - totalDays} days. Hold on while I get further details for you regarding those deducted days...");
                if (obj.getTotalAttendanceEntries(ecode, currentdate) < totalDaysInMonth)
                {
                    //string dateslist = obj.getMissingDates(ecode, currentdate);
                    string missingdatestring = obj.getMissingDates(ecode, currentdate);
                    string[] dates;
                    try
                    {
                        missingdatestring = missingdatestring.Trim('"');
                        missingdatestring = missingdatestring.Replace("\\/","/");
                        dates = missingdatestring.Trim('"').Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        for(int i=0; i<dates.Length; i++)
                        {
                            dates[i] = Convert.ToDateTime(dates[i]).ToShortDateString();
                        }
                    }
                    catch(Exception ex)
                    {
                        dates = null;
                        await context.PostAsync("Due to some technical issue I couldn't get the attendance details. Please try again later.");
                        context.Done<object>(null);
                        return;
                    }
                    if (dates.Length > 0)
                    {
                        var card = CreateListCard(dates, "Attendance entry not found for below date(s):");
                        var message = context.MakeMessage();
                        message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
                        await context.PostAsync(message);
                    }
                    else
                    {
                        await context.PostAsync("Attendance is already filled for the whole month. No missing entries.");
                    }
                }
                //find undervalidated and absent data and show to user --- later, also check if more leaves taken than available ---
                string deducedDates = obj.getDeducedDates(ecode, currentdate);
                if (!String.IsNullOrEmpty(deducedDates))
                {
                    string deduceddatestring = deducedDates.Trim('"');
                    if(string.IsNullOrEmpty(deduceddatestring))
                    {
                        return;
                    }
                    string[] data = deduceddatestring.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var card = CreateListCard(data, "Absent and Invalidated date(s):");
                    var message = context.MakeMessage();
                    message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
                    await context.PostAsync(message);
                    if (deducedDates.Contains("Not Validated"))
                        await context.PostAsync("Please contact your manager to get the invalidated dates approved.");
                }
            }
            //string currentdate1 = currentdate.ToString("yyyy/MM/dd");
            //await context.PostAsync("How may I be of further assistance.");
            return;
            //context.Done<object>(null);
        }

        public async Task DeductionsDialog(IDialogContext context, DateTime currentdate)
        {
            string ecode = context.Activity.From.Id;
            //DateTime currentdate = DateTime.Now;
            GetDetailsFromERM obj = new GetDetailsFromERM();
            //find deductions and inform user
            string deductions = obj.getDeductions(ecode, currentdate);
            if(string.IsNullOrEmpty(deductions))
            {
                await context.PostAsync("Unable to find the deductions. Please try again later.");
                return;
            }
            deductions = deductions.Trim('"');
            string[] dedList = deductions.Split(';');
            //string month = (currentdate.Day > 6 ? currentdate.AddMonths(-1).ToString("MMMM") : currentdate.AddMonths(-2).ToString("MMMM"));
            string month = currentdate.ToString("MMMM") + " " + currentdate.ToString("yyyy");
            var card = CreateListCard(dedList, "Here is the summary of the deductions for the month "+ month +":");
            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>
                        {
                            new Attachment
                            {
                                ContentType = "application/vnd.microsoft.card.adaptive",
                                Content = card,
                            }
                        };
            await context.PostAsync(message);
            return;
            //context.Done<object>(null);
        }

        public async Task AttendanceDialog(IDialogContext context, DateTime startdate, DateTime enddate)
        {
            GetDetailsFromERM obj = new GetDetailsFromERM();
            string ecode = context.Activity.From.Id.ToString();
            string data = obj.getAttendanceDetails(ecode, startdate, enddate);
            AttendanceInfo[] attendanceInfo;
            try
            {
                attendanceInfo = JsonConvert.DeserializeObject<AttendanceInfo[]>(data);
            }
            catch
            {
                attendanceInfo = null;
            }
            
            DateTime monthyear = new DateTime(startdate.Year, startdate.Month, 1);
            string[] missingDates;
            try
            {
                missingDates = (obj.getMissingDates(ecode, monthyear)).Trim('"').Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch(Exception ex)
            {
                missingDates = null;
            }
            if (missingDates != null && missingDates.Length > 0)
            {
                List<string> missingDateList = new List<string>();
                foreach(string str in missingDates)
                {
                    DateTime tempdate = Convert.ToDateTime(str.Replace("\\/","/"));
                    if (tempdate >= startdate && tempdate <= enddate)
                    {
                        missingDateList.Add(tempdate.ToString("dd/MM/yyyy"));
                    }
                }
                if(missingDateList.Count > 0)
                {
                    string[] dateList = missingDateList.ToArray();
                    //var card = CreateListCard(dateList, "The attendance is not filled for these dates:");
                    //var message = context.MakeMessage();
                    //message.Attachments = new List<Attachment>
                    //    {
                    //        new Attachment
                    //        {
                    //            ContentType = "application/vnd.microsoft.card.adaptive",
                    //            Content = card,
                    //        }
                    //    };
                    var message = context.MakeMessage();
                    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    message.Attachments = CreateScrollableListAttachment(dateList, "The attendance is not filled for these dates:");
                    await context.PostAsync(message);
                    await context.PostAsync("Fill the attendance for those dates and ask your manager to approve them.");
                }
            }
            if(attendanceInfo != null && attendanceInfo.Length > 0)
            {
                //find dates
                List<string> missingDateList = new List<string>();
                for(int i=0; i<attendanceInfo.Length; i++)
                {
                    if (attendanceInfo[i].AttendanceType == "Absent")
                    {
                        string tempStr = Convert.ToDateTime(attendanceInfo[i].Date).ToString("dd/MM/yyyy") + " : Absent";
                        missingDateList.Add(tempStr);
                    }
                    else if(attendanceInfo[i].Status == "U")
                    {
                        string tempStr = Convert.ToDateTime(attendanceInfo[i].Date).ToString("dd/MM/yyyy") + " : Not Validated";
                        missingDateList.Add(tempStr);
                    }
                    else
                    {
                        string tempStr = Convert.ToDateTime(attendanceInfo[i].Date).ToString("dd/MM/yyyy") + " : Accepted";
                        missingDateList.Add(tempStr);
                    }
                }
                if (missingDateList.Count > 0)
                {
                    string[] dateList = missingDateList.ToArray();
                    //var card = CreateListCard(dateList, "Find the filled attendance details below:");
                    //var message = context.MakeMessage();
                    //message.Attachments = new List<Attachment>
                    //    {
                    //        new Attachment
                    //        {
                    //            ContentType = "application/vnd.microsoft.card.adaptive",
                    //            Content = card,
                    //        }
                    //    };
                    var message = context.MakeMessage();
                    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    message.Attachments = CreateScrollableListAttachment(dateList, "Find the filled attendance details below:");
                    await context.PostAsync(message);
                    await context.PostAsync("In case of unvalidated entries contact your manager and ask him/her to approve the attendance.");
                }
                else
                {
                    await context.PostAsync("I'm sorry I could not find your Attendance details. \n\nIs there anything else that I can help you with?");
                    return;
                }
            }
            
        }

        public async Task SubQueryMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var response = await argument;
            this.subquery = response;
            await EnsureQuery(context);
        }

        public async Task QueryDescriptionMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var response = await argument;
            this.queryDescription = response;
            await EnsureQuery(context);
        }

        public async Task ConfirmAttachmentMessageReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var response = await argument;
            if (!response)
            {
                await context.PostAsync("Generating your ticket. Please wait.");

                //generate ticket
                string subQueryId = qry.SubQueryList.FirstOrDefault(x => x.Value == this.subquery).Key;
                bool res = qry.createTicket(EmpId, this.queryDescription, subQueryId, "", "");
                context.Done<object>(null);
            }
            else
            {
                PromptDialog.Attachment(context, this.AttachmentReceivedAsync, "Upload your desired attachment:");
                //PromptDialog.Attachment(context, this.AttachmentReceivedAsync, "Upload your desired attachment:");
                //await context.PostAsync("Upload your desired attachment:");
                //context.Wait<IMessageActivity>(this.AttachmentReceivedAsync1);
            }
        }

        public async Task ConfirmationMessageReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var response = await argument;
            if(response)
            {
                await context.PostAsync("Generating your ticket. Please wait.");
                context.Done<object>(null);
            }
            else
            {
                await context.PostAsync("Your Query has been cancelled.");
                context.Done<object>(null);
            }
        }

        public async Task EnsureQuery(IDialogContext context)
        {
            if (string.IsNullOrEmpty(this.EmpId))
            {
                Attachment plAttachment = null;
                //login card
                SigninCard plCard = new SigninCard("login to continue", GetCardActions("https://erm.e-pspl.com/employee", "signin"));
                plAttachment = plCard.ToAttachment();
                IMessageActivity response = context.MakeMessage();
                response.Recipient = context.Activity.From;
                response.Type = "message";

                response.Attachments = new List<Attachment>();
                response.Attachments.Add(plAttachment);

                await context.PostAsync(response);
            }
            else if (string.IsNullOrEmpty(this.query))
            {
                qry = new Queries();
                qry.setQueryList();
                string[] options = new string[qry.QueryList.Count];
                int i = 0;
                foreach (var item in qry.QueryList.Values)
                {
                    options[i] = item;
                    i++;
                }
                //PromptDialog.Choice(context, this.QueryMessageReceivedAsync, options, "query: ");
            }
            else if (string.IsNullOrEmpty(this.subquery))
            {
                //qry = new Queries();
                string selectedQueryId = qry.QueryList.FirstOrDefault(x => x.Value == this.query).Key;
                qry.setSubQueryList(selectedQueryId);
                string[] options = new string[qry.SubQueryList.Count];
                int i = 0;
                foreach (var item in qry.SubQueryList.Values)
                {
                    options[i] = item;
                    i++;
                }
                PromptDialog.Choice(context, this.SubQueryMessageReceivedAsync, options, "sub query: ");
            }
            else if (string.IsNullOrEmpty(this.queryDescription))
            {
                PromptDialog.Text(context, this.QueryDescriptionMessageReceivedAsync, "Please enter a description for the query:");
            }
            else
            {
                PromptDialog.Confirm(context, this.ConfirmAttachmentMessageReceivedAsync, "Do you want to add an attachment?");
            }
        }
        
        public async Task AttachmentReceivedAsync1(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            string FileName = message.Attachments[0].Name;
            byte[] OriginalBase64 = File.ReadAllBytes(message.Attachments[0].ContentUrl);
            //HttpPostedFileBase file = (HttpPostedFileBase)message.Attachments[0].Content;
            string filePath = HttpContext.Current.Server.MapPath("~/TempImages/" + FileName);
            //file.SaveAs(filePath);
            File.WriteAllBytes(filePath, OriginalBase64);

            if (message.Attachments != null && message.Attachments.Any())
            {
                var attachment = message.Attachments.First();
            }
        }

        public async Task AttachmentReceivedAsync(IDialogContext context, IAwaitable<IEnumerable<Attachment>> argument)
        {
            var response = await argument;

            var uploadedAtt = response.FirstOrDefault();
            string FileName = uploadedAtt.Name;
            string attFileName = EmpId + "-" + DateTime.Now.ToString().Replace("/", "-").Replace("-", "").Replace(":", "-") + "-" + FileName.Replace("'", "");
            if (uploadedAtt != null)
            {
                var attachment = uploadedAtt;
                using (HttpClient httpClient = new HttpClient())
                {
                    var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);
                    var contentLenghtBytes = responseMessage.Content.Headers.ContentLength;
                    byte[] contentbytes = await responseMessage.Content.ReadAsByteArrayAsync();
                    await context.PostAsync($"Attachment of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received.");
                    string targetPath = HttpContext.Current.Server.MapPath("~/TempImages/" + attFileName);
                    //file.SaveAs(filePath);
                    File.WriteAllBytes(targetPath, contentbytes);
                }
            }

            //generate ticket
            string subQueryId = qry.SubQueryList.FirstOrDefault(x => x.Value == this.subquery).Key;
            bool res = qry.createTicket(EmpId, this.queryDescription, subQueryId, attFileName, FileName);
            context.Done<object>(null);
        }

        private AdaptiveCard CreateCard(string ticketId, string personalInfoName, string category, string severity, string description, string domain)
        {
            AdaptiveCard card = new AdaptiveCard();

            var headerBlock = new TextBlock()
            {
                Text = $"Ticket #{ticketId}",
                Weight = TextWeight.Bolder,
                Size = TextSize.Large,
                Speak = $"<s>You've created a new Ticket #{ticketId}</s><s>We will contact you soon.</s>"
            };

            var columnsBlock = new ColumnSet()
            {
                Separation = SeparationStyle.Strong,
                Columns = new List<Column>
                {
                    new Column
                    {
                        Size = "1",
                        Items = new List<CardElement>
                        {
                            new FactSet
                            {
                                Facts = new List<AdaptiveCards.Fact>
                                {
                                    new AdaptiveCards.Fact("Ticket Owner:", personalInfoName),
                                    new AdaptiveCards.Fact("Severity:", severity),
                                    new AdaptiveCards.Fact("Category:", category),
                                    domain!=null?new AdaptiveCards.Fact("Account:", domain):null,
                                }
                            }
                        }
                    },
                    new Column
                    {
                        Size = "auto",
                        Items = new List<CardElement>
                        {
                            new Image
                            {
                                Url = "https://raw.githubusercontent.com/GeekTrainer/help-desk-bot-lab/master/assets/botimages/head-smiling-medium.png",
                                Size = ImageSize.Small,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }
                        }
                    }
                }
            };

            var descriptionBlock = new TextBlock
            {
                Text = description,
                Wrap = true
            };

            card.Body.Add(headerBlock);
            card.Body.Add(columnsBlock);
            card.Body.Add(descriptionBlock);
            return card;
        }

        private List<CardAction> GetCardActions(string authenticationUrl, string actionType)
        {
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction()
            {
                Value = authenticationUrl,
                Type = actionType,
                Title = "Authentication Required"
            };
            cardButtons.Add(plButton);
            return cardButtons;
        }

        private AdaptiveCard CreateListCard(string[] content, string heading)
        {
            AdaptiveCard card = new AdaptiveCard();
            var headerBlock = new TextBlock()
            {
                Text = $"{heading}",
                Weight = TextWeight.Bolder,
                Size = TextSize.Medium,
                Wrap = true
                // Speak = $"<s>You've created a new Ticket #{ticketId}</s><s>We will contact you soon.</s>"
            };
            card.Body.Add(headerBlock);

            for (int i = 0; i < content.Length; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Wrap = true;
                tb.Size = TextSize.Normal;
                //tb.Color = TextColor.Attention;
                //tb.Separation = SeparationStyle.Strong;
                tb.Text = $"\t{i + 1}: " + content[i];
                card.Body.Add(tb);

            }
            return card;
        }

        private List<Attachment> CreateScrollableListAttachment(string[] content, string heading)
        {
            //return card;
            List<Attachment> returnData;
            if (content.Length < 5)
            {
                string text = "";
                for(int i=0; i<content.Length; i++)
                {
                    text += $"{i+1}. {content[i]}\n\n";
                }
                returnData = new List<Attachment>()
                {
                    new HeroCard()
                    {
                        Title = heading,
                        Subtitle = "Page 1 of 1",
                        Text = text
                    }.ToAttachment()
                };
            }
            else
            {
                int count = content.Length / 5;
                if(content.Length % 5 != 0)
                {
                    count++;
                }
                returnData = new List<Attachment>();
                for (int i=0; i < count; i++)
                {
                    string text = "";
                    for (int j=(i*5); j < content.Length && j< (i * 5 +5); j++)
                    {
                        text += $"\t{j + 1}. {content[j]}\n\n";
                    }
                    returnData.Add(new HeroCard()
                    {
                        Title = heading,
                        Subtitle = $"Page {i+1} of {count}",
                        Text = text
                    }.ToAttachment());
                }
            }
            return returnData;
        }

        [Serializable]
        private class QnAMakerResult
        {
            [JsonProperty(PropertyName = "answers")]
            public QnAMakerResultAnswer[] Answers { get; set; }
        }
        [Serializable]
        private class QnAMakerResultAnswer
        {
            [JsonProperty(PropertyName = "answer")]
            public string Answer { get; set; }

            [JsonProperty(PropertyName = "score")]
            public double Score { get; set; }

            [JsonProperty(PropertyName = "questions")]
            public string[] Questions { get; set; }
        }
    }
}