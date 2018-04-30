using UnityEngine;
using System.Collections;

public class SceneManager{

    //用于临时储存一些场景信息

    static SceneManager instance;

    public string currentScene, nextScene;
    public int BornPositionNum;

    static public SceneManager getInstance()
    {
        if(instance == null)
        {
            instance = new SceneManager();
            return instance;
        }
        return instance;
    }

    public void enterNextScene(string _currentScene,string _nextScene,int _BornPositionNum)
    {
        currentScene = _currentScene;
        nextScene = _nextScene;
        BornPositionNum = _BornPositionNum;
    }
}
