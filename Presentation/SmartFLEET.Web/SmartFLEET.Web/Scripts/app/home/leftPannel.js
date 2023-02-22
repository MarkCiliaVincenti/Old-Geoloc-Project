var startDateTime;
var TimeZoneInfo;
var isPeriod = false;
$(document).ready(function () {
    $("#start-period").datetimepicker({
        inline: false,
        format: 'd/m/Y H:i',
        formatDate: 'd/m/Y',
        lang: currentLang.split("-")[0]
    });
    $("#end-period").datetimepicker({
        inline: false,
        format: 'd/m/Y H:i',
        formatDate: 'd/m/Y',
        lang: currentLang.split("-")[0]
    });
    
    iniDownloadReports();
})
function initFullReport(date, ) {
    $("#chronogram").window('close');
    $("#prg-wwin").window('open');
    $('#prgBar').progressbar('setValue', 0);
    var $reportScope = getScope('reportController');
    $reportScope.startPeriod = formatDate(date);
    $reportScope.vehicleId = anchorId;
    $reportScope.Download(date, null);
    $reportScope.$apply();
}
function initActivitiesReport(date, end) {
    var $positionScope = getScope('positionController');
    $positionScope.vehicleId = anchorId;
    $positionScope.Download(date,end );
    $positionScope.$apply();
}
function iniDownloadReports() {
    $("#period-choice").on("change", function () {
        if (anchorId === null || anchorId.indexOf('00000000-0000-0000-0000-000000000000') !== -1) {
            alert(localization.EmptyTreeGuid);
            return;
        }
        switch (this.value) {
        case "CD": {
                const today = new Date();
                $("#start_datetime").hide();
                $("#end_datetime").hide();
                isPeriod = false;
                if (downloadFullReport)
                    initFullReport(today);
                else initActivitiesReport(today);

            }
                break;
        case "D":
            {
                $("#btn-dwnl").show();
                $("#start_datetime").show();
                $("#end_datetime").hide();
                isPeriod = false;

            }
                break;
        case "P":
            {
                $("#btn-dwnl").show();
                $("#start_datetime").show();
                $("#end_datetime").show();

                isPeriod = true;
            }
            break;
        }
    });
}
function downloadReport() {
    var date = $('#start-period').datetimepicker('getValue') + ':00';
    var end = null;
    if (isPeriod) {
        end = $('#end-period').datetimepicker('getValue') + ':00';
        if (new Date(end) < new Date(date))
            alert(localization.EndGraterThanStart);
        return;
    }
    
    if (downloadFullReport) {
        initFullReport(new Date(date));
    } else {
        if (!isPeriod)
            initActivitiesReport(moment(new Date(date)).format("YYYY-MM-DDTHH:mm:ss"));
        else {
            initActivitiesReport(moment(new Date(date)).format("YYYY-MM-DDTHH:mm:ss"), moment(new Date(end)).format("YYYY-MM-DDTHH:mm:ss"));
        }
    }
}
function getZoneName() {
    var d = new Date();
    var s = d.toString();
    return s.match(".*(\\((.*)\\))")[2];
}