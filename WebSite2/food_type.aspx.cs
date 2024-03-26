using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

public partial class food_type : System.Web.UI.Page
{
    static MySqlConnection conn;
    static MySqlCommand cmd;
    static MySqlDataReader myData;
    public void Page_Load(object sender, EventArgs e)
    {
        DropDownList2.Visible = false;
        if (IsPostBack == false)
        {
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

            string SQL = "SELECT Distinct c3 from food_type";
            //Response.Write(SQL);
            cmd = new MySqlCommand(SQL, conn);
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
                    DropDownList1.Items.Add(myData.GetString(0));

                }
                myData.Close();
            }
            
        }
    }
    protected void index_change(object sender, EventArgs e)
    {
        DropDownList2.Visible = true;
        DropDownList2.Items.Clear();
      

        // 連線到資料庫 

        string SQL = "SELECT `c4` FROM `food_type` WHERE `c3`='"+DropDownList1.SelectedValue+"'";
       // Response.Write(SQL);
        cmd = new MySqlCommand(SQL, conn);
     


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
                DropDownList2.Items.Add(myData.GetString(0));

            }
            myData.Close();
        }

    }
}