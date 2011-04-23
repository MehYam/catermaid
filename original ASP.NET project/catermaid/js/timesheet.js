// TimeSheetLine.js File

var MS_PER_HOUR = 3600000;  //1000 * 60 * 60;
var MS_PER_MIN = 60000;     //1000 * 60


//--------------------------------------------------------------------------------------------------
// TimeSheet implementation
//--------------------------------------------------------------------------------------------------

//constructor function 
function TimeSheet()
{
    this.Lines = new Array();
}
TimeSheet.prototype.tooltipAMPM = "Click to toggle AM/PM";
TimeSheet.prototype.tooltipEnterTime = "Please enter time";
TimeSheet.prototype.tooltipMissingTime = "Please enter time, or this line won't be saved.";
TimeSheet.prototype.tooltipValidTime = "Time OK";
TimeSheet.prototype.tooltipInvalidTime = "Invalid time.  Please fix, or this line won't be saved.";

TimeSheet.prototype.AddLine = function(date, timein, ampm_in, timeout, ampm_out, total, regular, overtime)
{
    var line = new TimeSheetLine(date, timein, ampm_in, timeout, ampm_out, total, regular, overtime);
    this.Lines.push(line);
    line.TimeSheet = this;
    
    ampm_in.title = this.tooltipAMPM;
    ampm_out.title = this.tooltipAMPM;

    
};
TimeSheet.prototype.Fill = function(eid, start)
{
    for(var i = this.LineCount()-1; i >= 0; i--){  

        this.Lines[i].Clear();          

        var datestr = dateToStringAddDays(start, i);
        window.dates[i].innerText = datestr;
        
        //ajax invocation to fill line
        employees_Default.getTimeDataFromDB(i, eid, datestr, getTimeDataFromDB_callback);          
    }
    
    this.updateTotals();
    
};
TimeSheet.prototype.LineCount = function()
{
    return this.Lines.length;
};
TimeSheet.prototype.updateTotals = function()
{
    //sum up the totals
    var strTots = "", strRegs = "", strOvers = ""; 
    for(var i = this.LineCount()-1; i >= 0; i--){
        strTots = this.addHoursMins(strTots, window.tots[i].innerText);
        strRegs = this.addHoursMins(strRegs, window.regs[i].innerText);
        strOvers = this.addHoursMins(strOvers, window.overs[i].innerText);
    }
    document.getElementById("tthm").innerText = strTots;
    document.getElementById("trhm").innerText = strRegs;
    document.getElementById("tohm").innerText = strOvers;
//    document.getElementById("tth").innerText = hmStringToHours(strTots);
//    document.getElementById("trh").innerText = hmStringToHours(strRegs);
//    document.getElementById("toh").innerText = hmStringToHours(strOvers);
    
    //ajax invocation to get rate and calculate pay
    var eid = document.getElementById("ddEmployees").value;
    employees_Default.getRatesFromDB(eid, getRatesFromDB_callback);

};
TimeSheet.prototype.DBResponseGetData = function(index, tin, tout)
{
    if( index != null ){
        this.Lines[index].time_in.value = dateToShortTimeString(tin);
        this.Lines[index].setAMPM(this.Lines[index].ampm_in, dateToAMPMString(tin));
        //this.Lines[index].ampm_in.value = dateToAMPMString(tin);
        this.Lines[index].CheckCompleteInput(this.Lines[index].time_in);
        this.Lines[index].time_out.value = dateToShortTimeString(tout);
        this.Lines[index].setAMPM(this.Lines[index].ampm_out, dateToAMPMString(tout));
//        this.Lines[index].ampm_out.value = dateToAMPMString(tout);
        this.Lines[index].CheckCompleteInput(this.Lines[index].time_out);
                
        this.Lines[index].Update(false); //don't write to db again
    }
};
TimeSheet.prototype.DBResponseGetRates = function(rateReg, rateOver)
{
    if( rateReg > 0 && rateOver > 0 ){
    
        var payReg = hmStringCalcTotal(document.getElementById("trhm").innerText, rateReg);
        var payOver = hmStringCalcTotal(document.getElementById("tohm").innerText, rateOver);
        
        document.getElementById("regrate").innerText = rateReg;
        document.getElementById("overrate").innerText = rateOver;
        document.getElementById("regpay").innerText = numToMoneyString(payReg);
        document.getElementById("overpay").innerText = numToMoneyString(payOver);
        document.getElementById("totalpay").innerText = numToMoneyString(payReg+payOver);
    }
};
TimeSheet.prototype.addHoursMins = function(strAccumulate, strAdd)
{
    if( strAccumulate.length == 0 ){
        return strAdd;
    }
    else if( strAdd.length == 0 ){
        return strAccumulate;
    }
    else {
        var re = /^(\d\d):(\d\d)$/g;
        if( strAccumulate.match(re) != null ){
            var sumH = (RegExp.$1.length > 0 ? RegExp.$1*1 : 0);
            var sumM = (RegExp.$2.length > 0 ? RegExp.$2*1 : 0);          
            if( strAdd.match(re) != null ){
                var h = (RegExp.$1.length > 0 ? RegExp.$1*1 : 0);
                var m = (RegExp.$2.length > 0 ? RegExp.$2*1 : 0);
                sumH += h;
                sumM += m;
                if( sumM > 59 ){
                    sumH += Math.floor(sumM / 60);
                    sumM = Math.round(sumM % 60);
                }
                return hmToTimeString(sumH, sumM);
            }
        }          
   }
};


