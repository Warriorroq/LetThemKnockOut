using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMap : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<MapCamera>().enabled = false;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            RaycastBot();
    }
    private void RaycastBot()
    {
        RaycastHit hit;
<<<<<<< HEAD
        try {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);
            hit.collider.gameObject.GetComponent<Bot>().ActivePlayer();
            transform.parent = hit.collider.gameObject.transform;
            transform.localPosition = new Vector3(0, 1, 0);
=======

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);

        try
        { 
            hit.collider.gameObject.GetComponent<Bot>().ActivePlayer();
            transform.parent = hit.collider.gameObject.transform;
            transform.position = new Vector3(0, 1, 0);
>>>>>>> Player
            GetComponent<PlayerCameraRotation>().enabled = true;
            GetComponent<UserMap>().enabled = false;
        }
        catch
        {

        }
        
    }
}
