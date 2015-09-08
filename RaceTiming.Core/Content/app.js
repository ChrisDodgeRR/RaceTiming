(function(){
  var app = angular.module('raceEntrants', []);
  
	  app.controller('EntrantsController', function($scope, $http){
	     $http.get("/api/runners")
	     	.success(function(response) {
	     		$scope.entrants = response.entrants;
	     	});
	    
		  $scope.ordered_columns = [];
		  $scope.all_columns = [{
		    "title": "Number",
		    "ref": "number",
		    "type": "number",
		  }, {
		    "title": "First Name",
		    "ref": "firstName",
		    "type": "string",
		  }, {
		    "title": "Last Name",
		    "ref": "lastName",
		    "type": "string",
		  }, {
		    "title": "DoB",
		    "ref": "dateOfBirth",
		    "type": "date",
		  }, {
		    "title": "Club",
		    "ref": "club",
		    "type": "string",
		  }, {
		    "title": "Team",
		    "ref": "team",
		    "type": "string",
		  }];


	  });


	  app.controller('ResultsController', function($scope, $http){

			$http.get("/api/results")
		  		.success(function(response) {
		  			$scope.raceResults = response.raceResults;
		  		});
		    
			  $scope.ordered_columns = [];
			  $scope.all_columns = [{
			    "title": "Position",
			    "ref": "position",
			    "type": "number",
			  }, {
			    "title": "Time",
			    "ref": "time",
			    "type": "date",
			  }, {
			    "title": "Number",
			    "ref": "number",
			    "type": "number",
			  }];

		});

  
    
})();