//--------------------------------------------------------------------------------------------------
// TimeSheetLine implementation
//--------------------------------------------------------------------------------------------------

//constructor function
function TimeSheetLine(date, timein, ap_in, timeout, ap_out, total, regular, overtime)
{
    this.date = date; date.TSLine = this;
    this.time_in = timein; timein.TSLine = this;
    this.ampm_in = ap_in; ap_in.TSLine = this;
    this.time_out = timeout; timeout.TSLine = this;
    this.ampm_out = ap_out; ap_out.TSLine = this;
    this.total = total;
    this.regular = regular;
    this.overtime = overtime;
    this.time_in.isComplete = false;
    this.time_out.isComplete = false;
    this.timediff = 0;
}


//a time input value has changed, update the line to reflect the new value
TimeSheetLine.prototype.Update = function(bSynchDB)
{
    
    //mark one of the empty boxes yellow if it's not supposed to be empty
    if( this.completeTimeIn() && this.emptyTimeOut() ){
        this.setInputState("incompletetime", this.time_out);
    }
    else if( this.completeTimeOut() && this.emptyTimeIn() ){
        this.setInputState("incompletetime", this.time_in);    
    }
    else if( this.emptyTimeIn() && this.emptyTimeOut() ){
        this.setInputState("emptytime", this.time_in);            
        this.setInputState("emptytime", this.time_out);
    }
    
    //do line calculations if we've got both numbers
    if( this.completeTimeIn() && this.completeTimeOut() ){
        this.updateTotals();
    }  
    else {
        //otherwise, clear the totals
        this.total.innerText = "";
        this.regular.innerText = "";
        this.overtime.innerText = "";
    }

    if( bSynchDB || bSynchDB == undefined ){
        //update the database with the input values
        this.syncDB();  
    }

    //some change was made to the line, make sure to update sheet totals
    this.TimeSheet.updateTotals();                   
          
};
TimeSheetLine.prototype.CheckValidInput = function(elemInput)
{

    //Here's the tree of valid states 
    // for time input (without colons):
    //          
    //                [blank]                   (zero digits)
    //                   | 
    //      +------------+------------+
    //      |            |            |
    //      0            1          [1-9]       (one digit)
    //      |            |            |
    //    [1-9]        [0-2]        [0-5]       (two digits)      
    //      |            |            |
    //    [0-5]        [0-5]        [0-9]*      (three digits)
    //      |            |
    //    [0-9]*       [0-9]*                   (four digits)
    //
    //
    // and with colons:
    //          
    //                [blank]                   (zero digits)
    //                   | 
    //      +------------+------------+
    //      0            1          [2-9]       (one digit)
    //      |            |            |
    //      |        +-------+        |
    //    [1-9]    [0-2]    [:]      [:]        (two digits)      
    //      |        |       |        |
    //     [:]      [:]    [0-5]    [0-5]       (three digits)
    //      |        |       |        |
    //    [0-5]    [0-5]   [0-9]*   [0-9]*      (four digits)
    //      |        | 
    //    [0-9]*   [0-9]*                       (five digiits)
    //

    if( elemInput.value.length == 0 ){
        this.setInputState("emptytime", elemInput);
        return;
    }
    
     //breaking up long regexp for better readibility
     var ncolon_tree = "0|(([1-9]|0[1-9]|1[0-2])([0-5][0-9]?)?)";
     var colon_tree = "(0?[1-9]|1[0-2]):([0-5][0-9]?)?";      
     var reValidStates = new RegExp("^((" + ncolon_tree + ")|(" + colon_tree + "))?$", "g");  //could be empty
     if( elemInput.value.match(reValidStates) != null ){
        this.setInputState("validtime", elemInput);
        return true;
     }
     else {
        this.setInputState("invalidtime", elemInput);
        return false;
     }
};
TimeSheetLine.prototype.CheckCompleteInput = function(elemInput)
{
     //complete time entry
     var reComplete = /^(0[1-9]|1[0-2]):[0-5][0-9]$/g;  
     if( elemInput.value.match(reComplete) != null ){
        elemInput.isComplete = true;
        this.setInputState("validtime", elemInput);
     }       
     else {
        elemInput.isComplete = false;
        this.setInputState("invalidtime", elemInput);
     }
};
TimeSheetLine.prototype.Clear = function()
{
    this.date.innerText = "";
    this.time_in.value = "";
    this.time_out.value = "";
    this.setAMPM(this.ampm_in, "am");
    this.setAMPM(this.ampm_out, "pm");
//    this.ampm_in.value = "am";
//    this.ampm_out.value = "pm";
    this.ampm_helped = "false";
    
    this.setInputState("emptytime", this.time_in);
    this.setInputState("emptytime", this.time_out);

    this.total.innerText = "";
    this.regular.innerText = "";
    this.overtime.innerText = "";
    
};
TimeSheetLine.prototype.updateTotals = function()
{
    var diff_in_millisecs = this.getDateTimeOut().getTime() - this.getDateTimeIn().getTime();
    this.timediff = diff_in_millisecs/MS_PER_HOUR;
    this.total.innerText = timevalToTimeString(this.totalHours());
    this.regular.innerText = timevalToTimeString(this.regularHours());
    this.overtime.innerText = timevalToTimeString(this.overtimeHours());
    
    if( this.timediff < 0 ){
        alert("Time-in for " + this.date.innerText + " is AFTER time-out.  Please check AM/PM.  "); 
    }
    else if( this.timediff > 12 ){
        if( this.ampm_helped == "true" ){
            alert("Total for " + this.date.innerText + " is more than 12 hours.  Please check AM/PM.  ");
        }
        else {
            this.toggleAMPM(this.ampm_out);
//            this.ampm_out.value = (this.ampm_out.value == "am" ? "pm" : "am");
//            this.ampm_out.classname = (this.ampm_out.value == "am" ? "am" : "pm");
            this.ampm_helped = "true";
            this.updateTotals();
        }
    }    
};
TimeSheetLine.prototype.toggleAMPM = function(ampm)
{
    ampm.value = (ampm.value == "am" ? "pm" : "am");
    ampm.className = (ampm.value == "am" ? "am" : "pm");
};
TimeSheetLine.prototype.setAMPM = function(ampm, value)
{
    ampm.value = value;
    ampm.className = value;
};
TimeSheetLine.prototype.syncDB = function()
{
    var eid = document.getElementById("ddEmployees").value;
    var date = new Date(this.date.innerText);

    if( this.emptyTimeIn() && this.emptyTimeOut() ){
        //delete from DB
        employees_Default.deleteTimeDataDB(eid, date, void_callback);    
    }
    else if( this.completeTimeIn() && this.completeTimeOut() ){
        //update or insert DB
        employees_Default.setTimeDataToDB(eid, date, this.getDateTimeIn(),  this.getDateTimeOut(), void_callback);
    }
    
};
TimeSheetLine.prototype.getDateTimeOut = function()
{
    return new Date(this.date.innerText + " " + this.time_out.value + " " + this.ampm_out.value);
};
TimeSheetLine.prototype.getDateTimeIn = function()
{
    return new Date(this.date.innerText + " " + this.time_in.value + " " + this.ampm_in.value);
};
TimeSheetLine.prototype.setInputState = function(classname, input)
{
    //set style
    input.className = classname;
    
    //set tooltips
    if( classname == "incompletetime" ){
        input.title = this.TimeSheet.tooltipMissingTime;
    }
    else if( classname == "emptytime" ){
        input.title = this.TimeSheet.tooltipEnterTime;
    }
    else if( classname == "validtime" ){
        input.title = this.TimeSheet.tooltipValidTime;
    }
    else if( classname == "invalidtime" ){
        input.title = this.TimeSheet.tooltipInvalidTime;
    }   

};
TimeSheetLine.prototype.emptyTimeIn = function()
{
    return (this.time_in.value.length == 0);
};
TimeSheetLine.prototype.emptyTimeOut = function()
{
    return (this.time_out.value.length == 0);
};
TimeSheetLine.prototype.completeTimeIn = function()
{
    return this.time_in.isComplete;
};
TimeSheetLine.prototype.completeTimeOut = function()
{
    return this.time_out.isComplete;
};
TimeSheetLine.prototype.totalHours = function()
{
    return this.timediff;
};
TimeSheetLine.prototype.regularHours = function()
{
    return (this.timediff > 8 ? 8 : this.timediff);
};
TimeSheetLine.prototype.overtimeHours = function()
{
    return (this.timediff - this.regularHours());
};

