$(document).ready(function () {

    setTabActive($("#hdCurrentActiveSecondaryTab").val());

    $(".clickable").click(function (event) {
        //resetAllTabs();
        //  setTabActive(event.target.id);
       // alert(event.target.id);
        setTabActive(event.target.id);
        return true;
    });

});

/* set all tabs to the inactive state */

/* set the active tab */
function setTabActive(tab_id) {

    $("#" + tab_id).css("color", '#CC6600');
    $("#" + tab_id).css("background-color", '#f1ede3');
    $("#" + tab_id).css("text-decoration", 'underline');
    $("#" + tab_id).parent().css("background-color", '#f1ede3');
    $("#" + tab_id).parent().css("border-left", '1px solid #888888');
    $("#" + tab_id).parent().css("border-right", '1px solid #888888');
    $("#" + tab_id).parent().css("border-top", '1px solid #888888');
    $("#" + tab_id).parent().css("border-bottom", '1px solid #888888');
}

/* set all td's to the inactive state */
function hideAllCells(classname) {
    $("." + classname).css("display", 'none');
    $("." + classname).css("visibility", 'collapse');
}
function displayAllCells(classname, addname) {
    //$("." + classname).addClass(addname);
    $("." + classname).css("display", 'inline');
    $("." + classname).css("visibility", 'visible');
}

function RadConfirmAction(sender, args, actionDesc) {
	args.set_cancel(!confirm('Please confirm - do you wish to' + ' ' + actionDesc + ' ?'));
}

function confirmChange(changeDesc) {
    var xlat = document.getElementById('hfPromptChange').value;
    return confirm(xlat + ' ' + changeDesc + ' ?');
}
function confirmAction(actionDesc) {
    var xlat = document.getElementById('hfPromptAction').value;
    return confirm(xlat + ' ' + actionDesc + ' ?');
}
function confirmActionChecked(cb, actionDesc) {
    var sel = cb.checked;
    if (cb.checked == true) {
        var xlat = document.getElementById('hfPromptAction').value;
        sel = confirm(xlat + ' ' + actionDesc + ' ?');
        if (sel == false) {
            cb.checked = false;
        }
    }
    return sel;
}
function alertResult(msgCtl) {
    var xlat = document.getElementById(msgCtl).value;
    return alert(xlat);
}
function alertError(errorMsg) {
    //var xlat = document.getElementById(msgCtl).value;
    return alert(errorMsg);
}
function displayError(errorField, moreInfo, msgCtl) {
    var xlat = document.getElementById(errorField).value;
    document.getElementById(msgCtl).innerHTML = (xlat + ' ' + moreInfo);
    return true;
}
function copyTempValue(tempValue) {
    document.getElementById('hfTempValue').value = tempValue;
    return true;
}

function checkForValue(val, errorMsg) {
    var str = val;
    if (str.replace(/^\s+|\s+$/g, '').length < 1) {
        alertError(errorMsg);
        return false;
    }
    return true;
}

function isBlank(val) {
    var str = val;
    if (str.replace(/^\s+|\s+$/g, '').length < 1) {
        return true;
    }
    return false;
}

function checkForNumeric(val, errorMsg) {

    if (val.match(/^[0-9.-]+$/) == null) {
        alertError(errorMsg);
        return false;
    }
    return true;
}


function checkForSelect(ddl, errorMsg) {
    if (ddl.selectedIndex < 0  || isBlank(ddl.options[ddl.selectedIndex].text)) {
     //   if (isBlank(ddl.options[ddl.selectedIndex].value)) {
        alertError(errorMsg);
        return false;
    }
    return true;
}

function checkForSelectChecked(ddl, cb, errorMsg) {

    if ((ddl.selectedIndex < 0 || isBlank(ddl.options[ddl.selectedIndex].value)) &&  cb.checked == false) {
        alertError(errorMsg);
        return false;
    }
    return true;
}

function ValidNumeric(tb, errorMsg) {

    if (isBlank(tb.value) == false) {
        var str = tb.value;
        str = str.replace(/,/g, "");
        if (str.match(/^[0-9.-]+$/) == null || str.split('.').length > 2 || str.indexOf('-') > 0) {
            alertError(errorMsg);
            tb.value = '';
            tb.focus();
            return false;
        }
        else {
            tb.value = str;
        }
    }
    return true;
}

function ValidNumericNotBlank(tb, errorMsg) {
    var str = tb.value;
    if (isBlank(str) == true) {
        alertError(errorMsg);
        tb.value = '';
        tb.focus();
        return false;
    }

    str = str.replace(/,/g, "");
    if (str.match(/^[0-9.-]+$/) == null || str.split('.').length > 2 || str.indexOf('-') > 0) {
        alertError(errorMsg);
        tb.value = '';
        tb.focus();
        return false;
    }
    else {
        tb.value = str;
    }

    return true;
}


function gotoPage(url) {
    window.location = url;
}

function filterDependentList(list1, list2, hf1, hf2) {
    var ddl1 = document.getElementById(list1);
    var ddl2 = document.getElementById(list2);
    var ddlRef = document.getElementById('ddlMasterRef');
    var sel1 = ddl1.options[ddl1.selectedIndex].value;
    var hf2sel = false;

    ddl2.length = 0;
    for (var i = 0; i < ddlRef.options.length; i++) {
        var keys = ddlRef.options[i].value.split('|');
        if (i == 0 || keys[0] == sel1) {
            ddl2.options[ddl2.options.length] = new Option(ddlRef.options[i].text, ddlRef.options[i].value);
            if (document.getElementById(hf2).value == ddlRef.options[i].value) {
                ddl2.options[ddl2.options.length - 1].selected = true;
                hf2sel = true;
            }
        }
    }
    if (!hf2sel &&  ddl2.options.length > 1)
        ddl2.options[1].selected = true;
}

