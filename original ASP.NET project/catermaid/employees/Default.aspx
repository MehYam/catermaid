<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="employees_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >


<head runat="server">
    
    <title>Employees</title>

    <link rel="stylesheet" type="text/css" media="screen, projection" href="../css/screen.css" />
    <script type="text/javascript" src="../js/scripts.js"></script>
    <script type="text/javascript" src="../js/timesheet.js"></script>
    <script type="text/javascript" src="../js/employees.js"></script>
    <link rel="stylesheet" type="text/css" media="all" href="../jscalendar/skins/aqua/theme.css" />
    <script type="text/javascript" src="../jscalendar/calendar.js"></script>
    <script type="text/javascript" src="../jscalendar/calendar-setup.js"></script>
    <script type="text/javascript" src="../jscalendar/lang/calendar-en.js"></script>

<style type="text/css">
    input.am { 
        width:3em; 
        background-color:#fed; 
        border-style:solid;
        border-color:Gray;
        border-width:thin;
        height:2.2em;
        vertical-align:text-bottom;
        font: bold 7pt Tahoma;
    }
    input.pm { 
        width:3em; 
        background-color:#B4CFEC;
        border-style:solid;
        border-color:Gray;
        border-width:thin;
        height:2.2em;
        vertical-align:text-bottom;
        font: bold 7pt Tahoma;
    }
    input.am:hover input.pm:hover {
	    border-color:#fff;
	    background:#fff;
	}
    input.time { 
        width:4em;
        padding-left:0.75em;
        font-family:Courier New;
        background-color:White;
    }
    input.emptytime { 
        width:4em;
        padding-left:0.75em;
        font-family:Courier New;
        background-color:White;
    }
    input.validtime { 
        width:4em;
        padding-left:0.75em;
        font-family:Courier New;
        background-color:PaleGreen;
    }
    input.invalidtime { 
        width:4em;
        padding-left:0.75em;
        font-family:Courier New;
        background-color:Pink;
        border-color:Red;
    }
    input.incompletetime { 
        width:4em;
        padding-left:0.75em;
        font-family:Courier New;
        background-color:Yellow;
        border-color:Orange;
    }
    td.data {
        text-align:center;
        color:DarkGrey;
    }
    td.rowheader {
        font-family:Tahoma;
        font-size:10px;
        font-weight:bold;
        color:#999999;
    }
    tr.rowa {
        background-color:#E9E9E9;
    }
    tr.rowb {
        background-color:#F5F5F5;
    }
    td.footertotals {
        text-align:right;
        font-family:Tahoma;
        font-size:10px;
        font-weight:bold;
        color:#999999;
    }
    
</style>

<script type="text/javascript">


    window.onload = function()
    {            
        window.dates = new Array(d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14);
        var tins = new Array(ti1, ti2, ti3, ti4, ti5, ti6, ti7, ti8, ti9, ti10, ti11, ti12, ti13, ti14);
        var touts = new Array(to1, to2, to3, to4, to5, to6, to7, to8, to9, to10, to11, to12, to13, to14);
        var apis = new Array(api1, api2, api3, api4, api5, api6, api7, api8, api9, api10, api11, api12, api13, api14);
        var apos = new Array(apo1, apo2, apo3, apo4, apo5, apo6, apo7, apo8, apo9, apo10, apo11, apo12, apo13, apo14);
        window.tots = new Array(tot1, tot2, tot3, tot4, tot5, tot6, tot7, tot8, tot9, tot10, tot11, tot12, tot13, tot14);
        window.regs = new Array(reg1, reg2, reg3, reg4, reg5, reg6, reg7, reg8, reg9, reg10, reg11, reg12, reg13, reg14);
        window.overs = new Array(over1, over2, over3, over4, over5, over6, over7, over8, over9, over10, over11, over12, over13, over14);

        for(var i = 0; i < window.dates.length; i++){
            initInputs(window.dates[i],tins[i],apis[i],touts[i],apos[i],
                       window.tots[i],window.regs[i],window.overs[i]);      
        }

        document.forms[0].startweek.onchange = params_onchange;
        document.forms[0].ddEmployees.onchange = params_onchange;
        
        //pre-set time range to this week
        if( document.forms[0].startweek.value == "" ){
            var today = new Date();
            document.forms[0].startweek.value = dateToStringAddDays(today, 0 - today.getDay());  //get this week's Sunday
        }
        document.forms[0].startweek.fireEvent("onchange");        
               
    }        
</script>

</head>

<body id="home">

<div id="header">
	<div id="logo">
	    <span><img src="../images/logo-bw.gif" alt="CaterMaid" /></span>
	</div>
	
	<div id="nav">
		<ul>
			<li id="t-home"><a href="../">home</a></li>
			<li id="t-orders"><a href="../orders/">orders</a></li>
			<li id="t-employees"><a href="../employees/" class="active">{employees}</a></li>
			<li id="t-expenses"><a href="../expenses/">expenses</a></li>
		</ul>
	</div>
</div>


<div id="main-body">

<form runat="server" id="timesheetform">   

