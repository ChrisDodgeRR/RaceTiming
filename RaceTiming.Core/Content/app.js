(function() {
    var app = angular.module('RaceTiming', []);

    app.directive('modal', function() {
        return {
            template: '<div class="modal fade">' +
                '<div class="modal-dialog">' +
                '<div class="modal-content">' +
                '<div class="modal-header">' +
                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                '<h4 class="modal-title">{{title}}</h4>' +
                '</div>' +
                '<div class="modal-body" ng-transclude></div>' +
                '</div>' +
                '</div>' +
                '</div>',
            restrict: 'E',
            transclude: true,
            replace: true,
            scope: true,
            link: function postLink(scope, element, attrs) {
                scope.$watch(attrs.visible, function(value) {
                    if (value == true)
                        $(element).modal('show');
                    else
                        $(element).modal('hide');
                });

                $(element).on('shown.bs.modal', function() {
                    scope.$apply(function() {
                        scope.$parent[attrs.visible] = true;
                    });
                });

                $(element).on('hidden.bs.modal', function() {
                    scope.$apply(function() {
                        scope.$parent[attrs.visible] = false;
                    });
                });
            }
        };
    });

    app.directive('ngConfirmClick', [
      function () {
          return {
              priority: -1,
              restrict: 'A',
              link: function (scope, element, attrs) {
                  element.bind('click', function (e) {
                      var message = attrs.ngConfirmClick;
                      if (message && !confirm(message)) {
                          e.stopImmediatePropagation();
                          e.preventDefault();
                      }
                  });
              }
          }
      }
    ]);

    // Call function When <ENTER> is hit
    app.directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    });

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
                        // Clear form
                        $scope.runner.firstname = "";
                        $scope.runner.lastname = "";
                        $scope.runner.email = "";
                        $scope.runner.number = "";
                        $scope.runner.dob = "";
                        $scope.runner.club = "";
                        $scope.runner.team = "";
                        $scope.runner.urn = "";
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
    app.controller('EntrantsController', function ($scope, $http) {

        // Runner editor dialog
        $scope.showEditDialog = false;
        $scope.title = "";
        $scope.runnerNumber = "";
        $scope.dob = {
            value: new Date()
        };

        $scope.toggleDialog = function (number) {
            $scope.errormsg = "";
            $scope.runnerNumber = number;
            $scope.title = "Edit Runner Information - Number " + $scope.runnerNumber;
            $http({
                url: "/api/runner",
                method: "GET",
                params: { number: number }
            }).success(function(response) {
                $scope.runner = response.runner;
                $scope.dob.value = new Date($scope.runner.dob);
            });

            $scope.showEditDialog = !$scope.showEditDialog;
        };

        $scope.loadRunners = function() {
            $http.get("/api/runners")
                .success(function (response) {
                    $scope.entrants = response.entrants;
                });
        }

        $scope.updateRunner = function () {
            $http.post('/api/updaterunner', {
                    'number': $scope.runner.number,
                    'firstname': $scope.runner.firstName,
                    'lastname': $scope.runner.lastName,
                    'email': $scope.runner.email,
                    'gender': $scope.runner.gender,
                    'dob': $scope.dob.value,
                    'club': $scope.runner.club,
                    'team': $scope.runner.team,
                    'urn': $scope.runner.urn,
                })
                .success(function(data) {
                    // Close dialog
                    $scope.showEditDialog = !$scope.showEditDialog;
                    // Reload runner data
                    $scope.loadRunners();
                })
                .error(function(data) {
                    $scope.errormsg = "Error updating runner: " + data;
                    console.log($scope.errormsg);
                });
        }

        $scope.deleteRunner = function(number) {
            console.log('Delete runner: ' + number);
            $http.post('/api/deleterunner', {
                    'number': number,
                }).success(function(data) {
                    console.log("Runner deleted.");
                    // Reload runner data
                    $scope.loadRunners();
                })
                .error(function(data) {
                    $scope.errormsg = "Error deleting runner: " + data;
                    console.log($scope.errormsg);
                });
        }

        $scope.order = "number"; // Default
        $scope.loadRunners();
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

        // Shows the result editing dialog
        $scope.toggleEditDialog = function (position) {
            $scope.errormsg = "";

            $scope.resultPosition = position;
            $scope.raceResult = {};     // Not sure why need to declare here.
            $scope.title = "Edit Result - Position " + $scope.resultPosition;

            $http({
                url: "/api/result",
                method: "GET",
                params: { position: position }
            }).success(function (response) {
                $scope.raceResult = response.raceResult;
            });

            $scope.showEditDialog = !$scope.showEditDialog;
        };

        // Updates a result
        $scope.updateResult = function () {
            $http.post('/api/updateresult', {
                'position': $scope.raceResult.position,
                'raceNumber': $scope.raceResult.raceNumber,
                'hours': $scope.raceResult.time.hours,
                'minutes': $scope.raceResult.time.minutes,
                'seconds': $scope.raceResult.time.seconds,
            })
                .success(function (data) {
                    // Close dialog
                    $scope.showEditDialog = !$scope.showEditDialog;
                    // Reload result data
                    $scope.getData();
                })
                .error(function (data) {
                    $scope.errormsg = "Error updating result: " + data;
                    console.log($scope.errormsg);
                });
        };

        // Shows the result addition dialog
        $scope.toggleAddDialog = function (position) {
            $scope.errormsg = "";

            $scope.resultPosition = position;
            $scope.raceResult = {};     // Not sure why need to declare here.
            $scope.title = "Add New Result - Position " + $scope.resultPosition;

            // Populates the dialog with some of the current information
            $http({
                url: "/api/result",
                method: "GET",
                params: { position: position - 1 }  // We want the time of the previous position.
            }).success(function (response) {
                $scope.raceResult = response.raceResult;
                $scope.raceResult.position = position;  // Want to use current position
                $scope.raceResult.raceNumber = 0;       // Don't want this set.
            });

            $scope.showAddDialog = !$scope.showAddDialog;
        };

        // Adds a new result
        $scope.addResult = function () {
            $http.post('/api/addresult', {
                'position': $scope.raceResult.position,
                'raceNumber': $scope.raceResult.raceNumber,
                'hours': $scope.raceResult.time.hours,
                'minutes': $scope.raceResult.time.minutes,
                'seconds': $scope.raceResult.time.seconds,
            })
                .success(function (data) {
                    // Close dialog
                    $scope.showAddDialog = !$scope.showAddDialog;
                    // Reload result data
                    $scope.getData();
                })
                .error(function (data) {
                    $scope.errormsg = "Error adding result: " + data;
                    console.log($scope.errormsg);
                });
        };

        // Shows the result deletion dialog
        $scope.toggleDeleteDialog = function (position) {
            $scope.errormsg = "";

            $scope.deleteModel = {
                deleteTime: false,
                deleteNumber: false,
            }

            $scope.resultPosition = position;
            $scope.raceResult = {};     // Not sure why need to declare here.
            $scope.title = "Delete Result - Position " + $scope.resultPosition;

            // Populates the dialog with some of the current information
            $http({
                url: "/api/result",
                method: "GET",
                params: { position: position }
            }).success(function (response) {
                $scope.raceResult = response.raceResult;
            });

            $scope.showDeleteDialog = !$scope.showDeleteDialog;
        };

        // Adds a new result
        $scope.deleteResult = function() {
            $http.post('/api/deleteresult', {
                    'position': $scope.resultPosition,
                    'deleteTime': $scope.deleteModel.deleteTime,
                    'deleteNumber': $scope.deleteModel.deleteNumber,
                })
                .success(function(data) {
                    // Close dialog
                    $scope.showDeleteDialog = !$scope.showDeleteDialog;
                    // Reload result data
                    $scope.getData();
                })
                .error(function(data) {
                    $scope.errormsg = "Error deleting result: " + data;
                    console.log($scope.errormsg);
                });
        };

        $scope.resultsToList = 20;
        $scope.showAllLink = true;

        // Function to get the data
        $scope.getData = function() {
            $http({
                url: "/api/results",
                method: "GET",
                params: { resultsToList: $scope.resultsToList }
            }).success(function(response) {
                $scope.raceResults = response.raceResults;
            });
        };

        // Function to replicate setInterval using $timeout service.
        $scope.intervalFunction = function() {
            $timeout(function() {
                $scope.getData();
                $scope.intervalFunction();
            }, 1000);
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

    // TEAM RESULTS PAGE ********************************************************
    app.controller('TeamsController', function ($scope, $http) {

        $http.get("/api/raceinfo").
            success(function(response) {
                $scope.raceinfo = response;
            });

        $http.get("/api/teams")
            .success(function (response) {
                $scope.teams = response.teams;
            });
    });

})();