VAR specialEvent = "SeeGirl"
VAR winLose = ""

->main
=== main
#Player
怎麼會有個小女孩在這？

#Little_Girl_Back
……
艾……你究竟去了哪？
#Pause_Dialogue
……

//轉身

#Little_Girl
是艾的血。

#Player #Pause_Dialogue
甚麼？

//吸血鬼上前

#Little_Girl
你的劍，為甚麼會有艾的血？

#Little_Girl #Pause_Dialogue
你對艾做了甚麼！

//黑粒子放出
//展示吸血鬼形象

#Player
（她是……吸血鬼？）
（想不到這迷宮會有這種稀有的魔物）
（……既然她能釋放這麼濃厚的魔力）
#Pause_Dialogue
（那我打敗她之後也能獲得大量的魔力吧）

//兩人各有幾劍揮動
//判定輸贏

->DONE

===Win
#Vampire_Girl
……
對不起……艾……
#Pause_Dialogue
如果我……可以再強一點的話……

//吸血鬼漸漸消失

#Player
贏了……
……
哈哈
這麼強大的魔力，不管是甚麼魔物我也能輕鬆殺掉吧。
回地上給大家看看我的力量吧。
這樣大家就不會看少我，
#End #WinGoBack
就不會再說羅姆斯家族的壞話了！
//直接在19樓回去

->DONE
===Lose
#Player #End #LastLose
……

->END