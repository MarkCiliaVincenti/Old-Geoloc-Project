angular.module('app.controllers').controller('vehicleDetailController', vehicleDetailController);

vehicleDetailController.$inject = ['$scope', 'vehicleService', '$stateParams'];
function vehicleDetailController($scope, vehicleService, $stateParams) {
    $scope.id = $stateParams.vehicleId;
    console.log($stateParams.vehicleId);
    $scope.vehicle = {};
    vehicleService.getVehicleOfCustomer($stateParams.vehicleId).then(function (resp) {
        $scope.vehicle = resp.data;
    });
}
