angular.module('app.controllers').controller('positionController', positionController);

positionController.$inject = ['$scope', 'positionService', 'reportService'];
function positionController($scope, positionService,  reportService) {
    $scope.vehicleId = "";
    $scope.startPeriod = "";
   
    reportService.getVehicles().then(function (resp) {
        //   console.log(resp.data);
        $scope.vehicles = resp.data;
    });
    $scope.$watch("startPeriod",
        function(date) {
            if ($scope.vehicleId !== "") {
                $("#gps-activity").html("");
                $("#vehicle-name").html("");
                $("#date").html("");
                $("#begin-ser").html("");
                $("#end-ser").html("");
                $("#distance").html("");
                $("#speed").html("");
                //initPositionWind();
                removeLineMarckers();

                initWait();

                positionService.getPosition($scope.vehicleId, date).then(function(resp) {

                    console.log(resp.data);
                    if (resp.data === null || resp.data === undefined) {
                        $('#map').waitMe("hide");

                        return;

                    }
                    $("#vehicle-name").html("<b>Véhicule: </b>" + resp.data.Vehiclename);
                    $("#distance").html("<b>Distance: </b>" + resp.data.Distance);

                    if (resp.data.Periods != null && resp.data.Periods.length > 0) {
                        $("#date-pos").html("<b>Date:</b> " + resp.data.Periods[0].CurrentDate);
                        if (resp.data.Periods[0].BeginService != null)
                            $("#begin-ser").html("<b> Début de conduite : </b>" + resp.data.Periods[0].BeginService.split(" ")[1]);
                       else $("#begin-ser").html("<b>Début de conduite</b>: inconnu");
                       var length = resp.data.Periods.length - 1;
                       if (resp.data.Periods[length].EndService != null) $("#end-ser").html("<b>Fin de conduite :</b> " + resp.data.Periods[length].EndService.split(" ")[1]);
                       initGpsData(resp.data.Periods, resp.data.GpsCollection, "gps-activity");
                       var lat = resp.data.Periods[length].Latitude;
                       var lng = resp.data.Periods[length].Logitude;
                       map.setView([lat, lng], 15);
                   }
                    $('#map').waitMe("hide");
                   
                });
            }
        });

    $scope.Download = function (start,end)
    {
        if ($scope.vehicleId !== "") {
            $("#gps-activity").html("");
            $("#vehicle-name").html("");
            $("#date").html("");
            $("#begin-ser").html("");
            $("#end-ser").html("");
            $("#distance").html("");
            $("#speed").html("");
            $("#chronogram").window('open');
            //initPositionWind();
            removeLineMarckers();

            initWait();

            positionService.getPosition($scope.vehicleId, start, end).then(function (resp) {

                console.log(resp.data);
                if (resp.data === null || resp.data === undefined) {
                    $('#map').waitMe("hide");

                    return;

                }
                $("#vehicle-name").html("<b>Véhicule: </b>" + resp.data.Vehiclename);
                $("#distance").html("<b>Distance: </b>" + resp.data.Distance);

                if (resp.data.Periods != null && resp.data.Periods.length > 0) {
                    $("#date-pos").html("<b>Date:</b> " + resp.data.Periods[0].CurrentDate);
                    if (resp.data.Periods[0].BeginService != null)
                        $("#begin-ser").html("<b> Début de conduite : </b>" + resp.data.Periods[0].BeginService.split(" ")[1]);
                    else $("#begin-ser").html("<b>Début de conduite</b>: inconnu");
                    var length = resp.data.Periods.length - 1;
                    if (resp.data.Periods[length].EndService != null) $("#end-ser").html("<b>Fin de conduite :</b> " + resp.data.Periods[length].EndService.split(" ")[1]);
                    initGpsData(resp.data.Periods, resp.data.GpsCollection, "gps-activity");
                    var lat = resp.data.Periods[length].Latitude;
                    var lng = resp.data.Periods[length].Logitude;
                    addStopPins(resp.data.Periods);
                    map.setView([lat, lng], 8);
                }
                $('#map').waitMe("hide");
                //$('#chronogram').html($("#pos-result"));
            });
        }
    }
}
function addStopPins(periods) {
    var icon = new L.Icon();
    icon.options.iconUrl = "../../assets/route_stop.png";

    for (var i = 0; i <periods.length; i++) {
        if (periods[i].MotionStatus === "Stopped") {
            var tmp = initStopTempale(periods[i]);
            var marker = L.marker([periods[i].Latitude, periods[i].Logitude], { icon: icon }).bindPopup(tmp,
                {
                    permanent: false,
                    direction: 'topleft'
                }).addTo(map);
            markers.push(marker);
        }
    }

}
function initStopTempale(period) {
    console.log(period);
    var template = "<div><h4><b> <b>Véhicule</b>: " +
        period.VehicleName +
        "</b></h4> <b>Adresse</b>: " +
        period.ArrivalAddres +
        "" +
        "<p> <b>Durée</b>: " +
        secondsToHms( period.Duration) +
        "" +
        "<p> <b>Latitude</b>: " +

        period.Latitude +
        "</p>" +
        "</h5>" +
        "<p> <b>Longitude</b>:  " +
        period.Logitude +
        "</p>" +
        "</div>";
    return template;
}