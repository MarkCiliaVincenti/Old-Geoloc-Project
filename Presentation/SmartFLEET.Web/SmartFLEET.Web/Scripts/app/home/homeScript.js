var markerGroup;
var map;
var markers = [];
var layout;
var currentVehicleId;
var positionModalOpend = false;
var currentBarId;
var PinStopMarkers = [];
var targetMode = false;
var reportModalOpend = false;
var hub;
var anchorId = null;
var downloadFullReport = false;
var getPossition = false;
var layout;
var currentLang = "";

$(document).ready(function () {

    currentLang = getCookie("culture");
    
    //layout.close("west");
    initJstree();
    loadData(0.33);
    initMap('map');
    loadData(0.67);
    initSignalR();
    initCalender();
    //    $("#accordion").accordion();
    $("#vehicles").select2({
        // width: 175

    });
    $("#vehicles-pos").select2();
    $("#period-choice").select2();
    //$("#daily-report").tabs();
   

    loadData(1);
   
    var height = $(window).height()/2 + 130;
    $('#chronogram').window({
        left: 315,
        top: height,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        closable: false,
        draggable:false
    });
    $('#report-win').window({
        left: 315,
        top: 55,
        collapsible: false,
        minimizable: false,
        maximizable: false,
       // closable: false,
        draggable: false
    });
    $('#zone-interest').window({
        left: 315,
        top: 60,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        // closable: false,
        draggable: false
    });
    $('#zone-win').window({
        left: 315,
        top: 60,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        // closable: false,
        draggable: false
    });
    $('#prg-wwin').window({
        //left: 315,
        //top: 55,
        collapsible: false,
        minimizable: false,
        maximizable: false,
         closable: false,
        draggable: false
    });
    $("#chronogram").window('close');
    $("#report-win").window('close');
    $('#prg-wwin').window('close');
    $('#zone-interest').window('close');
    $('#zone-win').window('close');
    $("#btn-report").on('click', function() {
        downloadFullReport = true;
        getPossition = false;
        $("#report-win").window('open');
        layout.open('west');
    });
    $("#btn-position").on('click', function () {
        downloadFullReport = false;
        getPossition = true;
        $("#report-win").window('close');
        layout.open('west');
    });
    $("#btn-auto").on('click', function () {
        downloadFullReport = false;
        getPossition = false;
        $("#zone-interest").window('close');
        $("#chronogram").window('close');
        $("#report-win").window('close');
        $('#prg-wwin').window('close');
        //layout.toggle('west');
    });
    $('#btn-settings').on('click',
        function() {
            $("#zone-interest").window('open');
            $("#chronogram").window('close');
            $("#report-win").window('close');
            $('#prg-wwin').window('close');
        });
    window.addEventListener('resize', function (event) {
        // do stuff here
        $("#left-panel").height( $(window).height()-300);
    });

    $("#search-tree").on('keyup',
        function() {
            var term = $("#search-tree").val();
            $("#container").jstree('search', term);
        });
});


 
function initCalender() {
    $('#cc').calendar({
        onSelect: function (date) {

            if (downloadFullReport) {
                if (anchorId === "" || anchorId.indexOf('00000000-0000-0000-0000-000000000000') !== -1) {
                    alert(localization.EmptyTreeGuid);
                    return;
                }
                $("#chronogram").window('close');
                $("#prg-wwin").window('open');
                $('#prgBar').progressbar('setValue', 0);
                var $reportScope = getScope('reportController');
                $reportScope.startPeriod = formatDate(date);
                $reportScope.vehicleId = anchorId;
                $reportScope.Download();
                $reportScope.$apply();

            } else if (getPossition) {
                var $positionScope = getScope('positionController');
                $positionScope.vehicleId = anchorId;
                $positionScope.Download(formatDate(date));
                $positionScope.$apply();
            }
        }
    });
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}

