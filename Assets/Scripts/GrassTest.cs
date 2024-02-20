using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class GrassTest : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> _springs = new List<Rigidbody>();
    [SerializeField] private Vector3 worldPos;

    private void Update()
    {
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(worldPos, 0.1f);
    }

    public void ApplyForce(Vector3 force)
    {
        foreach (Rigidbody spring in _springs)
        {
            spring.AddForce(force, ForceMode.Impulse);
        }
    }

}