<div id="sidebar">

    <br />
    		
    <h3>Menu</h3>

   <h3>Actions</h3>  
    
    <dl class="teaser">
	    <dt><a><asp:ImageButton id="btnQBSync" runat="server" ImageUrl="../images/save.gif" width="25" height="25" OnClick="btnQBSync_Click" /></a></dt>
	    <dd>Synchronize with QuickBooks</dd>
    </dl>

</div><!-- end #sidebar -->

<div id="content">
<div id="title">

<h3 id="recent">Employee Time Sheet</h3>
<div class="intro">


Employee: <asp:DropDownList runat="server" id="ddEmployees" Width="12em"></asp:DropDownList>	

Week: <asp:TextBox runat="server" id="startweek" width="6em" /> to <asp:TextBox runat="server" id="endweek" width="6em" />

<!-- Week: <input type="text" id="startweek2" style="width:6em;" /> to <input type="text" id="endweek2" style="width:6em;" /> -->
        <img src="../jscalendar/img.gif" id="f_trigger_c" title="Date selector" alt="calendar" />

        
        
<script type="text/javascript">

    function dateStatusHandler(date, y, m, d) {
        var disable = true;
        if (date.getDay() != 0 )  //disable if not Sunday
            return disable;
        else {
            //check biweekly schedules starting with 4/15/2007
            var april = 3;   //months start at index 0
            var start = new Date(2007, april, 15);  
            if( date < start ){
                return disable;
            }
            else {
                var MS_PER_DAY = 1000*60*60*24;
                if (Math.round(((date-start)/MS_PER_DAY)%14) != 0){
                    return disable;
                } 
            }        
        }
    };

    Calendar.setup({
        inputField     :    "startweek",
        ifFormat       :    "%m/%d/%Y",
        button         :    "f_trigger_c",
        align          :    "Br",
        singleClick    :    true,
        showsTime      :    false,
        weekNumbers    :    true,
        cache          :    true,
        dateStatusFunc :    dateStatusHandler
    });
    
    
</script>
</form>
<br />
<hr />
<br />
<table id="timetable">
<tbody>
<!-- Header Row -->
<tr style="background-color:#F5F5F5;font-family:Tahoma;font-size:10px;font-weight:bold;color:#999999;text-align:left;border:0px;padding-left:0px;">
    <td style="border-bottom: 2px solid #CC2564;">Day</td>
    <td style="width:8em;border-bottom: 2px solid #CC2564;">Date</td>
    <td style="border-bottom: 2px solid #CC2564;">Time In</td>
    <td style="border-bottom: 2px solid #CC2564;"/>
    <td style="border-bottom: 2px solid #CC2564;">Time Out</td>
    <td style="border-bottom: 2px solid #CC2564;"/>
    <td style="border-bottom: 2px solid #CC2564;">Regular Hours</td>
    <td style="border-bottom: 2px solid #CC2564;">Overtime Hours</td>
    <td style="width:8em;border-bottom: 2px solid #CC2564;	">Total Hours</td>
</tr>

<tr class="rowa">
    <td colspan="9">&nbsp;</td>
</tr>

<!-- First week -->
<tr class="rowb">
    <td class="rowheader">Sunday</td><td id="d1" class="data"/>
    <td><input id="ti1" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api1" value="am" class="am" /> </td>
    <td/>
    <td><input id="to1" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo1" value="pm" class="pm" /> </td>
    <td /><td id="reg1" class="data"/><td id="over1" class="data"/><td id="tot1" class="data"/>
</tr>
<tr class="rowa">
    <td class="rowheader">Monday</td><td id="d2" class="data"/>
    <td><input id="ti2" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api2" value="am" class="am" /> </td>
    <td/>
    <td><input id="to2" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo2" value="pm" class="pm" /> </td>
    <td /><td id="reg2" class="data"/><td id="over2" class="data"/><td id="tot2" class="data"/>
</tr>
<tr class="rowb">
    <td class="rowheader">Tuesday</td><td id="d3" class="data"/>
    <td><input id="ti3" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api3" value="am" class="am" /> </td>
    <td/>
    <td><input id="to3" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo3" value="pm" class="pm" /> </td>
    <td /><td id="reg3" class="data"/><td id="over3" class="data"/><td id="tot3" class="data"/>
</tr>
<tr class="rowa">
    <td class="rowheader">Wednesday</td><td id="d4" class="data"/>
    <td><input id="ti4" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api4" value="am" class="am" /> </td>
    <td/>
    <td><input id="to4" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo4" value="pm" class="pm" /> </td>
    <td /><td id="reg4" class="data"/><td id="over4" class="data"/><td id="tot4" class="data"/>
</tr>
<tr class="rowb">
    <td class="rowheader">Thursday</td><td id="d5" class="data"/>
    <td><input id="ti5" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api5" value="am" class="am" /> </td>
    <td/>
    <td><input id="to5" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="apo5" value="pm" class="pm" /> </td>
    <td /><td id="reg5" class="data"/><td id="over5" class="data"/><td id="tot5" class="data"/>
