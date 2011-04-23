using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Xml;
using System.Windows.Forms;
using System.Collections.Generic;



public partial class employees_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Register the class containing the server-side function
        //we are interested in
        AjaxPro.Utility.RegisterTypeForAjax(typeof(employees_Default));
        
        if (!Page.IsPostBack)
        {
            using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
            {

                string query = "select eid, firstname from employees order by firstname";
                using (SqlDataAdapter cmd = new SqlDataAdapter(query, conn))
                {
                    DataSet ds = new DataSet();
                    cmd.Fill(ds, "employees");
                    ddEmployees.DataSource = ds;
                    ddEmployees.DataValueField = "eid";
                    ddEmployees.DataTextField = "firstname";
                    ddEmployees.DataBind();
                }
            }
        }
    }


    [AjaxPro.AjaxMethod()]
    public double[] getRatesFromDB(string eid)
    {
        double[] retArray = { 0, 0};
        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            conn.Open();
            string select = "select regularRate, overTimeRate from employees where eid='" + eid + "'";
            using (SqlCommand cmd = new SqlCommand(select, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retArray[0] = (double)reader[0];
                    retArray[1] = (double)reader[1];
                }
            }
        }
        return retArray;
    }

    [AjaxPro.AjaxMethod()]
    public object[] getTimeDataFromDB(int index, string eid, string date)
    {
        object[] retArray = { null, null, null };

        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            conn.Open();
            string select = "select time_in, time_out from worktime where eid='" + eid + "' and date='" + date + "'";
            using (SqlCommand cmd = new SqlCommand(select, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while( reader.Read()){
                    retArray[0] = index;
                    retArray[1] = (DateTime)reader[0];
                    retArray[2] = (DateTime)reader[1];
                }
            }
        }
        return retArray;
    }

    [AjaxPro.AjaxMethod()]
    public void setTimeDataToDB(string eid, DateTime date, DateTime tin, DateTime tout)
    {
        //INSERT or UPDATE data depending if the eid-date pair is already a row in the table

        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            conn.Open();
            string select = "select eid from worktime where eid='" + eid + "' and date='" + extractDateToSql(date) + "'";
            using (SqlCommand query = new SqlCommand(select, conn))
            {
                object exists = query.ExecuteScalar();
                if (exists != null && exists != System.DBNull.Value)
                {
                    string update = "update worktime set time_in = '" + extractTimeToSql(tin) + "', time_out = '" + extractTimeToSql(tout) + 
                        "' where eid='" + eid + "' and date='" + extractDateToSql(date) + "'";
                    using (SqlCommand nquery = new SqlCommand(update, conn))
                    {
                        nquery.ExecuteNonQuery();
                    }
                }
                else
                {
                    string insert = "insert into worktime(eid, date, time_in, time_out) values('" + eid + "','" + extractDateToSql(date) + "','" + extractTimeToSql(tin) + "','" + extractTimeToSql(tout) + "')";
                    using (SqlCommand nquery = new SqlCommand(insert, conn))
                    {
                        nquery.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    [AjaxPro.AjaxMethod()]
    public void deleteTimeDataDB(string eid, DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            conn.Open();
            string delete = "delete from worktime where eid='" + eid + "' and date='" + extractDateToSql(date) + "'";
            using (SqlCommand nquery = new SqlCommand(delete, conn))
            {
                nquery.ExecuteNonQuery();
            }
               

        }
    }

    private string extractDateToSql(DateTime dt)
    {
        //ignore the time part (by setting to 00:00) storable in the SQL smalldatetime type
        return dt.ToString("M/dd/yyyy") + " 00:00";
    }
    private string extractTimeToSql(DateTime dt)
    {
        //ignore the date part (by using the first possible date) storable in a SQL smalldatetime type
        return "1/1/1900 " + dt.ToString("H:mm");  //in 24-hour time format              
    }

    private string timespanToQBDuration(TimeSpan duration)
    {
        return ("PT" + duration.Hours + "H" + duration.Minutes + "M");
    }

    public void btnQBSync_Click(object sender, EventArgs e)
    {
        try
        {
            string employee = ddEmployees.SelectedValue;
            synchTimeSheetToQB(DateTime.Parse(startweek.Text), DateTime.Parse(endweek.Text));
            MessageBox.Show("Successfully synchronized data with QuickBooks", "catermaid");
        }
        catch(Exception excpt){
            MessageBox.Show("Error synchronizing data with QuickBooks: " + excpt.ToString());
        }
    }

    private void synchTimeSheetToQB(DateTime fromDate, DateTime toDate)
    {
        if (toDate < fromDate)
        {
            //error!
            return;
        }
        //perform this sync only on dates not yet processed by payroll.
        //simplifying assumption: only processing payroll week after biweekly period
        DateTime editDate = DateTime.Now.AddDays(-7);        
        DateTime dt = new DateTime(2007, 4, 15);  //first biweekly payroll
        for (; dt.AddDays(14) < editDate; dt = dt.AddDays(14)) ;
        //now dt pointing to the first sunday of the currently *not*-processed biweekly payroll
        if (fromDate < dt)
        {
            //tell user that we can't modify processed dates thru catermaid
            DialogResult result = MessageBox.Show("Payroll already processed for these dates.  Are you sure you want to modify this data?", "catermaid", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.No)
            {
                return;
            }

            //otherwise proceed at your own risk...
        }

        

        //query QB for all time tracking transactions within the 2-week payroll schedule       
        using (CaterMaid.QB qb = new CaterMaid.QB())
        {
            string xmlTimeQueryRq = buildQBTimeTrackingQueryRq(fromDate, toDate);
            XmlDocument xmlDocResponse = processQBRequest(qb, "TimeTrackingQueryRs", xmlTimeQueryRq);

            const int DAYS_IN_TIMESHEET = 14;
            //Hashtable caterdbTimes = new Hashtable(ddEmployees.Items.Count * DAYS_IN_TIMESHEET);
            Dictionary<Int64, TimeSpan> caterdbTimes = new Dictionary<long, TimeSpan>(ddEmployees.Items.Count * DAYS_IN_TIMESHEET);

            //next, get all caterdb worktime rows within the 2-week payroll schedule
            using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
            {
                conn.Open();

                string join = "select worktime.*, employees.qb_listid from employees, worktime where worktime.date >= '"
                    + extractDateToSql(fromDate) + "' and worktime.date <= '" + extractDateToSql(toDate)
                    + "' and worktime.eid=employees.eid";
                using (SqlCommand cmd = new SqlCommand(join, conn))
                {
                    //loop through each row that caterdb returned and match the regular and overtime hours entered
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[4] == System.DBNull.Value)  //qb_listid of employee
                        {
                            //employee not in Quickbooks... this is allowed
                            //no need to bother with this row
                            continue;
                        }

                        int eid = (int)reader[0];
                        DateTime d = (DateTime)reader[1];
                        DateTime timein = (DateTime)reader[2];
                        DateTime timeout = (DateTime)reader[3];
                        TimeSpan interval = (TimeSpan)(timeout - timein);
                        Int64 key = makeInt64Key(Convert.ToInt64(eid), d);

                        caterdbTimes[key] = interval;  
                                           
                    }
                }

                //for each employee and date in the timesheet, synch with QB
                for (int i = ddEmployees.Items.Count - 1; i >= 0; --i)
                {
                    string eid = ddEmployees.Items[i].Value;
                    string empl_listid = eidToQBListID(eid);

                    if (empl_listid == "")
                    {
                        continue;  //not a QB employee or not in caterdb
                    }

                    for (DateTime idt = fromDate; idt <= toDate; idt = idt.AddDays(1))
                    {
                        Int64 key = makeInt64Key(Convert.ToInt64(eid), idt);
                        if (caterdbTimes.ContainsKey(key))
                        {
                            TimeSpan interval = caterdbTimes[key];
                            syncDate(qb, empl_listid, idt, interval, xmlDocResponse);
                        }
                        else
                        {
                            //<eid, date> not in caterdb means zero hours for that day
                            //we need to delete transactions in QB if they're there
                            syncDate(qb, empl_listid, idt, new TimeSpan(0), xmlDocResponse);
                        }
                    }
                }
            }
        }
    }

    private Int64 makeInt64Key(Int64 eid, DateTime dt)
    {
        Int64 upper = eid * 0x100000000;
        return upper + dt.Ticks.GetHashCode();  //concatenate eid and date into a 64-bit number
    }

    private static Dictionary<int,string> m_dictEmployees = new Dictionary<int,string>();

    private static string eidToQBListID(string eid)
    {
        if (m_dictEmployees.Count == 0)
        {
            using (CaterMaid.QB qb = new CaterMaid.QB())
            {
                using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
                {
                    conn.Open();

                    string query = "select eid, qb_listid from employees";
                    using (SqlCommand cmdQ = new SqlCommand(query, conn))
                    {
                        //loop through each row that caterdb returned and match the regular and overtime hours entered
                        SqlDataReader reader = cmdQ.ExecuteReader();
                        while (reader.Read())
                        {
                            int key = (int)reader[0];
                            m_dictEmployees[key] = (reader[1] == System.DBNull.Value ? "" : (string)reader[1]);
                        }
                    }
                }
            }
        }

        int paramkey = Convert.ToInt32(eid);
        return (m_dictEmployees.ContainsKey(paramkey) ? m_dictEmployees[paramkey] : "");  
    }

    private void syncDate(CaterMaid.QB qb, string employee, DateTime date, TimeSpan duration, XmlDocument xmlDocResponse)
    {
        XmlNode nodeRegular = null, nodeOvertime = null;
        string strDate = date.ToString("yyyy-MM-dd"); 
        
        if (xmlDocResponse != null)
        {
            //for each row retrieved from caterdb, we need to add/mod QuickBooks
            //(Note: I'm making a simplifying assumption that there is only *one* regular and *one* overtime 
            //payroll item entry for each <employee, txnDate> pair, even though there can be more than one,
            //since catermaid should be the only one adding/modifying current and future entries, and we
            //don't allow catermaid to change past entries where we've already processed payroll.)
            nodeRegular = xmlDocResponse.SelectSingleNode("//TimeTrackingQueryRs/TimeTrackingRet[EntityRef/ListID = '"
                + employee + "' and TxnDate = '" + strDate + "' and PayrollItemWageRef/ListID = '" + CaterMaid.QB.ListIDHourlyWageItem + "']");
            nodeOvertime = xmlDocResponse.SelectSingleNode("//TimeTrackingQueryRs/TimeTrackingRet[EntityRef/ListID = '"
                + employee + "' and TxnDate = '" + strDate + "' and PayrollItemWageRef/ListID = '" + CaterMaid.QB.ListIDOvertimeWageItem + "']");
        }

        const int maxRegular = 8;  //anything over 8 hours is overtime
        TimeSpan intervalMaxRegular = new TimeSpan(maxRegular, 0, 0);
        TimeSpan hoursRegular = (duration.TotalHours > maxRegular ? intervalMaxRegular : duration);
        TimeSpan hoursOvertime = (duration.TotalHours > maxRegular ? duration.Subtract(intervalMaxRegular) : new TimeSpan(0));

        qbSyncTimeEntry(qb, nodeRegular, strDate, employee, hoursRegular, CaterMaid.QB.ListIDHourlyWageItem);
        qbSyncTimeEntry(qb, nodeOvertime, strDate, employee, hoursOvertime, CaterMaid.QB.ListIDOvertimeWageItem);

    }

    //add, modify or delete QB entries based on the new values and whether entries already exist or not
    private void qbSyncTimeEntry(CaterMaid.QB qb, XmlNode node, string strDate, string employee, TimeSpan duration, string payrollitem)
    {
        if (duration.Ticks > 0)
        {
            if (node == null)
            {
                //no existing entry recorded in QB, *add* overtime
                qbAddTime(qb, strDate, employee, duration, payrollitem);
            }
            else
            {
                //entry already there, *modify* if changed
                string txnID = node.SelectSingleNode("TxnID").InnerText;
                string editSeq = node.SelectSingleNode("EditSequence").InnerText;
                string oldduration = node.SelectSingleNode("Duration").InnerText;
                if (oldduration != timespanToQBDuration(duration))
                {
                    qbModifyTime(qb, txnID, editSeq, strDate, employee, duration, payrollitem);
                }
            }
        }
        else if( node != null )
        {
            //no duration, delete the existing entry
            string txnID = node.SelectSingleNode("TxnID").InnerText;
            qbDeleteTime(qb, txnID);
        }
    }

    //build the XML to pass into QB to query for time tracking data
    public string buildQBTimeTrackingQueryRq(DateTime fromDate, DateTime toDate)
    {
        XmlDocument docRq = new XmlDocument();
        XmlElement elemRqHead = initQBRequestXmlDoc(docRq, "TimeTrackingQueryRq");

        XmlElement elemDateFilter = docRq.CreateElement("TxnDateRangeFilter");
        elemRqHead.AppendChild(elemDateFilter);
        elemDateFilter.AppendChild(docRq.CreateElement("FromTxnDate")).InnerText = fromDate.ToString("yyyy-MM-dd");
        elemDateFilter.AppendChild(docRq.CreateElement("ToTxnDate")).InnerText = toDate.ToString("yyyy-MM-dd");

        return docRq.InnerXml;
    }

    private void qbAddTime(CaterMaid.QB qb, string strDate, string employee, TimeSpan duration, string payrollitem)
    {
        XmlDocument docRq = new XmlDocument();
        XmlElement elemRqHead = initQBRequestXmlDoc(docRq, "TimeTrackingAddRq");

        XmlElement elemTTAdd = docRq.CreateElement("TimeTrackingAdd");
        elemRqHead.AppendChild(elemTTAdd);
        elemTTAdd.AppendChild(docRq.CreateElement("TxnDate")).InnerText = strDate;
        XmlElement elemEntity = docRq.CreateElement("EntityRef");
        elemTTAdd.AppendChild(elemEntity);
        elemEntity.AppendChild(docRq.CreateElement("ListID")).InnerText = employee;
        elemTTAdd.AppendChild(docRq.CreateElement("Duration")).InnerText = timespanToQBDuration(duration);
        XmlElement elemPayroll = docRq.CreateElement("PayrollItemWageRef");
        elemTTAdd.AppendChild(elemPayroll);
        elemPayroll.AppendChild(docRq.CreateElement("ListID")).InnerText = payrollitem;
        elemTTAdd.AppendChild(docRq.CreateElement("BillableStatus")).InnerText = "NotBillable";

        processQBRequest(qb, "TimeTrackingAddRs", docRq.InnerXml);
    }

    private void qbModifyTime(CaterMaid.QB qb, string txnID, string editSeq, 
        string strDate, string employee, TimeSpan duration, string payrollitem)
    {
        XmlDocument docRq = new XmlDocument();
        XmlElement elemRqHead = initQBRequestXmlDoc(docRq, "TimeTrackingModRq");

        XmlElement elemTTMod = docRq.CreateElement("TimeTrackingMod");
        elemRqHead.AppendChild(elemTTMod);
        elemTTMod.AppendChild(docRq.CreateElement("TxnID")).InnerText = txnID;
        elemTTMod.AppendChild(docRq.CreateElement("EditSequence")).InnerText = editSeq;
        elemTTMod.AppendChild(docRq.CreateElement("TxnDate")).InnerText = strDate;
        XmlElement elemEntity = docRq.CreateElement("EntityRef");
        elemTTMod.AppendChild(elemEntity);
        elemEntity.AppendChild(docRq.CreateElement("ListID")).InnerText = employee;
        elemTTMod.AppendChild(docRq.CreateElement("Duration")).InnerText = timespanToQBDuration(duration);
        XmlElement elemPayroll = docRq.CreateElement("PayrollItemWageRef");
        elemTTMod.AppendChild(elemPayroll);
        elemPayroll.AppendChild(docRq.CreateElement("ListID")).InnerText = payrollitem;
        elemTTMod.AppendChild(docRq.CreateElement("BillableStatus")).InnerText = "NotBillable";

        processQBRequest(qb, "TimeTrackingModRs", docRq.InnerXml);
    }

    private void qbDeleteTime(CaterMaid.QB qb, string txnID)
    {
        XmlDocument docRq = new XmlDocument();
        XmlElement elemRqHead = initQBRequestXmlDoc(docRq, "TxnDelRq");
        elemRqHead.AppendChild(docRq.CreateElement("TxnDelType")).InnerText = "TimeTracking";
        elemRqHead.AppendChild(docRq.CreateElement("TxnID")).InnerText = txnID;

        processQBRequest(qb, "TxnDelRs", docRq.InnerXml);
    }

    private XmlElement initQBRequestXmlDoc(XmlDocument docRq, string rq)
    {
        docRq.AppendChild(docRq.CreateXmlDeclaration("1.0", null, null));
        docRq.AppendChild(docRq.CreateProcessingInstruction("qbxml", "version=\"6.0\""));
        XmlElement root = docRq.CreateElement("QBXML");
        docRq.AppendChild(root);
        XmlElement elemQBRq = docRq.CreateElement("QBXMLMsgsRq");
        elemQBRq.SetAttribute("onError", "stopOnError");
        root.AppendChild(elemQBRq);

        XmlElement elemRq = docRq.CreateElement(rq);
        elemRq.SetAttribute("requestID", "2");
        elemQBRq.AppendChild(elemRq);

        return elemRq;
    }

    private XmlDocument processQBRequest(CaterMaid.QB qb, string req, string reqxml)
    {
        string xmlResponse = qb.rp.ProcessRequest(qb.ticket, reqxml);
        XmlDocument xmldocResponse = new XmlDocument();
        xmldocResponse.LoadXml(xmlResponse);

        //check that we have an OK response
        XmlNode node = xmldocResponse.SelectSingleNode("//" + req + "/@statusCode");
        if (node.InnerText != "0" && node.InnerText != "1")
        {
            //unsuccessful add
            //TODO: throw some exception?
        }

        return xmldocResponse;
    }
}

            
           