var intMessageFadeTimeout = 10000;
var sSettingsURL = 'Scale/Settings';
var sUpdateLabelURL = '/Label/Update';
var sUpdateJobInfolURL = '/JobInfo/Update';
var sUpdateScaleInfoURL = '/Scale/Update';

$(document).ready(function () {
    getSettings();
});

//Prevent forms from submitting so that I can use the jquery button click method with an ajax call instead...
$(".preventsubmit").submit(function (event) {
    event.preventDefault();
});

$("#btnUpdateLabel").button().click(function () {
    $.ajaxSetup({ async: false, dataType: "json" });
    $.post(sUpdateLabelURL, $("#frmUpdateLabel").serialize())
        .done(function (data) {
            console.log("done posting label data...");
            $("#LabelMessages").text(data.Message).show().fadeOut(intMessageFadeTimeout);
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous
})

$("#btnUpdateJobInfo").button().click(function () {
    $.ajaxSetup({ async: false, dataType: "json" });
    $.post(sUpdateJobInfolURL, $("#frmUpdateJobInfo").serialize())
        .done(function (data) {
            console.log("done posting JobInfo data...");
            $("#JobInfoMessages").text(data.Message).show().fadeOut(intMessageFadeTimeout);
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous
})

$("#btnUpdateScaleSettings").button().click(function () {
    var data = $("#frmUpdateScaleSettings").serializeArray();

    data.push({ name: 'BacklightColor', value: $("#BacklightColors").val() });

    $.ajaxSetup({ async: false, dataType: "json" });
    $.post(sUpdateScaleInfoURL, data)
        .done(function (data) {
            console.log("done posting ScaleSettings data...");
            $("#ScaleSettingsMessages").text(data.Message).show().fadeOut(intMessageFadeTimeout);
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous
})

function getSettings() {
    $.ajaxSetup({ async: false, dataType: "json" });
    $.getJSON(sSettingsURL, {})
        .done(function (data) {
            $("#TitleMACAddress").text(data.MACAddress);
            $("#MACAddress").val(data.MACAddress);
            $("#IsDhcpEnabled").val(data.IsDhcpEnabled);
            $("#NetworkInterfaceType").val(data.NetworkInterfaceType);
            $("#IPAddress").val(data.IPAddress);
            $("#NetMask").val(data.NetMask);
            $("#Gateway").val(data.Gateway);
            $("#DnsAddresses").val(data.DnsAddresses);

            $("#LabelFormat").val(data.LabelFormat);

            $("#Job").val(data.Job);
            $("#Suffix").val(data.Suffix);
            $("#Operation").val(data.Operation);

            $("#ShopTrakTransactionsURL").val(data.ShopTrakTransactionsURL);
            $("#PieceWeight").val(data.PieceWeight);
            $("#NetWeightAdjustment").val(data.NetWeightAdjustment);

            data.BacklightColors.forEach(function (obj) {
                if (obj == data.BacklightColor)
                    $("#BacklightColors").append("<option selected=\"selected\" value=\"" + obj + "\">" + obj + "</option>");
                else
                    $("#BacklightColors").append("<option value=\"" + obj + "\">" + obj + "</option>");
            });
            $("#BacklightColors").multiselect({
                multiple: false,
                minWidth: 100,
                header: false, // "Select a Work Order Type",
                noneSelectedText: "Select Type",
                selectedList: 1 //this is what puts the selected value onto the select box...
            });
            $('#BacklightColors .ui-multiselect').css('width', '100px');

            $("#Item").val(data.Item);
            $("#Employees").val(data.Employees);
            $("#PieceCount").val(data.PieceCount);
            $("#NetWeight").val(data.NetWeight);
            $("#PrintDateTime").val(data.PrintDateTime);
        }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
            console.log("error thrown when getting settings...");
            $("#Messages").text(errorThrown).show().fadeOut(intMessageFadeTimeout);
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous
}