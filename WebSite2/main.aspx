<%@ Page Language="C#" AutoEventWireup="true" CodeFile="main.aspx.cs" Inherits="web1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style>
   
</style>
  
    <script>
       
        var lat;
        var lng;
       
       
        if (navigator.geolocation) {
            
            navigator.geolocation.getCurrentPosition(showPosition);
            
            
           } else {
            document.write("no support!");
            lat = "22.647383";
            lng = "120.309906";

        }

       

        function showPosition(position) {
           
            try {
               
                if (lat == null) {
                    lat = position.coords.latitude;
                    lng = position.coords.longitude;
                }


                if (lat != null) {
                    //document.write("<br> 取得位置中..... ");


                    //document.write(lat, lng);
                    var txt = "推薦系統";
                    var txt2 = "瀏覽附近美食";
                    var pic = '<img src="images/pic1.png" width="100%">';
                    var pic2 = '<img src="images/pic2.png" width="100%">';

                    //window.location.href =
                    
                    var url = "https://c110.hami.kmu.edu.tw/asp/recommender.aspx?lng=" + position.coords.longitude + "&lat=" + position.coords.latitude;
                    document.write("<p> 請選擇進入方式<br>" + pic.link(url) + "</p>");
                    var url2 = "https://c110.hami.kmu.edu.tw/asp/food_browser.aspx?lng=" + position.coords.longitude + "&lat=" + position.coords.latitude;
                    document.write("<p> " + pic2.link(url2) + "</p>");

                }
               
            }
            catch (aaa) {
                document.write("<br>"+aaa+"<br>");

            }

        }
       </script>
    
       <style>
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
           .btn-group
            {
                background-color:#FFE8BF; 
                width:100%; 
            }
            .btn-group .button 
            {
                background-color: #FF8800;
                border: none;
                color: white;
                text-align: center;
                font-size: 48px;
                font-weight:bold;
                height:0px;
                width:375px;
                visibility:hidden;
            }
            .btn-group .button:hover
            {
                background-color:#CC6600;
            }
       </style>
</head>
<body style="background-color:#FFE8BF;">
    <%--無法定位,將使用高醫作為所在位置<br />--%>
    <form id="form1" runat="server">
         <div class="btn-group" style="background-color:#FFE8BF; width:100%; top:0%;" >
            <div style="float:left; width:50%; " >
                <asp:Button ID="Button5" CssClass="button"  runat="server" OnClick="Button5_Click" Text="回主選單"  />
            </div>
      
            <div style="float:left; width:50%; text-align:right;" >
                <asp:Button ID="Button2" CssClass="button"  runat="server" OnClick="logout_Click" Text="登出"  />
            </div>
    </div>
    </form>
    <a href="https://c110.hami.kmu.edu.tw/asp/main.aspx" class="button big alt"><span style="font-size:30px;">
        <%--<img src="images/street food.jpg" alt="" width="950" height="1550" />--%></span></ a>
    <script>
                    var txt2 = "推薦系統";
                    var txt22 = "瀏覽附近美食";
                    var pic2 = '<img src="images/pic1.png" width="100%">';
                    var pic22 = '<img src="images/pic2.png" width="100%">';

                 
                    
                    var url2 = "https://c110.hami.kmu.edu.tw/asp/recommender.aspx?lng=" + "120.309627" + "&lat=" + "22.647423"; 
                    document.write("<p> 請選擇進入方式<br>" + pic2.link(url2) + "</p>");
                    var url22 = "https://c110.hami.kmu.edu.tw/asp/food_browser.aspx?lng=" + "120.309627" + "&lat=" + "22.647423";
                    document.write("<p> " + pic22.link(url22) + "</p>");
        </script>
</body>
</html>
