(function() {
    var app = angular.module('RaceTiming', []);

    // INDEX PAGE ************************************************************
    app.controller('IndexController', function ($scope, $http) {
        $http.get("/api/raceinfo")
            .success(function (response) {
                $scope.raceinfo = response;
            });
    });

    // ENTRY FORM PAGE *****************************************************
    app.controller('EntryFormController', function($scope, $http) {
        $scope.runner = {
            firstname: "",
            lastname: "",
            gender: "",
            dob: "",
            club: "",
            team: "",
            number: "",
            email: "",
            urn: "",
            affiliated: "",
            msg: "",
            colour: 'black',

            submit: function() {
                $http.post('/api/addrunner', {
                        'firstname': $scope.runner.firstname,
                        'lastname': $scope.runner.lastname,
                        'gender': $scope.runner.gender,
                        'dob': $scope.runner.dob,
                        'club': $scope.runner.club,
                        'team': $scope.runner.team,
                        'number': $scope.runner.number,
                        'email': $scope.runner.email,
                        'urn': $scope.runner.urn,
                        'affiliated': $scope.runner.affiliated,
                    })
                    .success(function(data) {
                        $scope.runner.msg = data;
                        console.log($scope.runner.msg);
                        $scope.runner.colour = 'black';
                    })
                    .error(function(data) {
                        $scope.runner.msg = "Error adding runner: " + data;
                        console.log($scope.runner.log);
                        $scope.runner.colour = 'red';
                    });
            }
        }
    });


    // RACE ENTRANTS PAGE ********************************************************
    app.controller('EntrantsController', function($scope, $http) {

        $scope.order = "number"; // Default

        $http.get("/api/runners")
            .success(function(response) {
                $scope.entrants = response.entrants;
            });
    });


    // FINISH POSITION ENTRY PAGE ***************************************************
    app.controller('ResultsController', function($scope, $http, $timeout) {

        $scope.position = {
            value: "",
            log: "",
            colour: 'black',

            save: function() {
                $http.post('/api/addfinishposition', { 'position': $scope.position.value })
                    .success(function(data) {
                        $scope.position.log = "Saved result: " + $scope.position.value;
                        console.log($scope.position.log);
                        $scope.position.value = "";
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

    // FINISHER RESULTS PAGE ********************************************************
    app.controller('FinishersController', function($scope, $http) {

        $http.get("/api/raceinfo")
            .success(function (response) {
                $scope.raceinfo = response;
            });

        $http.get("/api/finishers")
            .success(function(response) {
                $scope.finishers = response.finishers;
            });
    });

    // PRIZE WINNERS PAGE ********************************************************
    app.controller('PrizeWinnersController', function ($scope, $http) {

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
    });
})();