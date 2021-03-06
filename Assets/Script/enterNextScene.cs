﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class enterNextScene : MonoBehaviour {

    public string NextSceneName;
    public int BornPositionNum;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            TheSceneManager.getInstance().enterNextScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, NextSceneName, BornPositionNum);
            SceneManager.LoadScene(NextSceneName);
        }
    }
}
