// pay40.js
// function : Merchant Script & 傈磊瘤癌狼 Interface
// ㄏ 2006 INICIS.Co.,Ltd. All rights reserved.


var PLUGIN_SVR_NAME = "/wallet61/"
var PLUGIN_CLASSID = "<OBJECT ID=INIpay CLASSID=CLSID:24F6E6A8-852C-45A8-ADD3-C4AB0D6FD231 width=0 height=0 CODEBASE=http://plugin.inicis.com/wallet61/INIwallet61.cab#Version=1,0,0,1 onerror=OnErr()></OBJECT>"
var PLUGIN_ERRMSG =""

var VISA3D_INF = "12:13:01:14:04:03:34:42:45:51:52:33";
var IS_VISTA = 0;
var IS_64BIT = 0;

var ADD_KVP_FLAG = "ISP_CARD_INF=06:11&PUBCERT_FLAG=1001100110011001&PUBCERT_MSG=雀盔丛狼 救傈茄 傈磊惑芭贰甫 困秦 傍牢牢刘辑(陛蓝搬力盔 惯鞭)甫 烹茄 夯牢牢刘捞 鞘夸钦聪促.\n雀盔丛狼 傍牢牢刘辑甫 急琶窍脚饶 秦寸 厚剐锅龋甫 涝仿窍咯 林矫扁 官而聪促.\n傍牢牢刘辑啊 绝栏脚 版快, 陛蓝搬力盔俊辑 惯鞭窍绰 傍牢牢刘辑甫 脚没窍脚饶 捞侩秦 林绞矫坷.\n&PUBCERT_MSG2=傍牢 牢刘辑甫 荤侩窍矫摆嚼聪鳖?&KMPAY=300000&BCPAY=300000&URIPAY=300000&CHOPAY=300000&PUBIMG_URL=http://plugin.inicis.com/wallet00/files/&VISA_MSG=惫刮捞搁辑 厚磊老版快 3D拳搁栏肺 傈券 邓聪促.\n";
var NORMAL_INF = "21:31:35:43";
var KFTC_BANK_INFO = "04:11:20:23:03:05:07:88:27:31:32:34:35:37:39:81:71";
var VISA3D_PUBCERT_PRICE = "300000"

var JSINFO_NAME = "61_40sec"
       
function SetEnvironment()        
{
     if(navigator.userAgent.indexOf("Windows NT 6") > -1)
     {
	IS_VISTA=1;
	PLUGIN_CLASSID = "<OBJECT ID=INIpay CLASSID=CLSID:24F6E6A8-852C-45A8-ADD3-C4AB0D6FD231 width=0 height=0 CODEBASE=http://plugin.inicis.com/wallet61/INIwallet61_vista.cab#Version=1,0,0,1 onerror=OnErr()></OBJECT>"
	PLUGIN_ERRMSG = "绊按丛狼 救傈茄 搬力甫 困窍咯 搬力侩 鞠龋拳 橇肺弊伐狼 汲摹啊 鞘夸钦聪促.\n\n" +
			      "促澜 窜拌俊 蝶扼 柳青窍绞矫坷.\n\n\n" +
         			      "1. 宏扼快历(牢磐齿 劳胶敲肺绢) 惑窜狼 畴鄂祸 舅覆 钎矫临阑 付快胶肺 努腐 窍绞矫坷.\n\n" +
                                                "2. 'ActiveX 牧飘费 汲摹'甫 急琶窍绞矫坷.\n\n" +
                                                "3. 焊救 版绊芒捞 唱鸥唱搁 '汲摹'甫 喘矾辑 柳青窍绞矫坷.\n"	
      }
      else
      {
	IS_VISTA=0;
  	PLUGIN_CLASSID = "<OBJECT ID=INIpay CLASSID=CLSID:24F6E6A8-852C-45A8-ADD3-C4AB0D6FD231 width=0 height=0 CODEBASE=http://plugin.inicis.com/wallet61/INIwallet61.cab#Version=1,0,0,1 onerror=OnErr()></OBJECT>"
  	  	
  	if( navigator.userAgent.indexOf("Windows NT 5.1") > -1 || navigator.userAgent.indexOf("Windows NT 5.2") > -1 )
  	{
		PLUGIN_ERRMSG = "绊按丛狼 救傈茄 搬力甫 困窍咯 搬力侩 鞠龋拳 橇肺弊伐狼 汲摹啊 鞘夸钦聪促.\n\n" +
         				      "促澜 窜拌俊 蝶扼 柳青窍绞矫坷.\n\n\n" +
         				      "1. 宏扼快历(牢磐齿 劳胶敲肺绢) 惑窜狼 畴鄂祸 舅覆 钎矫临阑 付快胶肺 努腐 窍绞矫坷.\n\n" +
                                                              "2. 'ActiveX 牧飘费 汲摹'甫 急琶窍绞矫坷.\n\n" +
                                                              "3. 焊救 版绊芒捞 唱鸥唱搁 '汲摹'甫 喘矾辑 柳青窍绞矫坷.\n"
  	}
   	else 
   	{
   		PLUGIN_ERRMSG = "[INIpay傈磊瘤癌]捞 汲摹登瘤 臼疽嚼聪促.\n\n宏扼快历俊辑 [货肺绊魔]滚瓢阑 努腐窍脚 饶 [焊救版绊]芒捞 唱鸥唱搁 [抗]滚瓢阑 努腐窍技夸.";
              }		
      }
      
     if(window.navigator.appVersion.indexOf("Win64")>0 || window.navigator.appVersion.indexOf("WOW64")>0)
    {
 	IS_64BIT = 1;
     }        
}


