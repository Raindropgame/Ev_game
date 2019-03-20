using UnityEngine;
using System.Collections;

public class Door_2 : MonoBehaviour {

    static ParticleSystem m_PS_dust = null;
    static ParticleSystem PS_dust
    {
        get
        {
            if(m_PS_dust == null)
            {
                GameObject t = Instantiate(ResourcesManagement.getInstance().getResources<GameObject>("Door_Dust"), position: Vector3.zero, rotation: Quaternion.Euler(Vector3.forward * 90)) as GameObject;
                m_PS_dust = t.GetComponent<ParticleSystem>();
            }
            return m_PS_dust;
        }
    }

    public Door_2_button button;
    public Transform door_bottom, door_up,door_stone;
    public float Height_bottom, Height_up;

    private bool isOpen = false;
    Vector3 originPos_door_up;
    Vector3 originPos_door_bottom;
    Vector3 originScale_stone;

    private void Start()
    {
        originPos_door_up = door_up.position;
        originPos_door_bottom = door_bottom.position;
        originScale_stone = door_stone.localScale;
    }

    private void Update()
    {
        if(button.isPress)
        {
            if(!isOpen) //开门
            {
                isOpen = true;
                StartCoroutine(IE_openDoor());
            }
        }
        else
        {
            if(isOpen)  //关门
            {
                isOpen = false;
                StartCoroutine(IE_closeDoor());
            }
        }
    }

    IEnumerator IE_openDoor()
    {
        const float moveTime = 1;
        Time.timeScale = 0;
        CharacterControl.instance.setInputNone();

        float timer = 0;
        Vector3 originPos_camera = CameraFollow.instance.transform.position;
        
        while (timer < moveTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / moveTime;

            CameraFollow.instance.transform.position = Vector3.Lerp(originPos_camera, transform.position, t);
            yield return null;
        }

        timer = 0;
        const float time_stone = 1.5f;
        while(timer < time_stone)
        {
            timer += Time.unscaledTime;
            float t = timer / time_stone;

            door_stone.localScale = Vector3.Lerp(originScale_stone, Vector3.right, t);
            yield return null;
        }

        timer = 0;
        //开门动画
        const float Time_open = 3;
        const float scale_shake = 0.1f;
        Vector3 currentPos_up = door_up.position;
        Vector3 currentPos_bottom = door_bottom.position;
        while(timer < Time_open)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / Time_open;

            door_up.position = Vector3.Lerp(currentPos_up, originPos_door_up + Vector3.up * Height_up, t);
            door_bottom.position = Vector3.Lerp(currentPos_bottom, originPos_door_bottom + Vector3.down * Height_bottom, t);
            door_up.position += (Vector3)Random.insideUnitCircle * scale_shake;
            door_bottom.position += (Vector3)Random.insideUnitCircle * scale_shake;
            yield return null;
        }

        Time.timeScale = 1;
        CharacterControl.instance.getInput();
    }

    IEnumerator IE_closeDoor()
    {

        float timer = 0;

        timer = 0;
        //关门动画
        const float Time_close = 0.4f;
        Vector3 currentPos_up = door_up.position;
        Vector3 currentPos_bottom = door_bottom.position;
        while (timer < Time_close)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / Time_close;

            door_up.position = Vector3.Lerp(currentPos_up, originPos_door_up, t);
            door_bottom.position = Vector3.Lerp(currentPos_bottom, originPos_door_bottom, t);
            yield return null;
        }

        PS_dust.transform.position = transform.position;
        PS_dust.Play();

        timer = 0;
        const float time_stone = 1f;
        while (timer < time_stone)
        {
            timer += Time.unscaledTime;
            float t = 1 - timer / time_stone;

            door_stone.localScale = Vector3.Lerp(originScale_stone, Vector3.right, t);
            yield return null;
        }
    }
}
