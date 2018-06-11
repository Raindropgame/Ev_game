using UnityEngine;
using UnityEditor;
using System.Collections;

public class ResetFile : EditorWindow{

    //存档拓展编辑器

    [MenuItem("MyTools/ResetFileWindow")]
    static void CreateResetFileWindow()
    {
        ResetFile window = (ResetFile)EditorWindow.GetWindow(typeof(ResetFile),  false, "ResetFile");
        window.Show();
    }

    

    //人物属性
    bool[] isEnable = new bool[13];
    int MaxBreath = 0;
    int MaxHP = 0;
    int expend_run = 5;
    int expend_attack = 8;
    int expend_jump = 20;
    int expend_dash = 10;
    int expend_shoot = 10;
    int expend_jumpshoot = 15;
    int expend_throw = 10;
    int Speed_recovery = 30;
    int JumpTimes = 1;
    int MaxJumpShootTimes = 1;

    //场景
    string currentScene = "test";
    int BornPositionNum = 0;

    //天气
    float currentTime = 0;
    weather currentWeather;
    float Weather_duration, Weather_leftTime;
    float Rain_Odds = 0.2f, Thunder_Odds = 0.1f, RainAndThunder_Odds = 0.1f;   //各种天气的概率

    private void OnGUI()
    {

        //人物属性
        EditorGUILayout.LabelField("-- 人物属性 --");
        MaxBreath = EditorGUILayout.IntField("MaxBreath :", MaxBreath,GUILayout.Width(250));
        MaxHP = EditorGUILayout.IntField("MaxHP :", MaxHP, GUILayout.Width(250));
        expend_run = EditorGUILayout.IntField("expend_run :", expend_run, GUILayout.Width(250));
        expend_attack = EditorGUILayout.IntField("expend_attack :", expend_attack, GUILayout.Width(250));
        expend_jump = EditorGUILayout.IntField("expend_jump :", expend_jump, GUILayout.Width(250));
        expend_dash = EditorGUILayout.IntField("expend_dash :", expend_dash, GUILayout.Width(250));
        expend_shoot = EditorGUILayout.IntField("expend_shoot :", expend_shoot, GUILayout.Width(250));
        expend_jumpshoot = EditorGUILayout.IntField("expend_jumpshoot :", expend_jumpshoot, GUILayout.Width(250));
        expend_jumpshoot = EditorGUILayout.IntField("expend_throw :", expend_throw, GUILayout.Width(250));
        Speed_recovery = EditorGUILayout.IntField("Speed_recovery :", Speed_recovery, GUILayout.Width(250));
        JumpTimes = EditorGUILayout.IntField("jumpTimes :", JumpTimes, GUILayout.Width(250));
        MaxJumpShootTimes = EditorGUILayout.IntField("MaxJumpShootTimes :", MaxJumpShootTimes, GUILayout.Width(250));
        for (int i = 0;i< isEnable.Length;i++)
        {
            isEnable[i] = EditorGUILayout.Toggle(((state)i).ToString(), isEnable[i]);
            
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //------------

        //场景
        EditorGUILayout.LabelField("-- 场景管理 --");
        currentScene = EditorGUILayout.TextField("currentScene", currentScene);
        BornPositionNum = EditorGUILayout.IntField("BornPositionNum", BornPositionNum);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //-------------

        //天气
        EditorGUILayout.LabelField("-- 天气数据 --");
        currentTime = EditorGUILayout.FloatField("currentTime", currentTime);
        EditorGUILayout.LabelField("currentWeather : " + currentWeather.ToString());
        Weather_duration = EditorGUILayout.FloatField("Weather_duration", Weather_duration);
        Weather_leftTime = EditorGUILayout.FloatField("Weather_leftTime", Weather_leftTime);
        Rain_Odds = EditorGUILayout.FloatField("Rain_Odds", Rain_Odds);
        Thunder_Odds = EditorGUILayout.FloatField("Thunder_Odds", Thunder_Odds);
        RainAndThunder_Odds = EditorGUILayout.FloatField("RainAndThunder_Odds", RainAndThunder_Odds);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //-------------

        if (GUILayout.Button("重置存档",GUILayout.Width(200),GUILayout.Height(50)))
        {
            resetFile();
        }
        if (GUILayout.Button("设置存档", GUILayout.Width(200), GUILayout.Height(50)))
        {
            setFile();
        }
        if (GUILayout.Button("获取存档", GUILayout.Width(200), GUILayout.Height(50)))
        {
            getFile();
        }
        if (GUILayout.Button("删除存档", GUILayout.Width(200), GUILayout.Height(50)))
        {
            deleteFile();
        }
    }

    public void resetFile()  //重置存档(默认值)
    {
        //人物属性
        PlayerPrefs.SetInt("MaxBreath", 50);
        PlayerPrefs.SetInt("MaxHP", 3);
        PlayerPrefs.SetInt("expend_run", 5);
        PlayerPrefs.SetInt("expend_attack", 8);
        PlayerPrefs.SetInt("expend_jump", 20);
        PlayerPrefs.SetInt("expend_dash", 10);
        PlayerPrefs.SetInt("expend_shoot", 10);
        PlayerPrefs.SetInt("expend_jumpshoot", 15);
        PlayerPrefs.SetInt("expend_throw", 10);
        PlayerPrefs.SetInt("Speed_recovery", 30);
        PlayerPrefs.SetInt("JumpTimes", 1);
        PlayerPrefs.SetInt("MaxJumpShootTimes", 1);
        bool[] t_isEnable = { true, true, true, true, true, true, true, true, true, true, true, true,true };
        for (int i = 0; i < isEnable.Length; i++)
        {
            PlayerPrefs.SetInt(((state)i).ToString(), t_isEnable[i] == true ? 1 : 0);  //true:1  false:0
        }
        //---------

        //场景
        PlayerPrefs.SetString("currentScene", "test");
        PlayerPrefs.SetInt("BornPositionNum", 0);
        //---------

        //天气
        PlayerPrefs.SetFloat("currentTime", 0);
        PlayerPrefs.SetFloat("Weather_duration", 5);
        PlayerPrefs.SetFloat("Weather_leftTime", 5);
        PlayerPrefs.SetFloat("Rain_Odds", 0.2f);
        PlayerPrefs.SetFloat("Thunder_Odds", 0.1f);
        PlayerPrefs.SetFloat("RainAndThunder_Odds", 0.1f);
        //-------
    }

    public void setFile()   //存档设置为规定值
    {
        //人物属性
        PlayerPrefs.SetInt("MaxBreath", MaxBreath);
        PlayerPrefs.SetInt("MaxHP", MaxHP);
        PlayerPrefs.SetInt("expend_run", expend_run);
        PlayerPrefs.SetInt("expend_attack", expend_attack);
        PlayerPrefs.SetInt("expend_jump", expend_jump);
        PlayerPrefs.SetInt("expend_dash", expend_dash);
        PlayerPrefs.SetInt("expend_shoot", expend_shoot);
        PlayerPrefs.SetInt("expend_jumpshoot", expend_jumpshoot);
        PlayerPrefs.SetInt("expend_throw", expend_throw);
        PlayerPrefs.SetInt("Speed_recovery", Speed_recovery);
        PlayerPrefs.SetInt("JumpTimes", JumpTimes);
        PlayerPrefs.SetInt("MaxJumpShootTimes", MaxJumpShootTimes);
        for (int i = 0;i<isEnable.Length; i++)
        {
            PlayerPrefs.SetInt(((state)i).ToString(), isEnable[i] == true ? 1 : 0);  //true:1  false:0
        }
        //---------

        //场景
        PlayerPrefs.SetString("currentScene", currentScene);
        PlayerPrefs.SetInt("BornPositionNum", BornPositionNum);
        //---------

        //天气
        PlayerPrefs.SetFloat("currentTime", currentTime);
        PlayerPrefs.SetFloat("Weather_duration", Weather_duration);
        PlayerPrefs.SetFloat("Weather_leftTime", Weather_leftTime);
        PlayerPrefs.SetFloat("Rain_Odds", Rain_Odds);
        PlayerPrefs.SetFloat("Thunder_Odds", Thunder_Odds);
        PlayerPrefs.SetFloat("RainAndThunder_Odds", RainAndThunder_Odds);
        //-------

    }

    public void getFile()   //获取存档数据
    {
        //人物属性
        MaxBreath = PlayerPrefs.GetInt("MaxBreath", MaxBreath);
        MaxHP = PlayerPrefs.GetInt("MaxHP", MaxHP);
        expend_run = PlayerPrefs.GetInt("expend_run", expend_run);
        expend_attack = PlayerPrefs.GetInt("expend_attack", expend_attack);
        expend_jump = PlayerPrefs.GetInt("expend_jump", expend_jump);
        expend_dash = PlayerPrefs.GetInt("expend_dash", expend_dash);
        expend_shoot = PlayerPrefs.GetInt("expend_shoot", expend_shoot);
        expend_jumpshoot = PlayerPrefs.GetInt("expend_jumpshoot", expend_jumpshoot);
        expend_jumpshoot = PlayerPrefs.GetInt("expend_throw", expend_throw);
        Speed_recovery = PlayerPrefs.GetInt("Speed_recovery", Speed_recovery);
        JumpTimes = PlayerPrefs.GetInt("JumpTimes", JumpTimes);
        MaxJumpShootTimes = PlayerPrefs.GetInt("MaxJumpShootTimes", MaxJumpShootTimes);
        for (int i = 0; i < isEnable.Length; i++)
        {
            isEnable[i] = PlayerPrefs.GetInt(((state)i).ToString(), isEnable[i] == true ? 1 : 0) == 1 ? true : false;  //true:1  false:0
        }
        //---------

        //场景
        currentScene = PlayerPrefs.GetString("currentScene", currentScene);
        BornPositionNum = PlayerPrefs.GetInt("BronPositionNum", BornPositionNum);
        //---------

        //天气
        currentTime =  PlayerPrefs.GetFloat("currentTime", 0);
        currentWeather = (weather)PlayerPrefs.GetInt("currentWeather");
        Weather_duration = PlayerPrefs.GetFloat("Weather_duration", 5);
        Weather_leftTime = PlayerPrefs.GetFloat("Weather_leftTime", 5);
        Rain_Odds = PlayerPrefs.GetFloat("Rain_Odds");
        Thunder_Odds = PlayerPrefs.GetFloat("Thunder_Odds");
        RainAndThunder_Odds = PlayerPrefs.GetFloat("RainAndThunder_Odds");
        //-------
    }

    public void deleteFile()
    {
        PlayerPrefs.DeleteAll();
    }
}
