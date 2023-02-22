angular.module('app.services').service('customerService', ['$http', function ($http) {

    this.getCustomer = function getCustomer(customerId) {
        return $http({
            method: 'GET',
            url: '../Administrator/Customer/GetCustomer/' + customerId
        });
    }
    this.addCustomer = function addCustomer(customer) {
        //  console.log(customer);
        return $http.post("../Administrator/Customer/AddCustomer", customer);
    };
    this.getNewCustomer = function getNewCustomer() {
        return $http({
            method: 'GET',
            url: '../Administrator/Customer/GetNewCustomer/'
        });
    }
}]);
