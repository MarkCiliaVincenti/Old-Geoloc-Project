angular.module('app.controllers').controller('addCustomerController', addCustomerController);

customerController.$inject = ['$scope', 'customerService','userService', '$state', '$rootScope'];

function addCustomerController($scope, customerService, userService,$state, $rootScope) {
    $scope.customer = {};
    $scope.users = [];
    $scope.user = {};
    $scope.user.role = "User";
    var timeZoneInfos = [];
    $scope.onValidate = function (customer) {
        customer.UserVms = $scope.users;
        customerService.addCustomer(JSON.stringify(customer)).then(function (resp) {
            if (resp.data.ValidationStatus === 2) {
                var msg = "";
                for (var i = 0; i < resp.data.Errors.length; i++) {
                    msg = msg + resp.data.Errors[i];
                }
                $.bootstrapGrowl(msg,
                    {
                        ele: 'body', // which element to append to
                        type: 'danger' // (null, 'info', 'danger', 'success')
                    });
            } else {
                $.bootstrapGrowl("Operation has been terminated  successfully !", {
                    ele: 'body', // which element to append to
                    type: 'success' // (null, 'info', 'danger', 'success')
                });
                $scope.vehicle = {};
                // $("#vehicleModal").modal("hide");
            } 
        });
    }
    userService.getTimeZones().then(function (res) {
       
        $scope.user.TimeZoneInfos = timeZoneInfos = res.data.map(z => z.Id);
        
    });
    $scope.loadUser = function() {
        jQuery("#add-user").modal("show");
    }
    $scope.save = function() {
        $scope.users.push($scope.user);
        $scope.user = {};
        $scope.user.TimeZoneInfos = timeZoneInfos;
        $scope.user.role = "User";
    }
    customerService.getNewCustomer().then(function (response) {
        console.log(response.data);
        $scope.customer = response.data;
    });
}