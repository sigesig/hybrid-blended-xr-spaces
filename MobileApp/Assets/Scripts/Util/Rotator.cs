using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to make something rotate
/// </summary>
public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 speed;
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime, Space.Self);
    }
}
