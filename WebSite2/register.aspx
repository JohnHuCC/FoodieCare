<%@ Page Language="C#" AutoEventWireup="true" CodeFile="register.aspx.cs" Inherits="register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link rel="stylesheet" href="assets/css/checkbox.css" />
    <title></title>
    <style type="text/css">
       table
        {
            width:100%;
            height:80%;
            font-size:36px;
            color:black;
            border: none;
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
        body, input
        {
            font-family:'Microsoft JhengHei';
            font-size:36px;
        }
        input
        {
            width:80%;
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
            height:150px;
            width:375px;
        }
        .btn-group .button:hover
        {
            background-color:#CC6600;
        }
        .valid
        {
            font-size:36px;
        }
        .input
        {
            width:300px;
        }
        .th
        {
            font-size:48px;
        }
        </style>
</head>
<body style="background-color:#FFE8BF;">
    <form id="form1" runat="server">
    <table >
            <tr>
            <td  class="th">
            <asp:Label ID="Label1" runat="server" Text="帳號:"></asp:Label>
            </td>
             <td class="input">
                 <asp:TextBox ID="TextBox1" runat="server" ></asp:TextBox>
                <%-- OnTextChanged="TextBox1_TextChanged"--%>

             </td>
                        <td class="valid" >
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1" ErrorMessage="請輸入帳號"  Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                        </td>
             </tr>
       <tr>
            <td class="th">
            <asp:Label ID="Label2" runat="server" Text="密碼:"></asp:Label>

            </td>
             <td class="input">
                 <asp:TextBox ID="TextBox2" runat="server" TextMode="Password" ></asp:TextBox>
            </td>
             <td  class="valid">

                 <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox2" ErrorMessage="請輸入密碼" ForeColor="Red"></asp:RequiredFieldValidator>

             </td>
             </tr>
                
            <tr>
            <td class="th">
            <asp:Label ID="Label3" runat="server" Text="確認密碼:"></asp:Label>

            </td>
             <td class="input">
                 <asp:TextBox ID="TextBox3" runat="server" TextMode="Password"></asp:TextBox>

             </td>
               <td  class="valid">

                 <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="TextBox2" ControlToValidate="TextBox3" Display="Dynamic" ErrorMessage="密碼輸入不同" ForeColor="Red"></asp:CompareValidator>

                </td>
             </tr>
            <tr>
            <td class="th">
            <asp:Label ID="Label4" runat="server" Text="性別:"></asp:Label>

            </td>
                
             <td class="input" >
                 <asp:RadioButtonList ID="RadioButtonList1" runat="server" >
                     <asp:ListItem Value="0">女生</asp:ListItem>
                     <asp:ListItem Value="1" Selected="True">男生</asp:ListItem>
                 </asp:RadioButtonList>
                 
             </td>
             <td class="valid"></td>
             </tr>
        <tr>
            <td class="th">
                <asp:Label ID="Label5" runat="server" Text="年齡:"></asp:Label>
                            </td>
                             <td class="input">
                                 <asp:TextBox ID="TextBox5" runat="server" MaxLength="2" ></asp:TextBox>
                                <%-- OnTextChanged="TextBox5_TextChanged"--%>
                                 </td>
             <td class="valid">
                <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="TextBox5" CultureInvariantValues="True" ErrorMessage="請輸入正確年齡" ForeColor="Red" MaximumValue="100" MinimumValue="0" Display="Dynamic" Type="Integer"></asp:RangeValidator>
            </td>          
        </tr>
        <tr>
              <td class="th">
               是否吃素:
              </td>
               <td class="input">

                  <asp:RadioButtonList ID="RadioButtonList2" runat="server" >
                     <asp:ListItem Value="0">吃素</asp:ListItem>
                     <asp:ListItem Value="1" Selected="True">吃葷</asp:ListItem>
                 </asp:RadioButtonList>

              </td>
                <td class="valid"></td>    
          </tr>
         <tr>
              <td class="th">
               不吃:
              </td>
               <td class="input">
                   <asp:DropDownList ID="DropDownList_no_eat" runat="server">
                       <asp:ListItem>無</asp:ListItem>
                       <asp:ListItem>牛</asp:ListItem>
                       <asp:ListItem>豬</asp:ListItem>
                       <asp:ListItem>海鮮</asp:ListItem>
                       <asp:ListItem>辣</asp:ListItem>
                   </asp:DropDownList>

              </td>
                <td class="valid"></td>
          </tr>
        </table>     
    <br />
    <br />
         <div class="btn-group" >
            <div style="float:left; width:50%; " >
               <asp:Button ID="Button1" CssClass="button" runat="server" Text="送出" OnClick="Button1_Click" />
            </div>
      
            <div style="float:left; width:50%; text-align:right;" >
                <asp:Button ID="Button2" CssClass="button" runat="server" CausesValidation="False" Text="取消" OnClick="Button2_Click" />
            </div>
         </div>
    </form>
</body>
</html>
