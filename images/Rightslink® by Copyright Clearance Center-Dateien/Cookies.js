/*
just a simple little set of methods to set a default cookie 
(path, domain, security, expiration are all default values)
and to get a specific value from the cookie string
*/

function setDefaultCookie(name,value,secure)
{
    if (secure)
    {
        this.document.cookie = name + "=" + escape(value) + "; path=/ ;secure";
    }
    else
    {
        this.document.cookie = name + "=" + escape(value) + "; path=/";
    }
}

function getCookie(name)
{
    var myCookie = unescape(this.document.cookie);
    if (myCookie == "")
    {
        return "";
    }
    var start = myCookie.indexOf(name + "=");
    if (start== -1)
    { 
        return "";
    }
    start = start + name.length + 1;
    var end = myCookie.indexOf(";", start);
    if (end == -1)
    { 
        end = myCookie.length;
    }
    return myCookie.substring(start,end);
}



//removes cookie by setting expiration date in past
//Jeff Hayes	4/23/01
function removeCookie(name)
{
	var _cookie = name + '=; expires=Fri, 02-Jan-1970 00:00:00 GMT';
	document.cookie = _cookie;
}