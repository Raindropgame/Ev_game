using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
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

    void setFalse()
    {
        this.gameObject.SetActive(false);
        scale = 0;
    }
}
