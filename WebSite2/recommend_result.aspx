<%@ Page Language="C#" AutoEventWireup="true" CodeFile="recommend_result.aspx.cs" Inherits="Jcrop" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<!--meta name="viewport" content="width=device-width,initial-scale=1, maximum-scale=1, user-scalable=no" /-->
    <title></title>
    <script src="js/jquery-3.3.1.min.js"></script>
    <script src="js/jquery-ui.min.js"></script>
    <script src="js/jquery.ui.touch-punch.js"></script>
    <script src="js/jquery.ui.touch-punch.min.js"></script>
    <style>
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
       
         #draggable { 
             width: 700px;
             height: 600px; 
             padding: 0.5em; 
             text-align:center; 
             background-color:#FFE8BF;
            
         }
        .auto-style3 
        {
            text-align:center; 
            width: 50%;
            height: 60%;
            background-color:red;
            top:15%;
            left:500px;
            position:absolute;
            opacity: 0.4;
        }
        .auto-style2 {
            text-align:center; 
            width: 50%;
            height: 60%;
            background-color:lawngreen;
            top:15%;
            left:0px;
            position:absolute;
            opacity: 0.4;
        }
        .btn-group .button 
        {
            background-color: #FF8800;
            border: none;
            color: white;
            text-align: center;
            font-size: 48px;
            font-weight:bold;
        }
        .btn-group .button:hover
        {
            background-color:#CC6600;
        }
    </style>
   
