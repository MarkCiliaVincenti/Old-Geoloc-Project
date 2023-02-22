angular.module('app.services').service('mobileUnitService', ['$http', function ($http) {

    this.getMobileUnit = function getMobileUnit(mobileUnitId) {
        return $http({
            method: 'GET',
            url: '../Administrator/GpsDevice/GetMobileUnit/' + mobileUnitId
        });
    }
    this.addMobileUnit = function addMobileUnit(mobileUnit) {
        //  console.log(customer);
        return $http.post("../Administrator/GpsDevice/AddMobileUnit", mobileUnit);
    };
    
}]);
