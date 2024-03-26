using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

public partial class login : System.Web.UI.Page
{
    static MySqlConnection conn;
    static MySqlCommand cmd;
    static MySqlDataReader myData;
    static Boolean bbbb;
    protected void Page_Load(object sender, EventArgs e)
    {
        UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        if(Session["ID"]!=null)
        {
            Response.Redirect("main.aspx");
        }
        else
        {
            Session.Clear();
        }
    }



    protected void Button1_Click(object sender, EventArgs e)
    {
        string dbHost = "163.15.172.110";
        string dbUser = "foodieteam";
        string dbPass = "foodiecare";
        string dbName = "foodiecare";
        //Response.Write(TextBox1.Text);

        string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
        conn = new MySqlConnection(connStr);

        // 連線到資料庫 

        string SQL = "SELECT * from account WHERE `帳號`='"+TextBox1.Text+"'";
      
        //Response.Write(SQL);
        cmd = new MySqlCommand(SQL, conn);
        conn.Open();

       
            MySqlDataReader myData = cmd.ExecuteReader();
        // if (TextBox1.Text == "" & TextBox2.Text == "")
        //{
        //    Label1.Text = "請輸入帳號及密碼";
        //}
        //else if (TextBox1.Text == "")
        //{
        //    Label1.Text = "請輸入帳號";
        //}
       
        //else if (TextBox2.Text == "")
        //{
        //    Label1.Text = "請輸入密碼";
        //}
        //else
        //{

            if (myData.HasRows)
            {
                bbbb = true;
                if (myData.Read())
                {
                    if (myData.GetString(2) == TextBox2.Text.Trim().ToString())
                    {
                            Session["ID"] = myData[0];
                            Response.Redirect("main.aspx");
                    }
                    else
                    {
                        //Response.Write(myData.GetString(2));
                        //Response.Write(TextBox2.Text.Trim().ToString());
                        //Response.Write("密碼錯誤");
                        Label3.Text = "密碼錯誤";
                        Label3.Visible = true;
                    }
                }

            }
            else
            {
                // Response.Write("無此帳號");
                Label3.Text = "無此帳號";
                Label3.Visible = true;
                bbbb = false;
            }
            //Response.Write(bbbb);
        //}
       }

    protected void Button2_Click(object sender, EventArgs e)
    {
            Response.Redirect("register.aspx");
    }
}