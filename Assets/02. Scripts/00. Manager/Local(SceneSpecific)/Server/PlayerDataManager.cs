using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System;
using Newtonsoft.Json;
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    private const string SaveDataKey = "PlayerData"; // ������ �������� Ű

    // Inspector���� �Ҵ��� ScriptableObject ���� (PlayerDataSO ����)
    public PlayerDataSO playerDataSO;

    void Awake()
    {
        // �̱��� ����: ���� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void Start()
    {
        // ����Ƽ ���� �ʱ�ȭ
        await UnityServices.InitializeAsync();
        Debug.Log("����Ƽ ���� �ʱ�ȭ �Ϸ�.");

        // �͸� �α��� (�α��εǾ� ���� �ʴٸ�)
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("�α��� ����, �÷��̾� ID: " + AuthenticationService.Instance.PlayerId);
            }
            catch (Exception ex)
            {
                Debug.LogError("�α��� ����: " + ex.Message);
                return;
            }
        }

        // Cloud Save���� �÷��̾� ������ �ҷ����� (Newtonsoft.Json ���)
        bool loaded = await LoadPlayerDataAsync();
        if (!loaded)
        {
            // ����� �����Ͱ� ������ �⺻�� �Ҵ�
            SetDefaultPlayerData();
            Debug.Log("����� �÷��̾� �����Ͱ� �����ϴ�. �⺻ ������ ������.");
        }
        else
        {
            Debug.Log($"�ҷ��� ������ - �̸�: {playerDataSO.playerName}, ����: {playerDataSO.highScore}, ���: {playerDataSO.gold}");
        }
    }

    public async Task<bool> LoadPlayerDataAsync()
    {
        try
        {
#pragma warning disable CS0618
            var loadedData = await CloudSaveService.Instance.Data.LoadAllAsync();
#pragma warning restore CS0618
            if (loadedData.ContainsKey(SaveDataKey))
            {
                string jsonData = JsonConvert.SerializeObject(loadedData[SaveDataKey]);
                jsonData = jsonData.Trim();

                // JSON ������ ���� ����
                if (!jsonData.StartsWith("{") && !jsonData.StartsWith("["))
                {
                    Debug.LogError("ó���� JSON ������ ������ �߸��Ǿ����ϴ�. ��: " + jsonData);
                    return false;
                }

                try
                {
                    // JSON �����͸� PlayerDataSO�� ������ȭ
                    PlayerDataSO tempData = JsonConvert.DeserializeObject<PlayerDataSO>(jsonData);

                    // ������ ����
                    if (!IsValidPlayerData(tempData))
                    {
                        Debug.LogWarning("�ҷ��� �����Ͱ� ��ȿ���� �ʾ� �⺻������ �ʱ�ȭ�˴ϴ�.");
                        SetDefaultPlayerData();
                        return false;
                    }

                    // ScriptableObject�� �� ����
                    playerDataSO.playerName = tempData.playerName;
                    playerDataSO.highScore = tempData.highScore;
                    playerDataSO.gold = tempData.gold;

                    Debug.Log($"�÷��̾� ������ �ҷ����� �Ϸ� - �̸�: {playerDataSO.playerName}, ����: {playerDataSO.highScore}, ���: {playerDataSO.gold}");
                }
                catch (Exception innerEx)
                {
                    Debug.LogError("JSON ������ȭ ����: " + innerEx.Message);
                    SetDefaultPlayerData();
                    return false;
                }

                return true;
            }
            else
            {
                Debug.Log("Cloud Save�� ����� �÷��̾� �����Ͱ� �������� �ʽ��ϴ�.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Newtonsoft.Json�� ����� �÷��̾� ������ �ҷ����� ����: " + ex.Message);
            return false;
        }
    }

    // �÷��̾� �����͸� �����ϴ� �Լ� (ScriptableObject�� JSON���� ����ȭ�Ͽ� ����)
    public async Task SavePlayerDataAsync()
    {
        try
        {
            // ���� ���� ������ ����
            if (!IsValidPlayerData(playerDataSO))
            {
                Debug.LogError("�����Ϸ��� �����Ͱ� ��ȿ���� �ʽ��ϴ�. ������ ��ҵ˴ϴ�.");
                return;
            }

            // PlayerDataSO ��ü�� JSON���� ��ȯ
            string jsonData = JsonConvert.SerializeObject(playerDataSO);

            var dataToSave = new Dictionary<string, object>
            {
                { SaveDataKey, jsonData }
            };

#pragma warning disable CS0618
            await CloudSaveService.Instance.Data.ForceSaveAsync(dataToSave);
#pragma warning restore CS0618
            Debug.Log("�÷��̾� �����Ͱ� Ŭ���忡 ���������� ����Ǿ����ϴ�.");
        }
        catch (Exception ex)
        {
            Debug.LogError("�÷��̾� ������ ���� ����: " + ex.Message);
        }
    }

    // ������ ���� �Լ� (��ȿ�� ������ Ȯ��)
    private bool IsValidPlayerData(PlayerDataSO data)
    {
        if (data == null) return false;
        if (string.IsNullOrWhiteSpace(data.playerName)) return false; // �̸��� ��������� �ȵ�
        if (data.highScore < 0) return false; // ������ 0 �̻��̾�� ��
        if (data.gold < 0) return false; // ���� 0 �̻��̾�� ��
        return true;
    }

    // �⺻ �÷��̾� ������ ���� (�߸��� �����Ͱ� ���� ��� �⺻�� �Ҵ�)
    private void SetDefaultPlayerData()
    {
        playerDataSO.playerName = "Guest";
        playerDataSO.highScore = 0;
        playerDataSO.gold = 0;
    }
}
