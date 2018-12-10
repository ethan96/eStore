(function($){
  $.extend($.fn,{
    scrollTo:function(time,to){
      time=time||800;
      to=to||1;
            $('a[href*=#]', this).click(function(){
                if (location.hostname == this.hostname) {
                    var $target = $(this.hash);
                    $target = $target.length && $target || $('[name=' + this.hash.slice(1) + ']');
                    if ($target.length) {
                        if (to == 1) {
                            $('html,body').animate({
                                scrollTop: $target.offset().top
                            }, time, $(this).data.completeFunction);
                        }
                        else if(to==2){
                            $('html,body').animate({
                                scrollLeft: $target.offset().left
                            }, time, $(this).data.completeFunction);
                        }else{
              alert('argument error');
            }
                        return false;
                    }
                }
            });
    }
  });
})(jQuery);