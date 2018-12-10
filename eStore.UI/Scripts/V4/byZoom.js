// JavaScript Document
$(function(){
	var menuLeave,menuLeave2,menuEnter,mouseStyle;

	//** article zoom center **
	/*$(".eStore_product_picSmall li img").each(function(){
		var imgH = $(this).height();
		//console.log(imgH);
		var paddingT = (40-imgH)/2;
		$(this).css("padding-top",paddingT);
	});*/
	
	
	//** article productPic**
	$(".eStore_product_picSmall li").click(function(){
		var imgH;
		$(".eStore_product_picBig .eStore_product_picBigImg").css({
			"padding-top":0,
			"padding-bottom":0
		});
		$(this).addClass("on").siblings().removeClass("on");
        var src = $(this).find("img").attr('msrc') || $(this).find("img").attr('src');
        $(".eStore_product_picBigImg").one("load", function() {
			imgH = $(".eStore_product_picBigImg").height();
			console.log(imgH);
			var paddingT = (200-imgH)/2;
			$(".eStore_product_picBig .eStore_product_picBigImg").css({
				"padding-top":paddingT,
				"padding-bottom":paddingT
			});
		}).attr('src', src);
		var dataBG = $(this).find("img").attr('data-BG');
		//$(".eStore_article_picBig a.d1").attr('href', dataBG);
		$(".eStore_product_picZoom img").attr('src', dataBG);		
    });
	
	
	//** zoom**
	picX = $(".eStore_product_picBig").offset().left;
	picY = $(".eStore_product_picBig").offset().top;
	$(".eStore_product_picBig").mousemove(function( event ){
	  //$( ".eStore_article_focusBlock" ).text( "pageX: " + event.pageX + ", pageY: " + event.pageY + ", picX: " + picX + ", picY: " + picY );
	  	var pageX = event.pageX;
		var pageY = event.pageY;
		var mouseX = pageX-picX;
		var mouseY = pageY-picY;
		
		$(".eStore_product_focusBlock").css({
				left:mouseX,
				top:mouseY
		});	
		
		if( pageX < picX+50 ){
			$(".eStore_product_focusBlock").css({
				left:50,
				right:""
			});	
		} 
		if( pageY < picY+50 ){
			$(".eStore_product_focusBlock").css({
				top:50,
				bottom:""
			});	
		}
		if( pageX > picX+150 ){
			$(".eStore_product_focusBlock").css({
				left:"",
				right:0
			});	
		}
		if( pageY > picY+150 ){
			$(".eStore_product_focusBlock").css({
				top:"",
				bottom:0
			});	
		}
		//上面區塊針對滑鼠移動的透明框
		var bigH = $(".eStore_product_picZoom img").height();
		//console.log(bigH);
		var paddingBigT = (400-bigH)/2;
		$(".eStore_product_picZoom img").css({
			"padding-top":paddingBigT,
			"padding-bottom":paddingBigT
		});
		focusX = $(".eStore_product_focusBlock").offset().left;
		focusY = $(".eStore_product_focusBlock").offset().top;
		focusX = focusX-picX;
		focusY = focusY-picY;
		$(".eStore_product_picZoom img").css({
			"left": -(400*(focusX/200)),
			"top": -(400*(focusY/200))
		});
		//針對放大的圖片
	});
	
	$(".eStore_product_picBig")
	.mouseover(function() {
		if(device.mobile() || device.tablet() || $(".eStore_mobile .eStore_seeMore").is(':visible')){
			return;
		}
		$(".eStore_product_focusBlock,.eStore_product_picZoom").show();
	})
	.mouseout(function() {
		$(".eStore_product_focusBlock,.eStore_product_picZoom").hide();
	});
	
});