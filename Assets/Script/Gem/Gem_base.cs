using UnityEngine;
using System.Collections;

public class Gem_base : MonoBehaviour {

    //结晶基类

    public int index;  //索引
    public Sprite sprite;


    private void Start()
    {


        OnStart();
    }

    virtual public void OnStart()
    {

    }

}
