using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
public partial class Jcrop : System.Web.UI.Page
{
    static string SQL;
    static string SQL2;
    static string ins_SQL;
    static string[] name;
    static string[] img;
    static string[] lat;
    static string[] lng;
    static string sql_state;
    static string update_type;
    static int k = 0;
    static MySqlConnection conn;
    static MySqlCommand cmd;
    static MySqlDataReader myData;
    protected void Page_Load(object sender, EventArgs e)
    {
        SQL = Convert.ToString(Session["SQL"]);
        ins_SQL =Convert.ToString(Session["ins_SQL"]);
        //Response.Write(SQL);
        if (!IsPostBack)
        {
            k = 0;
            name = new string[1];
            data();
            try
            {
                if (name.Length > 0)
                {
                    //Response.Write(img[k]);
                    txt.Text = name[k]+"<br>";
                    
                    if(img[k]=="")
                    {
                        src.Text = "images/icon.jpg";
                    }
                    else
                    {
                        src.Text = img[k];
                    }
                    //Response.Write("<script> var $ = require(\"js/jquery-3.3.1.min.js\"); $(function () {  var img = document.getElementById('draggable'); var img_obj = document.createElement(\"img\"); img_obj.className = 'auto-style1'; img.appendChild(img_obj); img_obj.src = \"" + img[k] + "\"});</script>");
                    //Response.Write("<script>alert(\"123\");</script>");
                    //result_img.ImageUrl = img[k];
                    //Response.Write(k);
                    k = k + 1;
                }
            }
            catch
            {
                //Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"test.aspx\";</script>");
            }
            if (name==null||txt.Text=="")
            {
                Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
            }
        }
    }
    private void data()
    {
        string dbHost = "163.15.172.110";
        string dbUser = "foodieteam";
        string dbPass = "foodiecare";
        string dbName = "foodiecare";
        // 如果有特殊的編碼在database後面請加上;CharSet=編碼, utf8請使用utf8_general_ci 
        string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
        conn = new MySqlConnection(connStr);
        //Response.Write(SQL);
        cmd = new MySqlCommand(SQL, conn);
        conn.Open();
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                name = new string[100];
                img = new string[100];
                lat = new string[100];
                lng = new string[100];
                //Response.Write(reader.FieldCount);
                int i = 0;
                while (reader.Read())
                {
                    if (i < name.Length)
                    {
                        name[i] = Convert.ToString(reader[1]);
                        img[i] = Convert.ToString(reader[0]);
                        lat[i] = Convert.ToString(reader[3]);
                        lng[i] = Convert.ToString(reader[4]);
                        i = i + 1;
                    }
                }
                reader.Close();
            }
        }
        catch 
        {
            //Response.Write(a);
            txt.Text = "很抱歉，已無符合店家...";
            Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
        }
        
        conn.Close();
    }

    protected void Button_h_Click(object sender, EventArgs e)
    {
        try
        {  
                //Response.Write(img[k]);
                txt.Text = name[k] + "<br>";
                if (name == null || txt.Text == "" || txt.Text == "很抱歉，已無符合店家..." || txt.Text == "很抱歉，無符合店家..." || name[k] == "" || name[k] == null)
                {
                    name = new string[100];
                    img = new string[100];
                    lat = new string[100];
                    lng = new string[100];
                    k = 0;
                    Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
                }
                if (img[k] == "")
                {
                    src.Text = "images/icon.jpg";
                }
                else
                {
                    src.Text = img[k];
                }
                //Response.Write("<script> var $ = require(\"js/jquery-3.3.1.min.js\"); $(function () {  var img = document.getElementById('draggable'); var img_obj = document.createElement(\"img\"); img_obj.className = 'auto-style1'; img.appendChild(img_obj); img_obj.src = \"" + img[k] + "\"});</script>");
                //Response.Write("<script>alert(\"123\");</script>");
                result.Text = name[k];
                //result_img.ImageUrl = img[k];
                //Response.Write(k);
                k = k + 1;
        }
        catch (Exception a)
        {
            //Response.Write(a);
            txt.Text = "很抱歉，已無符合店家...";
            Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (name[k-1] != null)
        {
            ins_SQL += "'" + name[k-1] + "')";
            try
            {
                cmd = new MySqlCommand(ins_SQL, conn);
                conn.Open();
                cmd.ExecuteReader();
            }

            catch (Exception E)
            {
                // Response.Write(SQL_update);
            }
        }
        try
        {
            Session["dlat"] = lat[k];
        }
        catch
        {
        }
        try
        {
            Session["dlng"] = lng[k];
        }
        catch
        {
        }
        string sql = "";
        sql_state = "update";
        sql = "SELECT `c3` FROM `food_back` WHERE `商家名稱` ='" + name[k - 1] + "'";
        update_type = data_select(sql);

        sql = "SELECT `" + update_type + "` FROM `user_data` WHERE `ID` ='" + Convert.ToString(Session["ID"]) + "'";

        string result = data_select(sql);
        int update_count = Convert.ToInt16(result);
        update_count = update_count + 1;
        sql = "UPDATE `user_data` SET `" + update_type + "`=" + update_count + " WHERE `ID`='" + Convert.ToString(Session["ID"]) + "'";
        data_insert(sql);
        string url = "http://c110.hami.kmu.edu.tw:81/redirector.php?dlat=" + Session["dlat"] + "&dlng=" + Session["dlng"] + "&lat=" + Session["lat"] + "&lng=" + Session["lng"] + "&a=" + Session["dis"];
        string js = "window.open(\"" + url + "\",\"_blank\");";
        
        ScriptManager.RegisterClientScriptBlock(this, GetType(), "", js, true);
    }

    protected void btn_result_Click(object sender, EventArgs e)
    {
        //string url = "http://c110.hami.kmu.edu.tw/asp/store_detail.aspx";
        //string js = "window.open(\"" + url + "\",\"_blank\");";
        // ScriptManager.RegisterClientScriptBlock(this, GetType(), "", js, true);
        // Session["store_detail"] = name[k];
        //Response.Redirect("store_detail.aspx");
        SQL2 = "SELECT * FROM `food_back` WHERE `商家名稱`='" + name[k-1] + "'";
        jcrop.Attributes["style"] = "display:none";
        detail.Attributes["style"] = "display:block";
        Button5.Visible = false;
        Button_detail.Visible = true;
        data2();
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
    protected void Button_detail_Click(object sender, EventArgs e)
    {
        jcrop.Attributes["style"] = "display:block";
        detail.Attributes["style"] = "display:none";
        Button5.Visible = true;
        Button_detail.Visible = false;
    }
    private void data2()
    {
        //Response.Write(SQL);
        string dbHost = "163.15.172.110";
        string dbUser = "foodieteam";
        string dbPass = "foodiecare";
        string dbName = "foodiecare";
        // 如果有特殊的編碼在database後面請加上;CharSet=編碼, utf8請使用utf8_general_ci 
        string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
        conn = new MySqlConnection(connStr);

        cmd = new MySqlCommand(SQL2, conn);
        conn.Open();
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Response.Write(reader.FieldCount);
                while (reader.Read())
                {
                    detail_name.Text = Convert.ToString(reader[0]);
                    if (Convert.ToString(reader[1]) != "")
                    {
                        detail_phone.Text = Convert.ToString(reader[1]);
                    }
                    else
                    {
                        detail_phone.Text = "-";
                    }
                    if (Convert.ToString(reader[4]) != "0")
                    {
                        detail_point.Text = Convert.ToString(Convert.ToDouble(reader[4])/10)+"/5分";
                    }
                    else
                    {
                        detail_point.Text = "-";
                    }
                    if (Convert.ToString(reader[5]) != "0")
                    {
                        detail_price.Text = Convert.ToString(reader[5])+"元";
                    }
                    else
                    {
                        detail_price.Text = "-";
                    }
                    detail_type.Text = Convert.ToString(reader[8]);
                    detail_type2.Text = Convert.ToString(reader[9]);
                    if (Convert.ToString(reader[10]) != "")
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
            //Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
        }

        conn.Close();
    }
    public void data_insert(string sq)
    {

        //Response.Write(SQL);
        try
        {
            cmd = new MySqlCommand(sq, conn);
            conn.Open();
            cmd.ExecuteReader();
        }
        catch (Exception E)
        {
            Response.Write(E);
        }
    }
    private string data_select(string sq)
    {
        //Response.Write(SQL);
        string dbHost = "163.15.172.110";
        string dbUser = "foodieteam";
        string dbPass = "foodiecare";
        string dbName = "foodiecare";
        // 如果有特殊的編碼在database後面請加上;CharSet=編碼, utf8請使用utf8_general_ci 
        string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
        conn = new MySqlConnection(connStr);

        cmd = new MySqlCommand(sq, conn);
        conn.Open();
        string result = "";
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Response.Write(reader.FieldCount);

                while (reader.Read())
                {
                    if (sql_state == "count")
                    {
                        result = Convert.ToString(reader[2]);
                    }
                    else if (sql_state == "update")
                    {
                        result = Convert.ToString(reader[0]);
                    }
                }
                reader.Close();
                //Label2.Text = sq;
            }
        }
        catch
        {
            //Label2.Text = sq;
            //Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
        }

        conn.Close();
        return result;
    }
}