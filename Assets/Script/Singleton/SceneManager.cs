using UnityEngine;
using System.Collections;

public class SceneManager{

    //用于临时储存一些场景信息

    static SceneManager instance;

    public string currentScene, nextScene;
    public int BornPositionNum = 0;

    static public SceneManager getInstance()
    {
        if(instance == null)
        {
            instance = new SceneManager();
            return instance;
        }
        return instance;
    }

    SceneManager()
    {
        Init();
    }

    public void enterNextScene(string _currentScene,string _nextScene,int _BornPositionNum)
    {
        currentScene = _currentScene;
        nextScene = _nextScene;
        BornPositionNum = _BornPositionNum;
    }

    void Init()  //初始化
    {
        //读取存档
        currentScene = PlayerPrefs.GetString("currentScene", currentScene);
        BornPositionNum = PlayerPrefs.GetInt("BornPositionNum", BornPositionNum);
    }

    public void setFile()  //存档
    {
        PlayerPrefs.SetString("currentScene", currentScene);
        PlayerPrefs.SetInt("BornPositionNum", BornPositionNum);
    }
}
