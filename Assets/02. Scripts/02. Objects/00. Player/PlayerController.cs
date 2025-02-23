using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerStats stats;

    public Pet currentPet;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        if (currentPet != null)
            currentPet.GetComponent<PetController>().player = this.transform;
    }

    void Update()
    {
        HandleSpeedInput();
        HandleSizeInput();
    }

    // �ӵ� �Է� ó��
    private void HandleSpeedInput()
    {
        if (Input.GetKeyDown(Constants.InputKeys.IncreaseSpeed))
            stats.ModifySpeed(1f);

        if (Input.GetKeyDown(Constants.InputKeys.DecreaseSpeed))
            stats.ModifySpeed(-1f);
    }

    // ũ�� �Է� ó��
    private void HandleSizeInput()
    {
        if (Input.GetKeyDown(Constants.InputKeys.IncreaseSize))
            stats.IncreaseSize();

        if (Input.GetKeyDown(Constants.InputKeys.ResetSize))
            stats.ResetSize();
    }
}
