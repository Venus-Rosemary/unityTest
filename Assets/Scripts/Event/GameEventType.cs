using UnityEngine;

public enum GameEventType
{
    GameStart,                  //开始游戏的初始化
    
    FirstGameStart,             //第一关事件
    SecondGameStart,            //第二关事件
    ThirdGameStart,             //第三关事件

    GameOver,
    GamePause,
    GameResume,
} 
