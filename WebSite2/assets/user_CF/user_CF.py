


def get_user_value(user_data,i,N_a,X_a):
    N_b = user_N(user_data,i)
    #print(N_b)
    X_b = user_X(user_data,N_b,i)
    #print(X_b)
    value = count(N_a,N_b,X_a,X_b)
    return(value)
    
def user_N(user_data,i):
    N_b = 0
    for j in range(len(user_data[i])):
        if j > 0:
            N_b += user_data[i][j]
    return N_b

def user_X(user_data,N_b,i):
    for k in range(len(user_data[i])):
        X.append(0)
    for j in range(len(user_data[i])):
        if j > 0:
            X[j]=(user_data[i][j])
    #print(X)
    return X

def count(N_a,N_b,X_a,X_b):
    U = 0
    D_a = 0
    D_b = 0
    for i in range(len(X_a)):
        if i > 0:
            U += X_a[i]*X_b[i]
            D_a += X_a[i]**2
            D_b += X_b[i]**2
    D = (D_a*D_b)**0.5
    return U/D
class Demo:
    def __init__(self):
        self.text = ""
        self.CF_user = []
        self.CF_value = []
        self.result_name = []
        self.result_ID = []
    def user(self,ID,user_data):
        N_a = 0
        for i in range(len(user_data[ID])):
            X_a.append(0)
        for j in range(len(user_data[ID])):
            if j > 0:
                N_a += user_data[ID][j]
                X_a[j] = (user_data[ID][j])

        #print(N_a)
        for i in range(len(user_data)):
            value = 0
            result=""
            if(i!=ID):
                value = get_user_value(user_data,i,N_a,X_a)
            if value > 0.5:
                if k < 5:
                    self.CF_user.append(i+1)
                    self.CF_user.append(value)
                    result += str(i+1)+":"+str(value)
        return result
                
        
