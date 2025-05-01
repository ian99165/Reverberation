using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPick : MonoBehaviour, IInteractable
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Grab()
    {
        _rigidbody.useGravity = false;
    }

    public void Interact()
    {
        Grab();
    }
}
