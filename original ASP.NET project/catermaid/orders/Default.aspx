<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="orders_Default" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>

    
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>CaterMaid Orders</title>
    <link rel="stylesheet" type="text/css" media="screen, projection" href="../css/screen.css" />
    <script type="text/javascript" src="../js/scripts.js"></script>
    <link rel="stylesheet" type="text/css" media="all" href="../jscalendar/skins/aqua/theme.css" />
    <script type="text/javascript" src="../jscalendar/calendar.js"></script>
    <script type="text/javascript" src="../jscalendar/calendar-setup.js"></script>
    <script type="text/javascript" src="../jscalendar/lang/calendar-en.js"></script>
    
    <script type="text/javascript">
    
        function ordergrid_onDoubleClick(iRecordIndex) {
            window.navigate("Default.aspx?view=order&mode=view&oid="+ ordergrid.Rows[iRecordIndex].Cells[0].Value);
        }

    </script>
    
</head>

<body id="home">
<hr />

<div id="header">
	<div id="logo">
	    <span><img src="../images/logo-bw.gif" alt="CaterMaid" /></span>
	</div>
	
	<div id="nav">
		<ul>
			<li id="t-home"><a href="../">home</a></li>
			<li id="t-orders"><a href="../orders/" class="active">{orders}</a></li>
			<li id="t-employees"><a href="../employees/">employees</a></li>
			<li id="t-expenses"><a href="../employees/">expenses</a></li>
		</ul>
	</div>
</div>



<div id="main-body">

<div id="sidebar">
    <br />
    		
    <h3>Menu</h3>
    <dl class="teaser">
	    <dt><a href="Default.aspx?view=all"><img id="btnAllOrders" src="../images/inc.gif" style="width:25;height:25" alt="iconAllOrders" /></a></dt>
	    <dd>See all orders.</dd>
    </dl>	
    
    <dl class="teaser">
	    <dt><a href="Default.aspx?view=new"><img id="btnNewOrder" src="../images/odeo-25.gif" style="width:25;height:25" alt="iconNewOrder"/></a></dt>
	    <dd>Create a new order</dd>
    </dl>
        
    
<form runat="server" id="orderform" action="Default.aspx">

    <h3>Actions</h3>    
<%
    if( Request.QueryString["view"] == "new" ){
        Response.Write("<div id='sectionBtnSave' style='display:block'/");
    }
    else {
        Response.Write("<div id='sectionBtnSave' style='display:none'/");
    }

 %>    
 <p />
    <dl class="teaser">
	    <dt><a><asp:ImageButton id="btnSave" runat="server" ImageUrl="../images/save.gif" width="25" height="25" OnClick="btnSave_Click" /></a></dt>
	    <dd>Save this order</dd>
    </dl>
</div>
    
<%
    if (Request.QueryString["view"] == "order" && Request.QueryString["mode"] == "view")
    {
        Response.Write("<div id='sectionBtnPrint' style='display:block'/");
    }
    else {
        Response.Write("<div id='sectionBtnPrint' style='display:none'/");
    }

 %>    
 <p />
    <dl class="teaser" id="Dl1">
	    <dt><a><asp:ImageButton id="btnEdit" runat="server" imageurl="../images/edit.bmp" width="25" height="25" onclick="btnEdit_Click" CssClass="" /></a></dt>
	    <dd>Edit this order</dd>
    </dl>    
    <dl class="teaser" id="divPrintBtn">
	    <dt><a><asp:ImageButton id="btnPrint" runat="server" imageurl="../images/print.bmp" width="25" height="25" onclick="btnPrint_Click" CssClass="" /></a></dt>
	    <dd>Print this order (BEO)</dd>
    </dl>    
    <dl class="teaser" id="Dl2">
	    <dt><a><asp:ImageButton id="btnToOutlook" runat="server" imageurl="../images/outlook.bmp" width="25" height="25" onclick="btnToOutlook_Click" CssClass="" /></a></dt>
	    <dd>Go to Outlook</dd>
    </dl>    
    <dl class="teaser" id="Dl3">
	    <dt><a><asp:ImageButton id="btnToQB" runat="server" imageurl="../images/quickbooks.bmp" width="25" height="25" onclick="btnToQB_Click" CssClass="" /></a></dt>
	    <dd>Go to QuickBooks</dd>
    </dl>    
    
</div> 
	
</div><!-- end #sidebar -->


<div id="content">
<div id="title">

<%
    if( Request.QueryString["view"] == "all" ){
        Response.Write("<div id='sectionGrid' style='display:block'/");
    }
    else {
        Response.Write("<div id='sectionGrid' style='display:none'/");
    }

 %>
 <p />

<h3 id="hgrid">All Orders</h3>
 <br />
 <obout:Grid ID="ordergrid" runat="server" />
<br />
<hr />

</div> <!--end sectionGrid-->

<%
    if ((Request.QueryString["view"] == "order") && (Request.QueryString["mode"] == "view"))
    {
        Response.Write("<div id='sectionOrder' style='display:block'/");
    }
    else {
        Response.Write("<div id='sectionOrder' style='display:none'/");
    }

%>
<br />
<h3 id="H3_1">Order Details</h3>
<br />
<obout.Grid ID="gridOrder" runat="server" />
<br />

