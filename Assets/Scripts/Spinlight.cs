using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinlight : MonoBehaviour
{
    [SerializeField] private RectTransform _spinlight;
    [SerializeField] private float _velocity = 10f;

    // Update is called once per frame
    void Update()
    {
        _spinlight.Rotate(Vector3.forward * _velocity * Time.deltaTime);
    }
}
