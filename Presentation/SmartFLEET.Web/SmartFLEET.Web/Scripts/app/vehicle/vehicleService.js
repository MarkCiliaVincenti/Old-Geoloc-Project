
angular.module('app.services').service('vehicleService', ['$http', '$rootScope', function ($http, $rootScope) {

    
    this.getVehicleOfCustomer = function getVehicleOfCustomer(vehicleId) {
        return $http({
            method: 'GET',
            url: '../Administrator/Vehicle/GetVehicleDetail/?vehicleId=' + vehicleId
        });
    };
    this.getVehicle = function getVehicle() {
        return $http({
            method: 'GET',
            url: '../Administrator/Vehicle/GetVehicleDetail/'
        });
    };
    this.GetAllVehiclesForCustomer = function getAllVehiclesForCustomer(customerid) {
        console.log(customerid);
        return $http({
            method: 'GET',
            url: '../Administrator/Vehicle/GetAllVehiclesForCustomer/?customerId=' + customerid
        });
        
    };
    this.GetAllVehicles= function getAllVehicles() {
         return $http({
            method: 'GET',
            url: '../Administrator/Vehicle/GetAllVehicles/'
        });

    };
    this.getNewVehicle = function getNewVehicle() {
        return $http({
            method: 'GET',
            url: '../Administrator/Vehicle/GetNewVehicle/'
        });
    }
    this.addVehicle = function addVehicle(vehicle) {
        //  console.log(vehicle);
        return $http.post("../Administrator/Vehicle/AddNewVehicle", JSON.stringify(vehicle));
    }
}]);
