(function () {
    'use strict';

    angular
        .module('RaceTiming')
		.controller('IndexController', function ($scope, $http) {

            $http.get("/api/raceinfo")
                .success(function (response) {
                    $scope.raceinfo = response;
                });

		});

})();