# 针对开发者的帮助文档（9月6日）

## 脚本简介

 **_注意，仅限 2.Script文件夹中的脚本，PureAmaya命名空间内的类不做介绍_** 

- [TitleCtrl](#TitleCtrl)

- [UICtrl](#UICtrl)

- [LoadingCtrl](#LoadingCtrl)

- [PlayerInfUpdate](#PlayerInfUpdate)

- [PausePlayerInf](#PausePlayerInf)

- [StageCtrl](#StageCtrl)

- [APlayerCtrl](#APlayerCtrl)

- [SayakaCtrl(#SayakaCtrl)

- [GameScoreSettingsIOl(#GameScoreSettingsIO)


### TitleCtrl

控制Title场景，并负责主标题part，魔女选择part，魔法少女选择part的控制

### UICtrl

控制Majo场景中的UI与游戏暂停处理，负责调用PlayerInfUpdate和PausePlayerInf，详细内容请阅览相应脚本简介

### LoadingCtrl

控制Loading文本，QB与进度条，并负责在场景之间转换

### PlayerInfUpdate

游戏中上板面显示的血量，灵魂值，分数等信息的更新器

### PausePlayerInf

暂停界面中玩家信息与图片的显示

### StageCtrl

托管调用玩家受伤事件，音量修改（仅限听觉，视觉上对UI的修改在UICtrl中），相机控制（未实装），敌人的激活与消除（未实装）

### APlayerCtrl

玩家用抽象类。负责玩家之间能够通用的逻辑处理（比如移动，动画，跳跃），每个玩家不同的部分（比如魔法，攻击）
由每个玩家各自的脚本进行重写

### SayakaCtrl

负责控制沙耶加，重写每个沙耶加不同于其他玩家的逻辑（比如魔法，攻击）

### GameScoreSettingsIO 

负责整个游戏大部分临时变量与设置的存储，输入与输出



