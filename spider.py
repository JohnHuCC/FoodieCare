
import requests
import threading
from bs4 import BeautifulSoup
import pymysql
#import MySQLdb
url_all=[]
target=['商家名稱','電話']
target2="【已歇業】"
target3="【已搬遷】"
try:
                db = pymysql.connect(host="localhost",user="root",password="sarah0612",database="food",charset="utf8")#,port=1234
                
except Exception as a:
                print("fail",a)
cursor = db.cursor()
 
f = open('D://AppServ_Back//www//error.txt', 'w', encoding = 'UTF-8')
        
def database(sql_ex):
        # 打开数据库连接
       
       
        #db.set_character_set('utf8')
       
        # 使用cursor()方法获取操作游标 
       
        #sq=encode(sq,'utf-8')
        
        #print(sql_ex)
        try:
           # 执行sql语句
           cursor.execute(sql_ex)
           # 执行sql语句
           
        except Exception as e  :
                e=str(e)
                print(e)
                print(sql_ex)
                if(e[1:5]!="1062"):
                    f.write(str(e))
                    f.write("\r\n\r\n")
                    f.write(str(sql_ex))
                    f.write("\r\n\r\n")
                    f.write("\r\n\r\n")
        db.commit()
        

        
 
        
         
        





        
def link(t1,t2):
        for x in range(t1,t2+1):#(1,6)
                #url="http://www.ipeen.com.tw/search/all/000/0-100-0-0/%E9%AB%98%E9%9B%84%E7%BE%8E%E9%A3%9F/?p="+str(x)+"&adkw=%E9%AB%98%E9%9B%84"
                #url2="http://www.ipeen.com.tw/search/all/000/1-0-0-0/?p="+str(x)+"&adkw=%E9%AB%98%E9%9B%84"
                url3="http://www.ipeen.com.tw/search/all/000/1-0-0-0/?p="+str(x)+"&adkw=%E9%AB%98%E9%9B%84"
                res=requests.get(url3)
                
                star="*"*20
                res.encoding = 'utf-8'
                soup=BeautifulSoup(res.text,"html.parser")
                
                a=soup.select(".serShop")
                
                print("start page",x)
                for x in range (1,16):
                        status = ""
                        print(x)
                        if(a[x].find("span",{'class':"status"})):
                                status = a[x].find("span",{'class':"status"})
                                
                        
                        #print(a[x])

                        if((target2   in status)):
                                pass
                        elif ((target3  in status)):
                                pass
                                
                        else:
                                
                               
                                try:
                                              b=a[x].find("a")
                                              link="http://www.ipeen.com.tw"+b['href']
                                              #print(link)
                                              url_all.append(link)
                                              get_td(link)
                                              
                                              
                                except:
                                             print(star,"\n no  link\n",star)
                             
                
                
                #print("\n\n\n",star)

def get_td(Url):
        Res=requests.get(Url)
        Res.encoding = 'utf-8'
        lat_sql='\'\''
        lng_sql='\'\''
        rate_sql='\'\''
        dollar=''
        a=''
        b=''
        c=''
        d=''
        Soup=BeautifulSoup(Res.text,"html.parser")
        find_div=Soup.find("div",{'id':"shop-details"})#tr
        try:
                p=Soup.find("p",{"class":"cost i"}).text.strip()
                cc=p.find("元")
                dollar=p[6:cc]
              
                #print(dollar)
                if(dollar=="暫無資"):
                        dollar=''
                
                #print(dollar)
        except:
                pass
        
        try:
                #print("評分:",Soup.find("span", itemprop="ratingValue").text)#<span itemprop="ratingValue">45</span>評分
                rate_sql="\'"+Soup.find("span", itemprop="ratingValue").text+"\'"
        except AttributeError :
                                        print("無評分")
        try:
                lat=Soup.find("meta", property="place:location:latitude")['content']
                lng=Soup.find("meta", property="place:location:longitude")['content']
                lat_sql="\'"+lat+"\'"
                lng_sql="\'"+lng+"\'"
               
        except AttributeError :
                                        print("no lat")

        try:
             image=Soup.find("img", property="image")['src']
             print (image);
               
        except AttributeError :
                                        print("no pic")

                                        
        find_tr= find_div.find_all("tr")
        #find_td= find_div.find_all("td")
        star="*"*20
        str1=""
        str2=""
        for x in range(7):
                try:
                        if((find_tr[x].th.text)in target):
                                     str1=str1+"`"+(find_tr[x].th.text)+"`"+","
                                     str2=str2+"\""+find_tr[x].td.text.strip().replace(' ', '')+"\""+","
                                     #print(find_tr[x].th.text,":",find_tr[x].td.text.strip().replace(' ', ''))
                                     
                                     

                        elif("商家分類"==str(find_tr[x].th.text) ):
                                a,b,c,d=(find_tr[x].td.text.strip()).replace("\n","").split(">")
                                    
                                     
                                
                                   
                except (AttributeError ,IndexError) :
                        pass
                
                      #.text.strip().replace(' ', ''))

        str1=str1[:(len(str1)-1)]
        str2=str2[:(len(str2)-1)]
        sql=""
        sql="INSERT INTO `foodiecare` (`lat`,`lng`,`point`,`c1`,`c2`,`c3`,`c4`,`平均價格`,"+str1+") VALUES("+lat_sql+","+lng_sql+","+rate_sql+",'"+a+"','"+b+"','"+c+"','"+d+"','"+dollar+"',"+str2+")"
        #print(sql)
        database(str(sql))
       
                        
     
        
        







    




       

added_thread1 = threading.Thread(target=link(1,10))
db.close()
f.close()
# 執行 thread

"""
added_thread2= threading.Thread(target=link(2))
# 執行 thread

added_thread3= threading.Thread(target=link(3))
# 執行 thread

added_thread4= threading.Thread(target=link(4))
# 執行 thread"""

added_thread1.start()

"""added_thread2.start()
added_thread3.start()
added_thread4.start()"""
