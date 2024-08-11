using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim = null;
    private Vector3 direction;
    private Vector3 velocity;
    public float speed;
    private bool jumping;
    private bool flip;
    public float gravity;
    private float yVelocity;
    public float jumpHeight;
    private PlayerMove inputs;
    private float horizontalInput;
    bool active;
    int num;
    float def;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator is Null");
        }

        inputs = new PlayerMove();
        inputs.Enable();

        inputs.Player.Move.performed += ctx =>
        {
            horizontalInput = ctx.ReadValue<float>();
        };

        inputs.Player.Jump.performed += ctx =>
        {
            jumping = ctx.ReadValueAsButton();
        };

        def = speed;
    }
    
    void Update() 
    {
        if (controller.isGrounded == true)
        {
            direction = new Vector3(0, 0, horizontalInput);
            velocity = direction * speed;

            if (horizontalInput < 0 && flip == false) 
            {
                flip = true;
                transform.rotation = Quaternion.Euler(transform.rotation.x, -180f, transform.rotation.z);
            } 
            else if (horizontalInput > 0 & flip == true) 
            {
                flip = false;
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
            } 
            if (anim != null)
            {
                anim.SetFloat("Move", Mathf.Abs(horizontalInput));
            }
            if (active && jumping)
            {
                StartCoroutine(Timer());
                active = false;
            }
            else if(!active && !jumping)
            {
                StopCoroutine(Timer());
                speed = def;
            }

        }
        else 
        {
            yVelocity -= gravity * Time.deltaTime;
        }
        velocity.y = yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Object"))
        {
            anim.SetBool("Jump", true);
            if (other.GetComponent<CharacterFall>().power == global::Power.Walk)
            {
                active = true;
                num = other.GetComponent<CharacterFall>().up;
            }
        }
    }

    public void AfterJump()
    {
        anim.SetBool("Jump", false);
    }

    IEnumerator Timer()
    {
        float timer = 3;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            speed = num;
            yield return new WaitForSeconds(1);
        }
    }
}