</tr>
<tr class="rowa">
    <td class="rowheader">Friday</td><td id="d6" class="data"/>
    <td><input id="ti6" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api6" value="am" class="am" /> </td>
    <td/>
    <td><input id="to6" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="apo6" value="pm" class="pm" /> </td>
    <td /><td id="reg6" class="data"/><td id="over6" class="data"/><td id="tot6" class="data"/>
</tr>
<tr class="rowb">
    <td class="rowheader">Saturday</td><td id="d7" class="data"/>
    <td><input id="ti7" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api7" value="am" class="am" /> </td>
    <td/>
    <td><input id="to7" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo7" value="pm" class="pm" /> </td>
    <td /><td id="reg7" class="data"/><td id="over7" class="data"/><td id="tot7" class="data"/>
</tr>
<!-- end first week -->

<tr class="rowa">
    <td colspan="9">&nbsp;</td>
</tr>


<!-- Second week -->
<tr class="rowb">
    <td class="rowheader">Sunday</td><td id="d8" class="data"/>
    <td><input id="ti8" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api8" value="am" class="am" /> </td>
    <td/>
    <td><input id="to8" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo8" value="pm" class="pm" /> </td>
    <td /><td id="reg8" class="data"/><td id="over8" class="data"/><td id="tot8" class="data"/>
</tr>
<tr class="rowa">
    <td class="rowheader">Monday</td><td id="d9" class="data"/>
    <td><input id="ti9" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api9" value="am" class="am" /> </td>
    <td/>
    <td><input id="to9" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo9" value="pm" class="pm" /> </td>
    <td /><td id="reg9" class="data"/><td id="over9" class="data"/><td id="tot9" class="data"/>
</tr>
<tr class="rowb">
    <td class="rowheader">Tuesday</td><td id="d10" class="data"/>
    <td><input id="ti10" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api10" value="am" class="am" /> </td>
    <td/>
    <td><input id="to10" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo10" value="pm" class="pm" /> </td>
    <td /><td id="reg10" class="data"/><td id="over10" class="data"/><td id="tot10" class="data"/>
</tr>
<tr class="rowa">
    <td class="rowheader">Wednesday</td><td id="d11" class="data"/>
    <td><input id="ti11" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api11" value="am" class="am" /> </td>
    <td/>
    <td><input id="to11" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo11" value="pm" class="pm" /> </td>
    <td /><td id="reg11" class="data"/><td id="over11" class="data"/><td id="tot11" class="data"/>
</tr>
<tr class="rowb">
    <td class="rowheader">Thursday</td><td id="d12" class="data"/>
    <td><input id="ti12" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api12" value="am" class="am" /> </td>
    <td/>
    <td><input id="to12" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="apo12" value="pm" class="pm" /> </td>
    <td /><td id="reg12" class="data"/><td id="over12" class="data"/><td id="tot12" class="data"/>
</tr>
<tr class="rowa">
    <td class="rowheader">Friday</td><td id="d13" class="data"/>
    <td><input id="ti13" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api13" value="am" class="am" /> </td>
    <td/>
    <td><input id="to13" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="apo13" value="pm" class="pm" /> </td>
    <td /><td id="reg13" class="data"/><td id="over13" class="data"/><td id="tot13" class="data"/>
</tr>
<tr class="rowb">
    <td class="rowheader">Saturday</td><td id="d14" class="data"/>
    <td><input id="ti14" type="text" maxlength="5" class="time"/>&nbsp;<input type="button" id="api14" value="am" class="am" /> </td>
    <td/>
    <td><input id="to14" type="text" maxlength="5" class="time" />&nbsp;<input type="button" id="apo14" value="pm" class="pm" /> </td>
    <td /><td id="reg14" class="data"/><td id="over14" class="data"/><td id="tot14" class="data"/>
</tr>


<!-- Footer rows -->
<tr class="rowa">
    <td /><td /><td /><td />
    <td class="footertotals" >Total Hours:</td>
    <td /><td id="trhm" class="data" /><td id="tohm" class="data"/><td id="tthm" class="data" />
</tr>
<tr class="rowb">
    <td /><td /><td /><td />
    <td class="footertotals" >Rate:</td>
    <td /><td id="regrate" class="data"/><td id="overrate" class="data" /><td />
</tr>
<tr class="rowa">
    <td class="rowa" colspan="9">&nbsp;</td>
</tr>

<tr style="background-color:#F5F5F5;font-weight:bold;font-size:80%;">
    <td /><td /><td /><td />
    <td class="footertotals">Total Pay:</td>
    <td /><td id="regpay" class="data"/><td id="overpay" class="data" /><td id="totalpay" class="data" />
</tr>
<tr class="rowa">
    <td class="rowa" colspan="9">&nbsp;</td>
</tr>
<tr style="border-top:1px;color:#CC2564;background-color:#F5F5F5;">
    <td style="font-family:Tahoma;font-size:10px;color:#693;text-align:left;border-top: 1px solid #CC2564;" colspan="9">*Data is saved automatically</td>
</tr>

</tbody>
</table>

</div> <!-- end #intro -->
</div> <!-- end #title -->
</div> <!-- end #content -->

<hr />


</body>
</html>

