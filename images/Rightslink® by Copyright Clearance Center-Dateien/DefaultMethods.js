var submitForm = true;
var confirmBeforeProceed = false;

function languageChanged(permissionChoice) {
	document.TheForm.targetPage.value = document.TheForm.currentTargetPage.value;
    document.TheForm.action = "/AppDispatchServlet";
    document.TheForm.target = "_top";
	if (document.TheForm.doCalc) {
		document.TheForm.doCalc.value = "true";
	}
	if (document.TheForm.languageSelectionChanged) {
		document.TheForm.languageSelectionChanged.value = "true";
	}
	document.TheForm.submit();
}

// function updateErrorChangeEvent() {
//     var errorMessage = 'ErrorMessage';
//     var idSelectorSign = '#';
//     var inputEvent = 'input';
//     var changeEvent = 'change';
//     var errorElements = document.getElementsByClassName('FieldWithError');
//
//     for(var x of errorElements) {
//         removeErrorEventListener(x, idSelectorSign, errorMessage, changeEvent);
//         removeErrorEventListener(x, idSelectorSign, errorMessage, inputEvent);
//     }
// }

function removeErrorEventListener(x, idSelectorSign, errorMessage, event) {
    x.addEventListener(event, function () {
        removeErrorClass(this);
        var fieldIdSelector = idSelectorSign + this.id + errorMessage;
        $(fieldIdSelector).parent().parent().remove();
    });
}

