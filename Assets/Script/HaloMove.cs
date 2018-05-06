using UnityEngine;
using System.Collections;

public class HaloMove : MonoBehaviour {

    //  环形物运动

    public float speed = 0.3f;
    public Material material;
	

	void Update () {
        material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
        if(material.mainTextureOffset.x >= 1000)
        {
            material.mainTextureOffset = Vector2.zero;
        }
	}
}
