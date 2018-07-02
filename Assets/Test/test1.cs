using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class test1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(test.getInstance().plus());
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene("2");
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneManager.LoadScene("1");
        }
    }
}
