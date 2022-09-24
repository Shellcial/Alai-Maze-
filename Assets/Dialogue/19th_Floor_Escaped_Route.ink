VAR specialEvent = "SeeKnight"
VAR damage = 10
->main
===main
#Vampire_Knight
……
#Player
（是之前上層的骷髏騎士）
#Vampire_Knight
又是你嗎？
#Player
！？
#Vampire_Knight
這次你不再偷偷摸摸了嗎。
#Player
（是指之前上層的時候嗎？她當時已經發現了我？）
你究竟是誰？
你是魔物？還是說你是人類？
#Vampire_Knight #Pause_Dialogue
不要把我和那些低等種族相提並論。

//對方突然攻擊

#Narrator
（HP -0）
#Player
（似乎以我現在的實力可以輕鬆戰勝她）
（……）
（留她一命，試試從她身上問多點情報吧）
（畢竟如果她說的是真，又不是魔物又不是人類，那會是甚麼呢？）
#Pause_Dialogue
（是甚麼稀有的魔物嗎？真令人在意。）

//一輪斬擊
//對方被打到側邊

#Narrator
（拿塔將對方擊飛，對方的頭盔掉落至地上）
#Player
（！？）
（銀色頭髮，加上鮮紅色的眼睛……難道是——）

#Unknown #Pause_Dialogue
艾！！！
//吸血鬼公主在樓梯口淡入
//吸血鬼衝上來攻擊主角

#Narrator #Sub500HP
（HP -500）
#Player
（有翼的人型魔物！？果然，她們是吸血鬼。）
#Vampire_Girl
讓我來做你的對手！
#Vampire_Knight_True_Name
公主殿下！你還未掌握握如何使用魔力，這兒就交給我，你快點逃——
#Vampire_Princess
不要！我不會丟下艾逃走的！
#Vampire_Princess
一直以來都是艾你保護我，這次輪到我來守護你！
#Vampire_Knight_True_Name #Pause_Dialogue
不行！公主殿下，你還控制不到那份力量！

//公主身旁有魔素出現
//玩家轉身看周圍

#Player 
（很高濃度的魔力，這就是吸血鬼的力量嗎……）
（傳說他們是一種擁有高等智慧，模仿人類生活的魔物。）
（但他們應該存在於魔族中自己的部落內，不會出現在這人造迷宮……）
（……）
（攻擊力……防禦力……）
（換算成數字的話大概是300和50，生命值的話……）
#Pause_Dialogue
（只能打打看才知道了）

//決定輸贏

->DONE
===Win
#Player
哈……哈……贏了。
#Vampire_Knight_True_Name
公主殿下！
#Player #Pause_Dialogue
放心，她應該只是暈了。（畢竟在我斬中她要害之前她便自己暈倒了）

//遠處傳來腳步聲

#Player
（上層傳來腳步聲？）
#Pause_Dialogue
（如果被其他人看到的話，她們應該會被殺掉吧）
你們是從下層來的吧？
#Vampire_Knight_True_Name
……
#Player
這層沒有任何藏身處，下層的話……只要藏到樓層最角落的地方，應該就不會被其他人發現。
#Vampire_Knight_True_Name
……
#Player
……
#End #FinalWin
總之先往下層走吧。

->DONE
===Lose
#Narrator
（HP -{damage}）
#Player #End #LastLose
……

->END