//--------------------------------------------------------------------------------------
// AjaxPro callback functions
//--------------------------------------------------------------------------------------

function void_callback(response)  //Ajax callback function
{
    //nothing returned, nothing done
}
function getTimeDataFromDB_callback(response) //Ajax callback function
{
    if(response.error != null){
        //handle error?
        return;
    }
    
    g_TimeSheet.DBResponseGetData(response.value[0], response.value[1], response.value[2]);   
}
function getRatesFromDB_callback(response) //Ajax callback function
{
    if(response.error != null){
        //handle error?
        return;
    }
    
    g_TimeSheet.DBResponseGetRates(response.value[0].toFixed(2), 
                                   response.value[1].toFixed(2));
}                                   


//--------------------------------------------------------------------------------------------------
// date/time utilities
//--------------------------------------------------------------------------------------------------

//returns the date plus (or minus for negative) n days, as a string in the format mm/dd/yyyy
function dateToStringAddDays(d, ndays)
{
    var d2 = new Date(d.getFullYear(), d.getMonth(), d.getDate()+ndays);
    return (d2.getMonth()+1) + "/" + d2.getDate() + "/" + d2.getFullYear().toString();
}

function dateToShortTimeString(d)
{
    var time = d.toTimeString();  //24-hour format w/ seconds
    var hours = d.getHours()%12;
    return ( hours == 0 ? "12" : (hours < 10 ? "0" + hours : hours) ) + 
                    ":" + (d.getMinutes() < 10 ? "0" : "") + d.getMinutes();
}

