var notesDivCopy = null;
var notesPrevOnKeyDown = function () {return true};
var notesPrevOnKeyUp = function () {return true};
var ob_dnd_allowCopy = true;
var notesCoverDiv = null;
var ob_dnd_tve,ob_dnd_tvr,ob_dnd_tvt=0,ob_dnd_tvq=0,ob_dnd_tvy=false,ob_dnd_tvu=null,ob_dnd_tvi=null,tree_dd_path="",tree_dd_id="",ob_dnd_tvo=0,ob_dnd_tvp=0,ob_dnd_tva=true,ob_dnd_tvs=0,ob_dnd_tvd=0,objTree;

// register events
try {document.registerEvents(Event.KEYDOWN)} catch (e) {};
try {document.registerEvents(Event.KEYUP)} catch (e) {};

function ob_attachDragAndDrop(el)
{
	el.ondragstart = function(){return false;};
	if(document.all)
	{
		el.onmousedown=new Function("ob_dnd_t10(null,this);");
		el.onmouseup=new Function("ob_dnd_t12();");
	}
	else
	{
		el.setAttribute("onmousedown","ob_dnd_t10(event,this);");
		el.setAttribute("onmouseup","ob_dnd_t12();");
	}
}

function ob_dnd_t10(event,el)
{
	// EVENT. Before Drag start.
	objTree=document.getElementById(ob_tree_id);

	ob_dnd_tvy=true;
	ob_dnd_tvi=el;
	// if can copy notes
	if (ob_dnd_allowCopy)
	{
		// add event for key down
		notesPrevOnKeyDown = document.onkeydown;
		notesPrevOnKeyUp = document.onkeyup;
		document.onkeydown = function(e){NotesKeyModifierWatch(e)};
		document.onkeyup = function (e) {NotesKeyModifierWatch(e)};
	}
	
	// events for mouse move
	document.onmousemove=function(e){ob_dnd_t11(e);o_A=null;ob_t53(true, e);};
	document.onmouseup=function(e){ob_dnd_t13(e, null, null, null, null);ob_t53(false, e);};
	document.onselectstart=function(){return false;};
	document.onmousedown=function(){return false;};
}

function ob_dnd_t12()
{
	ob_dnd_tvy=false;
	
	// remove the events
	document.onmousemove=null;
	ob_t53(false);
	document.onselectstart=function(){return true;};
	document.onmousedown=function(){return true;};
	if (ob_dnd_allowCopy)
	{
		document.onkeydown = notesPrevOnKeyDown;
		document.onkeyup = notesPrevOnKeyDown;
	}
}

