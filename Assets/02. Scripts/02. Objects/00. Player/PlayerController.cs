using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;  // �̵� �� �ൿ ó�� ���
    private PlayerStats stats;        // �÷��̾� ���� ����

    [Header("�� ����")]
    public Pet currentPet;            // �÷��̾ ������ �ִ� �� (���� ����)

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();

        if (movement == null) Debug.LogError("[PlayerController] PlayerMovement�� �����Ǿ����ϴ�.");
        if (stats == null) Debug.LogError("[PlayerController] PlayerStats�� �����Ǿ����ϴ�.");
    }

    void Start()
    {
        if (currentPet != null)
        {
            PetController petController = currentPet.GetComponent<PetController>();
            if (petController != null)
                petController.player = transform;
            else
                Debug.LogWarning("[PlayerController] PetController�� Pet�� �����ϴ�.");
        }
    }

    void Update()
    {
        HandleMovementInput();  // ���� �� �����̵� �Է� ó��
        HandleSpeedInput();     // �ӵ� ���� �Է� ó��
        HandleSizeInput();      // ũ�� ���� �Է� ó��
        //Debug.Log($"{stats.CurrentHealth}");
        HandleDamageTestInput(); // �ӽ� ������ �Է� ó��
    }

    // �̵� ���� �Է� ó��
    private void HandleMovementInput()
    {
        // ���� �Է� ó��
        if (Input.GetKeyDown(Constants.InputKeys.Jump))
            movement.AttemptJump();

        // �����̵� ���� (�ٴڿ� ���� ���� ����)
        if (Input.GetKeyDown(Constants.InputKeys.Slide))
            movement.StartSlide();

        // �����̵� ����
        if (Input.GetKeyUp(Constants.InputKeys.Slide))
            movement.EndSlide();
    }

    // �ӵ� ���� �Է� ó��
    private void HandleSpeedInput()
    {
        if (Input.GetKeyDown(Constants.InputKeys.IncreaseSpeed))
            stats.ModifySpeed(1f);  // �ӵ� ����

        if (Input.GetKeyDown(Constants.InputKeys.DecreaseSpeed))
            stats.ModifySpeed(-1f); // �ӵ� ����
    }

    // ũ�� ���� �Է� ó��
    private void HandleSizeInput()
    {
        if (Input.GetKeyDown(Constants.InputKeys.IncreaseSize))
            stats.IncreaseSize();  // ũ�� ����

        if (Input.GetKeyDown(Constants.InputKeys.ResetSize))
            stats.ResetSize();     // ũ�� �ʱ�ȭ
    }

    // �ܺο��� �ǰ� �� ȣ�� (��: ���� ���� ��ũ��Ʈ���� ȣ��)
    public void OnHit(int damage)
    {
        movement.TakeDamage(damage);
    }
    
    // �ӽ� ������ �Է� ó�� (C Ű �Է� �� 10 ������ ����)
    private void HandleDamageTestInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            int testDamage = 10;
            Debug.Log($"[PlayerController] C Ű �Է����� {testDamage} ������ ����");
            OnHit(testDamage);
        }
    }
}
