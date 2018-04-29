using UnityEngine;
using System.Collections;
using UnityEditor.SceneManagement;

public class enterNextScene : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            EditorSceneManager.LoadScene("Test");
        }
    }
}