function ob_dnd_t11(event)
{
	if(ob_dnd_tva==true)
	{
		if(window.event)
		{
			var event=window.event;
			ob_dnd_tvo=event.x;
			ob_dnd_tvp=event.y;
		}
		else
		{
			ob_dnd_tvo=event.pageX;
			ob_dnd_tvp=event.pageY;
		}
		ob_dnd_tva=false;
		return;
	}
	else
	{
		if(window.event)
		{
			var event=window.event;
			ob_dnd_tvs=event.x;
			ob_dnd_tvd=event.y;
		}
		else
		{
			ob_dnd_tvs=event.pageX;
			ob_dnd_tvd=event.pageY;
		}
	}
	
	if((Math.abs(ob_dnd_tvs-ob_dnd_tvo)>5)||(Math.abs(ob_dnd_tvd-ob_dnd_tvp)>5)){}
	else{return;}
	
	if(ob_dnd_tvy==false) return;
	if(ob_dnd_tvu==null)
	{
		// create the dragable div
		ob_dnd_tvu = document.createElement('div');
		document.body.appendChild(ob_dnd_tvu);
		ob_dnd_tvu.id="ob_dnd_drag";

		// create the note ghost
		var tbl = document.createElement("TABLE");tbl.id = ob_dnd_tvi.id;
		var eb = tbl.appendChild(document.createElement("tbody"));
		var etr = eb.appendChild(document.createElement("tr"));
		var etdIcon = etr.appendChild(document.createElement("td"));
		var etdContent = etr.appendChild(document.createElement("td"));
		var img = etdIcon.appendChild(document.createElement("img"));
				
		img.src = ob_getRecordIconSrc(ob_dnd_tvi);
		
		//etdIcon.width=ob_dnd_tvi.firstChild.width;
		//etdIcon.className=ob_dnd_tvi.firstChild.className;
		etdIcon.className="ob_t2";
		//etdContent.width=ob_dnd_tvi.firstChild.nextSibling.width;
		//etdContent.className=ob_dnd_tvi.firstChild.nextSibling.className;
		etdContent.className="ob_t2";
		etdContent.innerHTML=ob_getRecordHtml(ob_dnd_tvi);		
		ob_dnd_tvu.appendChild(tbl);

		ob_dnd_tvu.style.width=200;
		ob_dnd_tvu.style.position = "absolute";
		ob_dnd_tvu.style.zIndex="0";
		ob_dnd_tvu.style.filter="Alpha(Opacity='70',FinishOpacity='0',Style='1',StartX='0',StartY='0',FinishX='100',FinishY='100')";		
		
		if(window.event) 
		{
			ob_dnd_tvt=document.body.scrollLeft;
			ob_dnd_tvq=document.body.scrollTop;
		}
		else
		{
			ob_dnd_tve=event.pageX;
			ob_dnd_tvr=event.pageY;
		}
	}

	// show the ghost div at the mouse coords
	if(window.event)
	{
		var event=window.event;
		ob_dnd_tvu.style.left=event.x+ob_dnd_tvt-5;
		ob_dnd_tvu.style.top=event.y+ob_dnd_tvq-5;

		// if copying
		if (notesDivCopy && ob_dnd_allowCopy)
		{
			// move the copy div at the mouse coords
			notesDivCopy.style.left = event.x + ob_dnd_tvt - 12;
			notesDivCopy.style.top = event.y + ob_dnd_tvq + 12;
		}
	}
	else
	{
		ob_dnd_tvu.style.left=event.pageX-5;
		ob_dnd_tvu.style.top=event.pageY-5;

		// if copying
		if (notesDivCopy && ob_dnd_allowCopy)
		{
			// move the copy div at the mouse coords
			notesDivCopy.style.left = event.pageX - 12;
			notesDivCopy.style.top = event.pageY + 12;
		}
	}
	
	// if ctrl is pressed, show the copy div
	if (notesDivCopy && ob_dnd_allowCopy) try {notesDivCopy.style.display = event.ctrlKey ? "block" : "none";} catch (e) {}
	
	// get the top and bottom of the treeview
	var top=ob_dnd_t14(objTree);
	var bottom=top+objTree.offsetHeight;

	// scroll if needed
	if((top-ob_dnd_tvu.offsetTop)>-20&&objTree.scrollTop>0)
	{
		objTree.scrollTop=objTree.scrollTop-6;
	}
	
	if((ob_dnd_tvu.offsetTop-bottom)>-40)
	{
		objTree.scrollTop=objTree.scrollTop+6;
	}
}

