using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DemoHelpDeskBot
{
    public class DebugActivityLogger : IActivityLogger
    {
        public async Task LogAsync(IActivity activity)
        {
            Debug.WriteLine($"From:{activity.From.Id} - To:{activity.Recipient.Id} - Message:{activity.AsMessageActivity()?.Text}");
            try
            {
                //string dir = Directory.GetCurrentDirectory();
                string dir = System.AppDomain.CurrentDomain.BaseDirectory;
                XmlDocument doc = new XmlDocument();
                doc.Load(dir + "/Conversations.xml");
                XmlNode root = doc.SelectSingleNode("Conversations");
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "Message", null);
                XmlNode textNode = doc.CreateNode(XmlNodeType.Element, "Text", null);
                textNode.InnerText = activity.AsMessageActivity()?.Text;
                XmlNode fromNode = doc.CreateNode(XmlNodeType.Element, "From", null);
                XmlNode fromIdNode = doc.CreateNode(XmlNodeType.Element, "ID", null);
                fromIdNode.InnerText = activity.From.Id.ToString();
                XmlNode fromNameNode = doc.CreateNode(XmlNodeType.Element, "Name", null);
                fromNameNode.InnerText = activity.From.Name.ToString();
                fromNode.AppendChild(fromIdNode);
                fromNode.AppendChild(fromNameNode);
                XmlNode toNode = doc.CreateNode(XmlNodeType.Element, "To", null);
                XmlNode toIdNode = doc.CreateNode(XmlNodeType.Element, "ID", null);
                toIdNode.InnerText = activity.Recipient.Id.ToString();
                XmlNode toNameNode = doc.CreateNode(XmlNodeType.Element, "Name", null);
                toNameNode.InnerText = activity.Recipient.Name.ToString();
                toNode.AppendChild(toIdNode);
                toNode.AppendChild(toNameNode);
                XmlNode timestampNode = doc.CreateNode(XmlNodeType.Element, "TimeStamp", null);
                timestampNode.InnerText = DateTime.Now.ToString();
                XmlNode convIdNode = doc.CreateNode(XmlNodeType.Element, "ConversationId", null);
                convIdNode.InnerText = activity.Conversation.Id.ToString();
                XmlNode serviceUrlNode = doc.CreateNode(XmlNodeType.Element, "ServiceUrl", null);
                serviceUrlNode.InnerText = activity.ServiceUrl.ToString();
                newNode.AppendChild(timestampNode);
                newNode.AppendChild(convIdNode);
                newNode.AppendChild(fromNode);
                newNode.AppendChild(toNode);
                newNode.AppendChild(textNode);
                newNode.AppendChild(serviceUrlNode);
                root.AppendChild(newNode);
                doc.Save(dir + "/Conversations.xml");
            }
            catch (Exception e)
            {

            }
        }
    }
}