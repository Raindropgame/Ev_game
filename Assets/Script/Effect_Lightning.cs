using UnityEngine;
using System.Collections;

public class Effect_Lightning : MonoBehaviour {

    public float scale = 0;

    private SpriteRenderer SR;
    private MaterialPropertyBlock block;

	void Start () {
        SR = GetComponent<SpriteRenderer>();
        block = new MaterialPropertyBlock();
	}
	
	
	void Update () {
        block.SetFloat("_Scale", scale);
        SR.SetPropertyBlock(block);
	}

    void _destroy()
    {
        Destroy(this.gameObject);
    }
}
