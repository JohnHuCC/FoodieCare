using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
public partial class store_detail : System.Web.UI.Page
{
    static string SQL;
    static MySqlConnection conn;
    static MySqlCommand cmd;
    static MySqlDataReader myData;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID"] == null)
        {
            Response.Redirect("login.aspx");
        }
        SQL = "SELECT * FROM `food_back` WHERE `商家名稱`='"+Convert.ToString(Session["store_detail"])+"'";
        data();
        lng.Text = Convert.ToString(Session["lng"]);
        dlng.Text = Convert.ToString(Session["dlng"]);
        lat.Text = Convert.ToString(Session["lat"]);
        dlat.Text = Convert.ToString(Session["dlat"]);
        Response.Write(lng.Text);
        //Response.Write(lat.Text);
        //Response.Write(dlng.Text);
        //Response.Write(dlat.Text);
    }
    protected void Button5_Click(object sender, EventArgs e)
    {
        //string url = "https://c110.hami.kmu.edu.tw/asp/main.aspx";
        //Response.Redirect(url);
        //Response.Write("<script language=javascript>history.go(-1);</script>");
        
    }

    protected void logout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("login.aspx");
    }
    private void data()
    {
        //Response.Write(SQL);
        string dbHost = "163.15.172.110";
        string dbUser = "foodieteam";
        string dbPass = "foodiecare";
        string dbName = "foodiecare";
        // 如果有特殊的編碼在database後面請加上;CharSet=編碼, utf8請使用utf8_general_ci 
        string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
        conn = new MySqlConnection(connStr);

        cmd = new MySqlCommand(SQL, conn);
        conn.Open();
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Response.Write(reader.FieldCount);
                while (reader.Read())
                {
                    Label1.Text = Convert.ToString(reader[0]);
                    if (Convert.ToString(reader[1]) != "")
                    {
                        Label2.Text = Convert.ToString(reader[1]);
                    }
                    else
                    {
                        Label2.Text = "-";
                    }
                    if (Convert.ToString(reader[4]) != "0")
                    {
                        Label3.Text = Convert.ToString(reader[4]);
                    }
                    else
                    {
                        Label3.Text = "-";
                    }
                    if (Convert.ToString(reader[5]) != "0")
                    {
                        Label4.Text = Convert.ToString(reader[5]);
                    }
                    else
                    {
                        Label4.Text = "-";
                    }
                    Label5.Text = Convert.ToString(reader[8]);
                    Label6.Text = Convert.ToString(reader[9]);
                    if(Convert.ToString(reader[10])!="")
                    {
                        Image1.ImageUrl = Convert.ToString(reader[10]);
                    }
                    else
                    {
                        Image1.ImageUrl = "images/icon.jpg";
                    }
                }
                reader.Close();
            }
        }
        catch
        {
            Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
        }

        conn.Close();
    }
}