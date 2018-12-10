$(function(){
        
  if($(window).width() > 767 ){
    $(".eStore_menuLink_linkList_block").each(function(){
      var $this = $(this),
          $thisLink = $this.find("a"),
          $thisList = $this.find("ol");
      if ($thisList.length > 0) {
        $thisLink.find("i").show();
      }
    })

    $(".eStore_menuLink_link").mouseenter(function(){
      $(this).find(".eStore_menuLink_linkList").show();
      $(this).addClass("onFocus");
    });
    $(".eStore_menuLink_link").mouseleave(function(){
      $(this).find(".eStore_menuLink_linkList").hide();
      $(this).removeClass("onFocus");
    })


    $(".eStore_menuLink_linkList_block").mouseenter(function(){
      $(this).find("ol").show();
      $(this).addClass("onFocus");
    })

    $(".eStore_menuLink_linkList_block").mouseleave(function(){
      $(this).find("ol").hide();
      $(this).removeClass("onFocus");
    })

    
  }else if($(window).width() <=767){
     $(".eStore_menuLink_link").click(function(){
      $(this).find(".eStore_menuLink_linkList").toggle();
      $(this).toggleClass("show");
     })
  }
  
  var navOpen = false;
  $(".eStore_mobile .eStore_seeMore").click(function(){
      if(navOpen == false){
        $(".eStore_wrapper,.eStore_footer").stop().animate({left: -251}, 500);
        $(".eStore_wrapper").css("overflow", "visible");
        hideMenuH();
        navOpen = true; 
      }else {
        $(".eStore_wrapper,.eStore_footer").stop().animate({left: 0}, 500,function(){
          $(".eStore_wrapper").css("overflow", "hidden");
        });
        hideMenuH();
        navOpen = false;
        
      }   
    })
  


function hideMenuH() {
    $(".eStore_headerBottom,.eStore_wrapper").css({
        "min-height": "0"
    });
    var n = $(".eStore_headerBottom").outerHeight(!0),
        t = $(".eStore_wrapper").height(),
        i = n > t ? n : t;
    $(".eStore_headerBottom,.eStore_wrapper").css({
        "min-height": i
    })
}




  
})


