// employees.js File

var g_TimeSheet = new TimeSheet();


//--------------------------------------------------------------------------------------------------
// initialization utilities
//--------------------------------------------------------------------------------------------------

function initInputs(date, timein, ampm_in, timeout, ampm_out, total, regular, overtime)
{
    g_TimeSheet.AddLine(date, timein, ampm_in, timeout, ampm_out, total, regular, overtime);
           
    bindEventsAmPm(ampm_in); bindEventsAmPm(ampm_out);
    bindEventsInputText(timein); bindEventsInputText(timeout);
}
function bindEventsAmPm(elem)
{
    elem.tabIndex = -1;  //don't include in tab order
    
    elem.onclick = function()
    {
        //toggle AM and PM of the button's caption
        this.TSLine.toggleAMPM(this);
//        this.value = (this.value == "am" ? "pm" : "am");
        this.TSLine.Update();
    }
}
function bindEventsInputText(elem)
{
    elem.onkeypress = timeinput_onkeypress;
    elem.onkeyup = timeinput_onkeyup;
    elem.onblur = timeinput_onblur;        
}



//--------------------------------------------------------------------------------------------------
// element event handlers
//--------------------------------------------------------------------------------------------------

function params_onchange()
{
    //re-populate the timesheet now that we have new parameters
    
    var eid = document.getElementById("ddEmployees").value;          
    var startstr = document.getElementById("startweek").value;
    if( eid.length > 0 && startstr.length > 0 ){
        var start = new Date(startstr);
        g_TimeSheet.Fill(eid, start);   
        document.forms[0].endweek.value = dateToStringAddDays(start, 13);          
    }
    
}

function timeinput_onkeypress()
{
    if( (window.event.keyCode != 58) &&   //colon :
         (window.event.keyCode < 48 || window.event.keyCode > 57) ){  //0-9
         window.event.returnValue = false;
     }
}

function timeinput_onkeyup()
{
    if( window.event.keyCode < 48 || window.event.keyCode > 90 ){
       this.TSLine.CheckValidInput(this);
       return;  //leave non-visible characters alone
    }

    //changing the contents of this input while this onkeyup event handler
    //is running might prevent the onchange event handler from being called
    //we change the contents asynchronously instead
    //window.setTimeout("timeinput_onkeyup_callback(" + this.id + ")", 1000);
    
//}
//function timeinput_onkeyup_callback(input)
//{         
    //let's be helpful to the user...

     //single 2-9 digit is not ambiguous for the hour; user must mean 2 to 9 o'clock
     //add zero before and colon after, e.g., "8" -> "08:"
     if( this.value.match( /^([2-9])$/g ) != null ){ 
        this.value = "0" + RegExp.$1 + ":";
     }
     //"01" to "09 is not ambiguous for the hour
     //add colon after, e.g., "03" -> "03:"
     else if( this.value.match( /^(0[1-9])$/g ) != null ){  
        this.value = RegExp.$1 + ":";
     }
     //except for "11 or "12" which can be ambiguous to mean one v. eleven o'clock,
     //or one v. twelve o'clock, any other first digit is not ambiguous
     //add zero and colon in the middle of the digits to separate hour and first digit of minutes
     //e.g., "23" can only mean 2 o'clock and thirty-some minutes
     else if( this.value.match(/^(?:(?:(1)([3-5]))|(?:([2-9])([0-5])))$/g) != null ){
        var hour = (RegExp.$1.length > 0 ? RegExp.$1 : RegExp.$3);
        var min = (RegExp.$2.length > 0 ? RegExp.$2 : RegExp.$4);
        this.value = "0" + hour + ":" + min; 
     }
     //three digits not starting with one is not ambiguous;
     //add leading zero and colon, e.g., "234" -> "02:34"
     else if( this.value.match( /^([^1])([0-5][0-9])$/g ) != null ){
        this.value = "0" + RegExp.$1 + ":" + RegExp.$2;
     }
     //if we end up here with four digits, it can only mean we previously had one 
     //of those ambiguous one v. eleven/twelve o'clock scenarios
     //now, however, we have all four digits, so we can add the colon
     //e.g., "1259" -> "12:59"
     else if( this.value.match( /^(?:(0[1-9]|1[0-2])([0-5][0-9]))$/g ) != null ){
        this.value = RegExp.$1 + ":" + RegExp.$2;
     }
     
    this.TSLine.CheckValidInput(this);    //ask the line to check this input
}

function timeinput_onblur()
{
    //the user is leaving the input box, fill in the leading
    //zero and/or colon to complete the input
    
    //three digits, parse minutes from right
    if( this.value.match( /^([1-9])([0-5][0-9])$/g ) != null ){
        this.value = "0" + RegExp.$1 + ":" + RegExp.$2;
    }
    //four digits
    else if( this.value.match( /^(0[1-9]|1[0-2])([0-5][0-9])$/g ) != null ){
        this.value = RegExp.$1 + ":" + RegExp.$2;
    }
    //editing caused missing leading zero, e.g., "4:55"
    else if( this.value.match( /^([1-9]:[0-5][0-9])$/g ) != null ){
        this.value = "0" + RegExp.$1;
    }
    //on the hour with colon, fill in the zeros
    else if( this.value.match( /^((?:0[1-9]|1[0-2]):)$/g ) != null ){
        this.value = RegExp.$1 + "00";
    }
    //two digits, on the hour, fill in the colon and zeros
    else if( this.value.match( /^(0[1-9]|1[0-2])$/g ) != null ){
        this.value = RegExp.$1 + ":00";
    }
    //one digit, on the hour, fill in the colon and zeros
    else if( this.value.match( /^([1-9])$/g ) != null ){
        this.value = "0" + RegExp.$1 + ":00";
    }

    this.TSLine.CheckCompleteInput(this);
    this.TSLine.Update();

}





