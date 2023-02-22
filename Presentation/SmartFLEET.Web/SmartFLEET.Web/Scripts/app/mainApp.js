
var app = angular.module("smart-fleet", ['ui.router', 'angularjs-gauge','app.controllers', 'app.services']);
angular.module('app.services', []);
angular.module('app.controllers', []);
app.config([
    '$stateProvider',function ($stateProvider) {
        $stateProvider.state("about",
            {
                url: '/About',
                templateUrl: '/Home/About'
            });
       // $urlRouterProvider.otherwise('/Home');
    }
]);

app.directive('reportVehicle', function () {
    return {
        restrict: 'E',
        templateUrl: currentLang+'/VehicleReport/Index',
    };
});
app.directive('listZone', function () {
    return {
        restrict: 'E',
        templateUrl: currentLang+'/InterestArea/Index',
    };
});


