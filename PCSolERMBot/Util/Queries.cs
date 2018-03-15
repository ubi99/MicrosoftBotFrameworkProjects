using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using System.Data.SqlClient;
using System.Data;

namespace DemoHelpDeskBot.Util
{
    [Serializable]
    public class Queries
    {
        public string constr = "Data Source=192.168.200.59;Initial Catalog=ERM;User ID=sso;Password=pspl@#123";
        SqlConnection con = null;
        public IDictionary<string,string> QueryList;
        public IDictionary<string,string> SubQueryList;
        public void setQueryList()
        {
            try
            {
                con = new SqlConnection(constr);
                con.Open();
                string query = @"SELECT [CatId], CatName + ' ('+deptName+')' [CatName] FROM [tblIssueCategory] TIC 
                            INNER JOIN tblDepartment TD ON TIC.DeptId=TD.deptID  WHERE Status='ACTIVE'
                            AND CatId IN(SELECT DISTINCT TIS.CatId FROM tblIssueSubcategory TIS 
				            INNER JOIN tblIssueTagging TIT 
                            ON TIS.SubCatId=TIT.SubCatId WHERE TIT.Status='ACTIVE') AND TIC.Status='ACTIVE'
				            ORDER BY [CatName];";
                SqlCommand cmd = new SqlCommand(query, con);
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    this.QueryList = new Dictionary<string, string>();
                    foreach(DataRow item in dt.Rows)
                    {
                        this.QueryList.Add(item[0].ToString(), item[1].ToString());
                    }
                }
                else
                {
                    this.QueryList = null;
                }
            }
            catch(Exception ex)
            {
                this.QueryList = null;
            }
            con.Close();
        }
        public void setSubQueryList(string selectedQuery)
        {
            try
            {
                con = new SqlConnection(constr);
                con.Open();
                string query = @"SELECT DISTINCT TIS.[SubCatId],TIS.[SubCatName] FROM [tblIssueSubcategory] TIS 
				INNER JOIN tblIssueTagging TIT
                ON TIS.SubCatId=TIT.SubCatId  WHERE TIS.CatId='"+ selectedQuery 
                +"' AND TIS.Status='ACTIVE' AND TIT.Status='ACTIVE' ORDER BY [SubCatName];";
                SqlCommand cmd = new SqlCommand(query, con);
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    this.SubQueryList = new Dictionary<string, string>();
                    foreach (DataRow item in dt.Rows)
                    {
                        this.SubQueryList.Add(item[0].ToString(), item[1].ToString());
                    }
                }
                else
                {
                    this.QueryList = null;
                }
            }
            catch (Exception ex)
            {
                this.SubQueryList = null;
            }
            con.Close();
        }
        public bool createTicket(string empid, string issue, string subcatid, string attachment, string attachmentname)
        {
            try
            {
                con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("PRInsertIssue", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Issue", issue);
                cmd.Parameters.AddWithValue("@CreatedBy", empid);
                cmd.Parameters.AddWithValue("@SubCatId", subcatid);
                cmd.Parameters.AddWithValue("@Attachment", attachment);
                cmd.Parameters.AddWithValue("@AttachmentName", attachmentname);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                con.Close();
                return false;
            }
            con.Close();
            return true;
        }
    };
}