using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _visionRadius = 0.1f;
    [SerializeField] private LayerMask _layerMask;
    
    [Header("Throw Setting")]
    [SerializeField] private Transform _target;

    public bool IsPickup { get; set; }
    
    private Vector3 _origin;
    private Vector3 _direction;
    private RaycastHit _hit;
    private Transform _item;

    private void Throw()
    {
        if (_item == null) return;

        Rigidbody rb = _item.GetComponent<Rigidbody>();
        Collider[] colliders = _item.GetComponentsInChildren<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        foreach (var col in colliders)
        {
            col.isTrigger = false; // 讓所有碰撞體恢復物理效果
        }

        _item.position = _target.position;
        _item.gameObject.SetActive(true);
        _item = null;
    }



    private void Dropoff()
    {
        if (_item == null) return;

        Rigidbody rb = _item.GetComponent<Rigidbody>();
        Collider[] colliders = _item.GetComponentsInChildren<Collider>(); // 取得所有 Collider

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (var col in colliders)
        {
            col.isTrigger = false; // 關閉 isTrigger，恢復物理碰撞
        }

        _item = null;
    }


    private void FixedUpdate()
    {
        if (_item != null)
        {
            Rigidbody rb = _item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                Vector3 newPos = Vector3.Lerp(_item.position, _target.position, Time.fixedDeltaTime * 10f);
                rb.MovePosition(newPos);
            }
        }
    }

    private void Update()
    {
        _origin = transform.position;
        _direction = transform.forward;

        if (!IsPickup && _item != null)
        {
            Dropoff();
        }

        if (_item == null && Physics.SphereCast(_origin, _visionRadius, _direction, out _hit, _maxDistance, _layerMask))
        {
            if (_hit.transform.TryGetComponent(out IInteractable item) && IsPickup)
            {
                _item = _hit.transform;
                Rigidbody rb = _item.GetComponent<Rigidbody>();
                Collider[] colliders = _item.GetComponentsInChildren<Collider>(); // 遍歷所有碰撞體

                if (rb != null)
                {
                    rb.isKinematic = true; // 讓物品不受物理影響
                }

                foreach (var col in colliders)
                {
                    col.isTrigger = true; // 讓所有碰撞體變成觸發器，不影響玩家
                }

                item.Interact();
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_hit.collider != null)
        {
            Gizmos.DrawRay(_origin, _direction * _hit.distance);
            Gizmos.DrawWireSphere(_origin + _direction * _hit.distance, _visionRadius);
        }
        else
        {
            Gizmos.DrawRay(_origin, _direction * _maxDistance);
        }
    }
}