<%@ Page Language="C#" AutoEventWireup="true" CodeFile="store_detail.aspx.cs" Inherits="store_detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        table
        {
            width:50%;
            font-size:32px;
            
        }
        select
        {
            font-size:48px;
            width:100%;
            background: rgba(144, 144, 144, 0.075);
		    border-color: rgba(144, 144, 144, 0.25);
            -moz-appearance: none;
		    -webkit-appearance: none;
		    -ms-appearance: none;
		    appearance: none;
		    border-radius: 4px;
		    border: none;
		    border: solid 1px;
		    color: inherit;
		    display: block;
		    outline: 0;
		    padding: 0 1em;
		    text-decoration: none;
        }
        select:invalid,
		textarea:invalid {
			box-shadow: none;
		}
        select:focus,
		textarea:focus {
			border-color: #e74c3c;
			box-shadow: 0 0 0 1px #e74c3c;
		}
        table tr,td {
			padding: 0.3em 0.3em;
		}
        form
        {
            width:100%;
        }
        input[type="submit"],
	    input[type="reset"],
	    input[type="button"],
	    button,
	    .button {
		    -moz-appearance: none;
		    -webkit-appearance: none;
		    -moz-transition: background-color 0.2s ease-in-out, color 0.2s ease-in-out;
		    -webkit-transition: background-color 0.2s ease-in-out, color 0.2s ease-in-out;
		    transition: background-color 0.2s ease-in-out, color 0.2s ease-in-out;
		    border-radius: 4px;
		    border: 0;
		    
	    }
        body, input, select, textarea
        {
            font-family:'Microsoft JhengHei';
        }
        .btn-group
        {
            background-color:#FFE8BF; 
            width:100%; 
            height:150px; 
        }
        .btn-group .button 
        {
            background-color: #FF8800;
            border: none;
            color: white;
            text-align: center;
            font-size: 48px;
            font-weight:bold;
            height:150px;
            width:375px;
        }
        .btn-group .button:hover
        {
            background-color:#CC6600;
        }
         #map {
              height: 40%;
              width:90%;
            }
    </style>
</head>

<body style="background-color:#FFE8BF">
    
    
    <form id="form1" runat="server">
        <div class="btn-group" style="top:0%;">
            <div style="float:left; width:50%; " >
                <asp:Button ID="Button5" CssClass="button"  runat="server" OnClick="Button5_Click" Text="回上一頁" />
            </div>
            <%--<div style="float:left; width:50%; text-align:right;" >
                <asp:Button ID="Button1" CssClass="button"  runat="server" OnClick="logout_Click" Text="登出"  />
            </div>--%>
        </div>
        <div>
            <div style="float:left; width:45%;">
                <asp:Image ID="Image1" Width="400px" Height="400px" runat="server" />
            </div>
            <div>
                <table >
                    <tr>
                        <td>商家名稱:</td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>電話:</td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>評分:</td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>平均價格:</td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>類型:</td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Font-Underline="True"></asp:Label>
                            <asp:Label ID="Label6" runat="server" Font-Underline="True"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
    <div id="map" style=" height:40%;">
        <asp:Label hidden="true" ID="lat" runat="server"  Text=""></asp:Label>
        <asp:Label hidden="true" ID="lng" runat="server" Text=""></asp:Label>
        <asp:Label hidden="true" ID="dlat" runat="server"  Text=""></asp:Label>
        <asp:Label hidden="true" ID="dlng" runat="server" Text=""></asp:Label>
    </div>
    <script>
        function initMap() {
            var map = new google.maps.Map(document.getElementById('map'), {
                zoom: 15,
                center: { lat: 22.647769, lng: 120.309809 },
                drggable: true,
            }
            );
            //22.644800, 120.309378

            var lat = document.getElementById('lat')
            var dlat = document.getElementById('dlat')
            var lng = document.getElementById('lng')
            var dlng = document.getElementById('dlng')
            function direction() {
                var directionsService = new google.maps.DirectionsService();
                var directionsDisplay = new google.maps.DirectionsRenderer();
                directionsDisplay.setMap(map);
                {
                    var start = '\''+lat.innerText+','+lng.innerText+'\'';
                    var end = '\'' + dlat.innerText + ',' + dlng.innerText + '\'';					
                    //var start = '"22.639058", "120.302010"';
                    //var end = '"22.639058", "120.302010"';

                    //'22.639058, 120.302010';
                    var request = {
                        origin: start,
                        destination: end,
                        provideRouteAlternatives: true,
                        travelMode: 'DRIVING'
                    };
                    directionsService.route(request, function (result, status) {

                        directionsDisplay.setDirections(result);
                        directionsDisplay.setPanel(document.getElementById("directionsPanel"));

                    });
                }
            }
            var markers = locations.map(function (location, i) {

                return new google.maps.Marker({


                    position: location,
                    //icon: './icon.png',

                });
            });
            // Add a marker clusterer to manage the markers.
            var markerCluster = new MarkerClusterer(map, markers,
                { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });


        }
        </script>
    <script src="https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/markerclusterer.js">
    </script>
    <script async defer src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCKUB6EwUO5N_ryZqer8wXxECWwkHrQoYA&callback=initMap">
    </script>
</body>
</html>
