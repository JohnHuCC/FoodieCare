<%@ Page Language="C#" AutoEventWireup="true" CodeFile="food_browser.aspx.cs" Inherits="web2"  MaintainScrollPositionOnPostback="true"%>

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
            font-size:48px;
        }
        select
        {
            font-size:36px;
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
            text-align:center;
            align-items:center;
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
        .auto-style1 {
            float: left;
            width: 164px;
        }
        .check_box {
            font-size:48px;
            width:100%;
        }
    </style>
</head>
<body style="background-color:#FFE8BF">
    <form id="form1" runat="server" >
        
        <div class="btn-group" style="position: fixed;top:0%;" >
            <div style="float:left; width:50%; " >
                <asp:Button ID="Button5" CssClass="button"  runat="server" OnClick="Button5_Click" Text="回主選單" />
                <asp:Button ID="Button_detail" CssClass="button"  runat="server" Visible="false" OnClick="Button_detail_Click" Text="回上一頁" />
            </div>
            <div style="float:left; width:50%; text-align:right;" >
                <asp:Button ID="Button1" CssClass="button"  runat="server" OnClick="logout_Click" Text="登出"/>
            </div>
             <div  class="check_box" id="check_box" runat="server" >
            <div style="width:33%;float:left; text-align:center;">
            <div style="height:100px">
            <asp:CheckBox ID="CheckBox_dis" AutoPostBack="True" Text="距離" runat="server" OnCheckedChanged="CheckBox_dis_CheckedChanged"></asp:CheckBox>
            </div>
            <div id="drop_dis" style="font-size:48px;" runat ="server" visible="false">
            <asp:DropDownList ID="DropDownList1"  runat ="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                <asp:ListItem Value=""></asp:ListItem>
                <asp:ListItem Value="0.5">0.5公里</asp:ListItem>
                <asp:ListItem Value="1">1公里</asp:ListItem>
                <asp:ListItem Value="1.5">1.5公里</asp:ListItem>
                <asp:ListItem Value="3">3公里</asp:ListItem>
                <asp:ListItem Value="5">5公里</asp:ListItem>
                <asp:ListItem Value="10">10公里</asp:ListItem>
            </asp:DropDownList> 
            </div>
            </div>
            <div style="width:33%;float:left;text-align:center;">
            <div style="height:100px">
            <asp:CheckBox ID="CheckBox_price" AutoPostBack="True" Text="價格" runat="server" OnCheckedChanged="CheckBox_price_CheckedChanged"></asp:CheckBox>
            </div>
            <div id="drop_price"  runat ="server" visible="false" style="font-size:48px;">

            <asp:DropDownList ID="DropDownList2"  runat ="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
                <asp:ListItem Value=""></asp:ListItem>
                <asp:ListItem Value="1">100元以下</asp:ListItem>
                <asp:ListItem Value="2">100~199元</asp:ListItem>
                <asp:ListItem Value="3">200~299元</asp:ListItem>
                <asp:ListItem Value="4">300~399元</asp:ListItem>
                <asp:ListItem Value="5">400~499元</asp:ListItem>
                <asp:ListItem Value="6">500元以上</asp:ListItem>
            </asp:DropDownList> 
            </div>
            </div>
            <div style="width:33%;float:left;text-align:center;">
            <div style="height:100px">
            <asp:CheckBox ID="CheckBox_type" AutoPostBack="True" Text="類型" runat="server" OnCheckedChanged="CheckBox_type_CheckedChanged"></asp:CheckBox>
            </div>
             <div id="drop_type"  runat ="server" visible="false" style="font-size:48px;float:left;width:auto "  >
                <asp:DropDownList ID="DropDownList3"  runat ="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList3_SelectedIndexChanged">
                <asp:ListItem></asp:ListItem>
            </asp:DropDownList>

                <asp:DropDownList ID="DropDownList4"  runat ="server"  Visible="false" AutoPostBack="True" OnSelectedIndexChanged="DropDownList4_SelectedIndexChanged">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList> 
            </div>
            </div>
        </div> 
        <div id="detail" style="display:none;" runat="server" >
            <div style="float:left; width:45%;">
                <asp:Image ID="Image1" Width="400px" Height="400px" runat="server" />
            </div>
            <div>
                <table style="font-size:32px;width:45%;" >
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
            <%--<div style="float:left;" >
                <asp:ImageButton ID="ImageButton_item_1" runat="server" />
                <asp:ImageButton ID="ImageButton_item_2" runat="server" />
                <asp:ImageButton ID="ImageButton_item_3" runat="server" />
                <asp:ImageButton ID="ImageButton_item_4" runat="server" />
                <asp:ImageButton ID="ImageButton_item_5" runat="server" />
            </div>--%>
        </div>
        </div>
       
        <br />
        <br />
        <div style="height:300px" ></div>
        <div style="text-align:center;">
            <asp:GridView ID="GridView1"  runat="server" CellPadding="4" ForeColor="#333333" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" EmptyDataText="no data" OnRowDataBound="GridView1_RowDataBound" OnPageIndexChanging="GridView1_PageIndexChanging" AllowPaging="True" OnDataBound="GridView1_DataBound" OnPageIndexChanged="GridView1_PageIndexChanged" OnRowCreated="GridView1_RowCreated" OnSelectedIndexChanging="GridView1_SelectedIndexChanging" ShowHeaderWhenEmpty="True" PageSize="20" AutoGenerateColumns="False"  >
               
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    
                    <asp:TemplateField HeaderText="選取" Visible="false" > 
                        <ItemTemplate>
                            <asp:Button ID="Button1"  OnClick="select" runat="server" CausesValidation="False" CommandName="Select" Text="選取" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="商家圖片">
                        <ItemTemplate>
                            <asp:ImageButton ID="ImageButton1" ImageUrl='<%# Eval("圖片") %>' Width="250px" Height="250px"  OnClick="abcd"  CausesValidation="False" CommandName="Select" runat="server" />
                            <%--<asp:Image ID="Image1"  ImageUrl='<%# Eval("圖片") %>' Width="250px" Height="250px" runat="server"></asp:Image>--%>
                            <%--<img src='<%# Eval("圖片") %>' width="150px" height="100px" />--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商家名稱" SortExpression="商家名稱">
                         <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="false" CommandName="Select" Text='<%# Bind("商家名稱") %>' OnClick="abcd"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="電話" HeaderText="電話" SortExpression="電話" />
                    <asp:BoundField DataField="lat" HeaderText="lat" ReadOnly="True" SortExpression="lat" />
                    <asp:BoundField DataField="lng" HeaderText="lng" ReadOnly="True" SortExpression="lng" />
                    <asp:BoundField DataField="point" HeaderText="評分" SortExpression="point" />
                    <asp:BoundField DataField="平均價格" HeaderText="平均價格" SortExpression="平均價格" />
                    <asp:BoundField DataField="DISTANCE" HeaderText="距離" SortExpression="DISTANCE" />
                    <%--<asp:TemplateField HeaderText="詳細資料">
                        <ItemTemplate>
                            <asp:Button ID="Button4" runat="server"  OnClick="abcd"  CausesValidation="False" CommandName="Select" Text="查看" BackColor="#FF8800" Height="100" Width="100" Font-Size="48" ForeColor="White" />
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    

                    <asp:TemplateField ShowHeader="False" HeaderText="導航">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="false" CommandName="Select" Text="導航" OnClick="abc"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <EmptyDataTemplate>
                    no data
                </EmptyDataTemplate>
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerSettings Position="TopAndBottom" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
            </div>
            <br />
            <br />
    </form>
</body>
</html>
