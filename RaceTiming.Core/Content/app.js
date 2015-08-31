(function() {
  var app = angular.module('raceTiming', []);
  
  app.controller('MainController', function(){
    this.entrants = runners;
  });
  
    var runners = [
    { firstName: 'Chris', lastName: 'Dodge' },
    { firstName: 'Vera', lastName: 'Dodge'}
    ];
    
})();