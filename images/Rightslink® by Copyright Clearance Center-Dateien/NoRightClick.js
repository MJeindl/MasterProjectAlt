function right(e) 
{
    var msg = "Sorry, you don't have permission to right-click.";
    if (navigator.appName == 'Netscape' && e.which == 3) 
    {
        alert(msg);
        return false;
    }
    if (navigator.appName == 'Microsoft Internet Explorer' && event.button==2) 
    {
        alert(msg);
        return false;
    }
    else 
        return true;
}

// netscape 6 is seemingly smart enough to not allow javascript 
// links to be opened in another window
function trap() 
{
    if (!browserInfo.isNetscape6)
	{
	    if(document.links)
        {
            for(i=0;i<document.links.length;i++)
            {
                document.links[i].onmousedown = right;
            }
		}
    }
}

// trap links embedded in div tags (necessary for netscape4)
// only goes one layer(div) down - would need to be revised if we begin
// using layers inside layers inside layers, etc
function trapAll()
{
   if (browserInfo.isNetscape4)
   {
       theLayers = document.layers;
	   if (theLayers)
       {
            for(x=0; x < theLayers.length; x++)
            {
		        theLinks = theLayers[x].document.links;
			    if (theLinks)
                {
                    for(i=0; i < theLinks.length; i++)
                    {
                        theLinks[i].onmousedown = right;
                    }
				}
		    }
        }
	}
	trap();
}	


