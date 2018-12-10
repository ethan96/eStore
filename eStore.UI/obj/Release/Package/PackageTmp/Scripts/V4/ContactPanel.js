// JavaScript Document

$(function(){
	$('.slide-out-div').tabSlideOut({
	  tabHandle: '.handle',                     //class of the element that will become your tab
	  pathToTabImage: 'images/contact_tab.gif', //path to the image for the tab //Optionally can be set using css
	  imageHeight: '165px',                     //height of tab image           //Optionally can be set using css
	  imageWidth: '30px',                       //width of tab image            //Optionally can be set using css
	  tabLocation: 'right',                      //side of screen where tab lives, top, right, bottom, or left
	  speed: 300,                               //speed of animation
	  action: 'click',                          //options: 'click' or 'hover', action to trigger animation
	  topPos: '210px',                          //position from the top/ use if tabLocation is left or right
	  leftPos: '0px',                          //position from left/ use if tabLocation is bottom or top
	  fixedPosition: true                      //options: true makes it stick(fixed position) on scroll
	});
});
var  hasCookie = $.cookie('tabSliceOut' );
if(hasCookie !== '9999'){
  setTimeout(function(){
    $('a.handle').click();
  },1000);
  setTimeout(function(){
    $(document).click();
  },4000);
 $.cookie('tabSliceOut','9999',{ expires: 365, path: '/'  });
}else{
 $.removeCookie('tabSliceOut', {path: '/' });
 $.cookie('tabSliceOut','9999',{ expires: 365, path: '/'  });
}