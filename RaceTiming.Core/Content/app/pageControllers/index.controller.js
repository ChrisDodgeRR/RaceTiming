(function () {
    'use strict';

    angular
        .module('RaceTiming')
		.controller('IndexController', indexModel)
    
    function indexModel($scope, $http) {

            $http.get("/api/raceinfo")
                .success(function (response) {
                    $scope.raceinfo = response;
                });

	}

})();