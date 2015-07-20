
function displayAllCells(classname, addname) {
    //$("." + classname).addClass(addname);
    $("." + classname).css("display", 'inline');
    $("." + classname).css("visibility", 'visible');
}

function confirmChange(changeDesc) {
    return confirm('Please confirm - do you wish to save your changes to this' + ' ' + changeDesc + ' ?');
}

function confirmAction(actionDesc) {
    return confirm('Please confirm - do you wish to' + ' ' + actionDesc + ' ?');
}

function RadConfirmAction(sender, args, actionDesc) {
    args.set_cancel(!confirm('Please confirm - do you wish to' + ' ' + actionDesc + ' ?'));
}

function confirmActionChecked(cb, actionDesc) {
    var sel = cb.checked;
    if (cb.checked == true) {
        sel = confirm('Please confirm - do you wish to' + ' ' + actionDesc + ' ?');
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
    return alert(errorMsg);
}

function displayError(errorField, moreInfo, msgCtl) {
    var xlat = document.getElementById(errorField).value;
    document.getElementById(msgCtl).innerHTML = (xlat + ' ' + moreInfo);
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

function checkTBForValue(tb, errorMsg) {
    var str = document.getElementById(tb).value
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


function ValidNumericPhone(tb, errorMsg) {

    if (isBlank(tb.value) == false) {
        var str = tb.value;
       // if (str.match(/^(()?\d{3}())?(-|\s)?\d{3}(-|\s)?\d{4}$/) == null) {
        if (str.match(/^(?!.*911.*\d{4})((\+?1[\/ ]?)?(?![\(\. -]?555.*)\( ?[2-9][0-9]{2} ?\) ?|(\+?1[\.\/ -])?[2-9][0-9]{2}[\.\/ -]?)(?!555.?01..)([2-9][0-9]{2})[\.\/ -]?([0-9]{4})$/) == null) {
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


function ElementHeight(elementID, addHeight) {
    var element = document.getElementById(elementID);
    return ((element.clientHeight+addHeight) + 'px');
}


function DisableComboSeparators(sender) {
    var items = sender.get_items();

    for (i = 0; i < items.get_count() ; i++) {
        var item = items.getItem(i);
        if (item.get_isSeparator() == true) {
            checkBoxElement = item.get_checkBoxElement(),
            itemParent = checkBoxElement.parentNode;
            itemParent.removeChild(checkBoxElement);
        }
    }
}



