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
using System.ComponentModel;
using Interop.QBXMLRP2;
using System.IO;
using System.Drawing;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Interop.Excel;
using Obout.Grid;
using System.Data.OleDb;


public partial class orders_Default : System.Web.UI.Page
{
 
    protected void Page_Load(object sender, EventArgs e)
    {
        init_ordergrid();

        if (!Page.IsPostBack)
        {
            if (Request.QueryString.AllKeys.Length == 0 )
            {
                Response.Redirect("Default.aspx?view=all");
                return;
            }


            if (Session["xmlCustomers"] == null)
            {
                XmlDocument xmlQuery = new XmlDocument();
                xmlQuery.Load(Request.PhysicalApplicationPath + "customerquery.xml");

                using (CaterMaid.QB qb = new CaterMaid.QB())
                {
                    Session["xmlCustomers"] = qb.rp.ProcessRequest(qb.ticket, xmlQuery.OuterXml);
                }
            }

            string view = Request.QueryString["view"];
            if (view == "new")
            {
                #region xpath to all customer names:listIDs and populate ddlist
                //remove the attributes of the root node so as to not confuse
                //the databinding
                XmlDocument xmlResponse = new XmlDocument();
                xmlResponse.LoadXml((string)Session["xmlCustomers"]);
                XmlNode node = xmlResponse.SelectSingleNode("//CustomerQueryRs");
                node.Attributes.RemoveAll();

                StringReader sr = new StringReader(node.OuterXml);
                DataSet dsResponse = new DataSet();
                dsResponse.ReadXml(sr);

                listCust.DataSource = dsResponse;
                listCust.DataValueField = "ListID";
                listCust.DataTextField = "Name";
                listCust.DataBind();
                #endregion
            }
            else
            {
                if (view == "all")
                {
                    BindGrid();
                }
                else if (view == "order")
                {
                    using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
                    {
                        string oid = Request.QueryString["oid"];
                        if (Request.QueryString["mode"] == "view")
                        {
                            lblOrderOid.Text = oid;
                            string query = "select * from orders where oid='" + oid + "'";
                            using (SqlDataAdapter cmd = new SqlDataAdapter(query, conn))
                            {
                                DataSet ds = new DataSet();
                                cmd.Fill(ds, "orders");
                                lblOrderDateTime.Text = ds.Tables["orders"].Rows[0].ItemArray.GetValue(1).ToString();
                                lblOrderCust.Text = ds.Tables["orders"].Rows[0].ItemArray.GetValue(3).ToString();
                                lblOrderContact.Text = ds.Tables["orders"].Rows[0].ItemArray.GetValue(4).ToString();
                                lblOrderDriver.Text = ds.Tables["orders"].Rows[0].ItemArray.GetValue(6).ToString();
                                lblOrderQty.Text = ds.Tables["orders"].Rows[0].ItemArray.GetValue(5).ToString();
                                string order = ds.Tables["orders"].Rows[0].ItemArray.GetValue(7).ToString();
                                lblOrder.Text = order.Replace(Environment.NewLine, "<br/>");

                            }
                        }
                        else
                        { //mode=edit

                        }
                    }
                }
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string qbdate = reformatToQBDate(textDateTime.Text);

        #region Create xmldoc of delivery extras
        XmlDocument docExtras = new XmlDocument();
        XmlElement elemRoot = docExtras.CreateElement("DeliveryExtras");
        docExtras.AppendChild(elemRoot);
        elemRoot.AppendChild(docExtras.CreateElement("LongChaffers")).InnerText = chaffl.Text;
        elemRoot.AppendChild(docExtras.CreateElement("OvalChaffers")).InnerText = chaffo.Text;
        elemRoot.AppendChild(docExtras.CreateElement("RoundChaffers")).InnerText = chaffr.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Spoons")).InnerText = utens.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Forks")).InnerText = utenf.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Knives")).InnerText = utenk.Text;
        elemRoot.AppendChild(docExtras.CreateElement("LargePlates")).InnerText = utenlp.Text;
        elemRoot.AppendChild(docExtras.CreateElement("SmallPlates")).InnerText = utensp.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Bowls")).InnerText = utenb.Text;
        elemRoot.AppendChild(docExtras.CreateElement("LargeNapkins")).InnerText = utenln.Text;
        elemRoot.AppendChild(docExtras.CreateElement("SmallNapkins")).InnerText = utensn.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Flower")).InnerText = miscf.Text;
        elemRoot.AppendChild(docExtras.CreateElement("PieServer")).InnerText = miscps.Text;
        elemRoot.AppendChild(docExtras.CreateElement("TableCloth")).InnerText = misctc.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Sternos")).InnerText = miscs.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Water")).InnerText = miscw.Text;
        elemRoot.AppendChild(docExtras.CreateElement("Matches")).InnerText = miscm.Text;
        #endregion

        string sInsertVals = "'" + DateTime.Parse(textDateTime.Text).ToString("MM/dd/yyyy hh:mm tt") + "', '" + listCust.SelectedValue + "', '" + listCust.SelectedItem.Text + "', '" + textContact.Text + "', '";
        sInsertVals += textQty.Text + "', '" + listDrivers.Text + "', '" + textOrder.Text + "', '" + docExtras.InnerXml + "'";
        Int32 oid = -1;

        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            conn.Open();
            string insert = "insert into orders(delivery_date, qb_custid, custname, contact, qty, driver, order_text, extras) values(" + sInsertVals + ")";
            using (SqlCommand cmd = new SqlCommand(insert, conn))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("select max(oid)from orders", conn))
            {
                oid = (Int32)cmd.ExecuteScalar();
            }       

            #region Create new QuickBooks invoice

            #region Build the qbXML request
            string qbListID = listCust.SelectedValue;

            XmlDocument docRq = new XmlDocument();
            docRq.AppendChild(docRq.CreateXmlDeclaration("1.0", null, null));
            docRq.AppendChild(docRq.CreateProcessingInstruction("qbxml", "version=\"2.0\""));
            XmlElement root = docRq.CreateElement("QBXML");
            docRq.AppendChild(root);

            XmlElement elemQBRq = docRq.CreateElement("QBXMLMsgsRq");
            elemQBRq.SetAttribute("onError", "stopOnError");
            root.AppendChild(elemQBRq);
            XmlElement elemIAddRq = docRq.CreateElement("InvoiceAddRq");
            elemIAddRq.SetAttribute("requestID", "2");
            elemQBRq.AppendChild(elemIAddRq);
            XmlElement elemIAdd = docRq.CreateElement("InvoiceAdd");
            elemIAddRq.AppendChild(elemIAdd);
            
            XmlElement elemCustRef = docRq.CreateElement("CustomerRef");
            elemIAdd.AppendChild(elemCustRef);
            XmlElement elemListId = docRq.CreateElement("ListID");
            elemCustRef.AppendChild(elemListId).InnerText = qbListID;
            XmlElement elemTxnD = docRq.CreateElement("TxnDate");
            elemIAdd.AppendChild(elemTxnD).InnerText = qbdate;
            XmlElement elemShipAddr = docRq.CreateElement("ShipAddress");
            elemIAdd.AppendChild(elemShipAddr);
            elemShipAddr.AppendChild(docRq.CreateElement("Addr1")).InnerText = textContact.Text;
            elemIAdd.AppendChild(docRq.CreateElement("IsPending")).InnerText = "1";
            elemIAdd.AppendChild(docRq.CreateElement("PONumber")).InnerText = oid.ToString();
            elemIAdd.AppendChild(docRq.CreateElement("IsToBePrinted")).InnerText = "1";
            XmlElement elemLine = docRq.CreateElement("InvoiceLineAdd");
            elemIAdd.AppendChild(elemLine);
            elemLine.AppendChild(docRq.CreateElement("Desc")).InnerText = textOrder.Text;
            elemLine.AppendChild(docRq.CreateElement("Quantity")).InnerText = textQty.Text;
            #endregion

            using (CaterMaid.QB qb = new CaterMaid.QB())
            {
                string sxmlInvoiceAddRs = qb.rp.ProcessRequest(qb.ticket, docRq.InnerXml);
                //todo: return result of invoice add to view order page
            }

            #endregion

            string eid = createOutlookEntry(DateTime.Parse(textDateTime.Text), 
                                            listCust.SelectedItem.Text, textOrder.Text);

            string update = "update orders set outlook_eid = '" + eid + "' where oid = '" + oid + "'";
            using (SqlCommand cmd = new SqlCommand(update, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
 
        Response.Redirect("Default.aspx?view=order&oid=" + oid + "&mode=view");
    }


    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string oid = Request.QueryString["oid"];
        DataSet ds = new DataSet();
        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            string query = "select * from orders where oid='" + oid + "'";
            using (SqlDataAdapter cmd = new SqlDataAdapter(query, conn))
            {
                cmd.Fill(ds, "orders");
            }
        }
        string custname = ds.Tables["orders"].Rows[0].ItemArray.GetValue(3).ToString();
        string contact = ds.Tables["orders"].Rows[0].ItemArray.GetValue(4).ToString();
        string driver = ds.Tables["orders"].Rows[0].ItemArray.GetValue(6).ToString();
        string qty = ds.Tables["orders"].Rows[0].ItemArray.GetValue(5).ToString();
        string ordertext = ds.Tables["orders"].Rows[0].ItemArray.GetValue(7).ToString();
        DateTime dt = (DateTime)ds.Tables["orders"].Rows[0].ItemArray.GetValue(1);
        string qbListID = ds.Tables["orders"].Rows[0].ItemArray.GetValue(2).ToString();
        string sExtras = ds.Tables["orders"].Rows[0].ItemArray.GetValue(8).ToString();
        XmlDocument docExtras = new XmlDocument();
        docExtras.LoadXml(sExtras);
        XmlNode nlc = docExtras.SelectSingleNode("//LongChaffers");
        XmlNode noc = docExtras.SelectSingleNode("//OvalChaffers");
        XmlNode nrc = docExtras.SelectSingleNode("//RoundChaffers");
        XmlNode nus = docExtras.SelectSingleNode("//Spoons");
        XmlNode nuf = docExtras.SelectSingleNode("//Forks");
        XmlNode nuk = docExtras.SelectSingleNode("//Knives");
        XmlNode nulp = docExtras.SelectSingleNode("//LargePlates");
        XmlNode nusp = docExtras.SelectSingleNode("//SmallPlates");
        XmlNode nub = docExtras.SelectSingleNode("//Bowls");
        XmlNode nuln = docExtras.SelectSingleNode("//LargeNapkins");
        XmlNode nusn = docExtras.SelectSingleNode("//SmallNapkins");
        XmlNode nmf = docExtras.SelectSingleNode("//Flower");
        XmlNode nmps = docExtras.SelectSingleNode("//PieServer");
        XmlNode nmtc = docExtras.SelectSingleNode("//TableCloth");
        XmlNode nms = docExtras.SelectSingleNode("//Sternos");
        XmlNode nmw = docExtras.SelectSingleNode("//Water");
        XmlNode nmm = docExtras.SelectSingleNode("//Matches");

        XmlDocument xmlCustomers = new XmlDocument();
        xmlCustomers.LoadXml((string)Session["xmlCustomers"]);
        XmlNode nodeAddr = xmlCustomers.SelectSingleNode("//CustomerRet[ListID='" + qbListID.Trim() + "']/BillAddress");
        XmlNode a1 = nodeAddr.SelectSingleNode("Addr1");
        XmlNode a2 = nodeAddr.SelectSingleNode("Addr2");
        XmlNode a3 = nodeAddr.SelectSingleNode("Addr3");
        XmlNode city = nodeAddr.SelectSingleNode("City");
        XmlNode state = nodeAddr.SelectSingleNode("State");
        XmlNode zip = nodeAddr.SelectSingleNode("PostalCode");
        
        Microsoft.Office.Interop.Excel.Application xl = new Microsoft.Office.Interop.Excel.Application();
        Workbook wb = xl.Workbooks.Open(Request.PhysicalApplicationPath + "beo_template.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        
        string filepath = Request.PhysicalApplicationPath + "beos\\beo_" + oid + ".xlsx";
        if (File.Exists(filepath))
        {
            File.Move(filepath, Request.PhysicalApplicationPath + "beos\\beo_" + oid + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
            File.Delete(filepath);
        }
        wb.SaveAs(filepath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        #region Fill Excel Ranges with order data
        xl.get_Range("date", Type.Missing).Value2 = dt.ToString("MM/dd/yyyy");
        xl.get_Range("time", Type.Missing).Value2 = dt.ToString("HH:mm tt");
        xl.get_Range("companyName", Type.Missing).Value2 = custname;
        xl.get_Range("contact", Type.Missing).Value2 = contact;
        xl.get_Range("driver", Type.Missing).Value2 = driver;
        xl.get_Range("quantity", Type.Missing).Value2 = qty;
        xl.get_Range("orderNum", Type.Missing).Value2 = oid;
        if( a1 != null )
            xl.get_Range("Addr1", Type.Missing).Value2 = a1.InnerText;
        if( a2 != null )
            xl.get_Range("Addr2", Type.Missing).Value2 = a2.InnerText;
        if( a3 != null )
            xl.get_Range("Addr3", Type.Missing).Value2 = a3.InnerText;
        string citystatezip = "";
        if (city != null)
            citystatezip = city.InnerText.Trim();
        if( state != null )
            citystatezip += ", " + state.InnerText.Trim();
        if (zip != null)
            citystatezip += " " + zip.InnerText;
        xl.get_Range("Addr4", Type.Missing).Value2 = citystatezip;
        
        Range rngorder = xl.get_Range("orderText", Type.Missing);
        string[] lines = System.Text.RegularExpressions.Regex.Split(ordertext, Environment.NewLine);
        int nLinesToMove = (lines.Length > rngorder.Count ? rngorder.Count : lines.Length);
        for (int i = 1; i <= nLinesToMove; ++i)
        {
            Range rngcell = (Range)rngorder.Cells[i, 1];
            rngcell.Value2 = lines[i - 1];
        }

        //extras section
        xl.get_Range("nchaffl", Type.Missing).Value2 = nlc.InnerText;
        xl.get_Range("nchaffo", Type.Missing).Value2 = noc.InnerText;
        xl.get_Range("nchaffr", Type.Missing).Value2 = nrc.InnerText;
        xl.get_Range("nutens", Type.Missing).Value2 = nus.InnerText;
        xl.get_Range("nutenf", Type.Missing).Value2 = nuf.InnerText;
        xl.get_Range("nutenk", Type.Missing).Value2 = nuk.InnerText;
        xl.get_Range("nutenlp", Type.Missing).Value2 = nulp.InnerText;
        xl.get_Range("nutensp", Type.Missing).Value2 = nusp.InnerText;
        xl.get_Range("nutenb", Type.Missing).Value2 = nub.InnerText;
        xl.get_Range("nutenln", Type.Missing).Value2 = nuln.InnerText;
        xl.get_Range("nutensn", Type.Missing).Value2 = nusn.InnerText;
        xl.get_Range("nmiscf", Type.Missing).Value2 = nmf.InnerText;
        xl.get_Range("nmiscps", Type.Missing).Value2 = nmps.InnerText;
        xl.get_Range("nmisctc", Type.Missing).Value2 = nmtc.InnerText;
        xl.get_Range("nmiscs", Type.Missing).Value2 = nms.InnerText;
        xl.get_Range("nmiscw", Type.Missing).Value2 = nmw.InnerText;
        xl.get_Range("nmiscm", Type.Missing).Value2 = nmm.InnerText;


        #endregion

        wb.Save();
        xl.Visible = true;
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
    }

    protected void btnToOutlook_Click(object sender, EventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            conn.Open();
            string select = "select outlook_eid from orders where oid = '" + Request.QueryString["oid"] + "'";
            using (SqlCommand cmd = new SqlCommand(select, conn))
            {
                object eid = cmd.ExecuteScalar();
                if (eid == null)
                {
                    MessageBox.Show("SQL select failed: " + select);
                }
                else if (eid == System.DBNull.Value)
                {
                    string msg = "Can't find associated Outlook calendar entry. Recreate?";
                    DialogResult ret = MessageBox.Show(msg, "CaterMaid Error", MessageBoxButtons.OKCancel);
                    if (ret == DialogResult.OK)
                    {
                        //create outlook
                    }
                }
                else
                {
                    Microsoft.Office.Interop.Outlook.Application ol = new Microsoft.Office.Interop.Outlook.Application();
                    NameSpace namesp = ol.GetNamespace("MAPI");
                    string storeid = namesp.GetDefaultFolder(OlDefaultFolders.olFolderCalendar).StoreID;
                    AppointmentItem appt = (AppointmentItem)namesp.GetItemFromID((string)eid, storeid);
                    appt.Display(false);
                }
            }
        }
    }
    private string createOutlookEntry(DateTime dt, string coname, string order)
    {
        Microsoft.Office.Interop.Outlook.Application ol = new Microsoft.Office.Interop.Outlook.Application();
        AppointmentItem appt = (AppointmentItem)ol.CreateItem(OlItemType.olAppointmentItem);
        appt.Start = dt;
        appt.Subject = "Delivery: " + coname;
        appt.Body = order;
        appt.Categories = "Business";
        appt.Save();
        return appt.EntryID;
    }

    protected void btnToQB_Click(object sender, EventArgs e)
    {
    }

    private string reformatToQBDate(string datetime)
    {
        DateTime dt = DateTime.Parse(datetime);
        return dt.ToString("yyyy-MM-dd");
    }


    private void init_ordergrid()
    {
        ordergrid.CallbackMode = true;
        ordergrid.Serialize = true;
        ordergrid.AutoGenerateColumns = false;
        ordergrid.FolderStyle = "styles/style_2";
        ordergrid.AllowAddingRecords = false;
        ordergrid.AllowRecordSelection = true;
        ordergrid.AllowSorting = true;
        ordergrid.PageSizeOptions = "1,10,15,20,25,30,50,100";

        // setting the event handlers
//        ordergrid.DeleteCommand += new Obout.Grid.Grid.EventHandler(DeleteRecord);
        ordergrid.Rebind += new Obout.Grid.Grid.DefaultEventHandler(RebindGrid);

        // creating the columns
        Obout.Grid.Column oCol1 = new Obout.Grid.Column();
        oCol1.DataField = "oid";
        oCol1.ReadOnly = true;
        oCol1.HeaderText = "Order";
        oCol1.Width = "70";

        Obout.Grid.Column oCol2 = new Obout.Grid.Column();
        oCol2.DataField = "delivery_date";
        oCol2.HeaderText = "Date";
        oCol2.Width = "200";

        Obout.Grid.Column oCol3 = new Obout.Grid.Column();
        oCol3.DataField = "custname";
        oCol3.HeaderText = "Customer";
        oCol3.Width = "320";

        //Obout.Grid.Column oCol4 = new Obout.Grid.Column();
        //oCol4.Width = "70";
        //oCol4.AllowDelete = true;

        // add the columns to the Columns collection of the grid
        ordergrid.Columns.Add(oCol1);
        ordergrid.Columns.Add(oCol2);
        ordergrid.Columns.Add(oCol3);
        //ordergrid.Columns.Add(oCol4);

        //add Double click event
        ordergrid.ClientSideEvents.OnClientDblClick = "ordergrid_onDoubleClick";

    }

    void BindGrid()
    {
        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            string query = "select oid, delivery_date, custname from orders";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                SqlDataReader myReader = cmd.ExecuteReader();
                ordergrid.DataSource = myReader;
                ordergrid.DataBind();
                //conn.Close & conn.Dispose called automatically by using statement
            }
        }
    }
    void RebindGrid(object sender, EventArgs e)
    {
        BindGrid();
    }
    void DeleteRecord(object sender, GridRecordEventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(CaterMaid.connection.info))
        {
            string query = "delete from orders where oid = @oid";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.Add("@oid", SqlDbType.Int).Value = e.Record["oid"];
                cmd.ExecuteNonQuery();
            }
        }
    }

}
