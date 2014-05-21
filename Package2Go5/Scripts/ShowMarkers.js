//#Variables  
var showAll = false;

//Routes
var maxDistance = $('input[name=distance]').val(),
    itemFrom = null,
    itemTo = null,
    Routes = null;

//Items
var itemsArray = [];
var dirRenderer = [];
var Items = null;
var selected = 0;
var addedItems = [];

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

//Both
var geocoder,
    markersArray = new Array(),
    infoWindows = []
    
//Markers Routes
function GetRoutesMarkers() {
    latlngbounds = new google.maps.LatLngBounds();

    var offerId = "";
    if (typeof $('#action').val() != 'undefined')
    {
        offerId = "?o="+$('#action').val();
    }

    $.getJSON(
    "/Home/FindRoutes" + offerId, {},
    function (data) {
        Routes = $.parseJSON(data);
        $.getJSON("/Home/GetMarkers", {}, function (data) {
            var markers = data.split(',');

            $.each(Routes, function (index, route) {
                var prev = null;

                $.each(route.waypoints, function (ind, waypoint) {

                    if (prev == null) {
                        markRoutesPoints(markers[0], Routes[index].from, waypoint, index);
                    } else {
                        markRoutesPoints(markers[0], prev, waypoint, index);
                    }
                    prev = waypoint;
                });

            });
        });
    });
}

function FindNear() {

    markersArray.forEach(function (data) {
        data.setMap(null);
    });

    if ($('input[name=itemFrom]').val() != "") {
        $.getJSON('http://maps.googleapis.com/maps/api/geocode/json?address=' + $('input[name=itemFrom]').val() + '&sensor=false', null, function (data) {
            itemFrom = data.results[0].geometry.location;
        });
    }
    if ($('input[name=itemTo]').val() != "") {
        $.getJSON('http://maps.googleapis.com/maps/api/geocode/json?address=' + $('input[name=itemTo]').val() + '&sensor=false', null, function (data) {
            itemTo = data.results[0].geometry.location;
        });
    }
    if ($('input[name=itemFrom]').val() != "" || $('input[name=itemTo]').val() != "")
        GetRoutesMarkers();
    else
        alert("At least one field should be filled");
}

function markRoutesPoints(markerIcon, address, delivery, id) {

    $.getJSON('http://maps.googleapis.com/maps/api/geocode/json?address=' + address + '&sensor=false', null, function (data) {

        var p = data.results[0].geometry.location;

        var latlng = new google.maps.LatLng(p.lat, p.lng);
        
        if (showAll || itemFrom != null && isItClose(p.lat, p.lng, itemFrom.lat, itemFrom.lng)
                || itemTo != null && isItClose(p.lat, p.lng, itemTo.lat, itemTo.lng)) {
            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                icon: markerIcon
            });
            markersArray.push(marker);

            var offer = "";

            if (!showAll && $('a[title=logout]').length > 0)
                offer = "<input type='button' value='Offer' onclick='OfferRoute(" + id + ")'/> ";

            addInfoWindow(marker,
            "<div class='infoWindow'><h4>Route</h4><p>FROM: " + address + "</p><p>TO: " + delivery + "</p>"
            + "<p>Date From: " + Routes[id].date_from.split(" ")[0] + "</p>"
            + "<p>Date Till: " + Routes[id].date_till.split(" ")[0] + "</p>"
            + offer
            + "<a href='/Routes/Details/" + id + "'>More</a>"
            + "</div>", address, delivery, id, false);
        }

    });
}

function OfferRoute(id)
{
    $.get("/Offers/Create?i=" + $('.ItemsDropDown select').val() + "&r=" + id, null, function () {
        if (dirRenderer.length > 0) {
            dirRenderer.forEach(function (data) {
                data.setMap(null);
            });
            dirRenderer = [];
        }
        FindNear();
        $('.notification').text("The proposal was sent successfully");
    });
}

function rad(x) { return x * Math.PI / 180; }
function isItClose(aLat, aLng, searchLat,  searchLng) {
    var lat = searchLat;
    var lng = searchLng;
    var R = 6371; // radius of earth in km
    var distances = 0;
    var dLat = rad(aLat - lat);
    var dLong = rad(aLng - lng);
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        Math.cos(rad(lat)) * Math.cos(rad(lat)) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;

    return d < maxDistance || maxDistance == 0;
}

function showDestForRoutes(id) {

    var waypoints = [];
    $.each(Routes[id].waypoints, function (index, waypoint) {
        waypoints.push({
            location: waypoint,
            stopover: true
        });
    });

    var dest = "";

    if (waypoints.length > 1) {
        dest = waypoints[waypoints.length - 1].location;
    }
    else{
        dest = waypoints[0].location;
        waypoints = [];
    }

    var directionsService = new google.maps.DirectionsService();

    directionsService.route({
        origin: Routes[id].from,
        destination: dest,
        waypoints: waypoints,
        travelMode: google.maps.DirectionsTravelMode.DRIVING
    }, function (result) {
        directionsRenderer = new google.maps.DirectionsRenderer();
        directionsRenderer.setMap(map);
        directionsRenderer.setOptions({ preserveViewport: true });
        directionsRenderer.setDirections(result);

        dirRenderer[id] = directionsRenderer;
    });

}

//#FindRoutes functions ----------------------------------------------------------------------------

    $('input[name=distance]').change(function (input) {
        maxDistance = $('input[name=distance]').val();
    });

