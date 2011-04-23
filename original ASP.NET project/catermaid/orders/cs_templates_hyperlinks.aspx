<%@ Page Language="C#"%>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.OleDb" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<script language="C#" runat="server">
    Grid grid1 = new Grid();
    
	void Page_load(object sender, EventArgs e)		
	{
        grid1.ID = "grid1";
        grid1.CallbackMode = true;
        grid1.Serialize = true;
        grid1.AutoGenerateColumns = false;
        
        grid1.FolderStyle = "styles/style_7";
        grid1.AllowAddingRecords = false;
        grid1.AllowRecordSelection = false;
        grid1.AllowMultiRecordSelection = false;
        
      
        // creating the Templates
		
		// creating the template for the Company name column (view mode)
		//------------------------------------------------------------------------
		GridRuntimeTemplate TemplateCompanyName = new GridRuntimeTemplate();
		TemplateCompanyName.ID = "TemplateCompanyName";				 
		TemplateCompanyName.Template = new Obout.Grid.RuntimeTemplate();
		TemplateCompanyName.Template.CreateTemplate += new Obout.Grid.GridRuntimeTemplateEventHandler(CreateCompanyTemplate);
		//------------------------------------------------------------------------

        grid1.Templates.Add(TemplateCompanyName);
       
        // creating the columns
        Column oCol1 = new Column();
        oCol1.DataField = "SupplierID";
        oCol1.ReadOnly = true;
        oCol1.HeaderText = "ID";
        oCol1.Width = "60";
        oCol1.Visible = false;

        Column oCol2 = new Column();
        oCol2.DataField = "CompanyName";
        oCol2.HeaderText = "Company Name";
        oCol2.Width = "350";
        oCol2.TemplateSettings.TemplateId = "TemplateCompanyName";        

        Column oCol3 = new Column();
        oCol3.DataField = "Address";
        oCol3.HeaderText = "Address";
        oCol3.Width = "175";        

        Column oCol4 = new Column();
        oCol4.DataField = "Country";
        oCol4.HeaderText = "Country";
        oCol4.Width = "115";        
        
        // add the columns to the Columns collection of the grid
        grid1.Columns.Add(oCol1);
        grid1.Columns.Add(oCol2);
        grid1.Columns.Add(oCol3);
        grid1.Columns.Add(oCol4);
        
        // add the grid to the controls collection of the PlaceHolder
        phGrid1.Controls.Add(grid1);
        
		if (!Page.IsPostBack)
		{
			CreateGrid();			
		}
	}
	
    // Create the methods that will load the data into the templates
	//------------------------------------------------------------------------
	public void CreateCompanyTemplate(Object sender, Obout.Grid.GridRuntimeTemplateEventArgs e)
	{
		Literal oLiteral = new Literal();	
		e.Container.Controls.Add(oLiteral);		
		oLiteral.DataBinding += new EventHandler(DataBindCompanyTemplate);
	}
	protected void DataBindCompanyTemplate(Object sender, EventArgs e)
	{
		Literal oLiteral = sender as Literal;
		Obout.Grid.TemplateContainer oContainer = oLiteral.NamingContainer as Obout.Grid.TemplateContainer;

        oLiteral.Text = "<a href=\"http://www.test.com/test.aspx?id=" + oContainer.DataItem["SupplierID"] + "\">" + oContainer.DataItem["CompanyName"] + "</a>";
	}
	//------------------------------------------------------------------------

    
	void CreateGrid()
	{
		OleDbConnection myConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath("../App_Data/Northwind.mdb"));

		OleDbCommand myComm = new OleDbCommand("SELECT * FROM Suppliers", myConn);
		myConn.Open();		
		OleDbDataReader myReader = myComm.ExecuteReader();

		grid1.DataSource = myReader;
		grid1.DataBind();

		myConn.Close();	
	}	
		
	
	</script>		

<html>
	<head>
		<title>obout ASP.NET Grid examples</title>
		<style type="text/css">
			.tdText {
				font:11px Verdana;
				color:#333333;
			}
			.option2{
				font:11px Verdana;
				color:#0033cc;
				background-color___:#f6f9fc;
				padding-left:4px;
				padding-right:4px;
			}
			a {
				font:11px Verdana;
				color:#315686;
				text-decoration:underline;
			}

			a:hover {
				color:crimson;
			}
		</style>		
	</head>
	<body>	
		<form runat="server">
					
		<br />
		<span class="tdText"><b>ASP.NET Grid - Hyperlinks inside the Grid</b></span>
		<br /><br />			
		
        <asp:PlaceHolder ID="phGrid1" runat="server"></asp:PlaceHolder>

		<br /><br /><br />
		
		<span class="tdText">
		    You can use Templates to insert hyperlinks in the cells of the Grid.<br /><br />
		    See the <a href="http://www.obout.com/grid/grid_tutorials_templates.aspx">Templates tutorial</a>
		</span>


		<br /><br /><br />
		
		<a href="Default.aspx?type=ASPNET">« Back to examples</a>
		
		</form>
	</body>
</html>