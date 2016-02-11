
//var title = ko.observable($("#search").val());
var availableTitles = ko.observableArray();
var title = ko.observable();
var tags = ko.observable("");
var skills = ko.observable("");
var minPrice = ko.observable(0);
var maxPrice = ko.observable(50000);
var gender = ko.observable();
var salaryType = ko.observable();
var availableCategories = ko.observableArray(new Array( "It Jobs", "Business and Marketing Jobs", "other jobs"));
var selectedCategory = ko.observable();
var availableQualifications = ko.observableArray();
var selectedQualification = ko.observable();
var availableExpriences = new Array("Not required", "Fresh Graduate", "1 year", "2 years", "3 years", "4 years", "5 years");
var selectedExprience = ko.observable();
var exprience = ko.observable();
var careerLevels = new Array("Entry Level", "Student/Internship", "Mid Career", "Management", "Executive/Director", "Senior Executive(President,CEO)");
var selectedCareerLevel = ko.observable();
var availableJobTypes = new Array("Full time", "Permanent", "Contract", "Internship", "Part time");
var selectedJobType = ko.observable();
var selectedLastDateToApply = ko.observable();
var selectedSeats = ko.observable();
var shift = ko.observable();
var isLoading = ko.observable(false);
availableCategories.subscribe(function () {
    console.log( "this" + availableCategories());
})
minPrice.subscribe(function () {
    RefreshSearch();
});
maxPrice.subscribe(function () {
    RefreshSearch();
});
tags.subscribe(function () {
    RefreshSearch();
})
skills.subscribe(function () {
    RefreshSearch();
})
gender.subscribe(function () {
    RefreshSearch();
})
salaryType.subscribe(function () {
    RefreshSearch();
})
selectedQualification.subscribe(function () {
    RefreshSearch();
})
selectedCategory.subscribe(function () {
    RefreshSearch();
})
selectedExprience.subscribe(function () {
    RefreshSearch();
})
selectedCareerLevel.subscribe(function () {
    RefreshSearch();
})
selectedJobType.subscribe(function () {
    RefreshSearch();
})
selectedLastDateToApply.subscribe(function () {
    RefreshSearch();
})
selectedSeats.subscribe(function () {
    RefreshSearch();
})
shift.subscribe(function () {
    RefreshSearch();
})
var loadQualification = function () {
    $.ajax({
        url: '/api/Job/GetAllQualifications',
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'GET',
        success: function (data) {
            $.each((data), function (i, item) { availableQualifications.push(item) });
            $('#select-category1').selectize();
            $('#select-careerLevel').selectize();
            $('#select-exprience').selectize();
            $('#select-jobtype').selectize();
            $('#select-qualification').selectize({
                sortField: {
                    field: 'text',
                    direction: 'asc'
                },
            });
        },
        error: function (jqXHR, status, thrownError) {
            toastr.error("failed to load Qualification data.Please refresh page and try again", "Error");
        }
    });

};

function RefreshSearch() {
    loadQualification();
    var self = this;
    searchingCity.subscribe(function () {
        RefreshSearch();
    })
    searchingPP.subscribe(function () {
        RefreshSearch();
    })
    
    self.showAds = ko.observableArray();
    isLoading(true);
    $.ajax({
        url: '/api/Job/SearchJobAds?gender=' + gender() + '&skills=' + skills() + '&tags=' + tags() + '&title=' + title() + '&minPrice=' + minPrice() + '&maxPrice=' + maxPrice() + '&city=' + searchingCity() + '&pp=' + searchingPP() + '&salaryType=' + salaryType() + '&category=' + selectedCategory() + '&qualification=' + selectedQualification() + '&exprience=' + exprience() + '&careerLevel=' + selectedCareerLevel() + '&jobType=' + selectedJobType() + '&lastDateToApply=' + selectedLastDateToApply() + '&seats=' + selectedSeats() + '&shift=' + shift() ,
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'POST',
        success: function (data) {
            isLoading(false);
            var mappedads = $.map(data, function (item) { return new Ad(item); });
            console.log(data);
            self.showAds(mappedads);
        },
        error: function () {
            isLoading(false);
            toastr.error("failed to search. Please refresh page and try again", "Error!");
        }
    });

}
var saveResult = function (data) {
    minPrice(data.fromNumber);
    maxPrice(data.toNumber);
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
    onFinish: saveResult
});
