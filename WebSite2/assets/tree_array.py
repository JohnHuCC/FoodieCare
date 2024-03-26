
# This Python file uses the following encoding: utf-8
labels = ['hunger','hot_cold','eat_mode','distance','price']
price_a = ['100_less','100_199','200_299','300_399','400_499','500_more']
eat_mode_a = ['alone','friend','couple','family']
hunger_a = ['eat_much','eat_less']
distance_a = ['1km_less','1to5km','5to10km','10km_more']
hot_cold_a = ['hot','cold']
temp = []
tree = []
tree_dic = {}


''':
def split_dataset(dataset,feat_idx):
        根据某个特征以及特征值划分数据集
        :param dataset: 待划分的数据集, 有数据向量组成的列表.
        :param classes: 数据集对应的类型, 与数据集有相同的长度
        :param feat_idx: 特征在特征向量中的索引
        :param splited_dict: 保存分割后数据的字典 特征值: [子数据集, 子类型列表]
        
        splited_dict = {}
        #print(dataset[feat_idx][1:])
        splited_dict.setdefault(dataset[feat_idx][0],dataset[feat_idx][1:])
        return splited_dict
def create_tree(dataset, feat_names,feat_idx):
         根据当前数据集递归创建决策树
        :param dataset: 数据集
        :param feat_names: 数据集中数据相应的特征名称
        :param classes: 数据集中数据相应的类型
        :param tree: 以字典形式返回决策树
        
        # 分裂创建新的子树
        
        feature = feat_names[0]
        tree_dic[feature] = {}
        # 创建用于递归创建子树的子数据集
        sub_feat_names = feat_names[:]
        sub_feat_names.pop(0)

        splited_dict = split_dataset(dataset, feat_idx)
        print(splited_dict)
        for feat_val in splited_dict:
            print(splited_dict[feat_val])
            tree_dic[feature][splited_dict[feat_val][0]] = {}
            print(tree_dic)
        return tree_dic
'''
def find_type (treee,key,length):
        answer = "No answer"
        for i in range(length):
            for j in range(len(key)):
                if(key[j] in treee[i]):
                    #print(treee[i])
                    answer = str(treee[0][1])
                    if(len(treee[i])==2):
                        #print(treee[i])
                        answer = str(treee[i][1])
                        break
                    answer = find_type(treee[i],key,len(treee[i]))
            if(answer !="No answer"):
                break
        return answer
def append(s,t_t2,t2):
        if(s>0):
            s = s-1
            t2[-1] = append(s,t_t2,t2[-1])
            #print(t2)
            return(t2)
        else:
            #print(t2)
            #print(t_t2)
            k=t2
            k.append(t_t2)
            return k
with open('C://Users//user//source//repos//WebSite2//WebSite2//assets//food_tree_rule.txt', 'r') as f:
        for line in f:
            comps = line.strip().split()
            k = comps.index('=')
            comps[k+1] = comps[k+1].strip(':')
            temp.append(comps)
            #print(comps)
    #print(temp)
for i in range(len(temp)):
        state = 0
        try:
            state = temp[i].count('|')

        except:
            pass
        for j in range(len(labels)):
            try:
                k = temp[i].index(labels[j])
                #print(k)
                #print(temp[i][k])
                try:
                    if(state==0):
                        tree_temp=[(temp[i][k+2]),(temp[i][k+3])]
                        tree.append(tree_temp)
                    else:
                        tree_temp=[(temp[i][k+2]),(temp[i][k+3])]
                        tree=append(state,tree_temp,tree)
                except:
                    tree_temp=[(temp[i][k+2])]
                    if(state==0):
                        tree.append(tree_temp)
                    else:
                        tree=append(state,tree_temp,tree)
            except:
                pass
class Demo:
    def __init__(self):
        self.text = ""
    def getText(self,price,eat_mode,hunger,distance,hot_cold):
        key = []
        key.append(price)
        key.append(eat_mode)
        key.append(hunger)
        key.append(distance)
        key.append(hot_cold)
        self.text = find_type(tree,key,len(tree))
        if(self.text == "Breakfast(good)"):
                self.text="早餐"
        if(self.text == "LittleFood(good)"):
                self.text="小吃"
        if(self.text == "JapnFood(good)"):
                self.text="日式料理"
        if(self.text == "Barbecue(good)"):
                self.text="燒烤類"
        if(self.text == "KoreaFood(good)"):
                self.text="韓式料理"
        if(self.text == "Sweet(good)"):
                self.text="烘焙、甜點、零食"
        if(self.text.strip() == "Afternoon_tea(good)"):
                self.text="下午茶"
        if(self.text == "HotPot(good)"):
                self.text="鍋類"
        if(self.text == "buffet(good)"):
                self.text="buffet自助餐"
        if(self.text == "AsiaFood(good)"):
                self.text="亞洲料理"
        return self.text
'''
key = []
price = input("請輸入價格位於:1.100元以下 2.100~199 3. 200~299 4. 300~399 5. 400~499 6. 500以上\n->")
key.append(price_a[int(price)-1])
eat_mode = input("與誰用餐:1.自己 2.朋友 3. 情侶 4. 家人\n->")
key.append(eat_mode_a[int(eat_mode)-1])
hunger = input("吃多少:1.較多 2.較少\n->")
key.append(hunger_a[int(hunger)-1])
distance = input("願意走多少距離:1.1公里以內 2.1~5公里 3. 5~10公里 4. 10公里以上\n->")
key.append(distance_a[int(distance)-1])
hot_cold = input("吃冷食還是熱食:1.熱食 2.冷食\n->")
key.append(hot_cold_a[int(hot_cold)-1])
print(key)
answer = find_type(tree,key,len(tree))
print(answer)
'''
'''for i in range(len(tree)):
    tree_dic = create_tree(tree,tree[i],i)
print(tree_dic)''' 
