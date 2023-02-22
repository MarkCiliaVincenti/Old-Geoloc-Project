angular.module('app.controllers').controller('addMobileUnitController', addMobileUnitController);

addMobileUnitController.$inject = ['$scope', 'mobileUnitService'];
function addMobileUnitController($scope, mobileUnitService) {
    $scope.Brands = ["Teltonika", "GTA02", "Tk103", "Unknown"];
    $scope.onValidate = function(mobileUnit) {
        mobileUnitService.addMobileUnit(JSON.stringify(mobileUnit)).then(function(resp) {
            if (resp.data == "Ok") {
                $.bootstrapGrowl("Operation has been terminated  successfully !",
                    {
                        ele: 'body', // which element to append to
                        type: 'success' // (null, 'info', 'danger', 'success')
                    });
                $scope.mobileUnit = {};
            } else {
                $.bootstrapGrowl(resp.data, {
                    ele: 'body', // which element to append to
                    type: 'danger' // (null, 'info', 'danger', 'success')
                });
            }
        });
    }
}