function dateToAMPMString(d)
{
    return (d.getHours() < 12 ? "am" : "pm");
}

function timevalToTimeString(timeval)
{
    if( timeval < 0 ){
        return "";
    }
    var hours = Math.floor(timeval);
    var mins = Math.round((timeval - hours)*60);
    return hmToTimeString(hours, mins);
}

function hmToTimeString(h, m)
{
    if( h == 0 && m == 0 )
        return "";
        
//    return (h > 0 ? h + "h " : "") + (m > 0 ? m + "m" : "");
    return (h > 0 ? (h < 10 ? "0"+h : h) + ":" : "00:") + (m > 0 ? (m < 10 ? "0"+m : m) : "00");
}

function hmStringCalcTotal(timestr,rate)
{
    var re = /^(\d\d):(\d\d)$/g;  
    if( timestr.match(re) != null ){
        var MINS_PER_HOUR = 60;
        return 1*((1*RegExp.$1 + RegExp.$2/MINS_PER_HOUR)*rate).toFixed(2);
    }
    
    else return 0;
}
function numToMoneyString(num)
{
    if( num == 0 ){
        return "";
    }
    
    var thousands = Math.floor(num/1000);
    return "$" + (num >= 1000 ? thousands + "," + (num-(thousands*1000)).toFixed(2) : num.toFixed(2) );

}

