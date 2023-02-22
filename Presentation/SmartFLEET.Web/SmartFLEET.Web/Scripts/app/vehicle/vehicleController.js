angular.module('app.controllers').controller('vehicleController', vehicleController);

vehicleController.$inject = ['$scope', 'vehicleService', '$state','$rootScope'];
function vehicleController($scope, vehicleService, $state, $rootScope) {
    $.AdminLTE.controlSidebar.activate();
    $scope.vehicle = {};
    $scope.vehicles = [];
    $scope.$state = $state;
    vehicleService.getNewVehicle().then(function (response) {
       // console.log(response.data);
        $scope.vehicle = response.data;
    });
    $scope.LoadDetail =function(vehicleId) {
        console.log(vehicleId);
        
    }
    setTimeout(function () {
        if ($rootScope.customerId == null || $rootScope.customerId == undefined) {
            initVehicleList();
        } else {
            initVehicleListofCusomer($rootScope.customerId);
            $rootScope.customerId = null;
        }
    }, 500);
    
    $scope.addVehicle = function(vehicle) {

        vehicleService.addVehicle(vehicle).then(function (resp) {
            if (resp.data.errors.length > 0) {
                var msg = "";
                for (var i = 0; i < resp.data.errors.length; i++) {
                    msg = msg + resp.data.errors[i];
                }
                $.bootstrapGrowl(msg,
                    {
                        ele: 'body', // which element to append to
                        type: 'danger' // (null, 'info', 'danger', 'success')
                    });
                $scope.vehicle = resp.data.vehicle;
            } else {
                $.bootstrapGrowl("Operation has been terminated  successfully !", {
                    ele: 'body', // which element to append to
                    type: 'success' // (null, 'info', 'danger', 'success')
                });
                $scope.vehicle = {};
                $("#vehicleModal").modal("hide");
            }
        });
    };
    $scope.LoadModal   =function() {
        $("#vehicleModal").modal("show");
    }
}
function initVehicleListofCusomer(customerId) {
    $('#vehicle-list').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": '../Administrator/Vehicle/GetAllVehiclesForCustomer/?customerId=' + customerId,
            "type": "POST"
        },
        "columns": [
            {
                "data": "VehicleName",
                "render": function (data, type, row, meta) {
                    console.log(row.Id);
                    return '<a  ui-sref="vehicleDetail({vehicleId:\'' + row.Id + '\'})"> ' + data + '</a>';
                }
            },
            { "data": "VehicleType" },
            { "data": "Brand" },
            { "data": "Model" },
            { "data": "Vin" },
            { "data": "LicensePlate" },
            { "data": "CreationDate" },
            { "data": "InitServiceDate" },
            { "data": "VehicleStatus" }

        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex) {

            var $injector = angular.element(document.body).injector();
            var scope = angular.element(document.body).scope();

            $injector.invoke(function ($compile) {
                $compile(nRow)(scope);
            });

        },

    });
} function initVehicleList() {
    $('#vehicle-list').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": '../Administrator/Vehicle/GetAllVehicles/' ,
            "type": "POST"
        },
        "columns": [
            {
                "data": "VehicleName",
                "render": function (data, type, row, meta) {
                    console.log(row.Id);
                    return '<a  ui-sref="vehicleDetail({vehicleId:\'' + row.Id + '\'})"> ' + data + '</a>';
                }
            },
            { "data": "VehicleType" },
            { "data": "Brand" },
            { "data": "Model" },
            { "data": "Vin" },
            { "data": "LicensePlate" },
            { "data": "CreationDate" },
            { "data": "InitServiceDate" },
            { "data": "VehicleStatus" }

        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex) {

            var $injector = angular.element(document.body).injector();
            var scope = angular.element(document.body).scope();

            $injector.invoke(function ($compile) {
                $compile(nRow)(scope);
            });

        },

    });
}
