﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParams : MonoBehaviour
{
    public enum state {
        idle,
        run,
        walk,
        slide,
        fall,
    }
    public state currentState;

    public int maxStamina = 100;    
    private int currentStamina;
    public int maxArmor;
    private int currentArmor;
    public int maxHp = 100;
    private int currentHp;

    public int GetArmor()
    => currentArmor;
    public int GetStamina()
        => currentStamina;
    public int GetHp()
    => currentHp;

    public void AddArmor(int value)
    { currentArmor += value; }
    public void AddStamina(int value)
    { currentStamina+= value; }
    public void AddHp(int value)
    { currentHp += value; }

    private void Start()
    {
        currentStamina = maxStamina;
        currentHp = maxHp;
        currentArmor = maxArmor;
        InvokeRepeating(nameof(AddStamina), 1f, 1f);
    }
    private void Update()
    {
        if (maxArmor < currentArmor)
            currentArmor = maxArmor;

        CheckHp();

    }
    private void FixedUpdate()
    {
        CheckStamina();
       
    }
    private void AddStamina()
    {
        if (currentStamina < maxStamina && (currentState == state.idle || currentState == state.walk))
            currentStamina++;
    }
    private void CheckHp()
    {
        if (currentHp <= 0)
            Destroy(this.gameObject);
        else if (currentHp > maxHp)
            currentHp = maxHp;
    }
    private void CheckStamina()
    {
        if (currentState == state.run && currentStamina > 0)
            currentStamina--;
    }
    public void TakeDamage(int damage)
    {
        if (currentArmor > 0 && currentArmor < damage)
        { damage = 0; currentArmor = 0; }
        else if (currentArmor >= damage)
        { currentArmor -= damage; damage = 0; }
        currentHp -= damage;
    }
    public void TakeFallDamage(int damage)
        =>currentHp -= damage;

    private void OnDestroy()
    {
       PlayerCameraRotation rot = GetComponentInChildren<PlayerCameraRotation>();
       rot.gameObject.transform.parent = null;
       rot.gameObject.GetComponent<PlayerCameraRotation>().enabled = false;
       rot.gameObject.GetComponent<MapCamera>().enabled = true;
    }
}