</head>
    <body style="background-color:#FFE8BF">
     <script>
         var touchValue = { x: 5, y: 5, sx: 0, sy: 0, ex: 0, ey: 0 }; //initialize the touch values
        
         window.addEventListener("touchstart", function () {
             var event = event || window.event;
             touchValue.sx = event.targetTouches[0].pageX;
             touchValue.sy = event.targetTouches[0].pageY;
             touchValue.ex = touchValue.sx;
             touchValue.ey = touchValue.sy;
         });
         window.addEventListener("touchmove", function (event) {
             var event = event || window.event;
             event.preventDefault();
             touchValue.ex = event.targetTouches[0].pageX;
             touchValue.ey = event.targetTouches[0].pageY;
         });
         window.addEventListener("touchend", function (event) {
             var event = event || window.event;
             var changeX = touchValue.ex - touchValue.sx;
             var changeY = touchValue.ey - touchValue.sy;
             //console.log("X:"+changeX+" Y:"+changeY);
             window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
         });
         function getIsTouch() {
             var changeX = touchValue.ex - touchValue.sx;
             var changeY = touchValue.ey - touchValue.sy;
             if (Math.abs(changeX) <= touchValue.x && Math.abs(changeY) <= touchValue.y) {
                 return true
             } else return false
         }
         function picMove01() {
             var img = document.getElementById('draggable');
             var x = img.offsetLeft;
             var movetime = 0;
             var id = setInterval(move(), 100);
             function move()
             {
                 if (movetime >= 100) {
                     clearInterval(id);
                 }
                 else {
                     x = x + 0.05;
                     img.style.left = x + "px";
                     img.style.opacity = img.style.opacity - 0.01;
                     movetime = movetime + 1;
                 }
             }   
         }
         function picMove02() {
             var img = document.getElementById('draggable');
             var x = img.offsetLeft;
             var movetime = 0;
             var id = setInterval(move(), 100);
             function move() {
                 if (movetime >= 100) {
                     clearInterval(id);
                 }
                 else {
                     x = x - 0.05;
                     img.style.left = x + "px";
                     img.style.opacity = img.style.opacity - 0.01;
                     movetime = movetime + 1;
                 }
             }
         }
         $(function () {
             var img = document.getElementById('draggable');
             var start = img.offsetLeft;
             var next;
             var src = document.getElementById('<%= src.ClientID %>');
             
             img_obj = document.createElement("img");
            
             img.appendChild(img_obj);
            img_obj.className = 'img_obj';
            img_obj.id = 'img_obj';
            img_obj.src = src.innerText;
            img_obj.style = 'width:500px;height:500px;';
            var txt = document.getElementById('<%= txt.ClientID %>');
            
            var bg1 = document.getElementById('bg1');
            var bg2 = document.getElementById('bg2');
            result.innerText = txt.innerText;
            $("#draggable").draggable(
                //{ revert: true },
                { axis: "x" },
                { scroll: false },
                {
                    start: function () {
                        bg1.style.opacity = 0.5;
                        bg2.style.opacity = 0.5;
                    },
                    drag: function () {
                        next = img.offsetLeft;
                        if (bg1.style.opacity >= 0.5) {
                            bg1.style.opacity = 0.5 + (start - next) / 200;
                        }
                        if (bg2.style.opacity >= 0.5) {
                            bg2.style.opacity = 0.5 + (next - start) / 200;
                        }
                        if (bg1.style.opacity < 0.5) {
                            bg1.style.opacity = 0.5;
                        }
                        if (bg2.style.opacity < 0.5) {
                            bg2.style.opacity = 0.5;
                        }
                    }
                    ,
                    stop: function () {
                        var btn = document.getElementById('<%= Button_h.ClientID %>');
                        var btn2 = document.getElementById('<%= Button1.ClientID %>');
                        if (bg2.style.opacity >= 1) {
                            picMove01();
                            btn.click();
                        }
                        else if (bg1.style.opacity >= 1) {
                            picMove02();
                            btn2.click();
                        }
                        bg1.style.opacity = 0.4;
                        bg2.style.opacity = 0.4;
                    }
                }
            );
         });
  </script>
        <form id="form1" runat="server" >
    
    <div class="btn-group" style="background-color:#FFE8BF; width:100%; height:150px; top:0%;" >
            <div style="float:left; width:50%; " >
                <asp:Button ID="Button5" CssClass="button"  runat="server" OnClick="Button5_Click" Text="回主選單" Height="150px" Width="375px" />
                <asp:Button ID="Button_detail" CssClass="button"  runat="server" Visible="false" OnClick="Button_detail_Click" Text="回上一頁" Height="150px" Width="375px" />
            </div>
      
            <div style="float:left; width:50%; text-align:right;" >
                <asp:Button ID="Button2" CssClass="button"  runat="server" OnClick="logout_Click" Text="登出" Height="150px" Width="375px"  />
            </div>
    </div>
    <div id="jcrop" runat="server">
            <div  style="background-color:#FFE8BF;  height:300px;" >
            </div>
    <asp:Label hidden="true" ID="txt" runat="server"  Text="很抱歉，無符合店家..."></asp:Label>
    <asp:Label hidden="true" ID="src" runat="server" Text="Label"></asp:Label>
    <div id ="bg1" class="auto-style2"  top:20%;>
        <asp:Label ID="Label2" runat="server"  style="font-size:128px; color:white;" Text="喜歡"></asp:Label>
    </div>
    <div id ="bg2" class="auto-style3" top:20%;>
        <asp:Label ID="Label3" runat="server"  style="font-size:128px; color:white;" Text="不喜歡"></asp:Label>
    </div>
   
    <center>
        <div id="draggable"  class="draggable"  top:20%;>
    
        <asp:LinkButton ID="result" runat="server" OnClick="btn_result_Click" style="font-size:44px" Text="123"></asp:LinkButton>
        <asp:Button ID="Button_h" runat="server" Text="Button" OnClick="Button_h_Click" style="display:none" ForeColor="#FF6600"/>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" style="display:none"/>     
        </div>
        <div style="height:100px" ></div>
         <div style="font-size:64px;text-align:center;" >
            <asp:Label ID="Label1" ForeColor="#666699" runat="server" Text="<---請左右滑動--->"></asp:Label>
        </div>
    </center>
    </div>
    <div id="detail" style="display:none;" runat="server" >
        <div style="height:100px" ></div>
            <div style="float:left; width:45%;">
                <asp:Image ID="Image1" Width="400px" Height="400px" runat="server" />
            </div>
            <div>
                <table style="font-size:32px;width:45%;" >
                    <tr>
                        <td>商家名稱:</td>
                        <td>
                            <asp:Label ID="detail_name" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>電話:</td>
                        <td>
                            <asp:Label ID="detail_phone" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>評分:</td>
                        <td>
                            <asp:Label ID="detail_point" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>平均價格:</td>
                        <td>
                            <asp:Label ID="detail_price" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>類型:</td>
                        <td>
                            <asp:Label ID="detail_type" runat="server" Font-Underline="True"></asp:Label>
                            <asp:Label ID="detail_type2" runat="server" Font-Underline="True"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <%--<div style="float:left;" >
                <asp:ImageButton ID="ImageButton_item_1" runat="server" />
                <asp:ImageButton ID="ImageButton_item_2" runat="server" />
                <asp:ImageButton ID="ImageButton_item_3" runat="server" />
                <asp:ImageButton ID="ImageButton_item_4" runat="server" />
                <asp:ImageButton ID="ImageButton_item_5" runat="server" />
            </div>--%>
        </div>
        </form>
</body>
</html>
