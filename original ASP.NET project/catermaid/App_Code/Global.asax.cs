using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Interop.QBXMLRP2;
using System.Collections.Generic;


namespace CaterMaid
{

    public class Global : HttpApplication
    {
        //session events here
    }

    public class QB : IDisposable  //RAII class
    {
        public RequestProcessor2 rp = null;
        public string ticket = "";

        public QB()  //constructor
        {
            rp = new RequestProcessor2();
            rp.OpenConnection("", "CaterMaid");
            string qbfile = ConfigurationManager.AppSettings["fileQB"];
            ticket = rp.BeginSession(qbfile, QBFileMode.qbFileOpenDoNotCare);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (rp != null)
                {
                    rp.EndSession(ticket);
                    rp.CloseConnection();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public const string ListIDHourlyWageItem = "80000011-1177810597";  
        public const string ListIDOvertimeWageItem = "80000012-1177810597"; 
    }

    public class connection
    {
        public static string info
        {
            get
            {
                if (System.Environment.MachineName.ToLower() == "ktsupc")
                {
                    return ConfigurationManager.AppSettings["connktsupc"];
                }
                else
                {
                    return ConfigurationManager.AppSettings["conncaterserv"];
                }
            }
        }
    }

}