var GpsData = [];
var timeLineData;
var lastSelectedBar;
var lastLine;
var lastarrowHead;
function initGpsData(periods, gpsCollection, divName) {
    // CleanTracePeriod();
    GpsData = gpsCollection;
    // console.log(periods);
    var container = document.getElementById(divName);
    var data = [];
    var end;
    if (periods.length === 0) return;
    if (periods[periods.length - 1] != undefined)
        end = periods[periods.length - 1].EndPeriod;
    var start = periods[0].StartPeriod;
    $.each(periods,
        function (i, v) {
            var activity = "";
            var style = "";
           
            switch (v.MotionStatus) {
                case "Stopped":
                    {
                        activity = localization.Stopped;
                        style =
                            "background-color:#DC143C;height:9px; border-radius:0;margin-top: 20px;border-color:transparent!important;border-width:0!important;";

                    }
                    break;
                case "Moving":
                    {
                        activity = localization.Moving;
                        style =
                            "background-color:#048b9a;height:30px;border-color:transparent!important; border-radius:0;margin-top: 20px;border-width:0!important;";
                    }
                    break;
                default:
                    {
                        activity = localization.Slowing;
                        style =
                            "background-color:#dab30a;height:30px;border-color:transparent!important; border-radius:0;margin-top: 20px;border-width:0!important;";
                    }
                    break;
            }
            var startTime = v.StartPeriod.split('T')[1].split(':')[0] +
                ':' +
                v.StartPeriod.split('T')[1].split(':')[1];
            var endTime = v.EndPeriod.split('T')[1].split(':')[0] +
                ':' +
                v.EndPeriod.split('T')[1].split(':')[1];
            var duration = "";
            if (v.Duration !== "")
                duration = secondsToHms(v.Duration);
            var template = '' + activity + ' ' + startTime + ' - ' + endTime;
            if (duration !== "") {
                template = template + ' (' + localization.Duration+': ' + duration + ')';
            }
            // console.log(v.DurationInSeconds);
            template = template + '\r';
            if (v.MotionStatus !== 'Stopped') {
                template = template + localization.DepartureAddress+": " + v.StartAddres + '\r';

                template = template + localization.ArrivalAddress+ ": " + v.ArrivalAddres + '\r';

                template = template + localization.AvgSpeed+": " + v.AvgSpeed + ' km/h\r';
            } else if (v.MotionStatus === 'Stopped' && v.StartAddres != null) {
                template = template + localization.Location+": " + v.StartAddres + '\r';

            } else {
                template = template + localization.Location + ": " + v.ArrivalAddres + '\r';

            }
            if (v.MotionStatus !== 'Stopped') template = template + localization.Distance +": " + v.Distance + " km." + '\r';

            data.push({
                id: i,
                group: null,
               // content: v.MovementState,
                style: style,
                start: v.StartPeriod,
                end: v.EndPeriod,
                title: template
            });

        });
    var result = new vis.DataSet(data);
    //  console.log(end);
    InitTimelineChart(container, result, start, end, 83);
}

function secondsToHms(seconds) {
    var d = Number(seconds);
    var h = Math.floor(d / 3600);
    var m = Math.floor(d % 3600 / 60);
    var s = Math.floor(d % 3600 % 60);

    var hDisplay = h > 0 ? h + (h == 1 ? " " +localization.Hour + ", " : localization.Hours+", ") : "";
    var mDisplay = m > 0 ? m + (m == 1 ? " " + localization.Minute : " " + localization.Minutes) : "";
    //  var sDisplay = s > 0 ? s + (s == 1 ? " second" : " seconds") : "";
    if (hDisplay !== "" || mDisplay !== "")
        return hDisplay + mDisplay;
    return "moins d'1 min";

}


function InitTimelineChart(container, data, start, end, height) {
    var today = new Date();
    timeLineData = data;
    today.setHours(24, 0, 0, 0);
    var max;
    if (sameDay(new Date(end), new Date())) {
        // ...
        max = today;
        end = today;
    } else max = end;
    var options = {
        width: '100%',
        locale: 'fr',
        height: height,
        editable: false,
        margin: {
            item: 10
        },
        selectable: true,
        start: start,
        zoomable: true,
        //maxZoom: 20,
        min: start,
        max: max,
        end: end,
        showCurrentTime: false,
        template: function (item) {
            var template = item.content;
            return "";
        },
        stack: false,
        format: {
            minorLabels: {
                minute: 'HH:mm',
                hour: 'HH'

            }
        }

    }

    // Create a Timeline
    var timeline = new vis.Timeline(container, null, options);
    timeline.setItems(data);
    timeline.on("click", function (properties) {
            onPeriodClick("click", properties);
        });


}

function onPeriodClick(event, properties) {
    if (properties.what != "item") return;
    var item = properties.item;
    var bar = $(properties.event.target);
    if (lastSelectedBar != null)
        lastSelectedBar.css("border", "color:transparent!important");
    lastSelectedBar = bar;
    bar.parent().css("border", "solid 2px #ff8000");
    var start = new Date(timeLineData.get(item).start);
    var end = new Date(timeLineData.get(item).end);
    var listOfGpsPoints = [];

    var s = -1;
    var e = -1;
    if (GpsData == null) return;
    for (var i = 0; i < GpsData.length; i++) {
        if (new Date(GpsData[i].GpsStatement).getTime() - start.getTime() >= 0 && s == -1) {
            s = i;
        }
        if (new Date(GpsData[i].GpsStatement).getTime() - end.getTime() > 0) {
            e = i - 1;
            break;
        }
    }

    if (e === -1 && s > -1) e = GpsData.length - 1;
    else if (s === -1 && e > -1) s = e;

    // console.log(s + " " + e);
    for (var j = s; j <= e; j++) {
        if (GpsData[j] != undefined)
            listOfGpsPoints.push(GpsData[j]);
    }
    if (listOfGpsPoints.length > 0) {

        var polygonArray = "[";
        for (var k = 0; k < listOfGpsPoints.length; k++) {
            var array = [];
            var gps = listOfGpsPoints[k];
            array.push(gps.Latitude, gps.Longitude);
            if (k < listOfGpsPoints.length - 1)
                polygonArray = polygonArray + "[" + gps.Latitude + " , " + gps.Longitude + "]" + ",";
            else polygonArray = polygonArray + "[" + gps.Latitude + " , " + gps.Longitude + "]";

        }
        polygonArray = polygonArray + "]";
        var arrow = L.polyline(JSON.parse(polygonArray), { color: 'blue' }).addTo(map);
        if (lastLine != null)
            map.removeLayer(lastLine);
        lastLine = arrow;
        if (lastarrowHead != null)
            map.removeLayer(lastarrowHead);
        var arrowHead = L.polylineDecorator(arrow,
            {
                patterns: [
                    {
                        offset: 25,
                        repeat: 50,
                        symbol: L.Symbol.arrowHead(
                            { pixelSize: 15, pathOptions: { fillOpacity: 1, weight: 0 } })
                    }
                ]
            }).addTo(map);
        map.fitBounds(JSON.parse(polygonArray));
        lastarrowHead = arrowHead;
    }

    //data.get(properties.time);
}
function sameDay(d1, d2) {
    return d1.getUTCFullYear() === d2.getUTCFullYear() &&
        d1.getUTCMonth() === d2.getUTCMonth() &&
        d1.getUTCDate() === d2.getUTCDate();
}