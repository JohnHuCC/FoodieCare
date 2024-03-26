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

public partial class web2 : System.Web.UI.Page
{
    static DataTable dataTable;
    static int dis_ck;
    static int price_ck;
    static int type_ck;
    static int dis_v=0;
    static int price_v = 0;
    static int type_v = 0;
    static string SQL ="";
    static string linkbtn;
    static string SQL2;
    static string name_choose;
    static dynamic demo;
    static string sql_state;
    static string update_type;
    static MySqlConnection conn;
    static MySqlCommand cmd;
    static MySqlDataReader myData;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID"] == null)
        {
            Response.Redirect("login.aspx");
        }
        if (IsPostBack)
        {
            
            //Response.Write(Session.Count);
            //Response.Write(Session["lat"]);
            Session["lat"] = Request.QueryString["lat"];
            ////Response.Write(Request.Params["lat"]);
            Session["lng"] = Request.QueryString["lng"];
            //Session["lat"] = this.lat.Value ;
            //Response.Write(Session["lat"]);
            ////Session["lng"] = Request.Params["lng"];
            ////Response.Write(Session["lat"]);
        }
        if(!IsPostBack)
        {
            SQL = "";
        }
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["dis"] = DropDownList1.SelectedValue.ToString();
        if (DropDownList1.SelectedValue != "" && SQL.Contains("`平均價格` <") == false && SQL.Contains("`平均價格` >") == false && SQL.Contains("`c3`")==false)
        {
            dis_v = 1;
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
             "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
              "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
              "food_back HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0  ORDER BY DISTANCE ASC";
        }
        else if (DropDownList1.SelectedValue != "")
        {
            dis_v = 1;

            if (SQL.Contains("HAVING") == false)
            {
                int k;
                k = SQL.IndexOf("ORDER");
                SQL = SQL.Insert(k - 1, " HAVING DISTANCE <" + Session["dis"] + " AND DISTANCE >0  ");
            }
            else
            {
                int a = SQL.IndexOf("DISTANCE <");
                int b = SQL.IndexOf("AND DISTANCE");
                int k = b - a;
                SQL = SQL.Remove(a, k);
                b = SQL.IndexOf("AND DISTANCE");
                SQL = SQL.Insert(b-1," DISTANCE < " + Session["dis"]);
            }
        }
        if (Session["dis"] == "" && SQL.Contains("HAVING"))
        {
            int a = SQL.IndexOf("HAVING");
            int b = SQL.IndexOf("ORDER");
            int k = b - a;
            //Response.Write("a" + a + "b" + b + "k" + k);
            SQL = SQL.Remove(a, k);
            //Response.Write(SQL);
            dis_v = 0;
        }
        data();
    }
    protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(SQL.Contains("HAVING")==false)
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
             "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
              "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
              " food_back ORDER BY DISTANCE ASC";
        }
        if (DropDownList2.SelectedValue != "")
        {
            price_v = 1;
            if (dis_v==1)
            {
                int k = SQL.IndexOf("HAVING");
                //Response.Write("1:" + k);
                SQL_price(k);
                data();
            }
            else
            {
                int k = SQL.IndexOf("ORDER");
                //Response.Write("2:" + k);
                SQL_price(k);
                data();
            }
        }
        else
        {
            int a = 0;
            if (SQL.Contains("AND `平均價格` <"))
            {
                a = SQL.IndexOf("AND `平均價格` <");
            }
            else if (SQL.Contains("AND `平均價格` >"))
            {
                a = SQL.IndexOf("AND `平均價格` >");
            }
            else if (SQL.Contains("`平均價格` <"))
            {
                a = SQL.IndexOf("`平均價格` <");
            }
            else if (SQL.Contains("`平均價格` >"))
            {
                a = SQL.IndexOf("`平均價格` >");
            }
            int b;
            if (SQL.Contains("HAVING"))
            {
                b = SQL.IndexOf("HAVING");
            }
            else
            {
                b = SQL.IndexOf("ORDER");
            }
            int k = b - a;
            //Response.Write("a" + a + "b" + b + "k" + k);
            SQL = SQL.Remove(a, k);
            if (SQL.Contains("`c3`") == false)
            {
                SQL = SQL.Replace("WHERE", "");
            }
            //Response.Write(SQL);
            price_v = 0;
            data();
        }
    }

    protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList4.Visible = true;
        DropDownList4.Items.Clear();
        DropDownList4.Items.Add("");

        // 連線到資料庫 

        string SQL2 = "SELECT `c4` FROM `food_type` WHERE `c3`='" + DropDownList3.SelectedValue + "'";
        // Response.Write(SQL);

        cmd = new MySqlCommand(SQL2, conn);
        conn.Open();


        myData = cmd.ExecuteReader();

        if (!myData.HasRows)
        {
            // 如果沒有資料,顯示沒有資料的訊息 
            Console.WriteLine("No data.");
        }
        else
        {
            // 讀取資料並且顯示出來 
            while (myData.Read())
            {
                DropDownList4.Items.Add(myData.GetString(0));
            }
            myData.Close();
        }
        conn.Close();
        if (SQL == "")
        {
            SQL = "SELECT `圖片`, `商家名稱`,`電話` ,`lat` ,`lng`,`point` ,`平均價格`, (6371 * ACOS(COS(RADIANS(" + Session["lat"] + ")) * " +
             "COS(RADIANS(lat)) * COS( RADIANS(lng) " + "- RADIANS(" + Session["lng"] + ") ) " +
              "+ SIN(RADIANS(" + Session["lat"] + ")) * SIN(RADIANS(lat)) ) ) AS DISTANCE FROM " +
              " food_back ORDER BY DISTANCE ASC";
        }
        if (DropDownList3.SelectedValue != "")
        {
            if (SQL.Contains("`c3`"))
            {
                int a = SQL.IndexOf("`c3`");
                int b = 0;
                String i = "";
                if (SQL.Contains("`平均價格` <") || SQL.Contains("`平均價格` >"))
                {
                    if (SQL.Contains("`平均價格` <"))
                    {
                        b = SQL.IndexOf("`平均價格` <");
                        i = "`平均價格` <";
                    }
                    else
                    {
                        b = SQL.IndexOf("`平均價格` >");
                        i = "`平均價格` >";
                    }
                }
                else
                {
                    if (SQL.Contains("HAVING"))
                    {
                        b = SQL.IndexOf("HAVING");
                        i = "HAVING";
                    }
                    else
                    {
                        b = SQL.IndexOf("ORDER");
                        i = "ORDER";
                    }
                }
                int k = b - a;
                SQL = SQL.Remove(a, k);
                //Response.Write(SQL);
                b = SQL.IndexOf(i);
                if (i == "`平均價格` >" || i == "`平均價格` <")
                {
                    SQL = SQL.Insert(b, " `c3`='" + DropDownList3.SelectedValue + "' AND ");
                }
                else
                {
                    SQL = SQL.Insert(b, " `c3`='" + DropDownList3.SelectedValue + "' ");
                }
            }
            else
            {
                if (SQL.Contains("`平均價格` <") || SQL.Contains("`平均價格` >"))
                {
                    int a = 0;
                    if (SQL.Contains("`平均價格` <"))
                    {
                        a = SQL.IndexOf("`平均價格` <");
                    }
                    else
                    {
                        a = SQL.IndexOf("`平均價格` >");
                    }
                    SQL = SQL.Insert(a - 1, " `c3`= '" + DropDownList3.SelectedValue + "' AND");
                }
                else
                {
                    int a = 0;
                    if (SQL.Contains("HAVING"))
                    {
                        a = SQL.IndexOf("HAVING");
                    }
                    else
                    {
                        a = SQL.IndexOf("ORDER");
                    }
                    SQL = SQL.Insert(a - 1, " WHERE `c3`= '" + DropDownList3.SelectedValue + "' ");
                }
            }
            type_v = 1;
        }
        else
        {
            int a = SQL.IndexOf("`c3`");
            int b = 0;
            
            if (SQL.Contains("`平均價格` <") || SQL.Contains("`平均價格` >"))
            {
                if (SQL.Contains("`平均價格` <"))
                {
                    b = SQL.IndexOf("`平均價格` <");
                    
                }
                else
                {
                    b = SQL.IndexOf("`平均價格` >");
                    
                }
            }
            else
            {
                if (SQL.Contains("HAVING"))
                {
                    b = SQL.IndexOf("HAVING");
                    
                }
                else
                {
                    b = SQL.IndexOf("ORDER");
                    
                }
            }
            int k = b - a;
            SQL = SQL.Remove(a, k);
            if (SQL.Contains("`平均價格` <")==false &&  SQL.Contains("`平均價格` >")==false)
            {
                SQL = SQL.Replace("WHERE", "");
            }
            type_v = 0;
        }
        data();
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
        dataTable = new DataTable();
        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
        try
        {
            da.Fill(dataTable);
        }
        catch
        {
        }


        GridView1.DataSource = dataTable;
        GridView1.DataBind();
        conn.Close();
    }

    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView1.SelectedRow.Cells[0].Text = "已選擇";
        
        GridView1.SelectedRow.BackColor = System.Drawing.Color.Yellow;

        Session["index"] = GridView1.SelectedIndex;

        MaintainScrollPositionOnPostBack = true;
        GridView1.EmptyDataText = "無資料";
        try
        {
            Session["dlat"] = GridView1.SelectedRow.Cells[4].Text;
        }
        catch
        {
        }
        try
        {
            Session["dlng"] = GridView1.SelectedRow.Cells[5].Text;
        }
        catch
        {
        }
        try
        {
            LinkButton name = (LinkButton)GridView1.SelectedRow.Cells[2].FindControl("LinkButton1");
            name_choose = name.Text;
        }
        catch
        {
        }
        if (linkbtn == "a")
        {
            string sql = "";
            sql_state = "update";
            sql = "SELECT `c3` FROM `food_back` WHERE `商家名稱` ='" + name_choose + "'";
            update_type = data_select(sql);
            
            sql = "SELECT `"+ update_type + "` FROM `user_data` WHERE `ID` ='" + Convert.ToString(Session["ID"]) + "'";
            
            string result = data_select(sql);
            int update_count = Convert.ToInt16(result);
            update_count = update_count + 1;
            sql = "UPDATE `user_data` SET `" + update_type + "`="+ update_count+" WHERE `ID`='"+ Convert.ToString(Session["ID"]) + "'";
            data_insert(sql);
            string url = "";
            if (DropDownList1.SelectedValue == "")
            {
                url = "http://c110.hami.kmu.edu.tw:81/redirector.php?dlat=" + Session["dlat"] + "&dlng=" + Session["dlng"] + "&lat=" + Session["lat"] + "&lng=" + Session["lng"] + "&a=10";
            }
            else
            {
                url = "http://c110.hami.kmu.edu.tw:81/redirector.php?dlat=" + Session["dlat"] + "&dlng=" + Session["dlng"] + "&lat=" + Session["lat"] + "&lng=" + Session["lng"] + "&a=" + DropDownList1.SelectedValue;
            }
            string js = "window.open(\"" + url + "\",\"_blank\");";
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "", js, true);
        }
        if (linkbtn == "b")
        {
            //string url = "http://c110.hami.kmu.edu.tw/asp/store_detail.aspx";
            //string js = "window.open(\"" + url + "\",\"_blank\");";
            //ScriptManager.RegisterClientScriptBlock(this, GetType(), "", js, true);
            SQL2 = "SELECT * FROM `food_back` WHERE `商家名稱`='" + name_choose + "'";
            check_box.Attributes["style"] = "display:none";
            detail.Attributes["style"] = "display:block";
            Button5.Visible = false;
            Button_detail.Visible = true;
            GridView1.Visible = false;
            data2();
            string sq = "SELECT * FROM user_click WHERE ID ='" + Convert.ToString(Session["ID"]) + "'AND 商家名稱='" + name_choose + "'";
            sql_state = "count";
            string count = data_select(sq);
            if(count!="")
            {
                sql_state = "count";
                sq = "SELECT * FROM user_click WHERE Year(update_time) = Year(Now()) And Month(update_time) = Month(Now()) And Day(update_time) = Day(Now()) AND ID ='" + Convert.ToString(Session["ID"]) + "'AND 商家名稱='" + name_choose + "'";
                string today = data_select(sq);
                if (today == "")
                {
                    int c = Convert.ToInt16(count);
                    c += 1;
                    count = Convert.ToString(c);
                    sq = "UPDATE user_click SET count ='" + Convert.ToString(count) + "' WHERE ID = '" + Convert.ToString(Session["ID"]) + "'AND 商家名稱='" + name_choose + "'";
                    data_insert(sq);
                }
            }
            else
            {
                sq = "INSERT INTO `user_click` (`ID`, `商家名稱`) VALUES('" + Convert.ToString(Session["ID"]) + "', '" + name_choose + "')";
                data_insert(sq);
            }

            //Response.Redirect("store_detail.aspx");
        }
        //Response.Write(name_choose);

    }
    
    protected void abc(object sender, EventArgs e)

    {
        
        linkbtn = "a";
    }
    protected void abcd(object sender, EventArgs e)

    {
        linkbtn = "b";
    }
    protected void select (object sender, EventArgs e)

    {
        linkbtn = "";
    }



    public int a = 0;



    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView1.SelectedIndex = -1;
        MaintainScrollPositionOnPostBack = false;
        try
        {
            e.Row.Cells[3].Visible = false;
            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;
        //e.Row.Cells[10].Visible = false;


            //if (e.Row.Cells[3].Text.Trim().Length < 7)//電話
            //{
            //    if (e.Row.RowIndex != -1) { e.Row.Cells[3].Text = "-"; }

            //}

            if (e.Row.Cells[6].Text.Trim().Equals("0"))//評分
            {
                e.Row.Cells[6].Text = "-";
            }
            else
            {
                if (e.Row.RowIndex != -1)
                {
                    String str = e.Row.Cells[6].Text.ToString();
                    e.Row.Cells[6].Text = str.Substring(0, 1) + "." + str.Substring(str.Length - 1);
                }
            }
            if (e.Row.Cells[7].Text.Trim().Equals("0"))//平均價格
            {
                e.Row.Cells[7].Text = "-";
            }
            ImageButton image1 = (ImageButton)e.Row.Cells[1].FindControl("ImageButton1");
            if (image1.ImageUrl.Length<5)
            {
                image1.ImageUrl = "images/icon.jpg";
            }
        }
        catch (Exception E)
        {
           
        }
        try
        {
            e.Row.Cells[8].Text = ((Convert.ToSingle(e.Row.Cells[8].Text.ToString())) ).ToString().Substring(0, 5);
            Double k = Convert.ToDouble(e.Row.Cells[8].Text);
            if (e.Row.Cells[8].Text.Trim().Equals(""))//距離
            {
                e.Row.Cells[8].Text = "-";
            }
            else if (k < 1)//距離
            {
                e.Row.Cells[8].Text = Convert.ToString(k * 1000) + "公尺";
            }
            else
            {
                e.Row.Cells[8].Text = e.Row.Cells[8].Text + "公里";
            }
        }
        catch (Exception a)
        {

        }
    }
  


    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.SelectedIndex = -1;
        MaintainScrollPositionOnPostBack = false;
        GridView1.PageIndex = e.NewPageIndex;
        data();
    }

    protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        int i = Convert.ToInt16(Session["index"]);
        GridView1.Rows[i].BackColor= System.Drawing.Color.White;
        MaintainScrollPositionOnPostBack = true;
        
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
       
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

    protected void CheckBox_dis_CheckedChanged(object sender, EventArgs e)
    {
        if(CheckBox_dis.Checked==true)
        {
            drop_dis.Visible = true;
            DropDownList1.SelectedValue = "";
        }
        else
        {
            if (dis_v == 1)
            {
                int a = SQL.IndexOf("HAVING");
                int b = SQL.IndexOf("ORDER");
                int k = b - a;
                //Response.Write("a" + a + "b" + b + "k" + k);
                SQL = SQL.Remove(a, k);
                //Response.Write(SQL);
                
                dis_v = 0;
                data();
            }
            drop_dis.Visible = false;
        }
    }

    protected void CheckBox_price_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox_price.Checked == true)
        {
            drop_price.Visible = true;
            DropDownList2.SelectedValue = "";
        }
        else
        {
            if (price_v == 1)
            {
                int a =0;
                if (SQL.Contains("`c3`") && SQL.Contains("AND `平均價格` <"))
                {
                    a = SQL.IndexOf("AND `平均價格` <");
                }
                else if (SQL.Contains("`c3`") && SQL.Contains("AND `平均價格` >"))
                {
                    a = SQL.IndexOf("AND `平均價格` >");
                }
                else if (SQL.Contains("`平均價格` <"))
                {
                    a = SQL.IndexOf("`平均價格` <");
                }
                else if(SQL.Contains("`平均價格` >"))
                {
                    a = SQL.IndexOf("`平均價格` >");
                }
                int b;
                if(SQL.Contains("HAVING"))
                {
                    b = SQL.IndexOf("HAVING");
                }
                else
                {
                    b = SQL.IndexOf("ORDER");
                }
                int k = b - a;
                //Response.Write("a" + a + "b" + b + "k" + k);
                SQL = SQL.Remove(a, k);
                if (SQL.Contains("`c3`") == false)
                {
                    SQL = SQL.Replace("WHERE", "");
                }
                //Response.Write(SQL);
                price_v = 0;
                data();
            }
            drop_price.Visible = false;
        }
    }

    protected void CheckBox_type_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox_type.Checked == true)
        {
            drop_type.Visible = true;
            string dbHost = "163.15.172.110";
            string dbUser = "foodieteam";
            string dbPass = "foodiecare";
            string dbName = "foodiecare";
            //string dbHost = "198.38.82.92";
            //string dbUser = "mannyhsu_team";
            //string dbPass = "foodiecare";
            //string dbName = "mannyhsu";


            // 如果有特殊的編碼在database後面請加上;CharSet=編碼, utf8請使用utf8_general_ci 
            string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
            conn = new MySqlConnection(connStr);

            // 連線到資料庫 

            string SQL2 = "SELECT Distinct c3 from food_type";
            //Response.Write(SQL);
            cmd = new MySqlCommand(SQL2, conn);
            conn.Open();


            myData = cmd.ExecuteReader();

            if (!myData.HasRows)
            {
                // 如果沒有資料,顯示沒有資料的訊息 
                Console.WriteLine("No data.");
            }
            else
            {
                // 讀取資料並且顯示出來 
                while (myData.Read())
                {
                    if (myData.GetString(0) != "")
                    {
                        DropDownList3.Items.Add(myData.GetString(0));
                    }
                }
                myData.Close();
            }
            conn.Close();
            DropDownList3.SelectedValue = "";
            DropDownList4.SelectedValue = "";
        }
        else
        {
            drop_type.Visible = false;
            if (type_v == 1)
            {
               
                int a = SQL.IndexOf("`c3`");
                int b = 0;

                if (SQL.Contains("`平均價格` <") || SQL.Contains("`平均價格` >"))
                {
                    if (SQL.Contains("`平均價格` <"))
                    {
                        b = SQL.IndexOf("`平均價格` <");

                    }
                    else
                    {
                        b = SQL.IndexOf("`平均價格` >");

                    }
                }
                else
                {
                    if (SQL.Contains("HAVING"))
                    {
                        b = SQL.IndexOf("HAVING");

                    }
                    else
                    {
                        b = SQL.IndexOf("ORDER");

                    }
                }
                int k = b - a;
                SQL = SQL.Remove(a, k);
                if (SQL.Contains("`平均價格` <") == false && SQL.Contains("`平均價格` >") == false)
                {
                    SQL = SQL.Replace("WHERE", "");
                }

                type_v = 0;
                data();
            }
            DropDownList4.Items.Clear();
        }
    }
    protected void SQL_price(int k )
    {
        k = k - 1;
        if (SQL.Contains("`平均價格` <") || SQL.Contains("`平均價格` >"))
        {
            int a =0;
            if (SQL.Contains("`平均價格` <"))
            {
                a = SQL.LastIndexOf("`平均價格` <");
            }
            else if (SQL.Contains("`平均價格` >"))
            {
                a = SQL.LastIndexOf("`平均價格` >");
            }
            //Response.Write(a);
            int j = k - a + 1;
            //Response.Write("a" + a + "b" + b + "k" + k);
            SQL = SQL.Remove(a, j);
            k = k - j;
            if (DropDownList2.SelectedValue == "1")
            {
                SQL = SQL.Insert(k, " `平均價格` < 100 ");
            }
            else if (DropDownList2.SelectedValue == "2")
            {
                SQL = SQL.Insert(k, " `平均價格` < 199 AND `平均價格` > 100 ");
            }
            else if (DropDownList2.SelectedValue == "3")
            {
                SQL = SQL.Insert(k, " `平均價格` < 299 AND `平均價格` > 200 ");
            }
            else if (DropDownList2.SelectedValue == "4")
            {
                SQL = SQL.Insert(k, " `平均價格` < 399 AND `平均價格` > 300 ");
            }
            else if (DropDownList2.SelectedValue == "5")
            {
                SQL = SQL.Insert(k, " `平均價格` < 499 AND `平均價格` > 400 ");
            }
            else if (DropDownList2.SelectedValue == "6")
            {
                SQL = SQL.Insert(k, " `平均價格` > 500 ");
            }
        }
        else
        {
            if (SQL.Contains("WHERE"))
            {
                if (DropDownList2.SelectedValue == "1")
                {
                    SQL = SQL.Insert(k, " AND `平均價格` < 100 ");
                }
                else if (DropDownList2.SelectedValue == "2")
                {
                    SQL = SQL.Insert(k, " AND `平均價格` < 199 AND `平均價格` > 100 ");
                }
                else if (DropDownList2.SelectedValue == "3")
                {
                    SQL = SQL.Insert(k, " AND `平均價格` < 299 AND `平均價格` > 200 ");
                }
                else if (DropDownList2.SelectedValue == "4")
                {
                    SQL = SQL.Insert(k, " AND `平均價格` < 399 AND `平均價格` > 300 ");
                }
                else if (DropDownList2.SelectedValue == "5")
                {
                    SQL = SQL.Insert(k, " AND `平均價格` < 499 AND `平均價格` > 400 ");
                }
                else if (DropDownList2.SelectedValue == "6")
                {
                    SQL = SQL.Insert(k, " AND `平均價格` > 500 ");
                }
            }
            else
            {
                if (DropDownList2.SelectedValue == "1")
                {
                    SQL = SQL.Insert(k, " WHERE `平均價格` < 100 ");
                }
                else if (DropDownList2.SelectedValue == "2")
                {
                    SQL = SQL.Insert(k, " WHERE `平均價格` < 199 AND `平均價格` > 100 ");
                }
                else if (DropDownList2.SelectedValue == "3")
                {
                    SQL = SQL.Insert(k, " WHERE `平均價格` < 299 AND `平均價格` > 200 ");
                }
                else if (DropDownList2.SelectedValue == "4")
                {
                    SQL = SQL.Insert(k, " WHERE `平均價格` < 399 AND `平均價格` > 300 ");
                }
                else if (DropDownList2.SelectedValue == "5")
                {
                    SQL = SQL.Insert(k, " WHERE `平均價格` < 499 AND `平均價格` > 400 ");
                }
                else if (DropDownList2.SelectedValue == "6")
                {
                    SQL = SQL.Insert(k, " WHERE `平均價格` > 500 ");
                }
            }
        }
        //Response.Write(SQL);
        //Response.Write(SQL.LastIndexOf("`平均價格`"));
    }


    protected void DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(DropDownList4.SelectedValue!="")
        {
            if (SQL.Contains("`c4`"))
            {
                int a = SQL.IndexOf("AND `c4`");
                int b = 0;
                String i = "";
                if (DropDownList2.SelectedValue!="")
                {
                    if (DropDownList2.SelectedValue!="6")
                    {
                        b = SQL.LastIndexOf("AND `平均價格` <");
                        i = "AND `平均價格` <";
                    }
                    else
                    {
                        b = SQL.LastIndexOf("AND `平均價格` >");
                        i = "AND `平均價格` >";
                    }
                }
                else
                {
                    if (SQL.Contains("HAVING"))
                    {
                        b = SQL.LastIndexOf("HAVING");
                        i = "HAVING";
                    }
                    else
                    {
                        b = SQL.LastIndexOf("ORDER");
                        i = "ORDER";
                    }
                }
                int k = b - a;
                //Response.Write(b);
                //Response.Write(i+":");
                //Response.Write(SQL);
                SQL = SQL.Remove(a, k);
                b = SQL.IndexOf(i);
                SQL = SQL.Insert(b, " AND `c4` = '" + DropDownList4.SelectedValue + "' ");
            }
            else
            {
                if (SQL.Contains("`平均價格` <") || SQL.Contains("`平均價格` >"))
                {
                    int a = 0;
                    if (SQL.Contains("AND `平均價格` <"))
                    {
                        a = SQL.IndexOf("AND `平均價格` <");
                    }
                    else
                    {
                        a = SQL.IndexOf("AND `平均價格` >");
                    }
                    //Response.Write(a);
                    SQL = SQL.Insert(a, " AND `c4` = '" + DropDownList4.SelectedValue + "' ");
                }
                else
                {
                    int a = 0;
                    if (SQL.Contains("HAVING"))
                    {
                        a = SQL.IndexOf("HAVING");
                    }
                    else
                    {
                        a = SQL.IndexOf("ORDER");
                    }
                    SQL = SQL.Insert(a, " AND `c4` = '" + DropDownList4.SelectedValue + "' ");
                }
            }
        }
        else
        {
            int a = SQL.IndexOf("AND `c4`");
            int b = 0;
            
            if (SQL.Contains("`平均價格` <") || SQL.Contains("`平均價格` >"))
            {
                if (SQL.Contains("AND `平均價格` <"))
                {
                    b = SQL.IndexOf("AND `平均價格` <");
                    
                }
                else
                {
                    b = SQL.IndexOf("AND `平均價格` >");
                    
                }
            }
            else
            {
                if (SQL.Contains("HAVING"))
                {
                    b = SQL.IndexOf("HAVING");
                    
                }
                else
                {
                    b = SQL.IndexOf("ORDER");
                    
                }
            }
            int k = b - a;
            SQL = SQL.Remove(a, k);
        }
        data();
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
                        Label3.Text = Convert.ToString(Convert.ToDouble(reader[4])/10)+"/5分";
                    }
                    else
                    {
                        Label3.Text = "-";
                    }
                    if (Convert.ToString(reader[5]) != "0")
                    {
                        Label4.Text = Convert.ToString(reader[5])+"元";
                    }
                    else
                    {
                        Label4.Text = "-";
                    }
                    Label5.Text = Convert.ToString(reader[8]);
                    Label6.Text = Convert.ToString(reader[9]);
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

    protected void Button_detail_Click(object sender, EventArgs e)
    {
        check_box.Attributes["style"] = "display:block";
        detail.Attributes["style"] = "display:none";
        Button5.Visible = true;
        Button_detail.Visible = false;
        GridView1.Visible = true;
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
    protected void GridView1_PageIndexChanged(object sender, EventArgs e)
    {

    }

    protected void GridView1_DataBound(object sender, EventArgs e)
    {

    }
}











    