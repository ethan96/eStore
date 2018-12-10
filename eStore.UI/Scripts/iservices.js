// JavaScript Document

var running,runningN,bannerN,myVar,myVar2;

$(function(){
	
	bannerN = $(".medical-home-banner-img div").length;
	var width = 980*bannerN;
	$(".medical-home-banner-img").css("width",width);
	
	for( i = 1 ; i <= bannerN ; i++){
		$(".medical-home-banner-point").append( "<span></span>" );
		$(".medical-home-banner-point span").eq(0).addClass("active");
	}
	
	$(".medical-home-banner-point span").click(function(){
		
		clearInterval(running);
		var p = $(this).index();
		$(this).addClass("active").siblings().removeClass("active");
		$(".medical-home-banner-img").stop().animate({
			"left":-(980*p)
		},700);
		
		running = setInterval("nextClick()", 5000);
	});

	running = setInterval("nextClick()", 5000);
	
	// ****************** 首頁bannner的動畫 ******************
	
	
	$(".medical-home-col.medical-col-left").hover(function() {
		clearHover();
		clearTimeout(myVar2);//每次滑出時都清除秒數，確保下次從零開始
		var $that = $(this).find("a.txt"); 
		$(this).css({
			"z-index":2
		});
		myVar = setTimeout(function(){bannerHover($that)},300);
	  }, function() {
		clearTimeout(myVar);//每次滑出時都清除秒數，確保下次從零開始
		var $that = $(this).find("a.txt");
		myVar2 = setTimeout(function(){bannerMouseOut($that)},250);  
	  }
	);
	$(".medical-home-col2.medical-col-left").hover(function() {
		clearHover();
		clearTimeout(myVar2);//每次滑出時都清除秒數，確保下次從零開始
		var $that = $(this).find("a.txt"); 
		$(this).css({
			"z-index":2
		});
		myVar = setTimeout(function(){bannerHover2($that)},300);
	  }, function() {
		clearTimeout(myVar);//每次滑出時都清除秒數，確保下次從零開始
		var $that = $(this).find("a.txt");
		myVar2 = setTimeout(function(){bannerMouseOut($that)},250);  
	  }
	);
	

});

function nextClick(){

	runningN = $(".medical-home-banner-point span.active").index();
	
	if( runningN < bannerN-1 ){ runningN = runningN+1; }
	else if( runningN = bannerN-1){ runningN = 0; }
	
	$(".medical-home-banner-point span").eq(runningN).addClass("active").siblings().removeClass("active");
	$(".medical-home-banner-img").stop().animate({
		"left":-(980*runningN)
	},700);
	
};

function bannerHover($obj){
	$obj.css({
	   "background-color":"#dfeef9",
	    
	   "z-index":2,
	   "width":409,
	   "height":228,
	   "top":-4,
	   "left":-4,
	   "box-shadow":"4px 4px 2px #888",
	   "padding":13
	});
	$obj.find(".medical-home-detail").css({
		"opacity":1,
		"z-index":3
	});
	$obj.siblings("a").css({
		"opacity":1,
		"z-index": 3,
		"display": "block"
	});
};
function bannerHover2($obj){
	$obj.css({
	   "background-color":"#dfeef9",
	  
	   "z-index":2,
	   "width":669,
	   "height":228,
	   "top":-4,
	   "left":-4,
	   "box-shadow":"4px 4px 2px #888",
	   "padding":13
	});
	$obj.find(".medical-home-detail").css({
		"opacity":1,
		"z-index":3
	});
	$obj.siblings("a").css({
		"opacity":1,
		"z-index":3,
		"display": "block"
	});
};


function bannerMouseOut($obj){
	$obj.css({
		   "background-color":"#fff",
		   
		   "z-index":"",
		   "width":"",
		   "height":"",
		   "top":0,
		   "left":0,
		   "box-shadow":"" ,
		   "padding":""
		});
		$obj.find(".medical-home-detail").css({
			"opacity":"",
			"z-index":""
		});
		$obj.siblings("a").css({
			"opacity":"",
			"z-index": "",
			"display": "none"
		});
};

function clearHover(){
	//alert(1);
	$(".medical-home-col,.medical-home-col2").css({
		"z-index":""
	});
	$(".medical-home-col,.medical-home-col2").find("a.txt").css({
	   "background-color":"#fff",
	  
	   "z-index":"",
	   "width":"",
	   "height":"",
	   "top":0,
	   "left":0,
	   "box-shadow":"" ,
	   "padding":""
	});
	$(".medical-home-col,.medical-home-col2").find(".medical-home-detail").css({
		"opacity":"",
		"z-index":""
	});
	$(".medical-home-col,.medical-home-col2").find("a.btn").css({
		"opacity":"",
		"z-index": "",
          "display":"none"
	});
};
