using UnityEngine;
using System.Collections;

public class LeafWave : MonoBehaviour {

    public float scale = 0;
    public AnimationCurve Curve;

    private MaterialPropertyBlock MBlock;
    private SpriteRenderer SR;
    private float _time0 = 0;

	void Start () {
        MBlock = new MaterialPropertyBlock();
        SR = GetComponent<SpriteRenderer>();
	}
	
	private void FixedUpdate()
    {
        _time0 += Time.deltaTime;
        MBlock.SetFloat("_Scale", scale * Curve.Evaluate(_time0));
        SR.SetPropertyBlock(MBlock);
    }
}
