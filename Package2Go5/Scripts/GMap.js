jQuery(document).ready(function ($) {
    Initialize();

    //SortList
    $('#waypoints').sortable({
        items: "li:not(:first)",
        update: function (event, ui) { calcRoute(); }
    });

    $('#waypoints').disableSelection();

    //Fill values
    $('#from').focusout(function () {
        $(this).attr("value", $(this).val());
        calcRoute();
    });

    $('#waypoints').on('focusout', '.waypoint', function (e) {
        $(this).attr("value", $(this).val());
        calcRoute();
    });

    WayPoints();

    if ($('#Items').length != 0)
        Datatable();

    $("form").submit(function (e) {
        e.preventDefault();
        $('fieldset input[name=Items]').each(function (index) {
            $(this).attr("name", "Items[" + index + "].id");
        });

        $('#waypoints li').each(function () {
            var input = $(this).children('input');
            if (typeof input.attr('id') != "undefined" && input.attr('id') != "from")
                input.val(input.attr('id') + ":" + input.val());
            else
                input.val(0 + ":" + input.val());
        });
        this.submit();
    });

    //Calculate travel price
    $('#oilPrice').focusout(function () {
        if ($('#oilperKm').val() != "") {
            var cost = $(this).val() * $('#oilperKm').val() * $('#distance').text() / 100;
            $('#totalCost').text("Estimated price: " + Math.ceil(cost) + " " + $('#currency').val());
            $('#travelCost').text("Travel Cost: " + (Math.ceil(cost)-parseInt($('#payments').text()))+" "+$('#currency').val());
        }
    });
    $('#oilperKm').focusout(function () {
        if ($('#oilPrice').val() != "") {
            var cost = $(this).val() * $('#oilPrice').val() * $('#distance').text() / 100;
            $('#totalCost').text("Estimated price: " + Math.ceil(cost) + " " + $('#currency').val());
            $('#travelCost').text("Travel Cost: " + (Math.ceil(cost) - parseInt($('#payments').text())) + " " + $('#currency').val());
        }
    });

});

function calculatePaymentsForService()
{
    var currencies = {};
    var paymentsForService = 0;
    var rows = $("#Items").dataTable().fnGetNodes();
    for (var i = 0; i < rows.length; i++) {
        if ($(rows[i]).find("td:eq(8)").children().html() == "Remove") {
            if (currencies[$(rows[i]).find("td:eq(5)").text()] == null) {
                currencies[$(rows[i]).find("td:eq(5)").text()] = parseInt($(rows[i]).find("td:eq(4)").text());
            }
            else {
                currencies[$(rows[i]).find("td:eq(5)").text()] += parseInt($(rows[i]).find("td:eq(4)").text());
            }
            paymentsForService += parseInt($(rows[i]).find("td:eq(6)").text());
        }
    }
    $('#paymentsforservice').html("Total Payments For Services: <span id='payments'>" + paymentsForService + " " + $('#currency').val() + "</span>");

    //Payments For Service By Curencies
    $('#paymentsByCurrencies').html("");
    for (var key in currencies) {
        if (currencies.hasOwnProperty(key)) {
            $('#paymentsByCurrencies').append("<li>" + key + ": " + currencies[key] + "</li>");
        }
    }
}

