//code for DatePicker used in some of the reports.
		var weekend = [0,6];
		var weekendColor = "#e0e0e0";
		var fontface = "Verdana, Courier New, Times New Roman, Arial";
		var fontsize = '1';
//		var gNow = new Date();
		var ggWinCal;
		var WinState;
		isNav = (navigator.appName.indexOf("Netscape") != -1) ? true : false;
		isIE = (navigator.appName.indexOf("Microsoft") != -1) ? true : false;

		Calendar.Months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

		// Non-Leap year Month days..
		Calendar.DOMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
		// Leap year Month days..
		Calendar.lDOMonth = [31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

		function show_calendar(str, strRetFn) {
		
		// New Code to disable date text entry
			if (str.disabled == true)
				return;
		//up to here
			if (arguments[3] == null)
				p_format = "MM/DD/YYYY";   //=g_sDateFormat            //"MM/DD/YYYY" DD-MON-YYYY;
			else
				p_format = arguments[3];												
			
			if (p_format=="DD/MM/YYYY")
				checkdateformat='ddDate';
			if (p_format=="MM/DD/YYYY")
				checkdateformat='mmDate';							
			
			p_item = str;
			//return function
			p_retFn = strRetFn;

			var val = document.getElementById(str).value.replace(/^\s+|\s+$/g, '');
			if (val.length > 1  &&  CheckDate(document.getElementById(str),checkdateformat))
			{	
					splitdt=(document.getElementById(str).value).split('/');
					
					//Since "new Date()" syntax only takes date in format "MM/DD/YYYY", converting normal date format to that.					
					if (checkdateformat=='ddDate')					
						gNow = new Date(splitdt[1]+'/'+splitdt[0]+'/'+splitdt[2]); //bring month in beginning
						
					if (checkdateformat=='mmDate')					
						gNow = new Date(splitdt[0]+'/'+splitdt[1]+'/'+splitdt[2]);						
			}	
			else
			{
				gNow = new Date();
			}	
			
			p_month = new String(gNow.getMonth());
			p_year = new String(gNow.getFullYear().toString());
			
			/*
			if (arguments[1] == null)
				p_month = new String(gNow.getMonth());
			else
				p_month = arguments[1];
			
			if (arguments[2] == "" || arguments[2] == null)
				p_year = new String(gNow.getFullYear().toString());
			else
				p_year = arguments[2];
			*/
/*-----------------------------------------------------------*/
	CalHeight=190;
	CalWidth=166;
	var objtxt = document.getElementById(str)
	DateWindowTop = parseFloat(objtxt.getBoundingClientRect().top) + window.screenTop+20; 
	DateWindowLeft = parseFloat(objtxt.getBoundingClientRect().left) + window.screenLeft; 									
			
	//If not enough space to show calendar below, show above the text box
	if ( (screen.availHeight-DateWindowTop) < (CalHeight-5) )
		DateWindowTop=DateWindowTop - (CalHeight+25);
	
	if ( (screen.availWidth-DateWindowLeft) < (CalWidth+10) )
		DateWindowLeft=parseFloat(objtxt.getBoundingClientRect().right)- CalWidth-20;	
	
	//Ending of getting co-ordinate									
	var winPos="left="+DateWindowLeft+",top="+DateWindowTop+"";
	//var winPos="left=420,top=250";
/*-----------------------------------------------------------*/
	var valTop;
	var strname= str.name;
	vWinCal = window.open("", "Calendar",winPos +",width=190,height=166,address=no,status=no,resizable=no");
	//vWinCal = window.showModalDialog("", "Calendar","dialogHeight:400px;status:0;help:0");
	vWinCal.focus();
	
			vWinCal.opener = self;
			ggWinCal = vWinCal;
			
			Build(p_item, p_month, p_year, p_format, p_retFn);
		}
		function Calendar(p_item, p_WinCal, p_month, p_year, p_format, p_retFn) {
			if ((p_month == null) && (p_year == null)) 
			return;
			if (p_WinCal == null)
				this.gWinCal = ggWinCal;
			else
				this.gWinCal = p_WinCal;
			if (p_month == null) {
				this.gMonthName = null;
				this.gMonth = null;
				this.gYearly = true;
			} else {
				this.gMonthName = Calendar.get_month(p_month);
				this.gMonth = new Number(p_month);
				this.gYearly = false;
			}
				this.gYear = p_year;
				this.gFormat = p_format;
				this.gBGColor = "white";
				this.gFGColor = "black";
				this.gTextColor = "black";
				this.gReturnItem = p_item;
				this.gReturnFn = p_retFn;
			}
			
		Calendar.get_month = Calendar_get_month;
		Calendar.get_daysofmonth = Calendar_get_daysofmonth;
		Calendar.calc_month_year = Calendar_calc_month_year;

		function Calendar_get_month(monthNo) {
			return Calendar.Months[monthNo];
		}

		function Calendar_get_daysofmonth(monthNo, p_year) {
		if ((p_year % 4) == 0) {
			if ((p_year % 100) == 0 && (p_year % 400) != 0)
				return Calendar.DOMonth[monthNo];
				return Calendar.lDOMonth[monthNo];
			} else
				return Calendar.DOMonth[monthNo];
			}

		function Calendar_calc_month_year(p_Month, p_Year, incr) {
			var ret_arr = new Array();
			if (incr == -1) {
			// B A C K W A R D
			if (p_Month == 0) {
				ret_arr[0] = 11;
				ret_arr[1] = parseInt(p_Year) - 1;
			}
			else {
				ret_arr[0] = parseInt(p_Month) - 1;
				ret_arr[1] = parseInt(p_Year);
			}
			} else if (incr == 1) {
			// F O R W A R D
		    if (p_Month == 11) {
				ret_arr[0] = 0;
				ret_arr[1] = parseInt(p_Year) + 1;
		    }
		    else {
				ret_arr[0] = parseInt(p_Month) + 1;
		        ret_arr[1] = parseInt(p_Year);
		    }
		}  
			return ret_arr;
		}


		function Calendar_calc_month_year(p_Month, p_Year, incr) {
			var ret_arr = new Array();
			if (incr == -1) {
			// B A C K W A R D
			if (p_Month == 0) {
				ret_arr[0] = 11;
				ret_arr[1] = parseInt(p_Year) - 1;
			}
			else {
				ret_arr[0] = parseInt(p_Month) - 1;
				ret_arr[1] = parseInt(p_Year);
			}
			} 
			else if (incr == 1) {
			// F O R W A R D
			if (p_Month == 11) {
				ret_arr[0] = 0;
				ret_arr[1] = parseInt(p_Year) + 1;
			}
			else {
				ret_arr[0] = parseInt(p_Month) + 1;
				ret_arr[1] = parseInt(p_Year);
			}
			}
			return ret_arr;
		}

		Calendar.prototype.getMonthlyCalendarCode = function() {
		var vCode = "";
		var vHeader_Code = "";
		var vData_Code = "";
		    
		// Begin Table Drawing code here....month table
		vCode = vCode + "<TABLE onselectstart='return false;' BORDER=0  style='border: 1 solid #8CBAB5' cellpadding=1 cellspacing=0 BGCOLOR=\"" + this.gBGColor + "\">";
		vHeader_Code = this.cal_header();
		vData_Code = this.cal_data();
		vCode = vCode + vHeader_Code + vData_Code;
		vCode = vCode + "</TABLE>";
		  
		return vCode;
		}

		Calendar.prototype.show = function() {
		var vCode = "";
		this.gWinCal.document.open();

		// Setup the page...
		this.wwrite("<html>");
		this.wwrite("<head><title>Calendar</title>");				
		this.wwrite("<style> body,td,table {font-face:"+fontface+";font-size:12px;}");
		this.wwrite(".navig{color:white;}</style>");
		this.wwrite("</head>");
		this.wwrite("<body TOPMARGIN=4 " + 	"link=\"" + this.gLinkColor + "\" " + "vlink=\"" + this.gLinkColor + "\" " +
				" alink=\"" + this.gLinkColor + "\" " + "text=\"" + this.gTextColor + "\">");
		this.wwriteA("<Label style='color:#808080;font-size:15px;font-weight:bold;' >"+this.gMonthName + " " + this.gYear);
		this.wwrite("");
		

		// Show navigation buttons
		var prevMMYYYY = Calendar.calc_month_year(this.gMonth, this.gYear, -1);
		var prevMM = prevMMYYYY[0];
		var prevYYYY = prevMMYYYY[1];

		var nextMMYYYY = Calendar.calc_month_year(this.gMonth, this.gYear, 1);
		var nextMM = nextMMYYYY[0];
		var nextYYYY = nextMMYYYY[1];
		    
		this.wwrite("<TABLE onselectstart='return false;' style='font-size:15px;' WIDTH='100%' BORDER=0 CELLSPACING=0 CELLPADDING=0 BGCOLOR='#3a6ea5'><TR><TD ALIGN=left>");
		this.wwrite("&nbsp;<A class=navig HREF=\"" + "javascript:window.opener.Build(" +  "'" + this.gReturnItem + "', '" + this.gMonth + "', '" + (parseInt(this.gYear)-1) + "', '" + this.gFormat + "', '" + this.gReturnFn + "');" + "\" ><<<\/A></TD><TD ALIGN=left>");
		this.wwrite("<A class=navig HREF=\"" + "javascript:window.opener.Build(" +  "'" + this.gReturnItem + "', '" + prevMM + "', '" + prevYYYY + "', '" + this.gFormat + "', '" + this.gReturnFn + "');" + "\"><<\/A></TD><TD ALIGN=right>");
		this.wwrite("<A class=navig HREF=\"" + "javascript:window.opener.Build(" + "'" + this.gReturnItem + "', '" + nextMM + "', '" + nextYYYY + "', '" + this.gFormat + "', '" + this.gReturnFn + "');" + "\">><\/A></TD><TD ALIGN=right>");
		this.wwrite("<A class=navig HREF=\"" + "javascript:window.opener.Build(" + "'" + this.gReturnItem + "', '" + this.gMonth + "', '" + (parseInt(this.gYear)+1) + "', '" + this.gFormat + "', '" + this.gReturnFn + "');" + "\">>><\/A>&nbsp;</TD></TR></TABLE>");

		vCode = this.getMonthlyCalendarCode();
		this.wwrite(vCode);
		this.wwrite("</body></html>");
		this.gWinCal.document.close();
		}

		Calendar.prototype.wwrite = function(wtext) {
		    this.gWinCal.document.writeln(wtext);
		}

		Calendar.prototype.wwriteA = function(wtext) {
		    this.gWinCal.document.write(wtext);
		}				

		Calendar.prototype.cal_header = function() {
		    var vCode = "";
		    
		    vCode = vCode + "<TR style='font-weight:bold;'>";
		    vCode = vCode + "<TD WIDTH='14%'>Sun</TD>";
		    vCode = vCode + "<TD WIDTH='14%'>Mon</TD>";
		    vCode = vCode + "<TD WIDTH='14%'>Tue</TD>";
		    vCode = vCode + "<TD WIDTH='14%'>Wed</TD>";
		    vCode = vCode + "<TD WIDTH='14%'>Thu</TD>";
		    vCode = vCode + "<TD WIDTH='14%'>Fri</TD>";
		    vCode = vCode + "<TD WIDTH='16%'>Sat</TD>";
		    vCode = vCode + "</TR>";
		    
		    return vCode;
		}

		Calendar.prototype.cal_data = function() {
		    var vDate = new Date();
		    vDate.setDate(1);
		    vDate.setMonth(this.gMonth);
		    vDate.setFullYear(this.gYear);

		    var vFirstDay=vDate.getDay();
		    var vDay=1;
		    var vLastDay=Calendar.get_daysofmonth(this.gMonth, this.gYear);
		    var vOnLastDay=0;
		    var vCode = "";

		    /*
		    Get day for the 1st of the requested month/year..
		    Place as many blank cells before the 1st day of the month as necessary. 
		    */

		    vCode = vCode + "<TR>";
		    for (i=0; i<vFirstDay; i++) {
				vCode = vCode + "<TD WIDTH='14%'" + this.write_weekend_string(i) + ">&nbsp;</TD>";
		    }

		    // Write rest of the 1st week
		    for (j=vFirstDay; j<7; j++) {
		            vCode = vCode + "<TD WIDTH='14%'" + this.write_weekend_string(j) + ">" + 
		                    "<A class=calendar title='Select this date' HREF='#' " + "onClick=\"self.opener.document.forms[0]." + this.gReturnItem + ".value='" + 

		                    this.format_data(vDay) + "';self.opener."+this.gReturnFn+";window.close();\">" + this.format_day(vDay) + "</A>" + "</TD>";
				vDay=vDay + 1;
		    }
		    vCode = vCode + "</TR>";
		    // Write the rest of the weeks
		    for (k=2; k<7; k++) {
				vCode = vCode + "<TR>";

			for (j=0; j<7; j++) {
				vCode = vCode + "<TD WIDTH='14%'" + this.write_weekend_string(j) + ">" + 
		               "<A class=calendar title='Select this date'  HREF='#' " + "onClick=\"self.opener.document.forms[0]." + this.gReturnItem + ".value='" + 
		               this.format_data(vDay) + "';self.opener."+this.gReturnFn+";window.close();\">" + this.format_day(vDay) + "</A>" + "</TD>";
				vDay=vDay + 1;
		    if (vDay > vLastDay) {
				vOnLastDay = 1;
		    break;
		    }
		  }
		    if (j == 6)
				vCode = vCode + "</TR>";
		    
		    if (vOnLastDay == 1) //If last day printed of month.		    		     
		     break;	    		     		    
		}
		    
			// Fill up the rest of last week with proper blanks, so that we get proper square blocks
			for (m=1; m<(7-j); m++) {
				if (this.gYearly)
					vCode = vCode + "<TD WIDTH='14%'" + this.write_weekend_string(j+m) + ">&nbsp;</TD>";
		        else
		            vCode = vCode + "<TD WIDTH='14%'" + this.write_weekend_string(j+m) + ">&nbsp;</TD>"; // If you want to print next month's date here user this -> COLOR='gray'>" + m + "</FONT>
		    }
		    
		    //Keep the last row blank in case of small months, so that table do not shrink while moving between months. 
		    //Code added by Tittle
		    if (k == 5) 		    
					vCode += "<TR><td BGCOLOR="+weekendColor+" style='color:"+weekendColor+"'>&nbsp;1</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td BGCOLOR="+weekendColor+">&nbsp;</td></tr>";					
		    
		    return vCode;
		}

		Calendar.prototype.format_day = function(vday) {
			var vNowDay = gNow.getDate();
		  var vNowMonth = gNow.getMonth();
		  var vNowYear = gNow.getFullYear();
				
		    if (vday == vNowDay && this.gMonth == vNowMonth && this.gYear == vNowYear)
		    {
					return ("<FONT COLOR=\"BLUE\" TITLE=\"Select Today's Date\"><B>" + vday + "</B></FONT>");
				}	
		    else
		    {				
					return (vday);
				}	
		}

		Calendar.prototype.write_weekend_string = function(vday) {
			var i;

		    // Return special formatting for the weekend day.
		    for (i=0; i<weekend.length; i++) {
			  if (vday == weekend[i])
			    return (" BGCOLOR=\"" + weekendColor + "\"");
		    }
		    
		    return "";
		}

		Calendar.prototype.format_data = function(p_day) {
		    var vData;
		    var vMonth = 1 + this.gMonth;
		    vMonth = (vMonth.toString().length < 2) ? "0" + vMonth : vMonth;
		    
		    var vMon = Calendar.get_month(this.gMonth).substr(0,3);//.toUpperCase();
		    var vFMon = Calendar.get_month(this.gMonth);//.toUpperCase();
		    var vY4 = new String(this.gYear);
		    var vY2 = new String(this.gYear.substr(2,2));
		    var vDD = (p_day.toString().length < 2) ? "0" + p_day : p_day;

		    switch (this.gFormat) {
				case "MM\/DD\/YYYY" :
					vData = vMonth + "\/" + vDD + "\/" + vY4;
		            break;
		        case "MM\/DD\/YY" :
					vData = vMonth + "\/" + vDD + "\/" + vY2;
					break;
				case "MM-DD-YYYY" :
					vData = vMonth + "-" + vDD + "-" + vY4;
		            break;
		        case "MM-DD-YY" :
		            vData = vMonth + "-" + vDD + "-" + vY2;
		            break;
		        case "DD\/MON\/YYYY" :
		            vData = vDD + "\/" + vMon + "\/" + vY4;
		            break;
		        case "DD\/MON\/YY" :
		            vData = vDD + "\/" + vMon + "\/" + vY2;
		            break;
		        case "DD-MON-YYYY" :
					vData = vDD + "-" + vMon + "-" + vY4;
		            break;
		        case "DD-MON-YY" :
		            vData = vDD + "-" + vMon + "-" + vY2;
		            break;

		        case "DD\/MONTH\/YYYY" :
					vData = vDD + "\/" + vFMon + "\/" + vY4;
		            break;
		        case "DD\/MONTH\/YY" :
		            vData = vDD + "\/" + vFMon + "\/" + vY2;
		            break;
		        case "DD-MONTH-YYYY" :
		            vData = vDD + "-" + vFMon + "-" + vY4;
					break;
		        case "DD-MONTH-YY" :
		            vData = vDD + "-" + vFMon + "-" + vY2;
		            break;

		        case "DD\/MM\/YYYY" :
					vData = vDD + "\/" + vMonth + "\/" + vY4;
		            break;
		        case "DD\/MM\/YY" :
					vData = vDD + "\/" + vMonth + "\/" + vY2;
		            break;
		        case "DD-MM-YYYY" :
					vData = vDD + "-" + vMonth + "-" + vY4;
		            break;
		        case "DD-MM-YY" :
		            vData = vDD + "-" + vMonth + "-" + vY2;
		            break;
		         default :
		            vData = vMonth + "\/" + vDD + "\/" + vY4;
		    }		    		    

		    return vData;
		}
		function Build(p_item, p_month, p_year, p_format, p_retFn) {
			//alert(p_item)
	        var p_WinCal = ggWinCal;
	        
	        gCal = new Calendar(p_item, p_WinCal, p_month, p_year, p_format, p_retFn);

	        // Customize your Calendar here..
	        gCal.gBGColor="white";
	        gCal.gLinkColor="black";
	        gCal.gTextColor="black";
	        
	        // Choose appropriate show function
			gCal.show();
		}
	
		
		function oncalendarload()
			{
				WinState = 1;				
			}
		function onclickpage()
			{
				WinState = 2;	
			}
		function oncalendarunload()
			{			
				if (WinState != 1 && ggWinCal != null)
				{
					WinState = 1;
					ggWinCal.close();
				}												
			}			

//Check Date
function CheckDate(obj,dtvalidformat)
{
  var arr_ret 
  arr_ret = new Array(); 
  
 //For years in the range '1970-1999'  should be entered as four char length strings 
 //For years in the range '2000-2099'  can be entered as 1-2 char length strings
 
	var mm=0, dd=1, yyyy=2, current_date, current_month, current_year, valid_year_length;
	
	var currentformat=''; //Default it is none i.e. MM/DD/YYYY
	
	if (arguments.length>1) //Default Parameter is None and Date format is MM/DD/YYYY
	{
		if (dtvalidformat=='ddDate') //DD/MM/YYYY
		{
			currentformat='ddDate';
			mm=1;
			dd=0;
			yyyy=2; 
		}
	}			
	
	var_str = obj.value;
	valid_year_length = false;
	
	if (obj.value.length == 0 ) 	 
		return true;
		
   if ( var_str.length < 5 )
    {     
      return false;
    }    
   arr_ret=var_str.split("/")
   
   if(arr_ret)
   {
		if (arr_ret.length < 3 || arr_ret.length > 3)
		{	 	  
	 	  return false;
		}
   }
   else // this case is when arr_ret is 'undefined'
   {	   
	   return false;
   }
	
   if (arr_ret[mm]>12 || arr_ret[mm]<1 || arr_ret[dd]>31 || arr_ret[dd]< 1 )     
   //This code was commented because year is being taken now in the range of 1970 and 2099 (as in system) and is manipulated later in the code|| arr_ret[yyyy]< 1970)
	 {	 
	  return false;
    }    
   if(arr_ret[yyyy].length > 4 || arr_ret[yyyy].length == 0)
    {
     return false;
    }
   
   if(arr_ret[yyyy].length == 4)
		valid_year_length = true;
		
   current_month = parseInt(arr_ret[mm],10);
   current_date = parseInt(arr_ret[dd],10);
   current_year = parseInt(arr_ret[yyyy],10);
	
   if ((valid_year_length == true && current_year < 1970)  || current_year > 2099 )
   {	  
	  return false;
   }	  

   if (current_year >= 0 && current_year <=99)
	   current_year = current_year + 2000;	

   if(isNaN(current_month) || isNaN(current_date) || isNaN(current_year)) 
   {
      return false;
   } 
   
   if ((current_month == 4 || current_month == 6 || current_month == 9 || current_month == 11 )&&(current_date > 30))
   {
	 return false;
   }
   
   if ((current_month == 1 || current_month == 3 || current_month == 5 || current_month == 7 || current_month == 8 || current_month == 10 || current_month == 12)&&(current_date > 31))
   {
	 return false;
   }
      
   by_4 = current_year%4; 
   by_400 = current_year%400;
   by_100 = current_year%100;
   
   if (((by_4 == 0 && (by_100 == 0)))|| (by_400 == 0))
     leap_year = true;
   else
     leap_year = false;  
     
   if (!leap_year && current_month == 2 && current_date > 28)
    {
	 return false;
    }  
    
  if (current_month <10)
  	 current_month = "0"+ current_month ;
  if (current_date <10)
	 current_date = "0"+ current_date ;	
	 
   if (currentformat=='' || currentformat=='mmDate' )
		obj.value = current_month +'/'+ current_date +'/'+ current_year;
	 
	 if (currentformat=='ddDate')
		obj.value = current_date + '/'+ current_month +'/'+ current_year;
	 
   return true;
    
}
//** checkDate Ends Here

