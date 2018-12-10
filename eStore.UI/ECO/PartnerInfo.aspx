<%@ Page Title="Partner Information" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master"
    AutoEventWireup="true" CodeBehind="PartnerInfo.aspx.cs" Inherits="eStore.UI.ECO.PartnerInfo" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="eStoreContent" runat="server" ContentPlaceHolderID="eStoreMainContent">
    <div class="newPartnerContext">
        <div class="partnerinfo_message"><b>Locate the Channel Partners in your area by selecting the appropriate country:</b></div>
        <div class="select_partners">
            <table>
                <tr>
                    <td width="150px"><b>Select by Country:</b></td>
                    <td width="200px">
                        <select id="ddlCountries" class="form-control" runat="server">
                        </select>
                    </td>
                </tr>
            </table>
        </div>
        <div id="map"></div>
        <div class="paging"><ul></ul></div>
        <div id="partnerInfoArea" class ="partnerInfoArea"></div>
        <div class="paging"><ul></ul></div>
    </div>

    <script>
        var map;
        var markers = []; 
        var bounds;
        function initMap() {
            var startLatLng = { lat: 52.520008, lng: 13.404954 };

            // Create a map object and specify the DOM element for display.
            map = new google.maps.Map(document.getElementById('map'), {
                center: startLatLng,
                scrollwheel: true,
                zoom: 4
            });
        }

        $(document).ready(function () {
            //When loading page
            RenderPartnerInfoByCountriesAndPageNum("All",1);

            //When Select country
            $('#<%= ddlCountries.ClientID %>').change(function () {
                var selectedCountry = $.trim($('#<%= ddlCountries.ClientID %> option:selected').val());
                RenderPartnerInfoByCountriesAndPageNum(selectedCountry,1);
            });
        });



        function RenderPartnerInfoByCountriesAndPageNum(country, pageNumber) {
            var geocoder = new google.maps.Geocoder();
            $.ajax(
                {
                    type: "POST",
                    async: false,
                    url: "<%=System.IO.Path.GetFileName(Request.PhysicalPath) %>/RenderPartnerInfoByCountriesAndPageNum",
                    data: "{country:'" + country + "', pageNumber:'" + pageNumber + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var PartnerInfoArray = msg.d.PartnerInfoList; //return result is JS array

                        $(".paging ul").empty();
                        var PageNumberArray = [];
                        for (var i = 1; i <= msg.d.PageNumber; i++) {
                            if (i == pageNumber) {
                                PageNumberArray.push("<li style='background-color: #CED1D6'><a class='page-link'>" + i + "</a></li>");
                            }
                            else {
                                PageNumberArray.push("<li><a class='page-link'>" + i + "</a></li>");
                            }

                        }
                        $(".paging ul").append(PageNumberArray);
                        //When click page number
                        $(".page-link").click(function () {
                            var selectedCountry = $.trim($('#<%= ddlCountries.ClientID %> option:selected').val());
                            RenderPartnerInfoByCountriesAndPageNum(selectedCountry, $(this).text());
                        });


                        // loop over the array to get each object
                        $(partnerInfoArea).empty();
                        deleteMarkers();
                        bounds = new google.maps.LatLngBounds();
                        var counter = 1;
                        for (var i in PartnerInfoArray) {
                            var partnerInfo = PartnerInfoArray[i]
                            var companyName = partnerInfo.CompanyName
                            var fullAddress = partnerInfo.Address + ", " + partnerInfo.City + ", " + partnerInfo.Country;
                            var phone = partnerInfo.Phone;
                            var markerInfo = generateMarkerInformation(companyName, fullAddress, phone);
                            // Append that value to partnerInfoArea
                            var partnerInfoBox = generatePartnerInfoBox(partnerInfo.StoreID, companyName, phone, partnerInfo.Address, partnerInfo.City, partnerInfo.Country, partnerInfo.Email, partnerInfo.File_imgLogo, partnerInfo.File_Certificate, partnerInfo.File_imgSymbol, partnerInfo.CompanyIntroduction, partnerInfo.CompanyURL);
                            $(partnerInfoArea).append(partnerInfoBox)

                            //using lat,lng to create marker on goole map 
                            var location = {
                                lat: partnerInfo.Latitude,
                                lng: partnerInfo.Longitude
                            };
                            if (location.lat != null && location.lng != null) {
                                createMarkerOnGoogleMap(map, companyName, markerInfo, bounds, location);
                                //geocodeAddress(geocoder, map, fullAddress, companyName, markerInfo, bounds);
                            }
                            counter++;
                        }


                        //fit the map to the newly inclusive bounds
                        var listener = google.maps.event.addListener(map, "idle", function () {
                            map.fitBounds(bounds);
                            var zoom = map.getZoom();
                            map.setZoom(zoom > 17 ? 17 : zoom);
                            google.maps.event.removeListener(listener);
                        });


                        // create readmore link if the content is too long(height>70px)
                        $('.companyIntroduction').each(function () {
                            var content = $(this).html();
                            var introHeight = $(this).height();

                            if (introHeight > 70) {
                                $(this).addClass("companyIntroduction-less");                             
                                $(this).after("<div class='more'><a class='morelink'>Read more...</a></div>");
                            }

                        });

                        $(".morelink").click(function () {
                            if ($(this).parent().prev().hasClass("companyIntroduction-less")) {
                                $(this).parent().prev().removeClass("companyIntroduction-less");
                                $(this).html("Show less");
                            } else {
                                $(this).parent().prev().addClass("companyIntroduction-less");
                                $(this).html("Show more...");
                            }
                        });

                    },
                    error: function (message) { console.log(message); }
                })
        }

        function geocodeAddress(geocoder,map, fullAddress, companyName, markerInfo, bounds) {
            geocoder.geocode({ 'address': fullAddress }, function (results, status) {
                if (status === 'OK') {
                    createMarkerOnGoogleMap(map, companyName, markerInfo, bounds, results[0].geometry.location);
                } else {
                    //alert('Geocode was not successful for the following reason: ' + status);
                }
            });
        }

        function createMarkerOnGoogleMap(map,companyName, markerInfo, bounds, location)
        {
            map.setCenter(location);
            var infowindow = new google.maps.InfoWindow({
                content: markerInfo,
                maxWidth: 200
            });
            var marker = new google.maps.Marker({
                map: map,
                position: location,
                title: companyName
            });

            marker.addListener('click', function () {
                infowindow.open(map, marker);
            });
            google.maps.event.addListener(map, 'click', function () {
                infowindow.close(map, marker);
            });
            markers.push(marker);

            //extend the bounds to include each marker's position
            bounds.extend(marker.position);
        }
        
        function generatePartnerInfoBox(storeID, companyName, phone, address, city, country, email, fileImgLogo, fileCertificate, fileImgSymbol, companyIntroduction, CompanyURL)
        {
            var content;
            if (storeID != 'ABB') {
                content = '<div class="panel panel-default ">' +
                            '<div class="panel-heading"><b>' + companyName + '</b></div>' +
                            '<table class="table">' +
                                '<tr>' +
                                    '<td class="overview-logo"><img src="' + fileImgLogo + '" /></td>' +
                                        '<td class="overview-info">' +
                                            '<div><span class="info-item">Phone:</span><span>' + phone + '</span></div>' +
                                            '<div><span class="info-item">Address:</span><span>' + address + '</span></div>' +
                                            '<div><span class="info-item">City:</span><span>' + city + '</span></div>' +
                                            '<div><span class="info-item">Country:</span><span>' + country + '</span></div>' +
                                            '<div><span class="info-item partner-email">Email:</span><span>' + email + '</span></div>' +
                                            '<div><span class="info-item">Website:</span><span><a href ="' + CompanyURL + '" target="_blank" >' + CompanyURL + '</a></span></div>' +
                                        '</td>' +
                                    '<td class="overview-certif partner-certificate"><a href="' + fileCertificate + '" target="_blank" ><img src="http://advcloudfiles.advantech.com/web/Images/partner/Certificate.gif" />' +
                                    '</a ></td > ' +
                                    '<td class="overview-logoadv partner-fileImgSymbol"><img src="' + fileImgSymbol + '" /></td>' +
                                '</tr>' +
                            '</table>' +
                            '<div class="panel-body companyIntroduction">' + companyIntroduction  + '</div>' +
                        '</div >';
            }
            else
            {
                content = '<div class="panel panel-default ">' +
                            '<div class="panel-heading"><b>' + companyName + '</b></div>' +
                            '<table class="table">' +
                                '<tr>' +
                                    '<td class="overview-logo"><img src="' + fileImgLogo + '" /></td>' +
                                        '<td class="overview-info">' +
                                            '<div><span class="info-item">Phone:</span><span>' + phone + '</span></div>' +
                                            '<div><span class="info-item">Address:</span><span>' + address + '</span></div>' +
                                            '<div><span class="info-item">City:</span><span>' + city + '</span></div>' +
                                            '<div><span class="info-item">Country:</span><span>' + country + '</span></div>' +
                                            '<div><span class="info-item">Website:</span><span><a href ="' + CompanyURL + '" target="_blank" >' + CompanyURL + '</a></span></div>' +
                                        '</td>' +
                                '</tr>' +
                            '</table>' +
                            '<div class="panel-body companyIntroduction">' + companyIntroduction + '</div>' +
                        '</div >';
            }
                        
            return content;
        }

        

        // Sets the map on all markers in the array.
        function setMapOnAll(map) {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(map);
            }
        }

        // Removes the markers from the map, but keeps them in the array.
        function clearMarkers() {
            setMapOnAll(null);
        }


        // Shows any markers currently in the array.
        function showMarkers() {
            setMapOnAll(map);
        }

        // Deletes all markers in the array by removing references to them.
        function deleteMarkers() {
            clearMarkers();
            markers = [];
        }

        function generateMarkerInformation(companyName,fullAddress,phone) {
            var content = '<div id="content">' +
                //'<div id="siteNotice">' +
                //'</div>' +
                '<h3 id="firstHeading" class="firstHeading"><b>' + companyName +'</b></h3>' +
                '<div id="bodyContent">' +
                    '<div>Tel:' + phone + '</div>' +
                    '<div>Address:' + fullAddress + '</div>' +
                '</div>' +
                '</div>';
            return content;
        }


    </script>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDKq4X_ZjRHYr38AWR__zQwCo5aQnjA7VM&callback=initMap">
    </script>

</asp:Content>






