angular.module('app.services').service('positionService', ['$http', function ($http) {

    this.getPosition = function getPosition(vehicleId, startPeriod, endPeriod) {
        return $http({
            method: 'GET',
            url: currentLang + '/Position/GetPositionByDate?vehicleId=' + vehicleId + "&start=" + startPeriod + "&end=" + endPeriod
        });
    }
    
}]);