<table id="tableOrder" style="background-color:#eeeeee; cellpadding:4; border-style:solid;" >
<tbody >
    <tr style=" font-weight:bold; font-family:Tahoma; border-style:solid; border-width:thin; border-color:Black;"><td>Order Details</td></tr>
    <tr><td>Order Number:</td><td><asp:Label runat="server" ID="lblOrderOid" /></td></tr>
    <tr><td>Date and Time:</td><td><asp:Label runat="server" ID="lblOrderDateTime" /></td></tr>
    <tr><td>Customer:</td><td><asp:Label runat="server" ID="lblOrderCust" /></td></tr>
    <tr><td>Contact:</td><td><asp:Label runat="server" ID="lblOrderContact" /></td></tr>
    <tr><td>Driver:</td><td><asp:Label runat="server" ID="lblOrderDriver" /></td></tr>
    <tr><td>Order Quantity:</td><td><asp:Label runat="server" ID="lblOrderQty" /></td></tr>
    <tr><td  valign="top">Order:</td><td><asp:Label runat="server" ID="lblOrder" /></td></tr>
</tbody>
</table>
</div>

<%
    if( Request.QueryString["view"] == "new" ){
        Response.Write("<div runat='server' id='sectionForm' style='display:block'/");
    }
    else  {
        Response.Write("<div runat='server' id='sectionForm' style='display:none'/");
    }

 %>
 
 <p />
<h3 id="hform">Order Form</h3>
 <br />

<div class="recent">

<table style="width:auto"><tbody>
<tr><td>Delivery Date:</td>
    <td><asp:TextBox runat="server" ID="textDateTime" BorderStyle="None" BorderWidth="1" BackColor="white" Width="18em"/>
        <img src="../jscalendar/img.gif" id="f_trigger_c" title="Date selector" alt="calendar" /></td> 
</tr>  
<tr><td>Customer Name:</td>
    <td><asp:DropDownList id="listCust" runat="server" Width="18em"/></td>
</tr>

<tr><td>Contact:</td><td><asp:TextBox runat="server" id="textContact" /></td></tr>
<tr><td>Quantity:</td><td><asp:TextBox runat="server" id="textQty" Width="8em" /></td></tr>
<tr><td>Driver:</td>
    <td><asp:DropDownList ID="listDrivers" runat="server" Width="8em">
            <asp:ListItem>Brian</asp:ListItem>
            <asp:ListItem>Carlo</asp:ListItem>
            <asp:ListItem>John</asp:ListItem>
            <asp:ListItem>Matt</asp:ListItem>
        </asp:DropDownList></td>
</tr>

</tbody></table>


<br />
<p/>Order:
<p><asp:TextBox runat="server" id="textOrder" TextMode="MultiLine" height="12em" Columns="55" Rows="8" Wrap="true"/> </p>
<p />

<script type="text/javascript">
    Calendar.setup({
        inputField     :    "textDateTime",
        ifFormat       :    "%A, %b %e, %Y %I:%M %p",
        button         :    "f_trigger_c",
        align          :    "Br",
        singleClick    :    true,
        showsTime      :    true,
        timeFormat     :    "12",
        weekNumbers    :    true,
        cache          :    true
    });
    
</script>

<table style="width:auto;">
<tbody>
<tr align="center"><th>Chaffers</th><th>#</th><th>Utensils</th><th>#</th><th>Misc.</th><th>#</th></tr>
<tr><td>Long:</td><td><asp:TextBox runat="server" id="chaffl" width="3em"/></td>
    <td>Spoons:</td><td><asp:TextBox runat="server" id="utens" width="3em"/></td>
    <td>Flower:</td><td><asp:TextBox runat="server" id="miscf" width="3em"/></td>
</tr>
<tr><td>Round:</td><td><asp:TextBox runat="server" id="chaffr" width="3em"/></td>
    <td>Forks:</td><td><asp:TextBox runat="server" id="utenf" width="3em"/></td>
    <td>Pie Server:</td><td><asp:TextBox runat="server" id="miscps" width="3em"/></td>
</tr>
<tr><td>Oval:</td><td><asp:TextBox runat="server" id="chaffo" width="3em"/></td>
    <td>Knives:</td><td><asp:TextBox runat="server" id="utenk" width="3em"/></td>
    <td>Table Cloth:</td><td><asp:TextBox runat="server" id="misctc" width="3em"/></td>
</tr>
<tr><td /><td />
    <td>Large Plates:</td><td><asp:TextBox runat="server" id="utenlp" width="3em"/></td>
    <td>Sternos:</td><td><asp:TextBox runat="server" id="miscs" width="3em"/></td>
</tr>
<tr><td /><td />
    <td>Small Plates:</td><td><asp:TextBox runat="server" id="utensp" width="3em"/></td>
    <td>Water:</td><td><asp:TextBox runat="server" id="miscw" width="3em"/></td>
</tr>
<tr><td /><td />
    <td>Bowls:</td><td><asp:TextBox runat="server" id="utenb" width="3em"/></td>
    <td>Matches:</td><td><asp:TextBox runat="server" id="miscm" width="3em"/></td>
</tr>
<tr><td /><td />
    <td>Large Napkins:</td><td><asp:TextBox runat="server" id="utenln" width="3em"/></td>
    <td /><td />
</tr>
<tr><td /><td />
    <td>Small Napkins:</td><td><asp:TextBox runat="server" id="utensn" width="3em"/></td>
    <td /><td />
</tr>

</tbody></table>

<hr />
</div>
</div> <!--end sectionForm-->

 
</div> <!-- end #title -->

</div> <!-- end #content -->

<hr />
</form>
</div> <!-- end #main-body -->


<hr />
<hr />
</body>
</html>