function Datatable()
{
    var oTable = $('#Items').dataTable({
        "aaSorting": [[4, "desc"]],
        "sDom": '<"top"l>rt<"bottom"ip>',
        "aoColumns": [null, null, null, null, null, null, null, null, null],
    });

    //Offer filter
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');

    if (sURLVariables != "") {
        oTable.fnFilter("^\\s*" + sURLVariables[0].split('=')[1] + "\\s*$", 0, true);
        $('#over_map').show();
        $('#showItems').text("Hide Items");
    }

    $('#action').change(function () {
        oTable.fnFilter($(this).val());
    });

    //filter
    $("tfoot input:not([id])").keyup(function () {
        oTable.fnFilter(this.value, $(this).attr("name"));
    });

    //User Price
    MinMax('userPriceMin', 'userPriceMax', 6, oTable);

    //Price
    MinMax('min', 'max', 4, oTable);

    //link to item details
    $("#Items").on("dblclick", "tr", function () {
        var iPos = oTable.fnGetPosition(this);
        var aData = oTable.fnGetData(iPos);
        window.location.href = 'Items/Details/' + aData[0];
    });

    //Add Remove Items Address
    $("tbody").on("click", "#Add", function (e) {
        e.preventDefault();
        if ($(this).attr("title") != "") {
            generateWaypoints($(this).attr("title") + ":" + $(this).parent().parent().children(':nth-child(3)').html());
            
            $(this).attr("id", "Remove");
            $(this).text("Remove");
            RedrawTable();
            calcRoute();
        }
    });

    $("tbody").on("click", "#Remove", function (e) {
        e.preventDefault();
        //Should be at least one waypoint
        if ($('#waypoints li').size() > 2 && $(this).attr("title") != "") {

            if ($(this).prev().attr("id") != 0) {
                $('input[name=Items][value=' + $(this).attr("title") + ']').remove();
            }

            $('#' + $(this).attr("title")).parent().remove();

            $(this).attr("id", "Add");
            $(this).text("Add");
            RedrawTable();
            calcRoute();
        }
    });
}

function MinMax(class1, class2, index, oTable)
{
    $.fn.dataTableExt.afnFiltering.push(
        function (oSettings, aData, iDataIndex) {
            var iMin = document.getElementById(class1).value * 1;
            var iMax = document.getElementById(class2).value * 1;
            var iVersion = aData[index] == "-" ? 0 : aData[index] * 1;
            if (iMin == "" && iMax == "") {
                return true;
            }
            else if (iMin == "" && iVersion <= iMax) {
                return true;
            }
            else if (iMin <= iVersion && "" == iMax) {
                return true;
            }
            else if (iMin <= iVersion && iVersion <= iMax) {
                return true;
            }
            return false;
        }
    );

    $('#'+class1).keyup(function () { oTable.fnDraw(); });
    $('#'+class2).keyup(function () { oTable.fnDraw(); });
}

function RedrawTable() {
    if ($('#Items').length != 0) {
        $('#Items').dataTable().fnDestroy();
        otable = $('#Items').dataTable({
            "aaSorting": [[4, "desc"]],
            "sDom": '<"top"l>rt<"bottom"ip>',
            "aoColumns": [null, null, null, null, null, null, null, null, null],
        });
    }
}

function WayPoints() {

    var addresses = $('input[name=waypoints]').val();

    if (typeof addresses != "undefined") {

        var array = addresses.split(';');

        if ($('input[name=from]').val().split(':')[0] != 0)
            $('input[name=from]').attr("id", $('input[name=from]').val().split(':')[0]);
        $('input[name=from]').val($('input[name=from]').val().split(':')[1]);

        for (var i = 0; i < array.length; i++) {
            generateWaypoints(array[i]);
        }

        calcRoute();
        $('input[name=waypoints]').val("");
    }

    $('#addWaypoint').click(function (e) {
        generateWaypoints();

        e.preventDefault();
    });

    $('#waypoints').on('click', '#remWaypoint', function (e) {
        if ($('#waypoints li').size() > 2) {

            var id = $(this).prev().attr("id");

            if (id != 0) {
                $('input[name=Items][value=' + id + ']').remove();


                $('a[title='+ id +']').text("Add");
                $('a[title=' + id + ']').attr("id", "Add");
            }

            $(this).parents('li').remove();
            RedrawTable();
            calcRoute();
        }
    });

}

