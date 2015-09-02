(function(){
  var app = angular.module('raceEntrants', []);
  
  app.controller('MainController', function($scope, $http){
     //$http.get("http://www.w3schools.com/angular/customers.php")
     $http.get("http://0.0.0.0:1234/api/runners")
    .success(function(response) {$scope.entrants = response.entrants;});
    
  $scope.ordered_columns = [];
  $scope.all_columns = [{
    "title": "number",
    "type": "string",
  }, {
    "title": "firstName",
    "type": "string",
  }, {
    "title": "lastName",
    "type": "string",
  }, {
    "title": "dateOfBirth",
    "type": "date",
  }, {
    "title": "club",
    "type": "string",
  }, {
    "title": "team",
    "type": "string",
  }];


  });
  
    
})();