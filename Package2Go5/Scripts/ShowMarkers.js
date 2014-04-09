    var maxDistance = 0;
    var geocoder, map, markersArray;
    var latlngbounds;
    markersArray = new Array();

    var Lithuania = new google.maps.LatLng(55.00, 23.50);
    var mapOptions = {
        zoom: 8,
        center: new google.maps.LatLng(0, 0),
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };

    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

    //GetMarkers();

    //Markers Items
    function GetMarkers2() {

        latlngbounds = new google.maps.LatLngBounds();
        $.getJSON(
        "/Home/FindItemsRoutes", {},
        function (data) {
            var addresses = data.split(';');
            var point = "";
            var id = 0;
            var address = "";

            $.getJSON("/Home/GetMarkers", {}, function (data) {
                var markers = data.split(',');
                for (var x = 0; x < addresses.length; x++) {
                    if (addresses[x] != "") {
                        point = addresses[x].split(':');
                        id = point[0];
                        address = point[1].split('|');
                    }
                }
                markPoints2(markers[1], address, id);
            });
        });
    }

    //Markers
    function GetMarkers(show) {

        latlngbounds = new google.maps.LatLngBounds();
        $.getJSON(
        "/Home/FindTripsPoints?show=" + show, {},
        function (points) {
            var addresses = points.split(',');
            var items = false;
            var id = 0;

            $.getJSON("/Home/GetMarkers", {}, function (data) {
                var markers = data.split(',');
                for (var x = 0; x < addresses.length; x++) {
                    var point = addresses[x].split(':');
                    var address = point[0];
                    if (point.length > 1) {
                        id = point[0];
                        address = point[1];
                    }
                    if (address == "items") {
                        items = true;
                        continue;
                    }

                    if (!items) {
                        markPoints(markers[0], address, items, id);
                    } else {
                        markPoints(markers[1], address, items, id);
                    }
                }

                //map.setCenter(latlngbounds.getCenter());
                //map.fitBounds(latlngbounds);
            });

        });
    }

    function markPoints2(markerIcon, addresses, id) {
        var address = addresses[0];
        var delivery = addresses[1];

        markPoints(markerIcon, address, delivery, id);

        //showDest(markerIcon, address, delivery, id);
    }

    var directionsRenderer, marker;

    function showDest(address, delivery, id) {
        var polylineOptionsActual = new google.maps.Polyline({
            strokeColor: '#FF0000',
            strokeOpacity: 0.5,
            strokeWeight: 6
        });
        console.log(address + " | " + delivery);
        directionsService.route({
            origin: address,
            destination: delivery,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        }, function (result) {
            directionsRenderer = new google.maps.DirectionsRenderer({ polylineOptions: polylineOptionsActual });
            directionsRenderer.setMap(map);
            directionsRenderer.setDirections(result);
        });
    }

    var ShowHide = "";

    //function markPoints(markerIcon, addresses, item, id) {
    function markPoints(markerIcon, address, delivery, id) {
        $.getJSON('http://maps.googleapis.com/maps/api/geocode/json?address=' + address + '&sensor=false', null, function (data) {
            var p = data.results[0].geometry.location;

            var latlng = new google.maps.LatLng(p.lat, p.lng);
            latlngbounds.extend(latlng);
            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                icon: markerIcon
            });

            addInfoWindow(marker,
                "<div class='infoWindow'><h4>Item</h4><p>FROM: " + address + "</p><p>TO: " + delivery + "</p><input type='button' value='Add' onclick='AddItem(" + id + ")' /> "
                //+ "<input type='button' value='Remove' onclick='RemoveItem(" + id + ")' /> "
                + "<a href='Items/Details/" + id + "'>More</a></div>",
                address, delivery, id);
        });
    }

    function addInfoWindow(marker, message, address, delivery, id) {
        var info = message;

        var infoWindow = new google.maps.InfoWindow({
            content: message
        });
        google.maps.event.addListener(marker, 'click', function () {
            infoWindow.open(map, marker);
            showDest(address, delivery, id);
        });

        google.maps.event.addListener(map, 'click', function () {
            infoWindow.close(map, marker);
            directionsRenderer.setMap(null);
        });
    }

    //google.maps.event.addListener(map, 'click', find_closest_marker);

    //function rad(x) { return x * Math.PI / 180; }
    ////function find_closest_marker(event) {
    //function find_closest_marker() {
    //    //console.log(event.latLng.lat() + " " + event.latLng.lng());
    //    //var lat = event.latLng.lat();
    //    //var lng = event.latLng.lng();
    //    var lat = 54.95;
    //    var lng = 23.89;

    //    var R = 6371; // radius of earth in km
    //    var distances = [];
    //    var closest = -1;
    //    for (i = 0; i < map.markers.length; i++) {
    //        var mlat = map.markers[i].position.lat();
    //        var mlng = map.markers[i].position.lng();
    //        var dLat = rad(mlat - lat);
    //        var dLong = rad(mlng - lng);
    //        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    //            Math.cos(rad(lat)) * Math.cos(rad(lat)) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
    //        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    //        var d = R * c;
    //        distances[i] = d;
    //        if (closest == -1 || d < distances[closest]) {
    //            closest = i;
    //        }
    //    }

    //    alert(map.markers[closest].title);
    //}

    function rad(x) { return x * Math.PI / 180; }
    function isItClose(aLat, aLng) {
        if (maxDistance == 0)
            maxDistance = 6000;
        var lat = 54.95;
        var lng = 23.89;
        var R = 6371; // radius of earth in km
        var distances = 0;
        var dLat = rad(aLat - lat);
        var dLong = rad(aLng - lng);
        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(rad(lat)) * Math.cos(rad(lat)) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        var d = R * c;

        return d < maxDistance;
    }