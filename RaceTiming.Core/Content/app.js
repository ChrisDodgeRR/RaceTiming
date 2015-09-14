(function() {
    var app = angular.module('RaceTiming', []);

    app.controller('EntrantsController', function($scope, $http) {
        $http.get("/api/runners")
            .success(function(response) {
                $scope.entrants = response.entrants;
            });

    });

    app.controller('ResultsController', function($scope, $http, $timeout) {

        $scope.position = {
            value: 0,
            log: "",

            save: function() {
                $http.post('/api/addfinishposition', { 'position': $scope.position.value })
                    .success(function(data) {
                        $scope.position.log = "Saved result: " + $scope.position.value;
                        console.log($scope.position.log);
                        $scope.position.value = 0;
                    })
                    .error(function(data) {
                        $scope.position.log = "Error saving result: " + data;
                        console.log($scope.position.log);
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
            }, 2000);
        };

        // Kick off the interval
        $scope.intervalFunction();
    });


})();