//#ShowItems functions -----------------------------------------------------------------------------
    function GetItemsMarkers() {
        var my = "";
        if (showAll)
            my = "?my=-1";
        $.getJSON(
        "/Home/FindItems" + my, {},
        function (data) {
            if (data.length > 1) {
                Items = $.parseJSON(data);
                WayPoints();

                $.getJSON("/Home/GetMarkers", {}, function (data) {
                    var markers = data.split(',');

                    $.each(Items, function (index, item) {
                        markItemsPoints(markers[1], index);
                    });
                });
            }
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
                addInfoWindow(marker, content.html(), Items[id].address, Items[id].delivery_address, id, true);
            } else {
                var add = "";
                var option = "<option class='AddItem itemOpt' value='" + id + "'>" + Items[id].title + "</option>";
                if ($.inArray(id, addedItems) > -1)
                    option = "<option class='RemoveItem itemOpt' value='" + id + "'>" + Items[id].title + "</option>";

                if (!showAll) {
                    if (!$.inArray(id, addedItems) > -1)
                        add = "<input name='add' type='button' value='Add' onclick='AddItem(" + id + ")'/>";
                    else {
                        add = "<input name='add' type='button' value='Remove' onclick='RemoveItem(" + id + ")'/>";
                    }
                }

                addInfoWindow(marker,
                        "<div class='infoWindow'>"
                            + "<h4>Items</h4>"
                            + "<p>FROM: <span class='address'>" + Items[id].address + "</span></p>"
                            + "<p>To: <span class='delivery_addr'>" + Items[id].delivery_address + "</span></p>"
                            + "<p>Delivery Date: <span class='delivery_date'>" + Items[id].date.split(" ")[0] + "</span></p>"
                            + "<p>Price: <span class='delivery_price'>" + Items[id].price + "</span></p>"
                            + "<select class='infoSelect'>" + option + "</select>"
                            + add
                            + " <a href='/Items/Details/" + id + "'>More</a>"
                        + "</div>",
                Items[id].address, Items[id].delivery_address, id, true);
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
                //polyline = selectedPolyline();
                polyline = itemsPolyline();
            }
            else
                polyline = itemsPolyline();

            var DirectionRenderer = new google.maps.DirectionsRenderer({
                polylineOptions: polyline
            });

            DirectionRenderer.setMap(map);
            DirectionRenderer.setOptions({ preserveViewport: true });
            DirectionRenderer.setDirections(result);

            dirRenderer[id] = DirectionRenderer;
        });

    }

    $(document).on('change', '.infoSelect', function (e) {

        //dirRenderer[selected].setMap();
        //showDestForItems(Items[selected].address, Items[selected].delivery_address, selected, 1);

        selected = $(this).find('option:selected').val();

        var onclick = "";

        if ($.inArray(selected, addedItems) == -1) {
            onclick = "AddItem";
            $(this).next().val("Add");
        }
        else {
            onclick = "RemoveItem";
            $(this).next().val("Remove");
        }

        $(this).next().attr("onclick", onclick + "(" + selected + ")");

        $(this).parent().find('.delivery_addr').text(Items[selected].delivery_address);
        $(this).parent().find('.delivery_date').text(Items[selected].date);
        $(this).parent().find('.delivery_price').text(Items[selected].price);

        //dirRenderer[selected].setMap();
        //showDestForItems(Items[selected].address, Items[selected].delivery_address, selected, 0);
    });

    $(document).on('click', 'input[name=add]', function (e) {

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

    var input;
    function AddItem(id) {
        addedItems.push(id);
        dirRenderer.forEach(function (data) {
            data.setMap(null);
        });
        dirRenderer = [];

        $('tbody tr').each(function () {
            var item = $(this).last().find('a');
            if (item.attr("title") == id) {
                AddItemToRoute(item);
            }
        });
    }

    function RemoveItem(id) {
        addedItems.splice(addedItems.indexOf(id) > -1, 1);
        $('tbody tr').each(function () {
            var item = $(this).last().find('a');
            if (item.attr("title") == id)
                RemoveItemFromRoute(item);
        });
    }

//#Both functions ----------------------------------------------------------------------------------
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

function addInfoWindow(marker, message, address, delivery, id, isItem) {

    var infoWindow = new google.maps.InfoWindow({
        content: message
    });

    infoWindows.push(infoWindow);

    google.maps.event.addListener(marker, 'click', function () {
        infoWindows.forEach(function (data) {
            data.setMap(null);
        });

        infoWindow.open(map, marker);

        if (dirRenderer.length > 0) {
            dirRenderer.forEach(function (data) {
                data.setMap(null);
            });
            dirRenderer = [];
        }
        if (isItem) {

            $(infoWindow.content).find('.itemOpt').each(function () {

                if ( this.selected && $.inArray($(this).val(), addedItems) > -1) {
                    $('input[name=add]').val("Remove");
                    $('input[name=add]').attr("onclick", $('input[name=add]').attr("onclick").replace("AddItem", "RemoveItem"));
                    $(this).removeClass("AddItem");
                }
            });

            var index = address.hashCode();
            var deliveryAddresses = itemsArray[index].split(';');

            for (var i = 0; i < deliveryAddresses.length - 1; i++) {
                var addr = deliveryAddresses[i].split(":");
                showDestForItems(address, addr[1], addr[0], i);
            }
        } else {
            showDestForRoutes(id);
        }
    });



    google.maps.event.addListener(map, 'click', function () {
        infoWindow.close(map, marker);

        dirRenderer.forEach(function (data) {
            data.setMap(null);
        });
        dirRenderer = [];

    });
}