function generateWaypoints(value)
{
    var last = $('#waypoints li').last();
    var newLast = $(last).clone();
    var name = "waypoint_" + (last.index() + 1);
    var array;

    if (newLast.find('input').attr('id') == "from") {
        newLast.find('span').remove();
        newLast.find('input').addClass("waypoint");
    }

    $(newLast).find('input').attr("name", name);

    if (typeof value != "undefined") {
        array = value.split(':');
        if (array[0] != 0) {
            $(newLast).find('input').attr("id", array[0]);
            $('fieldset').append('<input type=hidden name=Items value="' + array[0] + '"/>');

            //Change buttons in ItemsList
            $('a[title=' + array[0] + ']').text("Remove");
            $('a[title='+ array[0] +']').attr("id", "Remove");
        }
        $(newLast).find('input').val(array[1]);

        if (id == array[0]) {
            $(newLast).find('input').attr("value", "");
        }
        else {
            $(newLast).find('input').attr("value", value);
            $(newLast).find('input').attr("id", array[0]);
        }
    }
    else
        $(newLast).find('input').val("");

    if (last.index() == 0)
        newLast.append('<a id="remWaypoint">Remove</span></a>');

    $('#waypoints').append(newLast);

    new google.maps.places.Autocomplete(document.getElementsByName(name)[0]);
    
}

//Google Maps
var directionsDisplay;
var directionsService = new google.maps.DirectionsService();
var map;
var geocoder = new google.maps.Geocoder();

function Initialize() {
    
    google.maps.visualRefresh = true;

    directionsDisplay = new google.maps.DirectionsRenderer();

    var Lithuania = new google.maps.LatLng(55.00, 25.15);

    var mapOptions = {
        zoom: 8,
        center: Lithuania,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };
    
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

    $('#waypoints > li').each(function () {
        new google.maps.places.Autocomplete(
            document.getElementsByName(
                $(this).children('input').attr('name'))[0]);
    });

    directionsDisplay.setMap(map);
}

function calcRoute() {
    
    var start = $('#from').val();

    var waypts = [];

    var array = $(".waypoint").map(function () { return $(this).val(); }).get();

    if(array.length > 1)
    {
        for (var i = 0; i < array.length - 1; i++) {
            waypts.push({
                location: array[i],
                stopover: true
            });
        }
    }

    var request = {
        origin: start,
        destination: array[array.length - 1],
        waypoints: waypts,
        optimizeWaypoints: false,
        travelMode: google.maps.TravelMode.DRIVING
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
            var route = response.routes[0];

            var totalDistance = 0;
            var totalDuraction = 0;
            var summaryPanel = document.getElementById('directions_panel');
            summaryPanel.innerHTML = '';
            // For each route, display summary information.

            var ind = 0
            $('#waypoints li').each(function () {
                if (ind == 0) {
                    $(this).find('input').val(route.legs[ind].start_address);
                }
                else {
                    $(this).find('input').val(route.legs[ind - 1].end_address);
                }
                ind++;
            });


            for (var i = 0; i < route.legs.length; i++) {

                var routeSegment = i + 1;
                summaryPanel.innerHTML += '<b>Route Segment: ' + routeSegment + '</b><br>';
                summaryPanel.innerHTML += route.legs[i].start_address + ' to ';
                summaryPanel.innerHTML += route.legs[i].end_address + '<br>';
                summaryPanel.innerHTML += route.legs[i].distance.text + '<br><br>';
                summaryPanel.innerHTML += route.legs[i].duration.text + '<br><br>';
                totalDistance += route.legs[i].distance.value;
                totalDuraction += route.legs[i].duration.value;
            }
            $('#totalDirection_panel').html("");
            document.getElementById('totalDirection_panel').innerHTML += "Total distance: <span id='distance'>" + Math.round(totalDistance / 1000) + '</span> KM<br><br>'
            + "Total duration: " + Math.floor(totalDuraction / 60 / 60) % 60 + ' hours ' + Math.round(totalDuraction / 60) % 60 + ' mins<br><br>';
            
            if ($('#Items').length != 0)
                calculatePaymentsForService();

            if ($('#oilperKm').val() != "" && $('#oilPrice').val() != "") {
                var cost = $('#oilPrice').val() * $('#oilperKm').val() * Math.round(totalDistance / 1000) / 100;
                $('#totalCost').text("Estimated price: " + Math.ceil(cost) + " " + $('#currency').val());
                $('#travelCost').text("Travel Cost: " + (Math.ceil(cost) - parseInt($('#payments').text())) + " " + $('#currency').val());
            }
        }
    });


}