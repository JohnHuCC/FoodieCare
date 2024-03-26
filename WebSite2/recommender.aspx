<%@ Page Language="C#" AutoEventWireup="true" CodeFile="recommender.aspx.cs" Inherits="test" %>
<languages>  
      <language names="IronPython;Python;py" extensions=".py" displayName="Python" type="IronPython.Runtime.PythonContext, IronPython"/>  
</languages>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        table
        {
            width:100%;
            font-size:48px;
            
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
			padding: 0.75em 0.75em;
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
       
        .auto-style1 {
            height: 50px;
            vertical-align:middle;
            text-align: center;
            color: #FF0000;
        }
        .auto-style8 {
            font-size: 72px;
            vertical-align: middle;
            text-decoration-line:underline;
        }
        .auto-style9 {
            font-size: 48px;
            vertical-align: middle;
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
    </style>
</head>
    <body style="background-color:#FFE8BF">
    <form id="form1" runat="server">
       <div class="btn-group" style="top:0%;" >
            <div style="float:left; width:50%; " >
                <asp:Button ID="Button5" CssClass="button"  runat="server" OnClick="Button5_Click" Text="回主選單" />
            </div>
            <div style="float:left; width:50%; text-align:right;" >
                <asp:Button ID="Button1" CssClass="button"  runat="server" OnClick="logout_Click" Text="登出"  />
            </div>
       </div>
        <center>
            <div id="div1" runat="server">  
            <table id="table1" >
            <tr>
              <td>
                想吃鹹的還是甜的:
              </td>
               <td >
                  <asp:DropDownList ID="DropDownList6" runat="server" >                   
                       <asp:ListItem Value="salty">鹹</asp:ListItem>
                       <asp:ListItem Value="sweety">甜</asp:ListItem>
                   </asp:DropDownList>
              </td>
          </tr>
            <tr>
              <td >
                吃飯價位位於:
              </td>
               <td>
                   <asp:DropDownList ID="DropDownList1" runat="server">                    
                       <asp:ListItem Value="100_less">100元以下</asp:ListItem>
                       <asp:ListItem Value="100_199">100-199</asp:ListItem>
                       <asp:ListItem Value="200_299">200-299</asp:ListItem>
                       <asp:ListItem Value="300_399">300-399</asp:ListItem>
                       <asp:ListItem Value="400_499">400-499</asp:ListItem>
                       <asp:ListItem Value="500_more">500元以上</asp:ListItem>
                   </asp:DropDownList>
              </td>
          </tr>
         <tr>
              <td>
               與誰用餐:
              </td>
               <td>
                   <asp:DropDownList ID="DropDownList2" runat="server">
                      
                       <asp:ListItem Value="alone">自己</asp:ListItem>
                       <asp:ListItem Value="friend">朋友</asp:ListItem>
                       <asp:ListItem Value="couple">情侶</asp:ListItem>
                       <asp:ListItem Value="family">家人</asp:ListItem>
                   </asp:DropDownList>
              </td>
          </tr>
        <tr>
              <td>
               份量吃多少:
              </td>
               <td>
                   <asp:DropDownList ID="DropDownList3" runat="server">
                       <asp:ListItem Value="eat_much">多</asp:ListItem>
                       <asp:ListItem Value="eat_less">少</asp:ListItem>
                   </asp:DropDownList>
              </td>
          </tr>
         <tr>
              <td>
               願意走多少距離:
              </td>
               <td>
                   <asp:DropDownList ID="DropDownList4" runat="server">
                      
                       <asp:ListItem Value="1km_less">1公里內</asp:ListItem>
                       <asp:ListItem Value="1to5km">1-5公里</asp:ListItem>
                       <asp:ListItem Value="5to10km">5-10公里</asp:ListItem>
                       <asp:ListItem Value="10km_more">10公里以上</asp:ListItem>
                   </asp:DropDownList>
              </td>
          </tr>
        <tr>
              <td>
               吃冷食還是熱食:    
              </td>
               <td>
                   <asp:DropDownList ID="DropDownList5" runat="server">
                       <asp:ListItem Value="hot">熱食</asp:ListItem>
                       <asp:ListItem Value="cold">冷食</asp:ListItem>
                   </asp:DropDownList>
              </td>           
          </tr>
                </table>
                <div class="btn-group">
                <asp:Button ID="Button2" CssClass="button" runat="server" OnClick="Button2_Click" Text="送出" />
                </div>
                    <br /><br />            
        </div>
         </center>
        <center>
        <div id="div2" visible="false" runat="server">
            <table >
                <tr id ="result"  runat ="server" >
                    <td class="auto-style1">
                        <span>
                            <strong class="auto-style9"><asp:Label ID="Label1" Text="你是不是想吃:" runat="server"></asp:Label> 
                            </strong>
                             <strong>
                            <asp:Label ID="result_label" runat="server" CssClass="auto-style8"></asp:Label>    
                        </strong> 
                        </span>
                          
                    </td>
                </tr>      
             </table>
                <div class="btn-group">
              <p><asp:Button ID="B_4"  CssClass="button" runat="server" OnClick="Button3_Click" Text="是"  /> &nbsp; 
                  <asp:Button ID="B_84"  CssClass="button" runat="server" OnClick="Button4_Click" Text="否"  />
              </p>
               </div>
               <div>
                     <asp:Image ID="Image1"  Visible="false" ImageUrl="http://iphoto.ipeen.com.tw/photo/ipeen/200x200/def/4/5/2/758254/758254_20141106163158_8152.jpg" runat="server"></asp:Image>
               </div>
        </div>
        </center>
       <div id="div3" runat="server" visible="false" >
        <center>
            <table class="auto-style7" >
                <tr id ="Tr1"  runat ="server" >    
               <td class="auto-style13">
                   猜你喜歡:   
                   <asp:DropDownList  ID="DropDownList_result" runat="server">               
                   </asp:DropDownList>
              </td>           
          </tr>      
             </table>
            <div class="btn-group">
            <asp:Button ID="B_84_commit"  CssClass="button" runat="server" OnClick="B_84_commit_Click" Text="送出" />
            </div>
          </center> 
        </div>
    </form>
</body>
</html>