// Function for IE human acceptable version commented higher
function updateErrorChangeEvent() {
    var errorMessage = 'ErrorMessage';
    var idSelectorSign = '#';
    var inputEvent = 'input';
    var changeEvent = 'change';
    var errorElements = document.getElementsByClassName('FieldWithError');
    var _iteratorNormalCompletion = true;
    var _didIteratorError = false;
    var _iteratorError = undefined;

    try {
        for (var _iterator = errorElements[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true) {
            var x = _step.value;
            removeErrorEventListener(x, idSelectorSign, errorMessage, changeEvent);
            removeErrorEventListener(x, idSelectorSign, errorMessage, inputEvent);
        }
    } catch (err) {
        _didIteratorError = true;
        _iteratorError = err;
    } finally {
        try {
            if (!_iteratorNormalCompletion && _iterator.return != null) {
                _iterator.return();
            }
        } finally {
            if (_didIteratorError) {
                throw _iteratorError;
            }
        }
    }
}

function removeErrorClass(elem) {
    elem.classList.remove("FieldWithError");
}

function splitIntoTwoColumns(blockName) {
    var lengthCounter = (blockName === '.additional-data-inner-block') ? 0 : 1;
    var tableElements;
    if (blockName === '.order-shipping-with-options-inner-block' || blockName === '.order-shipping-inner-block') {
        var shippingText = 'Shipping Address ';
        for (i = 1; i <= 4; i++) {
            tableElements = $(blockName + " .content-block .content-block-header-text:contains('" + shippingText + i + "')").parent().parent().find(".line-container > .DefaultLabel").parent();
            splitTableElementsIntoTwoColumns(tableElements, lengthCounter);
        }
    } else {
        var searchString = blockName + " > .line-container > .DefaultLabel";
        tableElements = $(searchString).parent();
        splitTableElementsIntoTwoColumns(tableElements, lengthCounter);
    }
    $(blockName).css('opacity', 1);
}

function splitTableElementsIntoTwoColumns(tableElements, lengthCounter) {
    if (tableElements.length > lengthCounter) {
        var wrapped = tableElements.wrapAll("<div id='two-column-block-left' class='two-column-block-left' />");
        wrapped.parent().wrapAll("<div id='two-column-block-table' class='two-column-block-table' />");
        $("<div id='two-column-block-right' class='two-column-block-right' />").insertAfter(wrapped.parent());
        var rightBlock = wrapped.parent().parent().find(".two-column-block-right");

        // if there are only two elements on the page, just split it in two columns
        if ( tableElements.length === 2){
            tableElements.eq(1).appendTo(rightBlock);
        } else {
            var totalHeight = 0;
            var totalElements = 0;
            tableElements.each(function (index) {
                // calculate height of input element
                // skip error rows
                if (!$(this).hasClass("error-row")) {
                    totalHeight += $(this).children().eq(1).height();
                    totalElements++;
                }
            });

            // fix for RL-29760
            if (totalElements === 2) {
                tableElements.each(function (index) {
                    if (index !== 0) {
                        $(this).appendTo(rightBlock);
                    }
                });
                return;
            }

            // calculate height of one column
            var heightsLimit = (totalHeight - totalHeight % 2) / 2;

            var heightCounter = 0;
            var isWriteToRightColumn = false;
            tableElements.each(function (index) {
                if (isWriteToRightColumn) {
                    $(this).appendTo(rightBlock);
                } else if (index !== 0) {
                    if (heightCounter >= heightsLimit) {
                        $(this).appendTo(rightBlock);
                        isWriteToRightColumn = true;
                    }
                }
                if (!$(this).hasClass("error-row")) {
                    heightCounter += $(this).children().eq(1).height();
                }
            });
        }
    }
}


function clearTerms() {
    if (document.TheForm.TermsAndConditions) {
        if (document.scratch && document.scratch.TermsAndConditions) {
            document.TheForm.TermsAndConditions.value = " ";
        }
    }
}

// RL-29407
function clearUnappliedPromoCode() {
    var promoElement = document.getElementById("promotionCode");
    if (promoElement) {
        promoElement.value = "";
    }
}

function refreshTerms() {
    if (document.TheForm.TermsAndConditions) {
        if ((document.scratch) && (document.scratch.TermsAndConditions)) {
            if (document.TheForm.TermsAndConditions.value !== " ") {
                document.scratch.TermsAndConditions.value = document.TheForm.TermsAndConditions.value;
            }
            document.TheForm.TermsAndConditions.value = document.scratch.TermsAndConditions.value;
        }
    }
}

function handleKeyPress(e) {
    if (document.TheForm.keyPressTarget) {
        var keyCode = (window.Event) ? e.which : e.keyCode;
        if (keyCode == 13) {
            if (submitForm) {
                formSubmitted = true;
                document.TheForm.targetPage.value = document.TheForm.keyPressTarget.value;
                document.TheForm.target = "_top";
        	    document.TheForm.action = "/AppDispatchServlet";
                document.TheForm.isMyAccount.value = "false";
                clearTerms();
                document.TheForm.submit();
            }
        }
    }
}

function confirmBack(message) {
	var result = window.confirm(typeof rlConfirmBack !== 'undefined' ? rlConfirmBack : message);
	if (result == true) {
		history.back();
	}
}

async function doLogout() {
    document.TheForm.doCalc.value = "false";
    document.TheForm.targetPage.value = "logout";
    document.TheForm.loggedIn.value = "false";
    document.TheForm.ignoreErrors.value = "true";
    document.TheForm.passThroughTarget.value = "";
    document.TheForm.target = "_top";
    document.TheForm.action = "/AppDispatchServlet";
    document.TheForm.isMyAccount.value = "false";
    document.TheForm.buttonClicked.value = "logout";
    clearTerms();
    document.TheForm.submit();
}

function goHome() {
	if (confirmBeforeProceed === true) {
		showConfirm("home");
		return;
	}
	
    $('img[name=home]').parents('a').removeAttr('href'); // to click once
	if (document.TheForm) {

        gtag('event', 'Home button', {
            'event_category': 'Order flow'
        });

        if (document.TheForm.doCalc){
	    	document.TheForm.doCalc.value = "false";
	    }
	    if (document.TheForm.orderBeanReset) {
	    	document.TheForm.orderBeanReset.value = "true";
	    }
	    if (document.TheForm.targetPage) {
	    	document.TheForm.targetPage.value = "";
	    }
	    if (document.TheForm.passThroughTarget) {
	    	document.TheForm.passThroughTarget.value = "";
	    }
	    if (document.TheForm.ignoreErrors) {
	    	document.TheForm.ignoreErrors.value = "true";
	    }
	    document.TheForm.target = "_top";
	    if (document.TheForm.isMyAccount) {
	    	document.TheForm.isMyAccount.value = "false";
	    }
	    if (document.TheForm.buttonClicked) {
	    	document.TheForm.buttonClicked.value = "home";
	    }
	     document.TheForm.target = "_top";
	    document.TheForm.action = "/AppDispatchServlet";
	    
	    clearTerms();
	    document.TheForm.submit();		
	}
}

function goBack() {	
	if (confirmBeforeProceed === true) {
		showConfirm("back");
		return;
	}
	if (document.TheForm) {
		// when going back from the edit work page clear the publisher so that when clicking new work on the list page
		// the publisher field is not pre-popluated with the publisher of the work that was being editied.
	    if (document.TheForm.currentTargetPage.value === 'aboutyourwork-edit') {	    	
	    	document.TheForm.addNewPub.value = "";
	    	document.TheForm.currentTargetPage.value = "aboutyourwork-select";
	    	document.TheForm.ignoreErrors.value = "true";
	    	document.TheForm.submit();
	    } else if (document.TheForm.currentTargetPage.value === 'additionaldatapermissions') {	
	    	if (document.TheForm.previousTargetPage.value === 'aboutyourwork-enter') {	    		
	    		document.TheForm.targetPage.value = "aboutyourwork-enter";
	    		document.TheForm.previousTargetPage.value === 'additionaldatapermissions';
	    		document.TheForm.ignoreErrors.value = "true";
		    	document.TheForm.submit();	    			
	    	} else {
	    		window.history.back();
	    	}   		    	
	    } else {
	    	 window.history.back();
	    }	   	    
	} else {
		window.history.back();
	}			
}

function showConfirm(destination) {
	$.prompt('Please be advised any changes you have made to this form will be lost.\nPress Ok to continue, Cancel to remain on this page.', {
		title : 'Information',
		buttons:{Ok: true, Cancel: false},
		prefix : 'rightslink',
		submit:function(event, value , msg, f) {
			if (value) {				
				confirmBeforeProceed = false;
				if (destination === "back") {
					goBack();					
				} else if (destination === "home") {
					goHome();
				}				
				return true;
			}	else {
				return;
			}
		}
	});   	    
}

function closeWindow() {
    gtag('event', 'Close Window', {
        'event_category': 'Order flow',
        'event_label': 'Close Window'
    });
    var win = window.open('', '_self');
    win.close();
}

function aboutCCC() {
    document.TheForm.targetPage.value = "aboutccc";
    document.TheForm.ignoreErrors.value = "true";
    document.TheForm.target = "_top";
    document.TheForm.action = "/AppDispatchServlet";
    document.TheForm.isMyAccount.value = "false";
    clearTerms();
    document.TheForm.submit();
}

function viewAccount() {
	var currentAction = document.TheForm.action;
    document.TheForm.action = "/MyAccountDispatchServlet";
    document.TheForm.ignoreErrors.value = "true";
    document.TheForm.target = "_top";
    document.TheForm.popBackPage.value = "myaccountlogin";
    document.TheForm.isMyAccount.value = "true";
    document.TheForm.target = "_myAccount";
    document.TheForm.targetPage.value = "myaccountlogin";
    document.TheForm.buttonClicked.value = "accountinfo";
    clearTerms();
    document.TheForm.submit();
    refreshTerms();
    document.TheForm.action = currentAction;
}

function viewAccountHome() {
    var location = "/MyAccount";
    window.open(location, '_self');
}

function privacyLink() {
    document.TheForm.targetPage.value = "privacylink";
    document.TheForm.ignoreErrors.value = "true";
    document.TheForm.target = "_top";
    document.TheForm.action = "/AppDispatchServlet";
    document.TheForm.isMyAccount.value = "false";
    document.TheForm.linkClicked.value = "Privacy statement";
    clearTerms();
    document.TheForm.submit();
}

function forgot(textClicked) {
    document.TheForm.targetPage.value = "forgotpassworduserid";
    document.TheForm.ignoreErrors.value = "true";
    document.TheForm.target = "_top";
    document.TheForm.action = "/AppDispatchServlet";
    document.TheForm.isMyAccount.value = "false";
    if (textClicked) {
        document.TheForm.linkClicked.value = textClicked;
    } else {
        document.TheForm.linkClicked.value = "Forgot Password/User ID?";
    }
    document.TheForm.submit();
}

function submitInstructionPage() {
    document.TheForm.targetPage.value = arguments[0];
    document.TheForm.ignoreErrors.value = "true";
    document.TheForm.target = "_top";
    document.TheForm.action = "/AppDispatchServlet";
    document.TheForm.isMyAccount.value = "false";
    clearTerms();
    document.TheForm.submit();
}

function toggleCheckboxesBasedOnFirstCheckBox() {
    if (arguments.length > 0) {
        var firstCheckBox = document.getElementsByName(arguments[0])[0];
        if (firstCheckBox) {
            for ( i = 1; i < arguments.length; i++) {
                var checkbox = document.getElementsByName(arguments[i])[0];
                if (checkbox) {
                    if (firstCheckBox.checked) {
                        checkbox.checked = false;
                        checkbox.disabled = (firstCheckBox.checked);
                        checkbox.value = "";
                    } else {
                        if (checkbox.disabled === true) {
                            checkbox.disabled = false;
                            checkbox.value = "";
                        }
                    }
                }
            }
            if (firstCheckBox.checked) {
                if (document.TheForm.customReprintSelected) {
                    document.TheForm.customReprintSelected.value = "checked";
                }
                if (document.TheForm.disableValue) {
                    document.TheForm.disableValue.value = "disabled";
                }
                if (document.TheForm.excerptSelected) {
                    document.TheForm.excerptSelected.value = "";
                }
                if (document.TheForm.disclaimerLanguageSelected) {
                    document.TheForm.disclaimerLanguageSelected.value = "";
                }
                if (document.TheForm.textBoldingSelected) {
                    document.TheForm.textBoldingSelected.value = "";
                }
                if (document.TheForm.corporateLogoSelected) {
                    document.TheForm.corporateLogoSelected.value = "";
                }
                if (document.TheForm.photoInsertSelected) {
                    document.TheForm.photoInsertSelected.value = "";
                }
                if (document.TheForm.coverArtSelected) {
                    document.TheForm.coverArtSelected.value = "";
                }
                if (document.TheForm.thumbnailUpgradeCheckBox) {
                    document.TheForm.thumbnailUpgradeCheckBox.value = "";
                }
                if (document.TheForm.socialMediaRightsCheckBox) {
                    document.TheForm.socialMediaRightsCheckBox.value = "";
                }
            } else {
                if (document.TheForm.customReprintSelected) {
                    document.TheForm.customReprintSelected.value = "";
                }
                if (document.TheForm.disableValue) {
                    document.TheForm.disableValue.value = "";
                }
            }
        }
    }
}

function updateCheckBoxState() {
    toggleCheckboxesBasedOnFirstCheckBox('customReprintCheckBox', 'excerptCheckBox', 'disclaimerLanguageCheckBox', 'textBoldingCheckBox', 'corporateLogoCheckBox', 
        'photoInsertCheckBox','coverArtCheckBox');
}

function updateQuickPriceEstimateCheckBoxState() {
    if (document.TheForm.offerIDValue && (document.TheForm.offerIDValue.value == "NOTSELECTED" || document.TheForm.offerIDValue.value < 0)) {
        clearCheckBoxesInSessionStorage();
    }
    var formElements = document.TheForm.querySelectorAll("input[type='checkbox']");
    for (i = 0; i < formElements.length; i++) {
        var el = formElements[i];
        el.checked = 'checked' == sessionStorage.getItem('checkbox.' + el.name);
        el.defaultChecked = el.checked;
        if (el.checked) {
            $(el).prop("checked", true);
        } else {
            $(el).removeAttr("checked");
        }
    }
}

function disabledDropDown(name, disabledValues) {
	this.name = name;
	this.values = disabledValues;
}

function navigateToOriginalUrl() {
    $(".rightslinkexpiredsessionbox").css("display", "none");

    let initialUrl = document.TheForm.initialUrl.value;
    if (initialUrl != null && initialUrl != '' && initialUrl != 'null') {
        window.location.replace(initialUrl);
    }
}

function showExpiredSessionPrompt() {
    $(".rightslinkexpiredsessionbox").css("display", "block");
}

function startUserSessionTimer() {
    let userSessionTimeout = document.TheForm.userSessionTimeout.value;

    //If not specified, set the value to 20 minutes
    userSessionTimeout = (userSessionTimeout == null || userSessionTimeout == '' || userSessionTimeout == 'null') ?
        20 : userSessionTimeout;

    setTimeout(function () {
        showExpiredSessionPrompt();
    }, userSessionTimeout * 60 *1000);
}

function setup() {
    trapAll();
    refreshTerms();
    updateCheckBoxState();
    startUserSessionTimer();
}

function saveQuickPriceEstimateCheckBoxState() {
    if (sessionStorage.getItem('checkBoxesRemoved') !== 'true') {
        var formElements = document.TheForm.querySelectorAll("input[type='checkbox']");
        for (i = 0; i < formElements.length; i++) {
            var el = formElements[i];
            el.defaultChecked = el.checked;
            if (el.checked) {
                $(el).prop("checked", true);
                sessionStorage.setItem('checkbox.' + el.name, 'checked');
            } else {
                $(el).removeAttr("checked");
                sessionStorage.removeItem('checkbox.' + el.name);
            }
        }
    }
    sessionStorage.removeItem('checkBoxesRemoved');
}

function clearCheckBoxesInSessionStorage() {
    Object.keys(sessionStorage).filter(function (key) {
        return key.startsWith('checkbox.');
    }).forEach(function (key) {
        sessionStorage.removeItem(key);
    });
    sessionStorage.setItem('checkBoxesRemoved', 'true');
}

function doCasLogin() {

    // set target page as a current
    var targetPage = document.TheForm.targetPage;
    var currentTargetPage = document.TheForm.currentTargetPage;
    if (targetPage && currentTargetPage && currentTargetPage.value.length > 0) {
        targetPage.value = currentTargetPage.value;
    }
    // if attempt to login on initial page, than use "Home" button logic to avoid issues with incorrect result page
    // after CAS redirect
    if (targetPage.value == "quickprice" || targetPage.value == "createaccountstep1") {
        if (!(document.TheForm.offerIDValue) || (document.TheForm.offerIDValue.value < 0) || (document.TheForm.offerIDValue.value == "NOTSELECTED")) {
            if (document.TheForm.doCalc) {
                document.TheForm.doCalc.value = "false";
            }
            if (document.TheForm.orderBeanReset) {
                document.TheForm.orderBeanReset.value = "true";
            }
            if (document.TheForm.targetPage) {
                document.TheForm.targetPage.value = "";
            }
            if (document.TheForm.passThroughTarget) {
                document.TheForm.passThroughTarget.value = "";
            }
            if (document.TheForm.ignoreErrors) {
                document.TheForm.ignoreErrors.value = "true";
            }
            document.TheForm.target = "_top";
            if (document.TheForm.isMyAccount) {
                document.TheForm.isMyAccount.value = "false";
            }
            if (document.TheForm.buttonClicked) {
                document.TheForm.buttonClicked.value = "home";
            }
        }
    }

    var serviceUrl = getServiceUrl();
    document.TheForm.submit = function(){};
    createAccount();
    var registerUrl = getRegisterUrl();

    $.ajax("/AppDispatchServlet", {
        type: 'POST',
        cache: false,
        data: {
            'targetPage':'storeAfterCasLoginVariables',
            'serviceUrl': serviceUrl,
            'registerUrl': registerUrl
        },
        dataType: "json",
        async:false
    });
    window.location.replace("/cas/login?lang=" + getUserLang() +
        "&service=" + getCasActionUrl(serviceUrl ,"service") +
        "&register=" + getCasActionUrl(registerUrl ,"register"));
}

function getCasActionUrl (url, action) {
    return encodeURIComponent((url.indexOf('?') > 0 ? url.substring(0, url.indexOf('?')) : url) +
        "?action=" + action);
}

function doCasLogout() {
    $('img[name=logout]').parents('a').removeAttr('href'); // to click once
    document.TheForm.submit = function(){};
    doLogout().then(() =>  {
        window.location.replace("/cas/logout?service=" + encodeURIComponent(getServiceUrl()));
    });
}

function getRegisterUrl() {
    var register = getServiceUrl();
    var urlRegister = new URL(register);

    var params = new URLSearchParams(urlRegister.search);
    var firstTargetPageParam = params.get("targetPage");
    if ('quickpriceWRA' === firstTargetPageParam) {
        removeDuplicateParameterValues(params, "targetPage");
        removeDuplicateParameterValues(params, "orderBeanReset");
        register = urlRegister.protocol + '//' + urlRegister.host + urlRegister.pathname + '?' + params.toString();
    }
    return register;
}

function removeDuplicateParameterValues(params, parameterName) {
    var lastParamValue = params.getAll(parameterName)[params.getAll(parameterName).length - 1];
    params.delete(parameterName);
    params.append(parameterName, lastParamValue);
}

function getServiceUrl() {
    var s = removeAnchor(window.location.href);
    var i = s.indexOf('?');
    // In order to reduce the size of the link presented to CAS, eliminate 
    // original link parameters that are no longer needed
    s = removeParameter("author", s);
    s = removeParameter("title", s);
    s = removeParameter("copyright", s);
    s = removeParameter("contentID", s);
    s = removeParameter("contentId", s);
    s = removeParameter("contentid", s);
    return s + getQuery(i == -1 ? '?' : '&');
}

function removeParameter(param, source) {
	try {
		urlObject = new URL(source);
    	// try to remove original, upper, lower case, etc. param names
	    urlObject.searchParams.delete(param);
	    urlObject.searchParams.delete(param.toUpperCase());
	    urlObject.searchParams.delete(param.toLowerCase());
	    urlObject.searchParams.delete(upperCaseFirstChar(param));
        return urlObject.href;
	} catch (err) {
		var decodedUrl = decodeURIComponent(source);
		var index = decodedUrl.indexOf("?");
		if (index <= -1) {
			return source;
		}
		var newUrl = decodedUrl.slice(0,index+1);
		var queryStringUrl = decodedUrl.slice(index+1);  
		var paramArray = queryStringUrl.split("&");  
		var firstParamDone = false;
		var queryStringParam;
		for (var i in paramArray) {
		    queryStringParam = paramArray[i]; 
		    if (queryStringParam.indexOf(param) != 0 &&
				queryStringParam.indexOf(upperCaseFirstChar(param)) != 0 &&
				queryStringParam.indexOf(param.toUpperCase()) != 0 &&
				queryStringParam.indexOf(param.toLowerCase()) != 0) {
			    if (firstParamDone) {
					newUrl = newUrl + "&" + queryStringParam;		    	
			    } else {
			    	firstParamDone = true;
					newUrl = newUrl + queryStringParam;		    	
			    }
		    }
		}  	
		return newUrl;
	}
	return source;
}

function upperCaseFirstChar(s) {
	return s.charAt(0).toUpperCase() + s.slice(1);
}

function removeAnchor(s) {
    var i = s.indexOf('#');
    var i2 = s.indexOf('?');
    return i == -1 ? s : s.substring(0, i) + (i2 == -1 ? '' : s.substring(i2));
}

function getQuery(start) {
    var s = "";
    var formElements = document.TheForm.elements;
    for ( i = 0; i < formElements.length; i++) {
        var el = formElements[i];

/*        //removing empty parameters to fix CAS-client 3.4.1 behaviour
        if (el.value == null || el.value ==''){
            continue;
        }*/

        var notPresented = (window.location.href.indexOf(el.name + '=' + el.value) == -1);
        if (el.name != "" && notPresented && el.name != "orderThreadID") {
        	var $element = $("#"+el.name);
        	if ($element.is(':checkbox')) {
        		if ($element.prop('checked')) {
                    s += (i == 0 ? start : '&') + el.name + '=' + encodeURIComponent(el.value);      		        			
        		}
        	} else {
                s += (i == 0 ? start : '&') + el.name + '=' + encodeURIComponent(el.value);
        	}
        }
    }
    return s;
}

function getUserLang() {
    if (document.TheForm.languageSelection) {
        return document.TheForm.languageSelection.value.split("_")[0];
    }
    return (navigator.language || navigator.userLanguage).split("-")[0];
}

function goQuickPrice() {
    document.TheForm.doCalc.value = "false";
    document.TheForm.targetPage.value = "quickprice";
    document.TheForm.ignoreErrors.value = "true";
    document.TheForm.target = "_top";
    document.TheForm.isMyAccount.value = "false";
    document.TheForm.submit();
}

function paymentTerms() {
    var location = "/App/PaymentTermsAndConditions.jsp";
    window.open(location, '_Terms', 'location=no,toolbar=no,status=no,width=500,height=400,scrollbars=yes,resizable=yes');
}

// RL-30497
function encodePlusSymbol() {
    try {
        var elements = document.TheForm.querySelectorAll("input[type=text], textarea");
        Array.from(elements).forEach(function (element) {
            element.value = element.value.split("+").join("&#43;");
        });
    } catch (e) {
        console.log(e);
    }
}

// RL-31824
function trimTextFieldValues() {
    try {
        var elements = document.TheForm.querySelectorAll("input[type=text], textarea");
        Array.from(elements).forEach(function (element) {
            element.value = element.value.trim();
        });
    } catch (e) {
        console.log(e);
    }
}
