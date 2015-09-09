(function(){
  var app = angular.module('raceEntrants', []);
  
	  app.controller('EntrantsController', function($scope, $http){
	     $http.get("/api/runners")
	     	.success(function(response) {
	     		$scope.entrants = response.entrants;
	     	});

	  });


	app.controller('ResultsController', function($scope, $http, $timeout){

	  // Function to get the data
	  $scope.getData = function(){
    	$http.get("/api/results")
		  	.success(function(response) {
		  		$scope.raceResults = response.raceResults;

				console.log('Fetched data!');
    		});
  		};

			
		// Function to replicate setInterval using $timeout service.
  		$scope.intervalFunction = function(){
    		$timeout(function() {
      			$scope.getData();
      			$scope.intervalFunction();
    		}, 1000)
  		};

  		// Kick off the interval
  		$scope.intervalFunction();
		    

	});

  
    
})();