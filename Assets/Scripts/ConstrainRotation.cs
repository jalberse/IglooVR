using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constrains the rotation of this object to a specific transform.
/// </summary>
public class ConstrainRotation : MonoBehaviour
{
    public Transform Parent;
    private Vector3 pos, fw, up;

    void Start()
    {
        fw = Parent.transform.InverseTransformDirection(transform.forward);
        up = Parent.transform.InverseTransformDirection(transform.up);
    }
    void Update()
    {
        var newfw = Parent.transform.TransformDirection(fw);
        var newup = Parent.transform.TransformDirection(up);
        var newrot = Quaternion.LookRotation(newfw, newup);
        transform.rotation = newrot;
    }
}
