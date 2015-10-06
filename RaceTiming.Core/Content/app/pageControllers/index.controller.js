(function () {
    'use strict';

    angular
        .module('RaceTiming')
		.controller('IndexController', function ($http) {
            var indexModel = this;
            $http.get("/api/raceinfo")
                .success(function (response) {
                    indexModel.raceinfo = response;
                });

		});

})();