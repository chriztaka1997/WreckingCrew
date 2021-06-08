using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class KeyManager
{
    private bool thisPressed;
    private bool lastPressed;
    public KeyCode code;

    public KeyManager(KeyCode code)
    {
        this.code = code;
        thisPressed = GetKey;
        lastPressed = false;
    }

    public bool GetKey => Input.GetKey(code);
    public bool GetKeyDown => Input.GetKey(code) && !lastPressed;
    public bool GetKeyUp => !Input.GetKey(code) && lastPressed;

    // place at begining of fixed update loop
    public void Update()
    {
        lastPressed = thisPressed;
        thisPressed = GetKey;
    }
}