function MakePayMessage(payform)
{
 document.INIpay.IFplugin(100, "INIpay", ""); 
  
  if(IS_VISTA==1)
  {
  	if(IS_64BIT == 1)
	{
		if(document.INIpay.IFplugin(0, PLUGIN_SVR_NAME, "inipay|00") == "ERROR") return false;
	}
	else
	{
	  	if(document.INIpay.IFplugin(0, PLUGIN_SVR_NAME, "inipay|10") == "ERROR") return false;
	}
  }
  else
  {
  	if(document.INIpay.IFplugin(0, PLUGIN_SVR_NAME, "inipay") == "ERROR") return false;
  }

  if(SetField(payform) == false) {
    document.INIpay.IFplugin(1, "", "");
    return false;
  }
  
  if(document.INIpay.IFplugin(4, "", "") == "ERROR") {
    document.INIpay.IFplugin(1, "", "");
    return false;
  }
  
  if(GetField(payform) == false) {
    document.INIpay.IFplugin(1, "", "");
    return false;
  }
	
  document.INIpay.IFplugin(1, "", "");
  
  return true;
}	


//Set Merchant Payment Field
function SetField(payform)
{
  var nField = payform.elements.length;
  
  for(i = 0; i < nField; i++)
  {
    if(payform.elements[i].name == "mid")
    {
	document.INIpay.IFplugin(2, "mid", payform.mid.value);
    }
    if(payform.elements[i].name == "nointerest")
    {
	document.INIpay.IFplugin(2, "nointerest", payform.nointerest.value);
    }
    if(payform.elements[i].name == "quotabase")
    {
	document.INIpay.IFplugin(2, "quotabase", payform.quotabase.value);
    }
    if(payform.elements[i].name == "price")
    {
	document.INIpay.IFplugin(2, "price", payform.price.value);
    }
    if(payform.elements[i].name == "currency")
    {
	document.INIpay.IFplugin(2, "currency", payform.currency.value);
    }
    if(payform.elements[i].name == "buyername")
    {
	document.INIpay.IFplugin(2, "buyername", payform.buyername.value);
    }
    if(payform.elements[i].name == "goodname")
    {
	document.INIpay.IFplugin(2, "goodname", payform.goodname.value);
    }
    if(payform.elements[i].name == "acceptmethod")
    {
	document.INIpay.IFplugin(2, "acceptmethod", payform.acceptmethod.value);
    }
    if(payform.elements[i].name == "gopaymethod")
    {
	if(payform.gopaymethod.value != "")
	    document.INIpay.IFplugin(2, "gopaymethod", payform.gopaymethod.value); 
    }

    if(payform.elements[i].name == "ini_encfield")
    {
	document.INIpay.IFplugin(2, "ini_encfield", payform.ini_encfield.value);
    }
    
    if(payform.elements[i].name == "ini_certid")
    {
	document.INIpay.IFplugin(2, "ini_certid", payform.ini_certid.value);
    } 
    if(payform.elements[i].name == "INIregno")
	{
		document.INIpay.IFplugin(2, "INIregno", payform.INIregno.value);
    }
    if(payform.elements[i].name == "oid")
	{
		document.INIpay.IFplugin(2, "oid", payform.oid.value);
    }
    if(payform.elements[i].name == "buyeremail")
    {
    	document.INIpay.IFplugin(2, "buyeremail", payform.buyeremail.value);
    }
    if(payform.elements[i].name == "ini_menuarea_url")
    {
    	document.INIpay.IFplugin(2, "menuareaimage_url", payform.ini_menuarea_url.value);
    }
    if(payform.elements[i].name == "ini_logoimage_url")
    {
    	document.INIpay.IFplugin(2, "logoimage_url", payform.ini_logoimage_url.value);
    }
    if(payform.elements[i].name == "ini_bgskin_url")
    {
    	document.INIpay.IFplugin(2, "ini_bgskin_url", payform.ini_bgskin_url.value);
    }        
    if(payform.elements[i].name == "mall_noint")
    {
    	document.INIpay.IFplugin(2, "mall_noint", payform.mall_noint.value);
    }
    if(payform.elements[i].name == "ini_onket_flag")
    {
    	document.INIpay.IFplugin(2, "onket_flag", payform.ini_onket_flag.value);
    }
    if(payform.elements[i].name == "ini_pin_flag")
    {
    	document.INIpay.IFplugin(2, "pin_flag", payform.ini_pin_flag.value);
    }
    if(payform.elements[i].name == "buyertel")
    {
    	document.INIpay.IFplugin(2, "buyertel", payform.buyertel.value);
    }
    if(payform.elements[i].name == "ini_escrow_dlv")
    {
    	document.INIpay.IFplugin(2, "ini_escrow_dlv", payform.ini_escrow_dlv.value);
    }
    if(payform.elements[i].name == "ansim_cardnumber")
    {
    	document.INIpay.IFplugin(2, "ansim_cardnumber", payform.ansim_cardnumber.value);
    }
    if(payform.elements[i].name == "ansim_expy")
    {
    	document.INIpay.IFplugin(2, "ansim_expy", payform.ansim_expy.value);
    }
    if(payform.elements[i].name == "ansim_expm")
    {
    	document.INIpay.IFplugin(2, "ansim_expm", payform.ansim_expm.value);
    }
    if(payform.elements[i].name == "ansim_quota")
    {
    	document.INIpay.IFplugin(2, "ansim_quota", payform.ansim_quota.value);
    }
    if(payform.elements[i].name == "ini_onlycardcode")
    {
    	document.INIpay.IFplugin(2, "ini_onlycardcode", payform.ini_onlycardcode.value);
    }
    if(payform.elements[i].name == "ESCROW_LOGO_URL") {
      	document.INIpay.IFplugin(2, "ESCROW_LOGO_URL", payform.ESCROW_LOGO_URL.value);
    }
    if(payform.elements[i].name == "KVP_OACERT_INF") {
      	document.INIpay.IFplugin(2, "reserved6", payform.KVP_OACERT_INF.value);
    }
  }
	
  document.INIpay.IFplugin(2, "version", payform.version.value);      
  document.INIpay.IFplugin(2, "reqsign", payform.reqsign.value);
  document.INIpay.IFplugin(2, "ADD_KVP_FLAG", ADD_KVP_FLAG);
  document.INIpay.IFplugin(2, "visa3d_inf", VISA3D_INF);
  document.INIpay.IFplugin(2, "visa3d_pubcert_price", VISA3D_PUBCERT_PRICE);
  document.INIpay.IFplugin(2, "NORMAL_INF", NORMAL_INF);
  document.INIpay.IFplugin(2, "reserved3", KFTC_BANK_INFO);
  document.INIpay.IFplugin(2, "plugin_jsinfo", JSINFO_NAME);    
  document.INIpay.IFplugin(2, "plugin_cardtype", "1");

  return true;
}	