function getScope(ctrlName) {
    var sel = 'div[ng-controller="' + ctrlName + '"]';
    return angular.element(sel).scope();
}
function initMap(mapId) {
    map = L.map(mapId, {
        center: [36.7525000, 3.0419700],
        zoom: 8,
        zoomControl: true
    });
    markerGroup = L.layerGroup().addTo(map);
    layout = L;
    // load a tile layer
    var defaultLayer = L.tileLayer.provider('OpenStreetMap.Mapnik').addTo(map);
    map.addLayer(defaultLayer);

    initVehicleMarkers();
    initZones();
    window.dispatchEvent(new Event('resize'));
    map.invalidateSize();
}
function initVehicleMarkers() {

    $.ajax({
        url: currentLang+'/VehicleReport/AllVehiclesWithLastPosition',
        success: onGetAllVehiclesSuccess
    });
}
function initZones() {
    $.ajax({
        url: currentLang+'/InterestArea/GetAllZones',
        success: onGetAllZonesSuccess
    });
}
function onGetAllZonesSuccess(data) {
    console.log(data);
    for (var i = 0; i < data.length; i++) {
        var c = L.circle([data[i].Latitude, data[i].Longitude], {
            color: 'red',
            fillColor: '#f03',
            fillOpacity: 0.5,
            radius: data[i].Radius
        }).addTo(map);
        var marker = L.marker([data[i].Latitude, data[i].Longitude], { title: data[i].Name }).bindTooltip(data[i].Name,
            {
                permanent: true,
                direction: 'top'
            }).addTo(map);
    }
}
function initSignalR() {
    hub = $.connection.signalRHandler;
    hub.client.receiveGpsStatements = onRecieveData;
    hub.client.sendprogressVal = onRecieveProgressVal;
    hub.client.receiveVehicleEvent = onReceiveVehicleEvent;
    $.connection.hub.start().done(joinSignalRGroup);
}
function onRecieveProgressVal(val) {
    $('#prgBar').progressbar('setValue', val);
} 
function onRecieveData(gpsStatement) {
    var thisIcon = new L.Icon();
    removeMarker(gpsStatement);
    thisIcon.options.iconUrl = gpsStatement.ImageUri;
   
    var template = "<div><h4><b> <b>"+localization.Vehicle+"</b>: " +
        gpsStatement.VehicleName +
        "</b></h4> <b>" +localization.Address+"</b>: " +
        gpsStatement.Address +
        "" +
        "<p> <b>" + localization.Speed+"</b>: " +
        gpsStatement.Speed +
        "Km/H</p>" +
        "</h5>" +
        "<p> <b>Latitude</b>: " +
        gpsStatement.Latitude +
        "</p>" +
        "</h5>" +
        "<p> <b>Longitude</b>:  " +
        gpsStatement.Longitude +
        "</p>" +
        "</div>";
    var label = "<h5><b>" + gpsStatement.VehicleName + "</b></h5>";
    var marker = L.marker([gpsStatement.Latitude, gpsStatement.Longitude],
            { title: gpsStatement.VehicleId, icon: thisIcon })
        .bindPopup(template,
            {
                permanent: true,
                direction: 'topleft'
            }).bindTooltip(label,
            {
                permanent: true,
                direction: 'top'
            }
        ).addTo(map);
    markers.push(marker);
    if (anchorId != null && gpsStatement.VehicleId === anchorId) {
        map.setView([gpsStatement.Latitude, gpsStatement.Longitude], 15, { animation: false });
        marker.openPopup();
       // getChronogram();
    }
}
function joinSignalRGroup() {
    var groupName = $("#client-group").val();
    hub.server.join(groupName);

}
function onReceiveVehicleEvent(event) {
    $.bootstrapGrowl(event.Message, {
        ele: 'body', // which element to append to
        type: 'success' // (null, 'info', 'danger', 'success')
    });
}
function onGetAllVehiclesSuccess(result) {
    for (var i = 0; i < result.length; i++) {
        var item = result[i];
        var icon = new L.Icon();
        var template = "<div><h4><b> <b>" + localization.Vehicle+"</b>: " +
            item.VehicleName +
            "</b></h4> <b>" + localization.Address+"</b>: " +
            item.Address +
            "" +
            "<p> <b>" + localization.Speed+"</b>: " +
            item.Speed +
            "Km/H</p>" +
            "</h5>" +
            "<p> <b>Latitude</b>: " +
            item.Latitude +
            "</p>" +
            "</h5>" +
            "<p> <b>Longitude</b>:  " +
            item.Longitude +
            "</p>" +
            "</div>";

        icon.options.iconUrl = item.ImageUri;
        var label = "<h5><b>" + item.VehicleName + "</b></h5>"
        var marker = L.marker([item.Latitude, item.Longitude], { title: item.VehicleId, icon: icon })
            .bindPopup(template,
            {
                permanent: true,
                direction: 'topleft'
            }).bindTooltip(label,
            {
                permanent: true,
                direction: 'top'
            }
            ).addTo(map).on('click', clickZoom);
        console.log(marker.options);
        markers.push(marker);
       
    }

}
function clickZoom(e) {
    map.setView(e.target.getLatLng(), 15);
}

