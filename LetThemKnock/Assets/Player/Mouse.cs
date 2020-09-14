using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    private bool open = true;
    void Update()
    {
        ChangeLock();
        ChangeState();
    }
    private void ChangeLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            open = !open;
    }
    private void ChangeState()
    {
        if (open)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
