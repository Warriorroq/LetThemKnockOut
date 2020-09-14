using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField] private Transform pos;
    [SerializeField] private Vector3 movementDirection;
    [SerializeField] private Vector3 rotationDirection;
    private void OnEnable()
    {
        movementDirection = new Vector3(pos.position.x - transform.position.x, pos.position.y - transform.position.y, pos.position.z - transform.position.z);
    }
    private void Start()
    {
        InvokeRepeating(nameof(Transport),0f,0.05f);
    }
    void Update()
    {

        //enable mapControllermenu
    }
    private void Transport()
    {
        if (transform.position != pos.position)
            transform.position += movementDirection * 0.05f;
    }
}
