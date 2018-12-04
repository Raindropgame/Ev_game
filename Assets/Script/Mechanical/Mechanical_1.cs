using UnityEngine;
using System.Collections;

public class Mechanical_1 : MonoBehaviour {

    public float rotateSpeed = -30f;
    public Element elementTrigger;
    public float malfunctionTime = 5f;
    public float spaceTime;
    public Material material_lightning;
    public float lightning_SpaceTime;

    private bool isMalfunction = false;
    private float Timer_malfunction = 0;
    private float Timer_spaceTime = 0;
    private Vector3 originRotato;
    private float Timer_Lightning = 0;
    private MaterialPropertyBlock MB;
    private SpriteRenderer SR;
    private Material originMaterial;

    private void Start()
    {
        MB = new MaterialPropertyBlock();
        SR = GetComponent<SpriteRenderer>();
        originMaterial = SR.material;
        MB.SetTexture("_MainTex", SR.sprite.texture);
    }

    private void FixedUpdate()
    {
        if (!isMalfunction)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
        else
        {
            Timer_malfunction += Time.deltaTime;
            Timer_spaceTime += Time.deltaTime;
            Timer_Lightning += Time.deltaTime;

            if(Timer_Lightning > lightning_SpaceTime)
            {
                Timer_Lightning = 0;
                MB.SetFloat("_Random_x", Random.value);
                MB.SetFloat("_Random_y", Random.value);
                SR.SetPropertyBlock(MB);
            }

            if(Timer_spaceTime > spaceTime)  
            {
                Timer_spaceTime = 0;
                transform.Rotate(Vector3.forward * 1f * (Random.value * 2 - 1f));
            }

            if(Timer_malfunction > malfunctionTime)  //结束感电
            {
                isMalfunction = false;
                SR.material = originMaterial;
                elementTrigger.element = Attribute.normal;
            }
        }

        if(elementTrigger.isContainElement(Attribute.lightning))
        {
            isMalfunction = true;
            Timer_malfunction = 0;
            Timer_spaceTime = 0;
            SR.material = material_lightning;
            elementTrigger.element = Attribute.lightning;        
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag.CompareTo("Player") == 0)
        {
            CharacterControl.instance.hurt(1, Attribute.normal);
        }
    }

}
