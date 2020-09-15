using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speedWalk = 6.0f;
    public float runSpeed = 12.0f;
    private float currentSpeed;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private PlayerParams param;
    private float slideTime;

    private void OnEnable()
    {
        param = GetComponent<PlayerParams>();
        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        CheckRun();
        Grounded();

        moveDirection.y -= gravity * Time.deltaTime;

        if(controller.enabled)
            controller.Move(moveDirection * Time.deltaTime);

        CheckSlide();

        if (IfSlide())
            transform.localScale = new Vector3(1, 0.5f, 1);
        else
            transform.localScale = new Vector3(1, 1f, 1);

    }
    private void FixedUpdate()
    {
        if (controller.velocity.magnitude > 3f && controller.velocity.magnitude < 5f)
            param.currentState = PlayerParams.state.walk;
        else if (controller.velocity.magnitude <= 0.2f)
            param.currentState = PlayerParams.state.idle;
        TimeSlide();
    }
    //slide
    private void CheckSlide()
    {
        if (controller.velocity.magnitude > 0.5f && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            param.currentState = PlayerParams.state.slide;
            Slide();
        }
    }
    private void Slide()
    {
        controller.Move(moveDirection * Time.deltaTime);
        moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, 5 * Time.deltaTime);
    }
    private void TimeSlide()
    {
        if (IfSlide() && slideTime <= 1.5f)
            slideTime += Time.deltaTime;
        else if (slideTime > 1.5f && !RaycastUp())
        {
            slideTime = 0;
            param.currentState = PlayerParams.state.idle;
        }
        else if (!IfSlide())
            slideTime = 0;

    }
    private bool RaycastUp()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.up, out hit);
    }
    private bool IfSlide()
    => param.currentState == PlayerParams.state.slide;
    //running
    private void CheckRun()
    {
        if (!IfSlide())
            ChangeRun();
        StateRun();
    }
    private void ChangeRun()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && param.GetStamina() > param.maxStamina / 4 && !IfSlide())
            param.currentState = PlayerParams.state.run;
        else if (Input.GetKeyUp(KeyCode.LeftShift) || param.GetStamina() == 0 && !IfSlide())
            param.currentState = PlayerParams.state.walk;
    }
    private void StateRun()
    {
        if (param.currentState == PlayerParams.state.run || IfSlide())
            currentSpeed = runSpeed;
        else
            currentSpeed = speedWalk;
    }
    //other
    private void Grounded()
    {
        if (controller.isGrounded)
        {
            FallDamage();
            Move();
            Jump();
        }
    }
    private void Jump()
    {
        if (Input.GetButton("Jump") && !IfSlide())
            moveDirection.y = jumpSpeed;

    }
    private void Move()
    {
        if (!IfSlide())
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= currentSpeed;
        }
    }
    private void FallDamage()
    {
        if(controller.velocity.y < -10f) param.TakeFallDamage((int)Mathf.Abs((controller.velocity.y / 10f)));
    }
    private void AddForce(Vector3 direction,float strenght)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = false;
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(direction * strenght);
        TakeRbVelocity(rb);
    }
    private void AddForceWithLasftVelosity(Vector3 direction, float strenght)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = false;
        rb.velocity = new Vector3(0,0,0);
        rb.AddForce(direction * strenght);
        rb.velocity = moveDirection + rb.velocity;
        TakeRbVelocity(rb);
    }
    private IEnumerator TakeRbVelocity(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.05f);
        moveDirection = rb.velocity;
        GetComponent<CharacterController>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = false;
    }
}    