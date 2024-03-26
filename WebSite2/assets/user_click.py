import pymysql
import numpy as np

try:
    db = pymysql.connect(host="localhost",user="foodieteam",password="foodiecare",database="foodiecare",charset="utf8")
    #print("sucess")
except Exception as a:
    print("fail",a)
cursor = db.cursor()
#print("sucess")

name_arr=[]
count_arr = []
def database(sql_ex):       
        #print(sql_ex)
        data= ""
        try:
           cursor.execute(sql_ex)
           data = cursor.fetchall()
           
        except Exception as e  :
                e=str(e)
                print(e)
                print(sql_ex)
        db.commit()
        return data

def user(name):
    sql = "SELECT ID FROM user_click WHERE 商家名稱='"+name+"'"
    data = database(str(sql))
    #print(data)
    for i in range(len(data)):
        sql2 = "SELECT * FROM user_click WHERE ID='"+str(data[i][0])+"'"
        result = database(str(sql2))
        for j in range(len(result)):
            if(result[j][1]!=name):
                if(result[j][1] in name_arr):
                    k = name_arr.index(result[j][1])
                    count_arr[k] = count_arr[k]+result[j][2]
                else:
                    name_arr.append(result[j][1])
                    count_arr.append(result[j][2])
    final_result=[]
    for i in range(5):
        k = count_arr.index(max(count_arr))
        if(name_arr[k] in final_result):
            pass
        else:
            final_result.append(name_arr[k])
            count_arr[k] = 1
    return final_result
class Demo:
    def __init__(self):
        self.text = []
    def getText(self,name):
        
        self.text = user(name)
        return self.text
f = open(str(name)+'_click_recommend.txt', 'w', encoding = 'UTF-8')    # 也可使用指定路徑等方式，如： C:\A.txt
f.close()
db.close()
