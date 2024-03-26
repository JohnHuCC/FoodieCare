<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<link rel="stylesheet" href="assets/css/main.css" />
    <title></title>
    <style type="text/css">
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
        input
        {
            font-size:48px;
            text-align: center; 
        }
        .auto-style1 {
            position: relative;
            top: 100px;
        }
        @media screen and (max-width: 980px) {

            .auto-style1 {
            position: relative;
            top: 250px;

        }
            }
        .auto-style6 {
            left: 0;
            bottom: 0em;
        }
        .btn-group
        {
            background-color:#FFE8BF; 
            width:100%; 
            height:10%; 
        }
        .btn-group .button 
        {
            background-color: #FF8800;
            border: none;
            color: white;
            text-align: center;
            font-size: 48px;
            font-weight:bold;
            height:30%;
            width:75%;
        }
        .btn-group .button:hover
        {
            background-color:#CC6600;
        }
        #header .inner .button {
				-moz-transition: background 1s ease-in-out;
				-webkit-transition: background 1s ease-in-out;
				-ms-transition: background 1s ease-in-out;
				transition: background 1s ease-in-out;
				margin: 0;
				border-radius: 50px;
			}

    </style>
</head>
<body style="width:100%;" >
			<header id="header">
				<div class="inner">
					<div class="content">
						<h1>Foodiecare</h1>
						<a href="#" class="button big alt"><span style="font-size:50px;"><img src="images/logo.png" alt="" width="390" /></span></a>		
					</div>
					<a href="#" class="button hidden"><span>Let's Eat</span></a>
				</div>
			</header>
					<div class="auto-style1"  style="width:100%;">
                    <form id="form1" runat="server" style="width:100%;">
                            <div  style="text-align:center;" >
                                <asp:Label ID="Label1"  Font-Bold="true" Font-Size="50px" runat="server" Text="帳號"></asp:Label>
                               <asp:TextBox ID="TextBox1" runat="server" Width="100%"  ></asp:TextBox>
                                 <asp:Label ID="Label2"  Font-Bold="true" Font-Size="50px" runat="server" Text="密碼"></asp:Label>
                                <asp:TextBox ID="TextBox2" runat="server" Width="100%" TextMode="Password"></asp:TextBox>
                                <asp:Label ID="Label3"  Visible="False" Font-Bold="True" Font-Size="50px" runat="server" ForeColor="Red"></asp:Label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1" ErrorMessage="請輸入帳號<br>" Display="Dynamic" ForeColor="Red" Font-Size="48px"></asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox2" ErrorMessage="請輸入密碼" Display="Dynamic" ForeColor="Red" Font-Size="48px"></asp:RequiredFieldValidator>
                            </div>
                           <br />
                            <div class="btn-group">
                                <div style="float:left; width:50%; " >
                                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="登入" CssClass="button"  />
                                </div>
                                <div style="float:left; width:50%; text-align:right;" >
                                    <asp:Button ID="Button2" runat="server" CausesValidation="False" OnClick="Button2_Click" Text="註冊" CssClass="button" />
                                </div>
                            </div>
                    </form>	
                   </div>
		<!-- Footer -->
			<footer id="footer" class="auto-style6">
				<a href="#" class="info fa fa-info-circle"><span>About</span></a>
				<div class="inner">
					<div class="content">
						<h3>Kaohsiung Medical University</h3>
						<p>XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX</p>
					</div>
					<div class="copyright">
						<h3>Follow me</h3>
						<ul class="icons">
							<li><a href="#" class="icon fa-twitter"><span class="label">Twitter</span></a></li>
							<li><a href="#" class="icon fa-facebook"><span class="label">Facebook</span></a></li>
							<li><a href="#" class="icon fa-instagram"><span class="label">Instagram</span></a></li>
							<li><a href="#" class="icon fa-dribbble"><span class="label">Dribbble</span></a></li>
						</ul>
						&copy; Untitled. Design: <a href="https://templated.co">TEMPLATED</a>. Images: <a href="https://unsplash.com/">Unsplash</a>.
					</div>
				</div>
			</footer>

		<!-- Scripts -->
			<script src="assets/js/jquery.min.js"></script>
			<script src="assets/js/skel.min.js"></script>
			<script src="assets/js/util.js"></script>
			<script src="assets/js/main.js"></script>

    
</body>
</html>