function initJstree() {
   
    $('#container').jstree({
        "core": {
            "data": { "url": currentLang+"/Home/LoadNodes" }
        },
        "search": {
            "case_insensitive": false,
            "show_only_matches": true
        },
        'contextmenu': {
            'items': customMenu
        },

        "plugins": ['theme', "html_data", "search", "contextmenu"]
    });
    $(document).on('click',
        '.jstree-anchor',
        function (e) {
            anchorId = $(this).parent().attr('id');
            console.log(anchorId);
            currentVehicleId = anchorId;
            if (anchorId != 'vehicles-00000000-0000-0000-0000-000000000000'
                && anchorId != 'drivers-00000000-0000-0000-0000-000000000000') {
               
               // initPositionWind();
                positionModalOpend = true;
                if (!getPossition) {
                    markerFunction(anchorId);

                    //initWait();
                    //getChronogram();
                }

                
            } //
        });
}
function getChronogram() {
    $.ajax({
        url: currentLang+'/Position/GetCurrentDayPosition/?vehicleId=' + anchorId,
        dataType: 'json',
        success: onGetTargetsSuccess,
        error: onGetTargetsSuccess
    });
}
function onGetTargetsSuccess(result) {
    if (result.responseText != undefined) {
        result = JSON.stringify(result.responseText);
        result = jQuery.parseJSON(result);
        console.log(result);
    }

    removeLineMarckers();
    $("#gps-activity").html("");
    $("#vehicle").html("");
    $("#date").html("");
    if (result.length == 0) {
        $("#map").waitMe("hide");

        return;

    }
    $("#vehicle-name").html("Véhicule: " + result.Periods[0].VehicleName);
    $("#date-pos").html("Date: " + result.Periods[0].CurrentDate);
    initGpsData(result.Periods, result.GpsCollection, "gps-activity");
    $("#map").waitMe("hide");
    console.log(result.Periods);
    
}
function initWait() {
    $("#map").waitMe({
        effect: 'bounce',
        text: 'Téléchargement en cours ...',
        color: '#000',
        maxSize: '',
        waitTime: -1,
        textPos: 'vertical',
        fontSize: '',
        source: '',
        onClose: function () { }
    });
}

function customMenu(context) {
    var items = {
        aclRole: {
            label: "Ajouter un conducteur",
            action: function (obj) {
                 },
            icon: "fa fa-user"
        },
        deleteRole: {
            label: "Position par période",
            action: function (obj) {
               
            },
            icon: "fa fa-map-marker"
        }
    }
    return items;
}

function markerFunction(id) {
    for (var i in markers) {
        if (markers[i].options == undefined)
            continue;
        var markerID = markers[i].options.title;
        var position = markers[i].getLatLng();
        if (markerID == id) {
            map.setView(position, 15);
            markers[i].openPopup();
        };
    }
}

function removeLineMarckers() {
    if (lastLine != null && lastLine != undefined)
        map.removeLayer(lastLine);
    if (lastarrowHead != null && lastarrowHead != undefined)
        map.removeLayer(lastarrowHead);
}

function removeMarker(gpsStatement) {
    //console.log(markers);
    for (var i = 0; i < markers.length; i++) {
        var marker = markers[i];
        var markerID = marker.options.title;
        // console.log(markerID, gpsStatement.VehicleId);
        if (_guidsAreEqual("" + markerID + "", "" + gpsStatement.VehicleId + "") == 0) {
            map.removeLayer(markers[i]);
            //markers.splice(i, 1);
        }
    }

}

function _guidsAreEqual(left, right) {

    return left.localeCompare(right);
};

function loadData(percent) {
    $('body').loadie(percent);
}


function parseDate(input) {
    var parts = input.match(/(\d+)/g);
    // new Date(year, month [, date [, hours[, minutes[, seconds[, ms]]]]])
    return new Date(parts[0], parts[1] - 1, parts[2]); // months are 0-based
}
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}