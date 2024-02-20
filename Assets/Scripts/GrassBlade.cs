using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlade : MonoBehaviour
{
    [SerializeField] private Rigidbody _attachedSpring;
    [SerializeField] private Vector3 _additionalRotations;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 up = (_attachedSpring.position - transform.position).normalized * -1;
        // transform.rotation = Quaternion.LookRotation(transform.forward, up);

        transform.up = up;
        Vector3 rotations = transform.localEulerAngles;
        rotations += _additionalRotations;
        transform.localEulerAngles = rotations;
    }
}