function enableListItems(list1, list2) {
    var ddl1 = document.getElementById(list1);
    var ddl2 = document.getElementById(list2);
    var val = ddl1.options[ddl1.selectedIndex].value;

    for (var i = 0; i < ddl2.options.length; i++) {
        var keys = ddl2.options[i].value.split('|');
        if (keys[0] == val) {
            ddl2.options[i].style.display = "block";
            ddl2.options[i].style.disabled = false;
        }
        else {
            ddl2.options[i].style.display = "none";
            ddl2.options[i].style.disabled = true;
        }
    }
    return true;
}

function copyValueFromTo(fromID, toID) {
    var fromFld = document.getElementById(fromID);
    var toFld = document.getElementById(toID);
    toFld.value = fromFld.innerHTML;
}

function putSelectedValue(theList, putFld) {
    var ddl2 = document.getElementById(theList);
    document.getElementById(putFld).value = ddl2.options[ddl2.selectedIndex].value;
}

function putSelectedText(theList, putFld, refFld) {
    var ddl2 = document.getElementById(theList);
    var text = "";
    try{
        var ref = document.getElementById(refFld);
        var opts = ref.value.split('|');
        for (var n = 0; n < opts.length; n++) {
            var vals = opts[n].split(',');
            if (vals[0] == ddl2.options[ddl2.selectedIndex].value) {
                text = vals[1];
                break;
            }
        }
    }
    catch (e) {
        text = ddl2.options[ddl2.selectedIndex].text;
    }

    document.getElementById(putFld).innerHTML = text;
}

function PopupCenter(pageURL, title, w, h) {
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    var targetWin = window.open(pageURL, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
}

function Popup(pageURL, title, w, h) {
    var targetWin = window.open(pageURL, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h);
}

function SetFocus(elementID) {
    var fld = document.getElementById(elementID);
    if (fld) {
        fld.focus();
    }
}

function SetSectionVisible(elementID, ht) {
    var target = document.getElementById(elementID);
    var htpx = ht.toString() + 'px';
    if (ht < 5) {
        target.style.height = htpx;
        target.style.visibility = 'hidden';
    }
    else {
        target.style.height = htpx;
        target.style.visibility = 'visible';
    }
}

function ToggleSectionVisible(elementID, htpx) {
    var target = document.getElementById(elementID);
    var targetImg = document.getElementById(elementID + '_img');
    if (target.style.visibility == 'hidden') {
      /*  targetImg.src = "/images/minus.png"; */
        targetImg.src = "/images/defaulticon/16x16/arrow-8-up.png";
        if (htpx == '0') {
            target.style.height = null;
        }
        else {
            target.style.height = htpx;
        }
        target.style.visibility = 'visible';
    }
    else {
       /* targetImg.src = "/images/plus.png"; */
        targetImg.src = "/images/defaulticon/16x16/arrow-8-right.png";
        target.style.height = '1px';
        target.style.visibility = 'hidden';
    }
}

function ToggleSection(elementID) {
    var target = document.getElementById(elementID);
    var targetImg = document.getElementById(elementID + '_img');
    if (target.style.display == 'none') {
        /* targetImg.src = "/images/minus.png"; */
        targetImg.src = "/images/defaulticon/16x16/arrow-8-up.png";
        target.style.display = 'block';
    }
    else {
        /*   targetImg.src = "/images/plus.png"; */
        targetImg.src = "/images/defaulticon/16x16/arrow-8-right.png";
        target.style.display = 'none';
    }
}

function SetSection(elementID, option) {
    var target = document.getElementById(elementID);
    var targetImg = document.getElementById(elementID + '_img');
    if (option == 1) {
        targetImg.src = "/images/minus.png";
        target.style.display = 'block';
    }
    else {
        targetImg.src = "/images/plus.png";
        target.style.display = 'none';
    }
}

function ToggleAreaVisible(divID) {
	var area = document.getElementById(divID);
	if (area.style.display == 'none') {
		area.style.display = 'block';
	}
	else {
		area.style.display = 'none';
	}
}

function ElementHeight(elementID, addHeight) {
    var element = document.getElementById(elementID);
    return ((element.clientHeight+addHeight) + 'px');
}


function Validate_ProductLineList() {
    Page_ClientValidate();
    if (Page_IsValid) {
        return confirmChange('Product Lines');
    }
}

//There are validators on the Product Line grid, but the same
//grid is used to delete lines. A line to be deleted doesn't need
//the validators, so use this function when the user clicks the "Delete"
//checkbox to disable or enable the validators on that line.
function DisableEnableValidators(this_checkbox) {
    //use the name of the checkbox to find the validators that 
    //correspond to the checkbox
    var name = this_checkbox.id;
    var validateProdLine = name.replace("cbStatus", "valProdLine");
    var validateProdDesc = name.replace("cbStatus", "valProdDesc");

    var isChecked = this_checkbox.checked;

    ValidatorEnable(document.getElementById(validateProdLine), !isChecked);
    ValidatorEnable(document.getElementById(validateProdDesc), !isChecked);

}

function DisableComboSeparators(sender) {
    var items = sender.get_items();

    for (i = 0; i < items.get_count() ; i++) {
        var item = items.getItem(i);
        if (item.get_isSeparator() == true) {
            try {
                checkBoxElement = item.get_checkBoxElement(),
                itemParent = checkBoxElement.parentNode;
                itemParent.removeChild(checkBoxElement);
            }
            catch (e)
            {

            }
        }
    }
}



