using UnityEngine;
using System.Collections;

public class test{

    public int num = 1;

    private static test instance;

    public static test getInstance()
    {
        if(instance == null)
        {
            instance = new test();
        }
        return instance;
    }

    public int plus()
    {
        num++;
        return num;
    }
	
}
