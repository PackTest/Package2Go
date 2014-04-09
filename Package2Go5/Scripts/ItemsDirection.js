new google.maps.places.Autocomplete(document.getElementById('delivery_address'));
new google.maps.places.Autocomplete(document.getElementById('address'));

var directionsDisplay;
var directionsService = new google.maps.DirectionsService();
var map;
var geocoder = new google.maps.Geocoder();

Initialize();

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

    directionsDisplay.setMap(map);

    $('#address').focusout(function () {
        $(this).attr("value", $(this).val());
        calcRoute();
    });

    $('#delivery_address').focusout(function () {
        $(this).attr("value", $(this).val());
        calcRoute();
    });

    if ($('#address').val() != "" && $('#delivery_address').val() != "")
        calcRoute();
}

function calcRoute() {
    var start = $('#address').val();
    var end = $('#delivery_address').val();

    var request = {
        origin: start,
        destination: end,
        travelMode: google.maps.TravelMode.DRIVING
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
            var route = response.routes[0];

            $('#address').val(route.legs[0].start_address);
            $('#delivery_address').val(route.legs[0].end_address);
        }
    });
}