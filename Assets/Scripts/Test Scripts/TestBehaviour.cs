using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    private Vector3 start;
    [SerializeField] private Vector3 pointOne;
    [SerializeField] private Vector3 pointTwo;
    [SerializeField] private float moveSpeed;

    //[SerializeField] Material cubeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Oscillate between two points - Time.time makes it frame-rate independent as it is based on time.
        Vector3 add = Vector3.Lerp(pointOne, pointTwo, (Mathf.Sin(Time.time * moveSpeed) + 1) / 2);

        //Oscillate between colors
        //cubeMaterial.color = Color.Lerp(Color.red, Color.blue, (Mathf.Sin(Time.time) + 1)/2);

        //Set position
        transform.position = start + add;
    }
}
