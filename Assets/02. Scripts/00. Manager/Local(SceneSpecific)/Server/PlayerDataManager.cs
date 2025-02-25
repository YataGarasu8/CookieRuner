using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        bool loaded = await LoadPlayerDataWithNewtonsoftAsync();
        if (!loaded)
        {
            // ����� �����Ͱ� ������ �⺻�� �Ҵ�
            playerDataSO.highScore = 0;
            Debug.Log("����� �÷��̾� �����Ͱ� �����ϴ�. �⺻ ������ ������.");
        }
        else
        {
            Debug.Log($"�ҷ��� ������ - �ְ� ����: {playerDataSO.highScore}");
        }
    }
    public async Task<bool> LoadPlayerDataWithNewtonsoftAsync()
    {
        try
        {
#pragma warning disable CS0618
            var loadedData = await CloudSaveService.Instance.Data.LoadAllAsync();
#pragma warning restore CS0618
            if (loadedData.ContainsKey(SaveDataKey))
            {
                // �ҷ��� �����͸� rawData ������ ���� (Ÿ���� object)
                object rawData = loadedData[SaveDataKey];
                string jsonData = "";

                // rawData�� string�̸� �ٷ� ����մϴ�.
                if (rawData is string)
                {
                    jsonData = (string)rawData;
                    Debug.Log("�ҷ��� �����Ͱ� string Ÿ���Դϴ�: " + jsonData);
                }
                else
                {
                    // �׷��� �ʴٸ�, rawData�� ToString()�̳� SerializeObject�� ����� ���ϴ�.
                    // (����: ToString()�� ��ü�� Ÿ�� �̸��� ��ȯ�� �� �����Ƿ� SerializeObject�� ����ϴ� ���� �����ϴ�)
                    jsonData = JsonConvert.SerializeObject(rawData);
                    Debug.Log("SerializeObject�� ����Ͽ� ��ȯ�� JSON ������: " + jsonData);
                }

                // ���ʿ��� ���� ����
                jsonData = jsonData.Trim();

                // ���� jsonData�� ���� ���ڵ��� ��� (����ǥ�� ������ �ִٸ�) ó��
                if (jsonData.StartsWith("\"") && jsonData.EndsWith("\""))
                {
                    Debug.Log("���� ���ڵ� ������. �߰� ����ǥ �����մϴ�.");
                    jsonData = jsonData.Substring(1, jsonData.Length - 2);
                    jsonData = jsonData.Replace("\\\"", "\"");
                }

                Debug.Log("���� ó���� JSON ������: " + jsonData);
                Debug.Log("JSON ������ ����: " + jsonData.Length);

                // JSON ���ڿ��� �ùٸ� �������� Ȯ�� ('{' �Ǵ� '['�� �����ϴ���)
                if (!jsonData.StartsWith("{") && !jsonData.StartsWith("["))
                {
                    Debug.LogError("ó���� JSON �����Ͱ� '{' �Ǵ� '['�� �������� �ʽ��ϴ�. ��: " + jsonData);
                    return false;
                }

                try
                {
                    // �Ϲ� ������ Ŭ������ ������ȭ (��: PlayerDataPlain)
                    PlayerDataPlain tempData = JsonConvert.DeserializeObject<PlayerDataPlain>(jsonData);
                    // ScriptableObject�� �� ����
                    PlayerDataManager.Instance.playerDataSO.highScore = tempData.highScore;
                    Debug.Log("�÷��̾� ������ ������ȭ �� ScriptableObject ������Ʈ ����.");
                }
                catch (Exception innerEx)
                {
                    Debug.LogError("������ȭ ����: " + innerEx.Message);
                    throw;
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
            // ScriptableObject�� JSON ���ڿ��� ����ȭ (JsonUtility ���)
            string jsonData = JsonUtility.ToJson(playerDataSO);
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
}