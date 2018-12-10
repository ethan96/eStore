var eStoreTutorial = function (options) {
    options = options || {};
    this.Page = options.Page || "";
    this.Language = options.Language || "";
    this.Tutorial = options.Tutorial || "";
    this.Container = options.Container || "";
    this.NextTutorial = options.NextTutorial || "";
    this.NextContainer = options.NextContainer || "";
}
var eStoreTutorialModel = function (options) {
    var self = this;

    self.eStoreTutorial = ko.observable(new eStoreTutorial(options));
    self.loadTutorial = function () {

        //try {
        var container = $(self.eStoreTutorial().Container);
        $("body").addClass("tutorial-lock");
        $("body").prepend($("<div />").addClass("tutorial-BG"));

        $(container).parent().addClass("tutorial-position");
        $.get("/tutorial/" + self.eStoreTutorial().Language + "/" + self.eStoreTutorial().Page + "/" + self.eStoreTutorial().Tutorial, function (data) {
            $(container).parent().append(data);

            $(container).parent().find(".close").click(function () {
              
                $(".tutorial-pop").hide();
                $(".tutorial-lock,.tutorial-BG").removeClass();

                if (self.eStoreTutorial().NextTutorial != "" &&$(self.eStoreTutorial().NextContainer).length>0) {
                    self.eStoreTutorial(
                    { Page: self.eStoreTutorial().Page
                    , Language: self.eStoreTutorial().Language
                    , Tutorial: self.eStoreTutorial().NextTutorial
                    , Container: self.eStoreTutorial().NextContainer
                    , NextTutorial: ""
                    , NextContainer: ""
                    });
                    self.loadTutorial();
                    return false;
                }
                else {
                    return false;
                }
            });
            $(container).parent().find(".tutorial-animate").each(function () {
                var id = $(this).attr('id');
                id = "#" + id;
                $(id).find("ul").carouFredSel({
                    duration: 500000,

                    prev: id + ' .prev',
                    next: id + ' .next',
                    pagination: id + ' .pager',
                    scroll: 800
                });
            });
        });
     
    };
}
function InitTutorial(page,Language,Tutorial,Container,NextTutorial,NextContainer) {
    var cookiekey = page + "_Tutorial";
    var windowW = $(window).outerWidth(true);

    if (!$.cookie(cookiekey) == true && windowW>=980) {
        $.cookie(cookiekey, true, { expires: 365, path: '/' });
        eStoreTutorialModel = new eStoreTutorialModel({ Page: page, Language: Language, Tutorial: Tutorial, Container: Container, NextTutorial: NextTutorial, NextContainer: NextContainer });
        eStoreTutorialModel.loadTutorial();
        ko.applyBindings(eStoreTutorialModel);
    }
}
////InitTutorial("HomePage", "en-US", "myaccount.txt", ".eStore_MyAccount", "tabs.txt", ".eStore_index_Highlight_tabBlock");
////InitTutorial("Category", "en-US", "category-tutorial.txt", ".eStore_category_link", "", "");
////  InitTutorial("Product", "en-US", "product-tutorial.txt", ".eStore_category_link", "", "");
//$(function () {
//    InitTutorial("System", "en-US", "system-tutorial-image.txt", ".eStore_product_productPic", "system-tutorial-preview.txt", ".systemBomPreview");
//});
// 
 