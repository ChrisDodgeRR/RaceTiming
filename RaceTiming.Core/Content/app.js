(function(){
  var app = angular.module('raceEntrants', []);
  
  app.controller('MainController', function($scope, $http){
     //$http.get("http://www.w3schools.com/angular/customers.php")
     $http.get("/api/runners")
    .success(function(response) {$scope.entrants = response.entrants;});
    
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
  
    
})();