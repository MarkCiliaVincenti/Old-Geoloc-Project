angular.module('app.controllers').controller('customerDetailController', customerDetailController);

customerDetailController.$inject = ['$scope', 'customerService', '$stateParams', '$rootScope','vehicleService'];
function customerDetailController($scope, customerService, $stateParams, $rootScope, vehicleService) {
    $scope.id = $stateParams.customerId;
    $rootScope.customerId = $stateParams.customerId;
    $scope.customer = {};
    customerService.getCustomer($stateParams.customerId).then(function (resp) {
        $scope.customer = resp.data;
         // $rootScope.customerId = null;
      //vehicleService.sendCustomerId($stateParams.customerId);
        // initVehicleList($stateParams.customerId);
    });
}
