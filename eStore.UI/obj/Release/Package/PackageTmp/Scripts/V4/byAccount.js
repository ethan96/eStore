var Account = function (options) {
    options = options || {};
    var myorders = [];
    var myquotes = [];
    var mytquotes = [];
    //var mysearchitems = [];
    var myname = options.WelcomeName || "";

    if (options.Orders != undefined && options.Orders != null && options.Orders.length > 0) {
        ko.utils.arrayForEach(options.Orders, function (item) {
            myorders.push(new Order(item));
        });
        $("#MyOrder").show();
    }
    else {
        $("#MyOrder").hide();
    }

    if (options.Quotes != undefined && options.Quotes != null && options.Quotes.length > 0) {
        ko.utils.arrayForEach(options.Quotes, function (item) {
            myquotes.push(new Quote(item));
        });
        $("#MyQuote").show();
    }
    else {
        $("#MyQuote").hide();
    }

    if (options.tQuotes != undefined && options.tQuotes != null && options.tQuotes.length > 0) {
        ko.utils.arrayForEach(options.tQuotes, function (item) {
            mytquotes.push(new Quote(item));
        });
        $("#MyTquote").show();
    }
    else {
        $("#MyTquote").hide();
    }

//    if (options.SearchItems != undefined && options.SearchItems != null) {
//        ko.utils.arrayForEach(options.SearchItems, function (item) {
//            mysearchitems.push(new SearchItem(item));
//        });
//    }

    this.Orders = ko.observableArray(myorders);
    this.Quotes = ko.observableArray(myquotes);
    this.tQuotes = ko.observableArray(mytquotes);
    //this.SearchItems = ko.observableArray(mysearchitems);
    this.WelcomeName = ko.observable(myname);
};

var Order = function (options) {
    options = options || {};
    this.OrderNo = options.OrderNo || "";
    this.OrderNoUrl = options.OrderNoUrl || "";
    this.OrderDate = options.OrderDate || "";
    this.SubTotal = options.SubTotal || "";
    this.ShipTo = options.ShipTo || "";
    this.Status = options.Status || "";
};

var Quote = function (options) {
    options = options || {};
    this.QuoteID = options.QuoteID || "";
    this.QuoteNo = options.QuoteNo || "";
    this.QuoteNoUrl = options.QuoteNoUrl || "";
    this.QuoteReviseUrl = options.QuoteReviseUrl || "";
    this.SubTotal = options.SubTotal || "";
    this.TotalAmount = options.TotalAmount || "";
    this.CurrencySign = options.CurrencySign || "";
    this.ShipTo = options.ShipTo || "";
    this.Status = options.Status || "";
    this.QuoteDate = options.QuoteDate || "";
    this.QuoteExpiredDate = options.QuoteExpiredDate || "";
    this.Source = options.Source || "";
    var myorders = [];
    if (options.Orders != undefined && options.Orders != null) {
        ko.utils.arrayForEach(options.Orders, function (item) {
            myorders.push(new Order(item));
        });
    }
    this.Orders = myorders || "";
    this.QuoteAction = options.QuoteAction || "";
};

//var SearchItem = function (options) {
//    options = options || {};
//    this.mytext = options.Text;
//    this.myvalue = options.Value;
//};

var MyAccount = function () {
    var result;
    var self = this;
    self.account = ko.observable(new Account(undefined));

    self.loadmyorder = function () {
        result = $.getJSON( GetStoreLocation() + 'api/Account/GetAccountOrder/', function (data) {
            self.account(new Account(data));
        });
    }

    self.searchorder = function () {
        var no = $("#searchNo").val();
        var range = $("select[name=period]").val();
        result = $.getJSON(GetStoreLocation() + 'api/Account/GetAccOrder?orderNo=' + no + '&range=' + range, function (data) {
            self.account(new Account(data));
        });
    }

    self.loadmyquote = function () {
        result = $.getJSON(GetStoreLocation() + 'api/Account/GetAccountQuote/', function (data) {
            self.account(new Account(data));
        });
    }

    self.searchquote = function () {
        var no = $("#searchNo").val();
        var range = $("select[name=period]").val();
        result = $.getJSON(GetStoreLocation() + 'api/Account/GetAccQuote?quoteNo=' + no + '&range=' + range, function (data) {
            self.account(new Account(data));
        });
    }

};

// JavaScript Document
$(function () {
    $(".eStore_account_msgRight").hide();
    MyAccount = new MyAccount();
    ko.applyBindings(MyAccount);

    if ($("#MyOrder").length) 
        MyAccount.loadmyorder();
    else
        MyAccount.loadmyquote();
});