//Get PayMessage made
function GetField(payform)
{
  var nField = payform.elements.length;
  
  if((payform.paymethod.value = document.INIpay.IFplugin(3, "paymethod", "")) == "ERROR") {
  	 return false;
  	}
  if(payform.paymethod.value == "") {
  	return false;
  }
  if((payform.sessionkey.value = document.INIpay.IFplugin(3, "sessionkey", "")) == "ERROR") {
  	return false;
  }
  if(payform.sessionkey.value == "") {
  	return false;
  }
  if((payform.encrypted.value = document.INIpay.IFplugin(3, "encrypted", "")) == "ERROR") {
  	return false;
  }
  if(payform.encrypted.value == "") {
  	return false;
  }
  if((payform.uid.value = document.INIpay.IFplugin(3, "uid", "")) == "ERROR") {
  	return false;
  }
  if(payform.paymethod.value == "DirectBank")
  {
		if((payform.rbankcode.value = document.INIpay.IFplugin(3, "realbankcode", "")) == "ERROR") {
			alert("error bankcode");
			return false;
		}
  }
 

  for(i = 0; i < nField; i++)
  {
    if(payform.elements[i].name == "cardcode")
    {
    	payform.cardcode.value = document.INIpay.IFplugin(3, "cardcode", "");
    } 
    if(payform.elements[i].name == "cardquota")
    {
    	payform.cardquota.value = document.INIpay.IFplugin(3, "cardquota", "");
    }  	
    if(payform.elements[i].name == "quotainterest")
    {
        payform.quotainterest.value = document.INIpay.IFplugin(3, "quotainterest", "");
    }
    if(payform.elements[i].name == "buyeremail")
    {
        payform.buyeremail.value = document.INIpay.IFplugin(3, "buyeremail", "");
    }
    if(payform.paymethod.value == "VCard")
    { 
    	if(payform.elements[i].name == "ispcardcode")
      	 	payform.ispcardcode.value = document.INIpay.IFplugin(3,"vcard_cardcode",""); 
    	if(payform.elements[i].name == "kvp_card_prefix")
      	 	payform.kvp_card_prefix.value = document.INIpay.IFplugin(3,"vcard_prefix","");   	    
    }  		
  }

  return true;
}




function StartSmartUpdate()
{
	SetEnvironment();
	 document.writeln(PLUGIN_CLASSID);
}


function OnErr()
{
  alert(PLUGIN_ERRMSG);
  //return false;
}

