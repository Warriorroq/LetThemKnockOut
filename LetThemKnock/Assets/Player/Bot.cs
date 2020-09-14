using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public void ActivePlayer()
    {
        gameObject.AddComponent<PlayerParams>();
        gameObject.AddComponent<PlayerController>();
        Destroy(this.GetComponent<Bot>());
    }
}
