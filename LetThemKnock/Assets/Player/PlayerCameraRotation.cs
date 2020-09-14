using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    public float speedVertical;
    public float speedHorisontal;
    private Transform body;
    private float bodyY;
    private PlayerParams param;
    private void Start()
    {
        body = transform.parent;
        param = body.gameObject.GetComponent<PlayerParams>();
    }
    void Update()
    {
        float y = speedVertical * Input.GetAxis("Mouse X");
        float x = speedHorisontal * -Input.GetAxis("Mouse Y");
        CheckSlide(y);
        if(transform.eulerAngles.x + x < 90 || transform.eulerAngles.x + x > 270)
            transform.Rotate(x, 0, 0);
        CheckWallRun();

    }
    private void CheckWallRun()
    {
        if (param.currentState == PlayerParams.state.WallRunLeft)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 15f);
        else if (param.currentState == PlayerParams.state.WallRunRight)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 15f);
    }
    void CheckSlide(float y)
    {
        
        if (param.currentState != PlayerParams.state.slide)
            bodyY = body.transform.eulerAngles.y;

        if (param.currentState == PlayerParams.state.slide && Mathf.Abs(bodyY - (transform.eulerAngles.y + y)) < 50f)
            transform.Rotate(0, y, 0);
        else if (param.currentState != PlayerParams.state.slide)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, body.transform.eulerAngles.y, 0);
            body.transform.Rotate(0, y, 0);
        }
    }
}
