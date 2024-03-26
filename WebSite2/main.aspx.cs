using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class web1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Session["ID"]==null)
        {
            Response.Redirect("login.aspx");
        }
    }
    protected void Button5_Click(object sender, EventArgs e)
    {
        string url = "https://c110.hami.kmu.edu.tw/asp/main.aspx";
        Response.Redirect(url);
    }

    protected void logout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("login.aspx");
    }
}