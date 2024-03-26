using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

public partial class register : System.Web.UI.Page
{
    static MySqlConnection conn;
    static MySqlCommand cmd;
    static MySqlDataReader myData;
    static Boolean bbbb;
    protected void Page_Load(object sender, EventArgs e)
    {
        UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        string dbHost = "163.15.172.110";
        string dbUser = "foodieteam";
        string dbPass = "foodiecare";
        string dbName = "foodiecare";
        //Response.Write(TextBox1.Text);

        string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
        conn = new MySqlConnection(connStr);
        // 連線到資料庫
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        //if (TextBox2.Text != TextBox3.Text)
        //{

        //    Label7.Text = "密碼輸入不同";
        //}
        //else if (TextBox1.Text == "")
        //{
        //    Label7.Text = "請填入帳號";
        //}
        //else if (!(new System.Text.RegularExpressions.Regex("^[0-9]*$")).IsMatch(TextBox5.Text))
        //{
        //    Label7.Text = "年齡請輸入數字";
        //}

        //else
        //{
            //INSERT INTO `account`( `帳號`, `密碼`, `性別`, `年齡`) VALUES('admin', '123', '1', '15');
            //string SQL = "INSERT INTO `account`( `帳號`, `密碼`, `性別`, `年齡`,`通常吃飯價位位於`,`通常與誰用餐`,`願意走多少距離`,`吃冷食還是熱食`) VALUES ('" + TextBox1.Text.ToString() + "','" + TextBox2.Text.ToString() + "','" + RadioButtonList1.SelectedIndex + "','" + TextBox5.Text.ToString() +"','"+DropDownList1.SelectedValue.ToString()+"','"+DropDownList2.SelectedValue.ToString() + "','"+DropDownList3.SelectedValue.ToString() + "','" + DropDownList4.SelectedValue.ToString() + "');";
            string SQL = "INSERT INTO `account`( `帳號`, `密碼`, `性別`, `年齡`) VALUES ('" + TextBox1.Text.ToString() + "','" + TextBox2.Text.ToString() + "','" + RadioButtonList1.SelectedIndex + "','" + TextBox5.Text.ToString() + "');";
            //Response.Write(SQL);
            try
            {
                cmd = new MySqlCommand(SQL, conn);
                conn.Open();
                cmd.ExecuteReader();
            }
            catch (Exception E)
            {
                Response.Write(E);
            }
         Response.Redirect("login.aspx");


        //}
    }

     protected void Button2_Click(object sender, EventArgs e)
      {
            Response.Redirect("login.aspx");

      }
}