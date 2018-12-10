var args = new Object();
args = GetUrlParms(location.search.substring(1));
var _advWebTrackingPortal = "";
var _advWebTrackingEngagementLevel = "";
var _advWebTrackingPageType = "";
var _advWebTrackingContentID = "";
var _Email = "";
var Message = $.cookies.get('eStore_Adv_Webtracking');
if (Message != null) {
    _Email = Message;
}
else {
    _Email = "";
}
var _UID = args['uid'];
var _CampId = args['campid'];