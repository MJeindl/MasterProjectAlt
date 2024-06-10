/***************************************************
BrowserInfo.js
JavaScript class library

This code provides the browser info pertinent to 
facilitating cross browser development in an object 
based way.

John Boyea

Copyright 1999 Copyright Clearance Center
***************************************************/

var browserInfo = new BrowserInfo();

function BrowserInfo()
{
  this.appName = navigator.appName;
  this.appVersion = parseInt(navigator.appVersion);
  this.isNetscape = ((this.appName == "Netscape") && (this.appVersion >= 4));
  this.isNetscape4 = ((this.appName == "Netscape") && (this.appVersion == 4));
  this.isNetscape5 = ((this.appName == "Netscape") && (this.appVersion == 5));
  this.isNetscape6 = ((this.appName == "Netscape") && (this.appVersion == 6));
  
  //Netscape6  shows version as "5". 
  this.isNetscape6 = ((this.appName == "Netscape") && (this.appVersion >= 5));
  
  this.isIE = ((this.appName == "Microsoft Internet Explorer") && (this.appVersion >= 4));
  this.isIE4 = (navigator.userAgent.indexOf('MSIE 4') > 0);
  this.isIE5 = (navigator.userAgent.indexOf('MSIE 5') > 0);
  if (this.isIE5)
  {
    this.appVersion = 5;
  }
  
  if (this.isNetscape)
  {
    this.divBase = "document.";
    this.styleObj = "";
    this.minimumConfig = true;
  }
  else if (this.isIE)
  {
    this.divBase = "document.all.";
    this.styleObj = ".style";
    this.minimumConfig = true;
  }
  else
  {
    this.minimumConfig = false;
  }
}