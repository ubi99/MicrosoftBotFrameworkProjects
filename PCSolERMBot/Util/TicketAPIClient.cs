namespace DemoHelpDeskBot.Util
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Xml;

    public class Ticket
    {
        public string Category { get; set; }

        public string Severity { get; set; }

        public string Description { get; set; }

        public string Domain { get; set; }

        public string PersonalInfoName { get; set; }
    }

    public class TicketAPIClient
    {
        public string PostTicket(string personalInfoName, string category, string severity, string description, string domain)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                //var path = Environment.GetEnvironmentVariable("HOME").ToString();
                string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                doc.Load(path+"/Tickets.xml");
                XmlNode rootNode = doc.SelectSingleNode("Tickets");
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "Ticket", null);
                int TicketId = 1000;
                if (rootNode.ChildNodes.Count == 0)
                {
                }
                else
                {
                    TicketId = Convert.ToInt32(rootNode.LastChild.SelectSingleNode("TicketId").InnerText.Substring(3)) + 1;
                }

                XmlNode ticketInfo = doc.CreateNode(XmlNodeType.Element, "TicketId", null);
                ticketInfo.InnerText = "TCK" + TicketId;
                newNode.AppendChild(ticketInfo);

                ticketInfo = doc.CreateNode(XmlNodeType.Element, "TicketOwner", null);
                ticketInfo.InnerText = personalInfoName;
                newNode.AppendChild(ticketInfo);

                ticketInfo = doc.CreateNode(XmlNodeType.Element, "TicketCategory", null);
                ticketInfo.InnerText = category;
                newNode.AppendChild(ticketInfo);

                ticketInfo = doc.CreateNode(XmlNodeType.Element, "TicketSeverity", null);
                ticketInfo.InnerText = severity;
                newNode.AppendChild(ticketInfo);

                ticketInfo = doc.CreateNode(XmlNodeType.Element, "TicketDescription", null);
                ticketInfo.InnerText = description;
                newNode.AppendChild(ticketInfo);

                ticketInfo = doc.CreateNode(XmlNodeType.Element, "TicketStatus", null);
                ticketInfo.InnerText = "New";
                newNode.AppendChild(ticketInfo);

                rootNode.AppendChild(newNode);
                doc.Save(path +"/Tickets.xml");

                return "TCK" + TicketId;
            }
            catch
            {
                return null;
            }
        }

        public string CheckTicketStatus(string TicketId)
        {
            string response = null;
            XmlNode match = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //var path = Environment.GetEnvironmentVariable("HOME").ToString();
                doc.Load(path + "/Tickets.xml");
                XmlNode rootNode = doc.SelectSingleNode("Tickets");
                
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.SelectSingleNode("TicketId").InnerText == TicketId)
                    {
                        match = node;
                        break;
                    }
                }
            }
            catch { }
            if(match != null)
            {
                response = match.SelectSingleNode("TicketStatus").InnerText;
            }
            return response;
        }
    }
}