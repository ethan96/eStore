// JavaScript Document

$(function(){
	
	$(".iot-carouselBlock-content").each(function(){
	
		var id = $(this).attr('id');
			if(id == 'iot-carouselBannerIndex'){
				$(this).find("ul").carouFredSel({
				
				auto: true,
				pagination: '#' + id +' #iot-pager',
				scroll: {
					duration: 500,
					timeoutDuration: 8000,
					easing: 'easeOutSine'
					//pauseOnHover: 'immediate'
				}
			});	
		}else{
			$(this).find("ul").carouFredSel({
				
				auto: false,
				prev: '#' + id +' #iot-prev',
				next: '#' + id +' #iot-next',
				pagination: '#' + id +' #iot-pager',
				scroll: {
					duration: 500,
					timeoutDuration: 5000,
					easing: 'easeOutSine'
					//pauseOnHover: 'immediate'
				}
			});		
		}
		
	});

});