class Demo:
    def __init__(self):
        self.result = []
    def user(self,user_data,CF_user,CF_value):
        for i in range(5):
          CF = CF_value.index(max((CF_value)))
          if(CF_value[CF]==0):
                    break
          #print(CF)
          #print(CF_value[CF])
          CF_value[CF] = 0
          sql = "SELECT * FROM food_question_form_test WHERE ID='"+str(CF_user[CF])+"'"
          print(sql)
          data = database(str(sql))
          if(len(data)>0):
                    for j in range(len(data)):
                           result.append(data[j][0])
                           result.append(data[j][10])
        
