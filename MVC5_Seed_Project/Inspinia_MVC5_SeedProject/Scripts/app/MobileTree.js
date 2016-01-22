function Model(data) {
    var self = this;
    data = data || {};
    self.modelName = data.model;
}
function Company(data) {
    var self = this;
    data = data || {};
    self.companyName = data.companyName;
    self.showModels = ko.observableArray();
    if (data.models) {
        var models = $.map(data.models, function (item) { return new Model(item) });
        self.showModels(models);
    }
}
var brand = ko.observable("");
var model = ko.observable("");
var title = ko.observable($("#search").val());
var tags = ko.observable("");
var minPrice = ko.observable(0);
var maxPrice = ko.observable(50000);
minPrice.subscribe(function () {
    RefreshSearch();
});
maxPrice.subscribe(function () {
    RefreshSearch();
});
tags.subscribe(function () {
    RefreshSearch();
})

function TreeViewModel() {
    var self = this;
    self.showAds = ko.observableArray();
    

    self.showCompanies = ko.observableArray();
    self.loadTree = function () {
        $.ajax({
            url: '/api/Electronic/GetMobileTree',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                var dat = $.map(data, function (item) { return new Company(item) });
                self.showCompanies(dat);
                $("#navigation").jstree({
                    "themes": {
                        "theme": "classic"
                    },
                    "core": {
                        "themes": {
                            "icons": false
                        }
                    },
                    "plugins": ["search"]
                }).on('changed.jstree', function (e, data) {
                    brand( data.instance.get_node(data.node.parent).text);
                    model(data.instance.get_node(data.selected[0]).text);
                   // console.log(self.brand());
                   // console.log(self.model());
                    if (brand() == undefined) {
                        brand(model());
                        model("");
                    }
                    //console.log(self.brand());
                   // console.log(self.model());
                    self.loadad();
                })
  // create the instance
  .jstree();
                var to = false;
                $('#treeSearch').keyup(function () {
                    if (to) { clearTimeout(to); }
                    to = setTimeout(function () {
                        var v = $('#treeSearch').val();
                        $('#navigation').jstree(true).search(v);
                    }, 250);
                });
            },
            error: function (jqXHR, status, thrownError) {
                toastr.error("failed to laod category tree. Please refresh page", "Error");
            }
        });
    }
    self.loadTree();
    
    self.loadad = function () {
        url_address = '/api/Electronic/SearchMobileAds?brand=' + brand() + '&model=' + model() + '&tags= ' + tags() + '&title=' + title() + '&minPrice=' + minPrice() + '&maxPrice=' + maxPrice();
        $.ajax({
            url: url_address,
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                var mappedads = $.map(data, function (item) { return new ad(item); });
                self.showAds(mappedads);
            },
            error: function () {
                toastr.error("Unable to load data. Please try again", "Error");
            }
        });
    }
    self.loadad();
    //Search();
}


function Search() {
    var self = this;
   // self.title = $("#search").val();
   // self.tags = ko.observable();
   // self.minPrice = ko.observable(0);
   // self.maxPrice = ko.observable(50000);
    
    
}
function RefreshSearch() {
    $.ajax({
        url: '/api/Electronic/SearchMobileAds?brand=' + brand() + '&model=' + model() + '&tags=' + tags() + '&title=' + title() + '&minPrice=' + minPrice() + '&maxPrice=' + maxPrice(),
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'POST',
        success: function (data) {
            var mappedads = $.map(data, function (item) { return new ad(item); });
            self.showAds(mappedads);
        },
        error: function () {
            toastr.error("failed to search. Please refresh page and try again", "Error!");
        }
    });
}
var saveResult = function (data) {
    minPrice ( data.fromNumber );
    maxPrice  (data.toNumber);
};
$("#ionrange_1").ionRangeSlider({
    min: 0,
    max: 50000,
    type: 'double',
    prefix: "Rs",
    maxPostfix: "+",
    prettify: false,
    hasGrid: true,
    from: minPrice,
    to: maxPrice,
    //onStart: function (data) {
    //    saveResult(data);
    //    //writeResult();
    //},
   // onChange: saveResult,
    onFinish: saveResult
});