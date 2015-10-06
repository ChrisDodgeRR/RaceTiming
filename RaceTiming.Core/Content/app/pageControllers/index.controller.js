(function() {
	'use strict';
	
	angular
		.module('RaceTiming')
		.controller('IndexController', function($http) {

	    $http.get("/api/raceinfo")
            .success(function(response) {
                this.raceinfo = response;
            });
                
	});
		
})();