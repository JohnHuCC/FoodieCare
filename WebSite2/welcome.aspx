<%@ Page Language="C#" AutoEventWireup="true" CodeFile="welcome.aspx.cs" Inherits="welcome" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
           <center> FOODIECARE<br />
               <br />
               <br />
               <br />
               <br />
               <br />
               <br />
               <asp:Login ID="Login1" runat="server" BackColor="#F7F6F3" BorderColor="#E6E2D8" BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" Height="129px" Width="207px">
                   <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
                   <LoginButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" />
                   <TextBoxStyle Font-Size="0.8em" />
                   <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="0.9em" ForeColor="White" />
               </asp:Login>
               <br />
               <asp:LoginView ID="LoginView1" runat="server">
               </asp:LoginView>
               <br />
               <asp:LoginName ID="LoginName1" runat="server" />
               <br />
               <br />
               <br />
               <asp:LoginStatus ID="LoginStatus1" runat="server" />
               <asp:PasswordRecovery ID="PasswordRecovery1" runat="server">
               </asp:PasswordRecovery>
               <br />
               <br />
               <br />
               <br />
               <br />
               <br />
             <asp:ImageButton ID="ImageButton1" runat="server" OnClick="click" /></center>
           
        </div>
    </form>
</body>
</html>
