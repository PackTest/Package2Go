//#Variables  

//Routes
var maxDistance = $('input[name=distance]').val(),
    itemFrom = null;

//Items
var itemsArray = [];
//var dirRenderer = new Array();
var dirRenderer = [];
var Items = null;
var selected = 0;
var addedItems = [];
//var infoWindow = null;

function itemsPolyline() {
    return new google.maps.Polyline({
        strokeColor: '#FF0000',
        strokeOpacity: 0.5,
        strokeWeight: 6
    });
}

function selectedPolyline() {
    return new google.maps.Polyline({
        strokeColor: '#GG0000',
        strokeOpacity: 0.5,
        strokeWeight: 6
    });
}

$(document).ready(function () {
    GetItemsMarkers();
});

$(document).on('change', 'select', function (e) {

    //dirRenderer[selected].setMap();
    //showDestForItems(Items[selected].address, Items[selected].delivery_address, selected, 1);

    selected = $(this).find('option:selected').val();

    var onclick = "";

    if ($.inArray(parseInt(selected), addedItems) != 0) {
        onclick = "AddItem";
        $(this).next().val("Add");
    }
    else {
        onclick = "RemoveItem";
        $(this).next().val("Remove");
    }

    $(this).next().attr("onclick", onclick + "(" + selected + ")");


    $(this).prev().find('.delivery_addr').text(Items[selected].delivery_address);

    //dirRenderer[selected].setMap();
    //showDestForItems(Items[selected].address, Items[selected].delivery_address, selected, 0);
});

$(document).on('click', 'input[type=button]', function (e) {

    var id = $(this).attr("onclick").substring($(this).attr("onclick").indexOf("(") + 1, $(this).attr("onclick").length - 1);

    if ($(this).val() == "Add") {
        $(this).prev().find('option:selected').removeClass("AddItem");
        $(this).val("Remove");
        $(this).attr("onclick", "RemoveItem(" + id + ")");
    } else {
        $(this).prev().find('option:selected').addClass("AddItem");
        $(this).val("Add");
        $(this).attr("onclick", "AddItem(" + id + ")");
    }
});

//Both
var geocoder,
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions),
    markersArray = new Array(),
    Lithuania = new google.maps.LatLng(55.00, 23.50),
    infoWindows = [],
    mapOptions = {
        zoom: 8,
        center: Lithuania,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };
    directionsService = new google.maps.DirectionsService();

