
function SearchingLocation() {
    var self = this;
    console.log($.cookie("searchCity"));
    console.log($.cookie("searchPP"));
    self.searchingCity = ko.observable();
    self.searchingPP = ko.observable();
    self.searchingCities = ko.observableArray();
    self.searchingPPs = ko.observableArray();
    self.searchLocationBtn = function () {
        $("#setLocation").modal('hide');
    }

    self.loadCities = function () {
        $.ajax({
            url: '/api/Location/GetCities',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                $.each((data), function (i, item) { self.searchingCities.push(item) });
                self.searchingCity($.cookie("searchCity"));
                $('#select-city').selectize({
                    sortField: {
                        field: 'text',
                        direction: 'asc'
                    },
                });
            },
            error: function (jqXHR, status, thrownError) {
                toastr.error("failed to load Cities.Please refresh page and try again", "Error");
            }
        });
    }
    self.loadPopularPlaces = function () {
        self.searchingPPs.removeAll();
        $.ajax({
            url: '/api/Location/GetPopularPlaces?city=' + self.searchingCity(),
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                $.each((data), function (i, item) { self.searchingPPs.push(item) });
                self.searchingPP($.cookie("searchPP"));
                $('#select-popularPlace').selectize({

                    sortField: {
                        field: 'text',
                        direction: 'asc'
                    },
                });
            },
            error: function (jqXHR, status, thrownError) {
                toastr.error("failed to load Famous Places.Please refresh page and try again", "Error");
            }
        });
    }
    self.loadCities();
    var sub = self.searchingCity.subscribe(function (value) {
        if (!self.searchingCity()) {
            self.searchingPP("");
        }
        $.cookie("searchCity", self.searchingCity(), {path: '/',domain: 'localhost'});
        
        self.loadPopularPlaces();
    })
    self.searchingPP.subscribe(function (value) {
        $.cookie("searchPP", self.searchingPP(), { path: '/',domain: 'localhost' });
    })
}