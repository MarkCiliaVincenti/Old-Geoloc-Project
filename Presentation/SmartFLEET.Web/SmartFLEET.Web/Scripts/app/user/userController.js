angular.module('app.controllers').controller('userController', userController);

userController.$inject = ['$scope', 'userService'];
function userController($scope, userService) {
    $scope.user = {};
    //userService.getNewuser().then(function (response) {
    //    console.log(response.data);
    //    $scope.user = response.data;
    //});
    $scope.adduser = function(user) {
        console.log(user);
        //userService.adduser(JSON.stringify(user)).then(function (data) {
        //    console.log(data);
        //});
    };

}