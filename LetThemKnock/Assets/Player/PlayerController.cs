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

    private void Start()
    {
        param = GetComponent<PlayerParams>();
        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        CheckRun();
        Grounded();
        if(!IFWallRun())
            moveDirection.y -= gravity * Time.deltaTime;
        else
            moveDirection.y -= gravity * Time.deltaTime/4;

        if(controller.enabled)
            controller.Move(moveDirection * Time.deltaTime);

        CheckSlide();

        if (IfSlide())
            transform.localScale = new Vector3(1, 0.5f, 1);
        else
            transform.localScale = new Vector3(1, 1f, 1);

        RaycastSides();
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
        if (!IfSlide() && !IFWallRun())
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
    //wallrun 
    private void RaycastSides()
    {
        RaycastHit[] hit = new RaycastHit[2];
        Physics.Raycast(transform.position, transform.right, out hit[0], 1f);
        Physics.Raycast(transform.position, -transform.right, out hit[1], 1f);

        if (param.GetStamina() > 0)
            SetWallRun(hit);
        else param.currentState = PlayerParams.state.idle;
    }
    private void SetWallRun(RaycastHit[] hit)
    {
        if (controller.velocity.magnitude >= 2f && Input.GetKey(KeyCode.Space) && !IFWallRun())
            CheckStaminaForWallRun(hit);
        else if ((hit[0].collider == null && hit[1].collider == null) && IFWallRun())
            param.currentState = PlayerParams.state.idle;
    }
    private void CheckStaminaForWallRun(RaycastHit[] hit)
    { if (param.GetStamina() > param.maxStamina / 4) ActivateWallRun(hit); }
    private void ActivateWallRun(RaycastHit[] hit)
    {
        if (Input.GetKey(KeyCode.D) && hit[0].collider != null)
            param.currentState = PlayerParams.state.WallRunRight;
        else if (Input.GetKey(KeyCode.A) && hit[1].collider != null)
            param.currentState = PlayerParams.state.WallRunLeft;
    }
    private bool IFWallRun()
        => param.currentState == PlayerParams.state.WallRunLeft || param.currentState == PlayerParams.state.WallRunRight;
    private void JumpFromWall()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = false;

        AddPowerForRigitBody(rb);

        StartCoroutine(TakeRbVelocity(rb));

    }
    private void AddPowerForRigitBody(Rigidbody rb)
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(transform.up * jumpSpeed * 50f);
        rb.AddForce(transform.forward * jumpSpeed * 20f);
        if (param.currentState == PlayerParams.state.WallRunLeft)
            rb.AddForce(transform.right * jumpSpeed * 100f);
        else if (param.currentState == PlayerParams.state.WallRunRight)
            rb.AddForce(-transform.right * jumpSpeed * 100f);
    }
    private IEnumerator TakeRbVelocity(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.05f);
        moveDirection = rb.velocity;
        GetComponent<CharacterController>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = false;
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
        else
        {
            if (Input.GetKeyUp(KeyCode.Space) && IFWallRun())
                JumpFromWall();
        }
    }
    private void Jump()
    {
        if (Input.GetButton("Jump") && !IfSlide())
            moveDirection.y = jumpSpeed;

    }
    private void Move()
    {
        if (!IfSlide() && !IFWallRun())
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
        rb.AddForce(direction * strenght);
        TakeRbVelocity(rb);
    }
}    