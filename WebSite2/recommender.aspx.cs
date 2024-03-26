using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

public partial class test : System.Web.UI.Page
{
    static dynamic demo;
    public static DataTable dataTable;
    public static string type;
    public static string SQL;
    public static int[] arr;
    public static int taste;
    public static int db_ins;
    public static string data_show;
    public static string price_type;
    public static MySqlConnection conn;
    public static MySqlCommand cmd;
    public static string sql_state;
    public static int[] CF_user;
    public static double[] CF_value;
    public static string [,]user_data;
    public static string[] CF_type;
    public static int[] CF_type_count;
    public static string[] item_type;
    public static int[] item_type_count;
    public static int drop_count = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Session["ID"]==null)
        {
            Response.Redirect("Login.aspx");
        }
        ScriptEngine engine = Python.CreateEngine();

        //Execute demo.py
        ScriptScope scope = engine.ExecuteFile(@"C:\Users\user\source\repos\WebSite2\WebSite2\assets\tree_array.py");
        //Demo class
        dynamic mDemo;

        ////Try create class
        if (scope.TryGetVariable("Demo", out mDemo))
        {
            //Get Demo class
            demo = mDemo();
            //Get text
            //Label1.Text = demo.getText("1");

        }
        if (IsPostBack)
        {
            if(DropDownList4.SelectedValue.ToString()=="1km_less")
            {
                Session["dis"] = 1;
            }
            if (DropDownList4.SelectedValue.ToString() == "1to5km")
            {
                Session["dis"] = 5;
            }
            if (DropDownList4.SelectedValue.ToString() == "5to10km")
            {
                Session["dis"] = 10;
            }
            if (DropDownList4.SelectedValue.ToString() == "10km_more")
            {
                Session["dis"] = 20;
            }
            
            Session["lat"] = Request.QueryString["lat"];
            //Response.Write( Session["lat"]);
            Session["lng"] = Request.QueryString["lng"];
          
            
        }
        else
        {
            drop_count = 0;
        }
    }

 
    protected void Button1_Click(object sender, EventArgs e)
    {
        //Create Python Script
       
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
        cmd = new MySqlCommand(SQL, conn);
        conn.Open();
        if(data_show=="gridview")
        {
            //dataTable = new DataTable();
            if (db_ins==0)
            {
                Session["ins_SQL"] = "INSERT INTO `food_question_form_test`(`ID`,`taste`,`hunger`,`hot_cold`,`eat_mode`,`distance`,`price`,`type`,`agree`,`商家名稱`) VALUES('" + Convert.ToString(Session["ID"]) + "','" + Convert.ToString(DropDownList6.SelectedIndex+1)  + "','" + (DropDownList3.SelectedIndex+1) + "','" + (DropDownList5.SelectedIndex+1) + "','" + (DropDownList2.SelectedIndex+1) + "','" + (DropDownList4.SelectedIndex+1) + "','" + (DropDownList1.SelectedIndex+1) + "','" + result_label.Text + "','" + 1 + "',";
            }
            else
            {
                Session["ins_SQL"] = "INSERT INTO `food_question_form_test`(`ID`,`taste`,`hunger`,`hot_cold`,`eat_mode`,`distance`,`price`,`type`,`agree`,`商家名稱`) VALUES('" + Convert.ToString(Session["ID"]) + "','" + (DropDownList6.SelectedIndex+1) + "','" + (DropDownList3.SelectedIndex+1) + "','" + (DropDownList5.SelectedIndex+1) + "','" + (DropDownList2.SelectedIndex+1) + "','" + (DropDownList4.SelectedIndex+1) + "','" + (DropDownList1.SelectedIndex+1) + "','" + DropDownList_result.SelectedValue+ "','" + 1 + "',";
            }
        }
        else if(data_show == "dropdown")
        {
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                arr = new int[reader.FieldCount];
                while (reader.Read())
                {
                    for (int i =0; i < reader.FieldCount; i++)
                    {
                        arr[i] = Convert.ToInt16(reader[i]);
                    }
                }
            }
            reader.Close();
        }
        else if (data_show == "create_dropdown")
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                int temp = 0;
                while (reader.Read())
                {
                    if(arr[temp]>0)
                    {
                        if(Convert.ToString(reader[0])!= "烘焙、甜點、零食" && drop_count<5)
                        {
                            if (DropDownList_result.Items.FindByValue(Convert.ToString(reader[0])) == null)
                            {
                                DropDownList_result.Items.Add(Convert.ToString(reader[0]));
                                drop_count += 1;
                            }
                        }
                        if(drop_count>=5)
                        {
                            break;
                        }
                    }
                    temp = temp + 1;
                }
            }
            reader.Close();
        }
        conn.Close();
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        if(DropDownList6.SelectedValue=="sweety")
        {
            if(DropDownList3.SelectedValue=="eat_less")
            {
                result_label.Text = "烘焙、甜點、零食";
            }
            else
            {
                result_label.Text = "咖啡、簡餐、茶";
            }
            taste = 0;
        }
        else
        {
            type = demo.getText(DropDownList1.SelectedValue.ToString(), DropDownList2.SelectedValue.ToString(), DropDownList3.SelectedValue.ToString(), DropDownList4.SelectedValue.ToString(), DropDownList5.SelectedValue.ToString());
            if(type == "烘焙、甜點、零食")
            {
                if (DropDownList3.SelectedValue == "eat_less")
                {
                    type = "小吃";
                }
                else
                {
                    type = "日式料理";
                }
            }
            result_label.Text = type;
            taste = 1;
        }
       
        //Response.Write(type);
        div1.Visible = false;
        div2.Visible = true;
        //Image1.Visible = true;
        price_type = DropDownList1.SelectedValue;
        //Response.Write("<center><font color=\"#0000FF\" style=\"font-size: 25pt\">為您推薦:" + type+"</font></center>");

        //data();
    }
    public int a = 0;

    protected void Button3_Click(object sender, EventArgs e)
    {
        div1.Visible = false;
        div2.Visible = false;
        type = result_label.Text;
        if(price_type == "100_less")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<100 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "100_199")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<200 AND `平均價格`>100 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "200_299")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<300 AND `平均價格`>200 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "300_399")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<400 AND `平均價格`>300 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "400_499")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<500 AND `平均價格`>400 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "500_more")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`>500 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        //Response.Write(SQL);
        data_show = "gridview";
        Session["SQL"] = SQL;
        db_ins = 0;
        data();
        Response.Redirect("recommend_result.aspx");
        
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        string sq ="";
        sql_state = "CF_a";
        sq = "SELECT * FROM `user_data`";
        data_select(sq);
        sql_state = "CF";
        data_select(sq);
        user();
        CF_type = new string[100];
        CF_type_count = new int[100];
        for (int i = 0; i < 3; i++)
        {
               int maxIndex = CF_value.ToList().IndexOf(CF_value.Max());
                if (CF_value[maxIndex] <= 0)
                {
                     break;
                }
                CF_value[maxIndex] = 0;
                sq = "SELECT * FROM food_question_form_test WHERE ID='" + Convert.ToString(CF_user[maxIndex]) + "'";
                
                sql_state = "CF_type";
                data_select(sq);
     
        }
        if (CF_type != null)
        {
            if (CF_type.Length > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int maxIndex = CF_type_count.ToList().IndexOf(CF_type_count.Max());
                    if (drop_count < 5 && CF_type[maxIndex] != null)
                    {
                        if (DropDownList_result.Items.FindByValue(CF_type[maxIndex]) == null&& CF_type[maxIndex]!="")
                        {
                            DropDownList_result.Items.Add(CF_type[maxIndex]);
                            drop_count += 1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        if (drop_count < 5)
        {
            for (int i = 0; i < 10; i++)
            {
                sql_state = "item_type";
                sq = "SELECT `c3` FROM food_back WHERE `商家名稱` IN (SELECT  `商家名稱` FROM user_click WHERE `ID` = '"+Convert.ToString(Session["ID"])+ "' ORDER BY `update_time` DESC)";
                data_select(sq);
            }
            string[] result_item;
            result_item = new string[10];
            if (item_type.Length > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int maxIndex = item_type_count.ToList().IndexOf(item_type_count.Max());
                    if (drop_count < 5 && item_type[maxIndex]!=null)
                    {
                        if (DropDownList_result.Items.FindByValue(item_type[maxIndex]) == null && item_type[maxIndex] != "")
                        {
                            DropDownList_result.Items.Add(item_type[maxIndex]);
                            drop_count += 1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        if (drop_count < 5)
        {
            if (taste == 0)
            {
                DropDownList_result.Items.Clear();
                DropDownList_result.Items.Add("咖啡、簡餐、茶");
                DropDownList_result.Items.Add("烘焙、甜點、零食");
                DropDownList_result.Items.Add("冰品、飲料、甜湯");
            }
            else
            {
                SQL = "SELECT * From `food_association` WHERE `" + type + "` =2";
                data_show = "dropdown";
                data();
                SQL = "SELECT column_name from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'food_association'";
                data_show = "create_dropdown";
                data();
            }
        }
        div2.Visible = false;
        div3.Visible = true;
    }

    protected void B_84_commit_Click(object sender, EventArgs e)
    {
        type = DropDownList_result.SelectedValue;
        if (price_type == "100_less")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<100 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "100_199")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<200 AND `平均價格`>100 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "200_299")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<300 AND `平均價格`>200 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "300_399")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<400 AND `平均價格`>300 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "400_499")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`<500 AND `平均價格`>400 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        else if (price_type == "500_more")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
            "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
             "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
             "food_back WHERE (`c3` = \"" + type + "\" or `c4`=\"" + type + "\" ) AND `平均價格`>500 HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0.0001  ORDER BY DISTANCE ASC";
        }
        data_show = "gridview";
        Session["SQL"] = SQL;
        db_ins = 1;
        data();
        Response.Redirect("recommend_result.aspx");
        
        div3.Visible = false;
    }

    protected void Button5_Click(object sender, EventArgs e)
    {
        string url = "https://c110.hami.kmu.edu.tw/asp/main.aspx";
        Response.Redirect(url);
    }


    protected void Button6_Click(object sender, EventArgs e)
    {
        //RR.Visible = false;
        string url = "https://c110.hami.kmu.edu.tw/asp/recommender.aspx?lng=" + Session["lng"] + "&lat=" + Session["lat"];
        Response.Redirect(url);
        
    }

    protected void logout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("login.aspx");
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
        MySqlDataReader reader = cmd.ExecuteReader();
        string result = "";
        int i;
        if (sql_state == "CF_a")
        {
            i = 0;
            int k = reader.FieldCount;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    i += 1;
                }
            }
            user_data = new string[i, k];
        }
        else if (sql_state == "item_type")
        {
            item_type = new string[100];
            item_type_count = new int[100];
        }
        try
        {
            i = 0;
            if (reader.HasRows)
            {
                //Response.Write(reader.FieldCount);           
                while (reader.Read())
                {  
                    if (sql_state == "CF")
                    {
                        for (int k = 0; k < reader.FieldCount; k++)
                        {
                            user_data[i,k] = Convert.ToString(reader[k]);
                        }
                        i += 1;
                    }
                    else if(sql_state == "CF_type")
                    {
                        int index = Array.IndexOf(CF_type, Convert.ToString(reader[9]));
                        if (index>=0)
                        {
                            CF_type_count[index] += 1;
                            i += 1;
                        }
                        else
                        {
                            CF_type[i] = Convert.ToString(reader[9]);
                            CF_type_count[i] = 1;
                            i += 1;
                        }
                    }
                    else if (sql_state == "item_type")
                    {
                        int index = Array.IndexOf(item_type, Convert.ToString(reader[0]));
                        if (index >= 0)
                        {
                            item_type_count[index] += 1;
                            i += 1;
                        }
                        else
                        {
                            item_type[i] = Convert.ToString(reader[0]);
                            item_type_count[i] = 1;
                            i = i+1;
                        }
                    }
                }
            }
            reader.Close();
        }
        catch
        {
            //Label2.Text = sq;
            //Response.Write("<script>alert(\"很抱歉，無符合店家...\"); document.location.href=\"main.aspx\";</script>");
        }

        conn.Close();
        return result;
    }
    public double get_user_value(int i, int N_a, int []X_a)
    {
        int N_b = user_N(i);
        int []X_b = user_X(N_b, i);
        double value = count(N_a, N_b, X_a, X_b);
        return (value);
    }


    public int user_N(int i)
    {
        int N_b = 0;
        for (int j = 0; j < user_data.GetLength(1); j++)
        {
            if (j > 0)
                N_b += Convert.ToInt16(user_data[i,j]);
        }
        return N_b;
    }


    public int[] user_X(int N_b, int i)
    {
        int[] X = new int[user_data.GetLength(1)];
        for (int j = 0; j < user_data.GetLength(1); j++)
        {
            if (j > 0)
                X[j] = Convert.ToInt16(user_data[i,j]);
        }
        return X;
    }
    public double count(int N_a, int N_b, int []X_a, int []X_b)
    {
        int U = 0;
        int D_a = 0;
        int D_b = 0;
        for (int i = 0; i < X_a.Length; i++)
        {
            if (i > 0)
            {
                U += X_a[i] * X_b[i];
                D_a += X_a[i]* X_a[i];
                D_b += X_b[i]* X_b[i];
            }
        }
        double D = Math.Sqrt(D_a * D_b);
        return U / D;
     }
    public void user()
    {
        int ID = Convert.ToInt16(Session["ID"]);
        CF_user = new int[(user_data.GetLength(0))];
        CF_value = new double[(user_data.GetLength(0))];
        int N_a = 0;
        int[] X_a = new int[user_data.GetLength(1)];
        for (int j = 0; j < user_data.GetLength(1); j++)
        {
            if (j > 0)
            {
                int a = Convert.ToInt16(user_data[ID,j]);
                N_a += a;
                X_a[j] = Convert.ToInt16(user_data[ID,j]);
            }
        }
        int k = 0;
        for (int i = 0; i < user_data.GetLength(0); i++)
        {
            double value = 0;
            if (i != ID)
            {
                value = get_user_value(i, N_a, X_a);
            }
            
            if (value>0.5 && k < 5)
            {
                CF_user[k] = i + 1;
                CF_value[k]= value;
                k += 1;
            }
        }
    }
}