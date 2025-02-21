using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Range(1f, 10f)][SerializeField] float speed = 1f;


    void Start()
    {
        
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
