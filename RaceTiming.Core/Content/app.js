(function() {
    var app = angular.module('RaceTiming', []);


	// RACE ENTRANTS PAGE ********************************************************
    app.controller('EntrantsController', function($scope, $http) {
        $http.get("/api/runners")
            .success(function(response) {
                $scope.entrants = response.entrants;
            });

    });

    // FINISH POSITION ENTRY PAGE ********************************************************
    app.controller('ResultsController', function($scope, $http, $timeout) {

        $scope.position = {
            value: 0,
            log: "",
            colour: 'black',

            save: function() {
                $http.post('/api/addfinishposition', { 'position': $scope.position.value })
                    .success(function(data) {
                        $scope.position.log = "Saved result: " + $scope.position.value;
                        console.log($scope.position.log);
                        $scope.position.value = 0;
                        $scope.position.colour = 'black';
                    })
                    .error(function(data) {
                        $scope.position.log = "Error saving result: " + data;
                        console.log($scope.position.log);
                        $scope.position.colour = 'red';
                    });
            }
        }

        // Function to get the data
        $scope.getData = function() {
            $http.get("/api/results")
                .success(function(response) {
                    $scope.raceResults = response.raceResults;

                    console.log('Fetched data!');
                });
        };


        // Function to replicate setInterval using $timeout service.
        $scope.intervalFunction = function() {
            $timeout(function() {
                $scope.getData();
                $scope.intervalFunction();
            }, 500);
        };

        // Kick off the interval
        $scope.intervalFunction();
    });


    // ENTRY FORM PAGE *****************************************************
    app.controller('EntryFormController', ['$scope', '$log', function($scope, $log) {
    	$scope.$log = $log;
  	}]);

})();