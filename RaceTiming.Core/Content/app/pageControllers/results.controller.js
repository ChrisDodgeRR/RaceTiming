(function() {
	'use strict';
	
	angular
		.module('RaceTiming')
		.controller('ResultsController', ResultsController);
		
	function ResultsController($scope, $http, $timeout) {

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
        }

        // Kick off the interval
        $scope.intervalFunction();
		
		
	};
	
})();