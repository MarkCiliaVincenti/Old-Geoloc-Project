angular.module('app.services').service('userService', ['$http', function ($http) {

    this.getUser = function getUser(userId) {
        return $http({
            method: 'GET',
            url: '../Administrator/user/GetUser/' + userId
        });
    }
    this.addUser = function addUser(user) {
        //  console.log(user);
        return $http.post("../Administrator/user/AddUser", user);
    }
    this.getTimeZones = function getTimeZones() {
        //  console.log(user);
        return $http.post( "../Administrator/User/GetTimeZones");
    }
}]);