function ob_dnd_t13(event, ob_dnd_tvh, ob_dnd_tvj, copying)
{      
	// hide the cover div
	if (notesCoverDiv != null) notesCoverDiv.style.display = 'none';
	// don't highlight the nodes on hover
	ob_highlightHover = false;

	// get the tree object if not already taken
	if (objTree == null) objTree=document.getElementById(top.ob_tree_id);

	var e,lensrc,s,s2;ob_dnd_tva=true;
	
	// hide the copy div
	if (notesDivCopy != null) notesDivCopy.style.display = 'none';
	
	// if not allowed copy, set copy to false
	if (!ob_dnd_allowCopy) copying = false;

	// if no node as start node
	if(ob_dnd_tvu==null){return;}
	
	// get mouse position
	if (ob_dnd_tvh == null || ob_dnd_tvj == null)
	{
		if(window.event)
		{
			var event=window.event;
			var ob_dnd_tvh=event.x+ob_dnd_tvt;
			var ob_dnd_tvj=event.y+ob_dnd_tvq;
		}
		else
		{
			var ob_dnd_tvh=event.pageX;
			var ob_dnd_tvj=event.pageY;
		}
	}
	
	// adjust position with scroll
	cNode = document.getElementById(ob_tree_id);
	do
	{
		cNode = cNode.parentNode;
		if (cNode != null && cNode != document.body)
		{
			if (typeof(cNode.scrollLeft) != 'undefined') ob_dnd_tvh += cNode.scrollLeft;
			if (typeof(cNode.scrollTop) != 'undefined') ob_dnd_tvj += cNode.scrollTop;
		}
		else break;
	}
	while (true);
	
	var ob_dnd_tvf,flagReturn=false;
	ob_dnd_tvu.style.display="none";
	
	// get all child table tags of treeview
	items=objTree.getElementsByTagName("TABLE");
	
	for(i=0;i<items.length;i++)
	{
		// get top of item
		var top=ob_dnd_t14(items[i])-objTree.scrollTop;
		// get left of item
		var left=ob_dnd_t15(items[i])-objTree.scrollLeft;
		// if mouse up was inside this item
		if(items[i].tagName=="TABLE"&&(ob_dnd_tvj>=top&&ob_dnd_tvj<=items[i].offsetHeight+top)&&(ob_dnd_tvh>=left&&ob_dnd_tvh<=items[i].offsetWidth+left))
		{
			// set this as the drop target
			ob_dnd_tvf=items[i];
		}
	} 

	// if drop target is not null
	if(ob_dnd_tvf!=null)
	{	    
		// if partent is a div
		if(ob_dnd_tvf.parentNode.tagName=="DIV")
		{		    
			// if first row has 3 children
			if(ob_dnd_tvf.firstChild.firstChild.childNodes.length==3)
			{
				// and first one has an image as first child
				if(ob_dnd_tvf.firstChild.firstChild.firstChild.firstChild.tagName=="IMG")
				{				    
					// get the img src
					s=ob_dnd_tvf.firstChild.firstChild.firstChild.firstChild.src.toLowerCase();
					lensrc=(s.length-6);
					s=s.substr(lensrc, 6);
					// if src is one of a plus/minus image or a vertical rule
					if((s=="ik.gif")||(s=="hr.gif")||(s=="_l.gif")||(s=="us.gif")||(s=="_r.gif"))
					{
					    var c = ob_dnd_tvf.parentNode.lastChild.firstChild.firstChild.lastChild.className;						
						if ((c != "ob_t7") && (c != "none"))
						{
						}
						else if(ob_dnd_tvf.firstChild.firstChild.firstChild.firstChild && ob_dnd_tvf.firstChild.firstChild.firstChild.firstChild.src.toString().indexOf("plusik_l.gif") != -1 && !ob_isExpanded(ob_dnd_tvf.firstChild.firstChild.childNodes[2]))
						{							
							if(typeof ob_HighlightOnDnd != "undefined" && ob_HighlightOnDnd == true)
		                    {			
		                        if(ob_dnd_tvf.firstChild.firstChild.childNodes[2].className != "ob_t2" && ob_dnd_tvf.firstChild.firstChild.childNodes[2].id != tree_selected_id && (ob_sn2 == null || (ob_sn2 != null && ("|" + ob_sn2 + "|").indexOf("|" + ob_dnd_tvf.firstChild.firstChild.childNodes[2].id + "|") == -1)))
		                        {
		                            ob_dnd_tvf.firstChild.firstChild.childNodes[2].className = "ob_t2";
		                        }
		                    }
							//!
							alert("Please expand the root of dynamically loaded subtree before " + (!copying ? "dropping" : "copying") + ".");
							//!
							flagReturn = true;
						}
					}
					
					// else not a tree node
					else{flagReturn=true;}
				}
				else{flagReturn=true;}
			}
			else{flagReturn=true;}
		}
		else{flagReturn=true;}
	}
	
	// if not a tree node, set drop target to null
	if(flagReturn==true) ob_dnd_tvf=null;
	
	// if drop target not null
	if(ob_dnd_tvf!=null)
	{	    
		// if there are nodes restricted to drop
		if(typeof sNoDrop != "undefined" && sNoDrop!="")
		{
			// get the id of the drop target
			var ob_dnd_dd_target_id=ob_dnd_tvf.firstChild.firstChild.firstChild.nextSibling.nextSibling.id;            
			// get a list of nodes restricted
			var a=new Array;a=sNoDrop.split(",");
			if(a.length>0)
			{
				for(i=0;i<a.length;i++)
				{
					// if id of the drop target in the list
					if(ob_dnd_dd_target_id==a[i])
					{
						// set drop target to null
						ob_dnd_tvf=null;
	
	showAlertDialog("Can not " + (copying ? "copy" : "move") + ". The destination folder is restricted.");

	
					}
				}
			}
		}
	}
	
	// if drop target not null
	if(ob_dnd_tvf!=null)
	{
		// if copying not defined
		if (copying == null)
		{
			if (window.event) event = window.event; 
			// copying is set to the ctrl key press
			copying = event.ctrlKey;
		}

		// copying
		if (copying) 
			ob_dnd_handleCopy (ob_dnd_tvi, ob_dnd_tvf);
		// moving
		else				    
			ob_dnd_handleMove (ob_dnd_tvi, ob_dnd_tvf);		

		// remove the mouse events
		document.onmousemove="";
		ob_t53(false);
		// remove the ghost div
		document.body.removeChild(ob_dnd_tvu);
		// hide the plus div
		if (notesDivCopy != null) notesDivCopy.style.display = 'none';
	}
	else
	{
		// remove the ghost div
		document.body.removeChild(document.getElementById("ob_dnd_drag"));
	}

	ob_dnd_tvu=null;
	ob_dnd_tvy=false;
	// restore key events
	document.onkeydown = notesPrevOnKeyDown;
	document.onkeyup = notesPrevOnKeyUp;
	// restore mouse events
	document.onselectstart=function(){return true;};
	document.onmousedown=function(){return true;};
	document.onmouseup=null;
	document.onmousemove = "";
	// load pages in right panel when clicking on node
	followLinkInNode = true;		
	// EVENT. After Drag & Drop finished.
}

