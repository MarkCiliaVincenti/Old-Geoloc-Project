angular.module('app.controllers').controller('customerController', customerController);

customerController.$inject = ['$scope', 'customerService'];
function customerController($scope, customerService) {
    
    $scope.customerDetail = function(id) {
        console.log(id);
    }
   

}
