﻿using UnityEngine;
using System.Collections;

public class JointControl : MonoBehaviour {

    public bool isHead = false;

    public float activeDistance;
    public Transform ForcePos;
    public float force;
    public bool isActive = false;
    public float changeTime;

    private Rigidbody2D rig;
    private float _time0 = 0;
    private BoxCollider2D Coll;

	void Start () {
        rig = GetComponent<Rigidbody2D>();
        Coll = GetComponent<BoxCollider2D>();
	}


    private void FixedUpdate()
    {
        if (((Vector2)(CharacterControl.instance.transform.position - ForcePos.position)).sqrMagnitude < activeDistance * activeDistance)  //一定范围内攻击玩家
        {
            if (isHead)
            {
                if (isActive)
                {
                    Vector2 direction = ((Vector2)CharacterControl.instance.transform.position - (Vector2)ForcePos.position).normalized;
                    rig.AddForceAtPosition(direction * force, ForcePos.position);
                }
            }
            else
            {
                if (isActive)
                {
                    Vector2 direction = Vector3.Cross(((Vector2)CharacterControl.instance.transform.position - (Vector2)ForcePos.position).normalized, Vector3.back);
                    _time0 += Time.deltaTime;
                    if (_time0 < changeTime)
                    {
                        rig.AddForceAtPosition(direction * force, ForcePos.position);
                    }
                    else
                    {
                        _time0 = 0;
                        force = 0 - force;
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            CharacterControl.instance.hurt(1, Attribute.normal,Coll.bounds.center);
        }
    }

}
