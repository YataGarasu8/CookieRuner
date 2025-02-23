using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PetController : MonoBehaviour
{
    public Transform player;
    public float followDistance = 1.5f;
    private float followSpeed;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        followSpeed = GetComponent<Pet>().petData.followSpeed;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > followDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;
        }
    }
}
