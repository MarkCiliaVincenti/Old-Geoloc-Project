'use strict';
var app = angular.module("smart-admin", ['ui.router', 'app.controllers', 'app.services']);

angular.module('app.services', []);
angular.module('app.controllers', []);

app.config([
    '$stateProvider', function ($stateProvider) {
        $stateProvider.state("users",
                {
                    url: 'users',
                    templateUrl: '../Administrator/User/Index',
                    service: 'userService',
                    controller: 'userController',
                }).state("boxes",
                {
                    url: 'gpsDevices',
                    templateUrl: '../Administrator/GpsDevice/Index'
                   
                    })
                .state("AddMobileUnit",
                {
                    url: 'addMobileUnit',
                    templateUrl: '../Administrator/GpsDevice/Create',
                    controller: "addMobileUnitController"
                }).state("vehicles",
                {
                    url: 'vehicles',
                    templateUrl: '../Administrator/Vehicle/Index',
                    //service: 'vehicleService',
                    //controller: 'vehicleController'
                })
            .state("vehicleDetail",
                {
                    url: '/detail',
                    templateUrl: '../Administrator/Vehicle/Detail',

                    params: {
                        vehicleId: null
                    }
            })
            .state("AddVehicle",
                {
                    url: 'addvehicle',
                    templateUrl: '../Administrator/Vehicle/Create',
                    service: 'vehicleService',
                    controller: 'addVehicleController'
                })

            .state("customers",
                {
                    url: 'customers',
                    templateUrl: '../Administrator/Customer/Index',
                    service: 'customerService',
                    controller: 'customerController'
                })
            .state("customerDetail",
                {
                    url: '/detail',
                    templateUrl: '../Administrator/Customer/Detail',

                    params: {
                        customerId: null
                    }
            }).state("AddCustomer",
                {
                    url: 'addcutomer',
                    templateUrl: '../Administrator/Customer/Create',
                    service: 'CustomerService',
                    controller: 'addCustomerController'
                });
        // $urlRouterProvider.otherwise('/Home');
    }
]);

app.run(function ($rootScope, $state) {

    $rootScope.navigate = function ($event, to, params) {
        console.log(params);
        // If the command key is down, open the URL in a new tab
        if ($event.metaKey) {
            var url = $state.href(to, params, { absolute: true });
            window.open(url, '_blank');

        } else {

            try {
                $state.go(to, params);
            } catch (e) {

            } 
        }

    };

});
app.directive('listVehicle', function () {
    return {
        restrict: 'E',
        templateUrl: '../Administrator/Vehicle/Index',
    };
});
app.directive('listUsers', function () {
    return {
        restrict: 'E',
        templateUrl: '../Administrator/User/Users',
    };
});
app.directive('newUser', function () {
    return {
        restrict: 'E',
        templateUrl: '../Administrator/User/NewUser',
    };
});
app.directive('validateEmail', function () {
    var EMAIL_REGEXP = /^[_a-z0-9]+(\.[_a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/;
    return {
        link: function (scope, elm) {
            elm.on("keyup", function () {
                var isMatchRegex = EMAIL_REGEXP.test(elm.val());
                if (isMatchRegex && elm.hasClass('warning') || elm.val() == '') {
                    elm.removeClass('warning');
                } else if (isMatchRegex == false && !elm.hasClass('warning')) {
                    elm.addClass('warning');
                }
            });
        }
    }
});
