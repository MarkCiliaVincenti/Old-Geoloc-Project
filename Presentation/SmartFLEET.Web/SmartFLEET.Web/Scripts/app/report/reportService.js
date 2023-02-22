angular.module('app.services').service('reportService', ['$http', function ($http) {
    var culture = getCookie("culture");
    this.getReport = function getReport(vehicleId, startPeriod) {
        return $http({
            method: 'GET',
            url: 'en/VehicleReport/GetDailyVehicleReport/?vehicleId=' + vehicleId + "&startPeriod=" + startPeriod
        });
    };
    this.getVehicles = function() {
        return $http({
            method: 'GET',
            url: 'en/VehicleReport/GetVehicles'
        });
    }
    this.getReportContent = function getReportContent() {
        return $http({
            method: 'GET',
            url: '/en/VehicleReport/Index'
        });
    }

}]);
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}