//#FindRoutes functions ----------------------------------------------------------------------------

    $('input[name=distance]').change(function (input) {
        maxDistance = $('input[name=distance]').val();
    });

    function FindNear() {
        $.getJSON('http://maps.googleapis.com/maps/api/geocode/json?address=' + $('input[name=itemFrom]').val() + '&sensor=false', null, function (data) {

            markersArray.forEach(function (data) {
                data.setMap(null);
            });

            itemFrom = data.results[0].geometry.location;
            GetRoutesMarkers();
            map.setCenter(itemFrom);
        });
    }

    //Markers Routes
    function GetRoutesMarkers() {

        latlngbounds = new google.maps.LatLngBounds();
        $.getJSON(
        "/Home/FindRoutes", {},
        function (data) {
            var addresses = data.split('/');
            var point = "";
            var id = 0;
            var address = "";
            $.getJSON("/Home/GetMarkers", {}, function (data) {
                var markers = data.split(',');
                for (var x = 0; x < addresses.length; x++) {
                    if (addresses[x] != "") {
                        point = addresses[x].split('>');
                        id = point[0];
                        address = point[1].split(';');
                    }
                    for (var i = 0; i < address.length; i++) {
                        markPoints(markers[0], address[i].split(':')[1], address, id);
                    }
                }
            });
        });
    }

    function markPoints(markerIcon, address, delivery, id) {

        $.getJSON('http://maps.googleapis.com/maps/api/geocode/json?address=' + address + '&sensor=false', null, function (data) {
            var p = data.results[0].geometry.location;

            var latlng = new google.maps.LatLng(p.lat, p.lng);

            if (itemFrom != null && isItClose(p.lat, p.lng)) {
                var marker = new google.maps.Marker({
                    position: latlng,
                    map: map,
                    icon: markerIcon
                });

                markersArray.push(marker);

                addInfoWindow(marker,
                "<div class='infoWindow'><h4>Item</h4><p>FROM: " + address + "</p><p>TO: " + delivery + "</p><input type='button' value='Add' onclick='AddItem(" + id + ")' /> "
                + "<a href='Items/Details/" + id + "'>More</a></div>",
                address, delivery, id);
            }

        });
    }

    function showDestForRoutes(address, id) {

        var waypoints = [];

        if (address.length > 1) {
            for (var i = 1; i < address.length - 1; i++) {
                waypoints.push({
                    location: address[i].split(':')[1],
                    stopover: true
                });
            }
        }

        directionsService.route({
            origin: address[0].split(':')[1],
            destination: address[address.length - 1].split(':')[1],
            waypoints: waypoints,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        }, function (result) {
            directionsRenderer = new google.maps.DirectionsRenderer();
            directionsRenderer.setMap(map);
            directionsRenderer.setOptions({ preserveViewport: true });
            directionsRenderer.setDirections(result);
        });
    }

    function rad(x) { return x * Math.PI / 180; }
    function isItClose(aLat, aLng) {
        var lat = itemFrom.lat;
        var lng = itemFrom.lng;
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
//#ShowItems functions -----------------------------------------------------------------------------

    function GetItemsMarkers() {

        $.getJSON(
        "/Home/FindItems", {},
        function (data) {
            Items = $.parseJSON(data);

            $('input[name=Items]').each(function () {
                console.log($(this));
            });

            $.getJSON("/Home/GetMarkers", {}, function (data) {
                var markers = data.split(',');

                $.each(Items, function (index, item) {
                    markItemsPoints(markers[1], index);
                });
            });
        });

    }

    function markItemsPoints(markerIcon, id) {
        $.getJSON('http://maps.googleapis.com/maps/api/geocode/json?address=' + Items[id].address + '&sensor=false', null, function (data) {
            var p = data.results[0].geometry.location;
            var latlng = new google.maps.LatLng(p.lat, p.lng);

            var i = Items[id].address.hashCode();
            if (typeof itemsArray[i] === "undefined")
            {
                itemsArray[i] = "";
            }

            itemsArray[i] += id+":"+Items[id].delivery_address + ";";

            var isInfoWindow = false;

            var content = null;

            infoWindows.forEach(function (data) {
                if ($(data.content).find('.address').html() == Items[id].address) {

                    content = $(data.content);
                    content.find('select').append("<option class='AddItem itemOpt' value='" + id + "'>" + Items[id].title + "</option>");
                    
                    data.setMap(null);
                }
            });

            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                icon: markerIcon
            });

            markersArray.push(marker);

            var className = "";

            if (content != null) {
                addInfoWindow(marker, content.html(), Items[id].address, Items[id].delivery_address, id);
            } else {
                addInfoWindow(marker,
                        "<div class='infoWindow'>"
                            + "<h4>Items</h4>"
                            + "<p>FROM: <span class='address'>" + Items[id].address + "</span></p>"
                            + "<p>To: <span class='delivery_addr'>" + Items[id].delivery_address + "</span></p>"
                            + "<select><option class='AddItem itemOpt' value='" + id + "'>" + Items[id].title + "</option></select>"
                            + "<input name='add' type='button' value='Add' onclick='AddItem(" + id + ")'/>"
                        + "</div>",
                Items[id].address, Items[id].delivery_address, id);
            }

                
        });
    }


    function showDestForItems(address, delivery, id, i) {

        directionsService.route({
            origin: address,
            destination: delivery,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        }, function (result) {

            var polyline;
            if (i == 0) {
                selected = id;
                polyline = selectedPolyline()
            }
            else
                polyline = itemsPolyline()

            var DirectionRenderer = new google.maps.DirectionsRenderer({
                polylineOptions: polyline
            });

            DirectionRenderer.setMap(map);
            DirectionRenderer.setOptions({ preserveViewport: true });
            DirectionRenderer.setDirections(result);

            dirRenderer[id] = DirectionRenderer;
        });

    }


//#Both functions ----------------------------------------------------------------------------------
    function addInfoWindow(marker, message, address, delivery, id) {

        var infoWindow = new google.maps.InfoWindow({
            content: message
        });

        infoWindows.push(infoWindow);

        google.maps.event.addListener(marker, 'click', function () {

            infoWindows.forEach(function (data) {
                data.setMap(null);
            });

            infoWindow.open(map, marker);

            console.log(addedItems);

            $(infoWindow.content).find('.itemOpt').each(function () {
                if ($(this).hasClass("AddItem") && this.selected && $.inArray(parseInt($(this).val()), addedItems) == 0) {
                    $('input[name=add]').val("Remove");
                    $('input[name=add]').attr("onclick", $('input[name=add]').attr("onclick").replace("AddItem", "RemoveItem"));
                    $(this).removeClass("AddItem");
                }
            });

            if (dirRenderer.length > 0) {
                dirRenderer.forEach(function (data) {
                    data.setMap(null);
                });
                dirRenderer = [];
            }
            //if (delivery.length > 1)
            //    showDestForRoutes(delivery, id);
            //else {

                var index = address.hashCode();
                var deliveryAddresses = itemsArray[index].split(';');

                for (var i = 0; i < deliveryAddresses.length - 1; i++) {
                    var addr = deliveryAddresses[i].split(":");
                    showDestForItems(address, addr[1], addr[0], i);
                }

            //}
        });



        google.maps.event.addListener(map, 'click', function () {
            infoWindow.close(map, marker);

            dirRenderer.forEach(function (data) {
                data.setMap(null);
            });
            dirRenderer = [];

        });
    }

    String.prototype.hashCode = function () {
        var hash = 0, i, chr, len;
        if (this.length == 0) return hash;
        for (i = 0, len = this.length; i < len; i++) {
            chr = this.charCodeAt(i);
            hash = ((hash << 5) - hash) + chr;
            hash |= 0;
        }
        return hash;
    };