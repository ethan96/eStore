/*
* JTip
* By Cody Lindley (http://www.codylindley.com)
* Under an Attribution, Share Alike License
* JTip is built on top of the very light weight jquery library.
*/

//on page load (as soon as its ready) call JT_init
$(document).ready(JT_init);

function JT_init() {
    $(".jTipProductDetail")
		   .hover(function () {
		       $('#JT').remove(); ;
		       var url = GetStoreLocation() + "proc/html.aspx?type=ProductDetailTip&ProductID=" + $(this).attr("name");
		       JT_show(url, this.id, $(this).attr("name"));
		   }, function () { $('#JT').remove() });

    $(".jTipProductDetailWithoutImage")
		   .hover(function () {
		       $('#JT').remove(); ;
		       var url = GetStoreLocation() + "proc/html.aspx?type=ProductDetailTip&showimage=false&ProductID=" + $(this).attr("name");
		       JT_show(url, this.id, $(this).attr("name"));
		   }, function () { $('#JT').remove() });
    $(".eStoreHelper")
		   .hover(function () {
		       var url = GetStoreLocation() + "proc/html.aspx?type=eStoreHelper&helperID=" + $(this).attr("name");
		       JT_show(url, this.id, $(this).html());
		   }, function () { $('#JT').remove() });
}
function JT_show(url, linkId, title) {
    if (title == false) title = "&nbsp;";
    var de = document.documentElement;
    var w = self.innerWidth || (de && de.clientWidth) || document.body.clientWidth;
    var hasArea = w - getAbsoluteLeft(linkId);
    var clickElementy = getAbsoluteTop(linkId) - 3; //set y position

    var queryString = url.replace(/^[^\?]+\??/, '');
    var params = parseQuery(queryString);
    if (params['width'] === undefined) { params['width'] = 320 };
    if (params['link'] !== undefined) {
        $('#' + linkId).bind('click', function () { window.location = params['link'] });
        $('#' + linkId).css('cursor', 'pointer');
    }

    if (hasArea > ((params['width'] * 1) + 75)) {
        $("body").append("<div id='JT' style='width:" + params['width'] * 1 + "px'><div id='JT_close_left'>" + title + "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //right side
        var arrowOffset = getElementWidth(linkId) + 11;
        var clickElementx = getAbsoluteLeft(linkId) + arrowOffset-12; //set x position
    } else {
        $("body").append("<div id='JT' style='width:" + params['width'] * 1 + "px'><div id='JT_close_right'>" + title + "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //left side
        var clickElementx = getAbsoluteLeft(linkId) - ((params['width'] * 1) + 15); //set x position
    }

    $('#JT').css({ left: clickElementx + "px", top: clickElementy + "px" });
    $('#JT').show();
    $('#JT_copy').load(url, function () {
        var h = self.clientHeight || ($(window).height() + $(document).scrollTop());
        if (clickElementy + $('#JT_copy').height() > h - 15)
        { $('#JT').css({ left: clickElementx + "px", top: (h - $('#JT_copy').height() - 15) + "px" }); }
    });
}
function JT_showPro(url, linkId, title, parentCon) {
    if (title == false) title = "&nbsp;";
    var de = document.documentElement;
    var w = self.innerWidth || (de && de.clientWidth) || document.body.clientWidth;
    var hasArea = w - getAbsoluteLeft(linkId);
    var clickElementy = getAbsoluteTop(linkId) - 3; //set y position
    var areaHeight = getAbsoluteTop("ui-id-2");

    var queryString = url.replace(/^[^\?]+\??/, '');
    var params = parseQuery(queryString);
    if (params['width'] === undefined) { params['width'] = 320 };
    if (params['link'] !== undefined) {
        $('#' + linkId).bind('click', function () { window.location = params['link'] });
        $('#' + linkId).css('cursor', 'pointer');
    }
    if (hasArea > ((params['width'] * 1) + 75)) {
        parentCon.append("<div id='JT' class='searchResult_product_content' style='display: block;'><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //right side
        var arrowOffset = getElementWidth(linkId) + 11;
        var clickElementx = getAbsoluteLeft(linkId) + arrowOffset - 12; //set x position
    } else {
        parentCon.append("<div id='JT' class='searchResult_product_content' style='display: block;'><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //left side
        var clickElementx = getAbsoluteLeft(linkId) - ((params['width'] * 1) + 15); //set x position
    }

    $('#JT').css({ top: clickElementy - areaHeight + "px"});
    $('#JT').show();
    $('#JT_copy').load(url, function () {
        var h = self.clientHeight || ($(window).height() + $(document).scrollTop());
        if (clickElementy + $('#JT_copy').height() > h - 15) { $('#JT').css({ left: "auto", top: (h - $('#JT_copy').height() - areaHeight) + "px", right: "100%" }); }
    });

    //if (hasArea > ((params['width'] * 1) + 75)) {
    //    parentCon.append("<div id='JT' style='width:" + params['width'] * 1 + "px'><div id='JT_close_left'>" + title + "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //right side
    //    var arrowOffset = getElementWidth(linkId) + 11;
    //    var clickElementx = getAbsoluteLeft(linkId) + arrowOffset - 12; //set x position
    //} else {
    //    parentCon.append("<div id='JT' style='width:" + params['width'] * 1 + "px'><div id='JT_close_right'>" + title + "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //left side
    //    var clickElementx = getAbsoluteLeft(linkId) - ((params['width'] * 1) + 15); //set x position
    //}

    //$('#JT').css({ left: "auto", top: clickElementy + "px", right: "100%" });
    //$('#JT').show();
    //$('#JT_copy').load(url, function () {
    //    var h = self.clientHeight || ($(window).height() + $(document).scrollTop());
    //    if (clickElementy + $('#JT_copy').height() > h - 15) { $('#JT').css({ left: "auto", top: (h - $('#JT_copy').height()) + "px", right: "100%" }); }
    //});
}
function JT_showContent(content, linkId, title) {
    JT_showContent(content, linkId, title, null);
}
function JT_showContent(content, linkId, title, params) {
    if (title == false) title = "&nbsp;";
    var de = document.documentElement;
    var w = self.innerWidth || (de && de.clientWidth) || document.body.clientWidth;
    var hasArea = w - getAbsoluteLeft(linkId);
    var clickElementy = getAbsoluteTop(linkId) - 3; //set y position
    if (params == null)
        params = new Object();
    if (params['width'] === undefined) { params['width'] = 320 };
    if (params['link'] !== undefined) {
        $('#' + linkId).bind('click', function () { window.location = params['link'] });
        $('#' + linkId).css('cursor', 'pointer');
    }

    if (hasArea > ((params['width'] * 1) + 75)) {
        $("body").append("<div id='JT' style='width:" + params['width'] * 1 + "px'><div id='JT_close_left' class='homepagepopuptitle'>" + title + "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //right side
        var arrowOffset = getElementWidth(linkId) ;
        var clickElementx = getAbsoluteLeft(linkId) + arrowOffset; //set x position
    } else {
        $("body").append("<div id='JT' style='width:" + params['width'] * 1 + "px'><div id='JT_close_right'  class='homepagepopuptitle'>" + title + "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"); //left side
        var clickElementx = getAbsoluteLeft(linkId) - ((params['width'] * 1) + 15); //set x position
    }

    $('#JT').css({ left: clickElementx + "px", top: clickElementy + "px" });
    $('#JT').show();
     $('#JT_copy').html(content);
     var h = self.clientHeight || ($(window).height()+$(document).scrollTop());
     if ($('#JT_copy').height() > ($(window).height())) {
         { $('#JT').css({ left: clickElementx + "px", top:  $(document).scrollTop()+"px" }); }
     }
     else if (clickElementy + $('#JT_copy').height() > h - 45)
        { $('#JT').css({ left: clickElementx + "px", top: (h - $('#JT_copy').height() - 45) + "px" }); }


}

function getElementWidth(objectId) {
    x = document.getElementById(objectId);
    return x.offsetWidth;
}

function getAbsoluteLeft(objectId) {
    // Get an object left position from the upper left viewport corner
    o = document.getElementById(objectId)
    oLeft = o.offsetLeft            // Get left position from the parent object
    while (o.offsetParent != null) {   // Parse the parent hierarchy up to the document element
        oParent = o.offsetParent    // Get parent object reference
        oLeft += oParent.offsetLeft // Add parent left position
        o = oParent
    }
    return oLeft
}

function getAbsoluteTop(objectId) {
    // Get an object top position from the upper left viewport corner
    o = document.getElementById(objectId)
    oTop = o.offsetTop            // Get top position from the parent object
    while (o.offsetParent != null) { // Parse the parent hierarchy up to the document element
        oParent = o.offsetParent  // Get parent object reference
        oTop += oParent.offsetTop // Add parent top position
        o = oParent
    }
    return oTop
}

function parseQuery(query) {
    var Params = new Object();
    if (!query) return Params; // return empty object
    var Pairs = query.split(/[;&]/);
    for (var i = 0; i < Pairs.length; i++) {
        var KeyVal = Pairs[i].split('=');
        if (!KeyVal || KeyVal.length != 2) continue;
        var key = unescape(KeyVal[0]);
        var val = unescape(KeyVal[1]);
        val = val.replace(/\+/g, ' ');
        Params[key] = val;
    }
    return Params;
}

function blockEvents(evt) {
    if (evt.target) {
        evt.preventDefault();
    } else {
        evt.returnValue = false;
    }
}