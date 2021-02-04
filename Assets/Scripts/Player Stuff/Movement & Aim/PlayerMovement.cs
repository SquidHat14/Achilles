using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float jumpSpeed = 10f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.5f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    bool isDashing;
    bool canDash = true;
    float dashCooldown = .1f;
    float dashDuration = .4f;
    float dashSpeed;


    void Start()
    {
        dashSpeed = speed * 4f;
    }
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        float jump = Input.GetAxisRaw("Jump");
        bool dashInput = Input.GetMouseButtonDown(1);

        if(dashInput != false && canDash)
        {
            velocity.y = 0;
            StartCoroutine(Dash(inputX, inputZ));
            return;
        }
        
        else if(!isDashing)
        {
        Debug.Log("Moving Normally");
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 move = transform.right * inputX + transform.forward * inputZ;

        velocity.y += gravity * Time.deltaTime;

        if(isGrounded)
        {
            if(velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if(jump != 0)
            {
                velocity.y = jumpSpeed;
            }
        }
        controller.Move(move * speed * Time.deltaTime); //Horizontal Movement

        controller.Move(velocity * Time.deltaTime); //Vertical Movement
        }
    }

    IEnumerator Dash(float inputX, float inputZ)
    {
        isDashing = true;
        canDash = false;

        Vector3 right = transform.right;
        Vector3 forward = transform.forward;

        if(inputX == 0 && inputZ == 0)
        {
            inputZ = 1;
        }

        float timeLeft = dashDuration;

        while(timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            Vector3 move = right * inputX + forward * inputZ;

            controller.Move(move * dashSpeed * Time.deltaTime); //Horizontal Movement

            yield return null;
        }
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        Debug.Log("Can Dash Again");
    }
}
