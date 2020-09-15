using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField] private Transform pos;
    private Vector3 movementDirection;
    private Vector3 rotationDirection;
    private bool isMove;
    private void OnEnable()
    {
        movementDirection = new Vector3(pos.position.x - transform.position.x, pos.position.y - transform.position.y, pos.position.z - transform.position.z);
        rotationDirection = new Vector3(pos.eulerAngles.x - transform.eulerAngles.x, pos.eulerAngles.y - transform.eulerAngles.y, pos.eulerAngles.z - transform.eulerAngles.z);

        isMove = true;

        Invoke(nameof(disableMove), 1f);
    }
    void Update()
    {
        if (isMove)
            Transport();

    }
    private void Transport()
    {
        transform.position += movementDirection * Time.deltaTime;
        transform.eulerAngles += rotationDirection * Time.deltaTime;
    }
    private void disableMove()
    {
        isMove = false;
        transform.position = pos.position;
        transform.rotation = pos.rotation;
        GetComponent<UserMap>().enabled = true;
    }

    
}
