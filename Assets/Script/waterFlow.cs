using UnityEngine;
using System.Collections;

public class waterFlow : MonoBehaviour {
    
    //水的流动  一个场景只有一个

    public float speed = 1;

    public Material material; 

	// Update is called once per frame
	void Update () {
        material.mainTextureOffset += new Vector2(0, speed * Time.deltaTime);
        if(material.mainTextureOffset.y >= 1000)
        {
            material.mainTextureOffset = new Vector2(0, 0);
        }
	}
}
