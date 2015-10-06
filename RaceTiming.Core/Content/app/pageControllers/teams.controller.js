
// TEAM RESULTS PAGE ********************************************************


(function() {
	'use strict';
	
	angular
		.module('RaceTiming')
		.controller('TeamsController', TeamsController);
		
	function TeamsController($scope, $http) {
		
		$http.get("/api/raceinfo").
            success(function(response) {
                $scope.raceinfo = response;
            });

        $http.get("/api/teams")
            .success(function (response) {
                $scope.teams = response.teams;
            });
	};
	
})();