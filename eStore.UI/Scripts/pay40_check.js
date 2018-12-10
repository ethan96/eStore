function StartSmartUpdate()
{
     if(navigator.userAgent.indexOf("Windows NT 6") > -1)
     {
	document.writeln("<OBJECT ID=INIpay CLASSID=CLSID:24F6E6A8-852C-45A8-ADD3-C4AB0D6FD231 width=0 height=0 CODEBASE=http://plugin.inicis.com/wallet61/INIwallet61_vista.cab#Version=1,0,0,1 onerror=OnErr()></OBJECT>");
      }
      else
      {
	document.writeln("<OBJECT ID=INIpay CLASSID=CLSID:24F6E6A8-852C-45A8-ADD3-C4AB0D6FD231 width=0 height=0 CODEBASE=http://plugin.inicis.com/wallet61/INIwallet61.cab#Version=1,0,0,1 onerror=OnErr()></OBJECT>");  	
      }  	
}