// JavaScript Document
$(function(){
	var menuLeave,menuLeave2,menuEnter,mouseStyle;

	//** article zoom center **
	$(".eStore_article_picSmall li img").each(function(){
		var imgH = $(this).height();
		//console.log(imgH);
		var paddingT = (40-imgH)/2;
		$(this).css("padding-top",paddingT);
	});
	
	
	//** article productPic**
	$(".eStore_article_picSmall li").click(function(){
		$(this).addClass("on").siblings().removeClass("on");
        var src = $(this).find("img").attr('src');
        $(".eStore_article_picBigImg").attr('src', src);
		var dataBG = $(this).find("img").attr('data-BG');
		//$(".eStore_article_picBig a.d1").attr('href', dataBG);
		$(".eStore_article_picZoom img").attr('src', dataBG);
		var imgH = $(".eStore_article_picBigImg").height();
		//console.log(imgH);
		var paddingT = (200-imgH)/2;
		$(".eStore_article_picBig").css({
			"padding-top":paddingT,
			"padding-bottom":paddingT
		});
    });
	
	
	//** zoom**
	picX = $(".eStore_article_picBig").offset().left;
	picY = $(".eStore_article_picBig").offset().top;
	$(".eStore_article_picBig").mousemove(function( event ){
	  //$( ".eStore_article_focusBlock" ).text( "pageX: " + event.pageX + ", pageY: " + event.pageY + ", picX: " + picX + ", picY: " + picY );
	  	var pageX = event.pageX;
		var pageY = event.pageY;
		var mouseX = pageX-picX;
		var mouseY = pageY-picY;
		
		$(".eStore_article_focusBlock").css({
				left:mouseX,
				top:mouseY
		});	
		
		if( pageX < picX+50 ){
			$(".eStore_article_focusBlock").css({
				left:50,
				right:""
			});	
		} 
		if( pageY < picY+50 ){
			$(".eStore_article_focusBlock").css({
				top:50,
				bottom:""
			});	
		}
		if( pageX > picX+150 ){
			$(".eStore_article_focusBlock").css({
				left:"",
				right:0
			});	
		}
		if( pageY > picY+150 ){
			$(".eStore_article_focusBlock").css({
				top:"",
				bottom:0
			});	
		}
		//上面區塊針對滑鼠移動的透明框
		var bigH = $(".eStore_article_picZoom img").height();
		console.log(bigH);
		var paddingBigT = (400-bigH)/2;
		$(".eStore_article_picZoom img").css({
			"padding-top":paddingBigT,
			"padding-bottom":paddingBigT
		});
		focusX = $(".eStore_article_focusBlock").offset().left;
		focusY = $(".eStore_article_focusBlock").offset().top;
		focusX = focusX-picX;
		focusY = focusY-picY;
		$(".eStore_article_picZoom img").css({
			"left": -(400*(focusX/200)),
			"top": -(400*(focusY/200))
		});
		//針對放大的圖片
	});
	
	$(".eStore_article_picBig")
	.mouseover(function() {
		$(".eStore_article_focusBlock,.eStore_article_picZoom").show();
	})
	.mouseout(function() {
		$(".eStore_article_focusBlock,.eStore_article_picZoom").hide();
	});
	
});