using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;

    float offsetX;

    void Start()
    {
        offsetX = targetObject.transform.position.x - transform.position.x;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = targetObject.transform.position.x;
        transform.position = pos;
    }
}
