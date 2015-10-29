(function() {
	'use strict';
	
	angular
		.module('RaceTiming')
		.controller('EntryFormController', EntryFormController);

		function EntryFormController($scope, $http) {

			var vm = this;

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
	
	
})();