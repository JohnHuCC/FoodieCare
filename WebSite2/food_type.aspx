<%@ Page Language="C#" AutoEventWireup="true" CodeFile="food_type.aspx.cs" Inherits="food_type" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <center> <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="index_change">
            </asp:DropDownList>  
              <br />
                <br />


            <asp:DropDownList ID="DropDownList2" runat="server">
            </asp:DropDownList></center>
           
        </div>
    </form>
</body>
</html>
