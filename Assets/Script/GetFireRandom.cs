using UnityEngine;
using System.Collections;

public class GetFireRandom : MonoBehaviour {

	void Start () {
        MaterialPropertyBlock MB = new MaterialPropertyBlock();
        Renderer renderer = this.GetComponent<MeshRenderer>();
        renderer.GetPropertyBlock(MB);
        MB.SetFloat("_Random", Random.Range(0.1f,5));
        renderer.SetPropertyBlock(MB);

        Destroy(this);
	}
}