// get left position
function ob_dnd_t15(vz){var pos=0;if(vz.offsetParent){while(vz.offsetParent){pos+=vz.offsetLeft;vz=vz.offsetParent;}}else if(vz.x)pos+=vz.x;return pos;}
// get top position
function ob_dnd_t14(ue){var pos=0;if(ue.offsetParent){while(ue.offsetParent){pos+=ue.offsetTop;ue=ue.offsetParent;}}else if (ue.y)pos+=ue.y;return pos;}

function NotesKeyModifierWatch(event)
{
	// if copying not allowed, return
	if (!ob_dnd_allowCopy) return;
	
	// if copy div is null, create it
	if (notesDivCopy == null) 
	{
		notesDivCopy = document.createElement ('DIV');
		notesDivCopy.style.position = "absolute";
		notesDivCopy.style.zIndex = "0";
		notesDivCopy.style.left = 0;
		notesDivCopy.style.top = 0;
		notesDivCopy.id = ob_tree_id + '_copy_div';
		notesDivCopy.innerHTML = '<img src="../ASPTreeView/tree2/icons/plus.gif">';
		notesDivCopy.style.display = 'none';
		document.body.appendChild (notesDivCopy);
	}

	// move the copy div to the mouse coords
	if (ob_dnd_tvu != null)
	{
		notesDivCopy.style.left = ob_dnd_tvu.style.left.replace('px', '')*1 - 5;
		notesDivCopy.style.top = ob_dnd_tvu.style.top.replace('px', '')*1 + 17;
	}
	
	if (window.event) event = window.event;
	
	// if ctrl key pressed, show the copy div, else hide it
	try {notesDivCopy.style.display = (event.ctrlKey && ob_dnd_tvu != null ? "block" : "none");} catch (e) {};
}

function ob_dnd_handleCopy (ob_drag_item, ob_drag_destination)
{    
	//alert("Copied item with id: " + ob_drag_item.id + " to folder with id: " + ob_drag_destination.firstChild.firstChild.firstChild.nextSibling.nextSibling.id);	
	ob_t2_Add(ob_drag_destination.firstChild.firstChild.firstChild.nextSibling.nextSibling.id, ob_drag_item.id + "_" + parseInt(Math.random() * 1000).toString(), ob_getRecordHtml(ob_drag_item), true, ob_getRecordIconSrc(ob_drag_item).toString().substr(ob_getRecordIconSrc(ob_drag_item).toString().lastIndexOf("/") + 1), null);	
	return true;
}

function ob_dnd_handleMove (ob_drag_item, ob_drag_destination)
{    		
	ob_t2_Add(ob_drag_destination.firstChild.firstChild.firstChild.nextSibling.nextSibling.id, ob_drag_item.id + "_" + parseInt(Math.random() * 1000).toString(), ob_getRecordHtml(ob_drag_item), true, ob_getRecordIconSrc(ob_drag_item).toString().substr(ob_getRecordIconSrc(ob_drag_item).toString().lastIndexOf("/") + 1), null);
	//ob_drag_item.parentNode.removeChild(ob_drag_item);
	//alert("Moved item with id: " + ob_drag_item.id + " to folder with id: " + ob_drag_destination.firstChild.firstChild.firstChild.nextSibling.nextSibling.id);	
	return true;
}

function ob_getRecordHtml(oRecord)
{
     if(oRecord.childNodes[1].firstChild.nodeName == "SPAN") {
        return oRecord.childNodes[1].childNodes[1].firstChild.innerHTML;
    } else {
        return oRecord.childNodes[1].firstChild.innerHTML;
    }   
}

function ob_getRecordIconSrc(oRecord)
{
   
    if(oRecord.firstChild.firstChild.nodeName == "SPAN") {
        return oRecord.firstChild.childNodes[1].firstChild.firstChild.src;
    } else {
        return oRecord.firstChild.firstChild.firstChild.src;
    } 
}
