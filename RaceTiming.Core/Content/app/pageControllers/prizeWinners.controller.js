(function() {
	'use strict';
	
	angular
		.module('RaceTiming')
		.controller('PrizeWinnersController', PrizeWinnersController);
		
		function PrizeWinnersController($scope, $http) {
			$http.get("/api/raceinfo").
            success(function(response) {
                $scope.raceinfo = response;
            });

        $http.get("/api/winners")
            .success(function (response) {
                $scope.winners = response.winners;

                $scope.categorywinners = [
                    { name: 'Male Open', list: $scope.winners.m },
                    { name: 'Female Open', list: $scope.winners.f },
                    { name: 'Male V40', list: $scope.winners.mV40 },
                    { name: 'Female V40', list: $scope.winners.fV40 },
                    { name: 'Male V50', list: $scope.winners.mV50 },
                    { name: 'Female V50', list: $scope.winners.fV50 },
                    { name: 'Male V60', list: $scope.winners.mV60 },
                    { name: 'Female V60', list: $scope.winners.fV60 }
				];
            });
		};
	
})();