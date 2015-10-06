(function() {
	'use strict';
	
	angular
		.module('RaceTiming');
		.controller('EntrantsController', EntrantsController);
		
		function EntrantsController($scope, $http) {
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
		}
		
})();