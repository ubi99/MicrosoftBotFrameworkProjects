using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace DemoHelpDeskBot.Util
{
    public class GetDetailsFromERM
    {
        //static string constr = ConfigurationManager.AppSettings["localconstr"].ToString();
        string constr = "Data Source=104.211.181.34;Initial Catalog=ERMDEMO;User ID=sa;Password=pspl@123";
        public string getTotalAndPaidDays(string empid, DateTime currentdate)
        {
            empid = empid.ToLower();
            string date = currentdate.ToString("MM-dd-yyyy");
            try
            {
                string webserviceurl = WebConfigurationManager.AppSettings["WebServiceEndpoint"] + "getTotalAndPaidDays";

                string urldata = $"/{empid}/{date}";

                var request = HttpWebRequest.Create(webserviceurl + urldata);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (responseString == "")
                {
                    return null;
                }
                return responseString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int getTotalAttendanceEntries(string empid, DateTime currentdate)
        {
            empid = empid.ToLower();
            currentdate = currentdate.AddDays(-(currentdate.Day) + 1);
            string date = currentdate.ToString("MM-dd-yyyy");
            try
            {
                string webserviceurl = WebConfigurationManager.AppSettings["WebServiceEndpoint"] + "getTotalAttendanceEntries";

                string urldata = $"/{empid}/{date}";

                var request = HttpWebRequest.Create(webserviceurl + urldata);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (responseString == "")
                {
                    return -1;
                }
                return Convert.ToInt32(responseString);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public string getMissingDates(string empid, DateTime currentdate)
        {
            empid = empid.ToLower();
            currentdate = currentdate.AddDays(-(currentdate.Day)+1);
            string date = currentdate.ToString("MM-dd-yyyy");
            try
            {
                string webserviceurl = WebConfigurationManager.AppSettings["WebServiceEndpoint"] + "getMissingDates";

                string urldata = $"/{empid}/{date}";

                var request = HttpWebRequest.Create(webserviceurl + urldata);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (responseString == "")
                {
                    return null;
                }
                return responseString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string getDeducedDates(string empid, DateTime currentdate)
        {
            currentdate.AddDays(-(currentdate.Day) + 1);
            string date = currentdate.ToString("MM-dd-yyyy");
            try
            {
                string webserviceurl = WebConfigurationManager.AppSettings["WebServiceEndpoint"] + "getDeducedDates";

                string urldata = $"/{empid}/{date}";

                var request = HttpWebRequest.Create(webserviceurl + urldata);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (responseString == "")
                {
                    return null;
                }
                return responseString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //support previous deductions
        public string getDeductions(string empid, DateTime currentdate)
        {
            empid = empid.ToLower();
            //check tommorow
            //if (currentdate.Day > 6)
            //{
            //    currentdate = currentdate.AddMonths(-1);
            //}
            //else
            //{
            //    currentdate = currentdate.AddMonths(-2);
            //}
            string date = currentdate.ToString("MM-dd-yyyy");
            
            try
            {
                string webserviceurl = WebConfigurationManager.AppSettings["WebServiceEndpoint"] + "getDeductions";

                string urldata = $"/{empid}/{date}";

                var request = HttpWebRequest.Create(webserviceurl + urldata);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (responseString == "")
                {
                    return null;
                }
                return responseString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string getLeaveInfo(string empid, DateTime currentdate)
        {
            string date = currentdate.ToString("MM-dd-yyyy");
            try
            {
                string webserviceurl = WebConfigurationManager.AppSettings["WebServiceEndpoint"] + "getLeaveInfo";
                empid = empid.ToLower();
                string urldata = $"/{empid}/{date}";

                var request = HttpWebRequest.Create(webserviceurl + urldata);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (responseString == "")
                {
                    return null;
                }
                return responseString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string getAttendanceDetails(string ecode, DateTime startdate, DateTime enddate)
        {
            ecode = ecode.ToLower();
            string startDate = startdate.ToString("MM-dd-yyyy");
            string endDate = enddate.ToString("MM-dd-yyyy");
            try
            {
                string webserviceurl = WebConfigurationManager.AppSettings["WebServiceEndpoint"] + "getAttendanceDetails";

                string urldata = $"/{ecode}/{startDate}/{endDate}";

                var request = HttpWebRequest.Create(webserviceurl + urldata);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (responseString == "")
                {
                    return null;
                }
                return responseString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}














//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Web;

//namespace DemoHelpDeskBot.Util
//{
//    public class GetDetailsFromERM
//    {
//        //static string constr = ConfigurationManager.AppSettings["localconstr"].ToString();
//        string constr = "Data Source=104.211.181.34;Initial Catalog=ERMDEMO;User ID=sa;Password=pspl@123";
//        public string getTotalAndPaidDays(string empid, DateTime currentdate)
//        {
//            string ecode = empid.Substring(1);
//            string monthyear = currentdate.ToString("MMM") + "-" + currentdate.ToString("yy");
//            try
//            {
//                SqlConnection con = new SqlConnection(constr);
//                con.Open();
//                string query = "select a = concat(NOD, '-', NDP) from Earnings where empid = '" + ecode + "' and monthtxt = '" + monthyear + "'";
//                SqlCommand cmd = new SqlCommand(query, con);
//                var temp = cmd.ExecuteScalar();
//                con.Close();
//                if (temp == null)
//                    return null;
//                else
//                    return temp.ToString();
//            }
//            catch(Exception ex)
//            {
//                return null;
//            }
//        }

//        public int getTotalAttendanceEntries(string empid, DateTime currentdate)
//        {
//            string date = currentdate.ToString("dd/MM/yyyy");
//            try
//            {
//                SqlConnection con = new SqlConnection(constr);
//                con.Open();
//                string query = "select Distinct Date from tblattendancedetails where attendanceID = (select AttendanceId from tblAttendanceMaster where FromDate='"+ date +"' and empcompempid='"+ empid +"')";
//                SqlCommand cmd = new SqlCommand(query, con);
//                int returndata = Convert.ToInt32(cmd.ExecuteScalar().ToString());
//                con.Close();
//                return returndata;
//            }
//            catch (Exception ex)
//            {
//                return -1;
//            }
//        }

//        public string getMissingDates(string empid, DateTime currentdate)
//        {
//            string date = currentdate.ToString("yyyy/MM/dd");
//            try
//            {
//                SqlConnection con = new SqlConnection(constr);
//                con.Open();
//                SqlCommand cmd = new SqlCommand("getMissingDates", con);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@date", date);
//                cmd.Parameters.AddWithValue("@empcode", empid);
//                DataTable dt = new DataTable();
//                SqlDataAdapter sda = new SqlDataAdapter(cmd);
//                sda.Fill(dt);
//                string returndata = "";
//                foreach(DataRow dr in dt.Rows)
//                {
//                    returndata += dr[0].ToString();
//                    returndata += ";";
//                }
//                con.Close();
//                return returndata;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public string getDeducedDates(string empid, DateTime currentdate)
//        {
//            string date = currentdate.ToString("dd/MM/yyyy");
//            try
//            {
//                using (SqlConnection con = new SqlConnection(constr))
//                {
//                    string query = "select Date, Leave, MgrApprovedStatus from tblattendancedetails where attendanceID = (select AttendanceId from tblAttendanceMaster where FromDate='"+ date +"' and empcompempid='"+ empid +"') and (MgrApprovedStatus='U' or Leave='Absent') order by Date";
//                    SqlCommand cmd = new SqlCommand(query, con);
//                    DataSet ds = new DataSet();
//                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
//                    sda.Fill(ds);
//                    string returndata = "";
//                    foreach (DataRow dr in ds.Tables[0].Rows)
//                    {
//                        returndata += dr[0].ToString();
//                        returndata += " : ";
//                        if(dr[2].ToString()=="U")
//                        {
//                            returndata += "Not Validated;";
//                        }
//                        else
//                        {
//                            returndata += "Absent;";
//                        }
//                    }
//                    return returndata;
//                }
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public string getDeductions(string empid, DateTime currentdate)
//        {
//            string date = currentdate.ToString("MMM") + "-" + currentdate.ToString("yy");
//            string ecode = empid.Substring(1);
//            try
//            {
//                SqlConnection con = new SqlConnection(constr);
//                con.Open();
//                SqlCommand cmd = new SqlCommand("spGetDeductions", con);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@empcode", ecode);
//                cmd.Parameters.AddWithValue("@date", date);
//                DataTable dt = new DataTable();
//                SqlDataAdapter sda = new SqlDataAdapter(cmd);
//                sda.Fill(dt);
//                string response = "";
//                if (dt.Rows.Count > 0)
//                {
//                    response += $"PF :               {dt.Rows[0][0].ToString()};";
//                    response += $"PFVOL :            {dt.Rows[0][1].ToString()};";
//                    response += $"ESI :              {dt.Rows[0][2].ToString()};";
//                    response += $"PT :               {dt.Rows[0][3].ToString()};";
//                    response += $"TDS :              {dt.Rows[0][4].ToString()};";
//                    response += $"Advance :          {dt.Rows[0][5].ToString()};";
//                    response += $"Loan :             {dt.Rows[0][6].ToString()};";
//                    response += $"Insurance :        {dt.Rows[0][7].ToString()};";
//                    response += $"Group Insurance :  {dt.Rows[0][8].ToString()};";
//                    response += $"SSS :              {dt.Rows[0][9].ToString()};";
//                    response += $"DCOMP01 :          {dt.Rows[0][10].ToString()};";
//                    response += $"DCOMP02 :          {dt.Rows[0][11].ToString()};";
//                    response += $"DCOMP03 :          {dt.Rows[0][12].ToString()};";
//                    response += $"Other Deductions : {dt.Rows[0][13].ToString()};";
//                    response += $"**Total :            {dt.Rows[0][14].ToString()}**";
//                }
//                con.Close();
//                return response;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public DataTable getLeaveInfo(string empid, DateTime currentdate)
//        {
//            string ecode = empid.Substring(1);
//            string date = currentdate.ToString("yyyy/MM/dd");
//            try
//            {
//                SqlConnection con = new SqlConnection(constr);
//                con.Open();
//                SqlCommand cmd = new SqlCommand("spGetLeaveInfo", con);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@LoginId", ecode);
//                cmd.Parameters.AddWithValue("@CurrentDate", date);
//                DataTable dt = new DataTable();
//                SqlDataAdapter sda = new SqlDataAdapter(cmd);
//                sda.Fill(dt);
//                con.Close();
//                if (dt.Rows.Count > 0)
//                    return dt;
//                else
//                    return null;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public DataTable getAttendanceDetails(string ecode, DateTime startdate, DateTime enddate)
//        {
//            string startDate = startdate.ToString("yyyy/MM/dd");
//            string endDate = enddate.ToString("yyyy/MM/dd");
//            try
//            {
//                SqlConnection con = new SqlConnection(constr);
//                con.Open();
//                SqlCommand cmd = new SqlCommand("spGetAttendanceDetails", con);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@ecode", ecode);
//                cmd.Parameters.AddWithValue("@startdate", startDate);
//                cmd.Parameters.AddWithValue("@enddate", endDate);
//                DataTable dt = new DataTable();
//                SqlDataAdapter sda = new SqlDataAdapter(cmd);
//                sda.Fill(dt);
//                con.Close();
//                if (dt.Rows.Count > 0)
//                    return dt;
//                else
//                    return null;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//    }
//}