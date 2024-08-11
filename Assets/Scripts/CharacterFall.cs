using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CharacterFall : MonoBehaviour
{
    private Animator anim = null;
    public Vector3 direction;

    private Rigidbody rb = null;

    public int point;
    public int health;

    bool isAnim;

    public Power power;

    public int up;

    public void Stat(Stat stat, Power powerU, int ups)
    {
        point = stat.point;
        health = stat.health;
        power = powerU;
        up = ups;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator is Null");
        }
        rb = GetComponent<Rigidbody>();
    }

    public void Init()
    {
        isAnim = false;
        gameObject.GetComponent<Collider>().enabled = true;
        StartCoroutine(FallingDone(-12f, 0));
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAnim = true;
            anim.SetTrigger("IsSafe");
            anim.SetBool("IsTriggered", isAnim);
        }
        else if (collision.CompareTag("Ground"))
        {
            isAnim = false;
            anim.SetTrigger("IsFallen");
        }else if (collision.CompareTag("Object"))
        {
            anim.SetBool("IsTriggered", isAnim);
        }
    }

    public void OnPlayer()
    {
        if (isAnim)
        {
            ScreenManager.Instance.PlayeSFX();
            ScreenManager.Instance.AddPoint(point);
            GetComponent<Collider>().enabled = false;
        }
        
    }

    public void OnGround()
    {
        if (!isAnim) 
        {
            ScreenManager.Instance.AddHealth(-health);
            GetComponent<Collider>().enabled = false;
        }
    }

    public void EndSpawn()
    {
        Spawner.Instance.RemoveObject(this.gameObject);
    }

    IEnumerator FallingDone(float xPos, float yPos)
    {
        while (true)
        {
            if (transform.position.y > yPos)
            {
                rb.velocity = Vector3.down * 10;
                Debug.Log(rb.velocity);
            }
            else
            {
                transform.position = new Vector3(xPos, yPos, transform.position.z);
                rb.velocity = Vector3.zero;
                Debug.Log(rb.velocity);
            }

            yield return null;
        }
    }

    IEnumerator Timer()
    {
        float timer = 3;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            isAnim = true;
            yield return new WaitForSeconds(1);
        }
    }
}
