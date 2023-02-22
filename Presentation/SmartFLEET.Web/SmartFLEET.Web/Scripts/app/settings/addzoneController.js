
angular.module('app.controllers').controller('addzoneController', addzoneController);

addzoneController.$inject = ['$scope', '$http', ];

function addzoneController($scope, $http) {
    $scope.zone = {};
    $scope.Radius = 200;
    var newMarker;
   var map2 = L.map("map2", {
        center: [36.7525000, 3.0419700],
        zoom: 8,
        zoomControl: true
    });
   
    map2.on('click', function (e) {
        var coord = e.latlng;
        $scope.zone.Latitude = coord.lat;
        $scope.zone.Longitude = coord.lng;
        if (newMarker != undefined)
            map2.removeLayer(newMarker);
         newMarker = new L.marker(e.latlng).addTo(map2);
        console.log($scope.zone);
    });
    var defaultLayer = L.tileLayer.provider('OpenStreetMap.Mapnik').addTo(map2);
    map2.addLayer(defaultLayer);
    
    window.dispatchEvent(new Event('resize'));
    $scope.addZone = function () {
        $('#zone-win').window('open');
        map2.invalidateSize();
    }
    $scope.saveZone = function() {
        console.log($scope.zone);
        $http.post(currentLang + + "/InterestArea/AddNewZone", $scope.zone).then(function(resp) {
            console.log(resp);
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
                $('#dg').datagrid('reload');
                // $("#vehicleModal").modal("hide");
            } 
        });
    }
}