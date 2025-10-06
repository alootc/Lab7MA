using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public PlayerData CurrentPlayerData { get; private set; }
    public bool IsDataLoaded { get; private set; } = false;

    public System.Action OnDataLoaded;
    public System.Action OnDataUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CurrentPlayerData = new PlayerData();
    }
    public async Task LoadPlayerData()
    {
        try
        {
            var savedData = await LoadData("playerData");

            if (!string.IsNullOrEmpty(savedData))
            {
                CurrentPlayerData = JsonUtility.FromJson<PlayerData>(savedData);
                Debug.Log("Datos del jugador cargados exitosamente");
            }
            else
            {
                Debug.Log("Creando datos por defecto para nuevo jugador");
                await SavePlayerData();
            }

            IsDataLoaded = true;
            OnDataLoaded?.Invoke();
            OnDataUpdated?.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error cargando datos: " + ex.Message);
            CurrentPlayerData = new PlayerData();
            IsDataLoaded = true;
            OnDataLoaded?.Invoke();
        }
    }
    public async Task SavePlayerData()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(CurrentPlayerData);
            await SaveData("playerData", jsonData);
            Debug.Log("Datos del jugador guardados en Cloud Save");
            OnDataUpdated?.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error guardando datos: " + ex.Message);
        }
    }
    public void AddExperience(int amount)
    {
        if (!IsDataLoaded) return;

        CurrentPlayerData.experience += amount;
        CurrentPlayerData.totalClicks++;
        CheckLevelUp();
        _ = SavePlayerData(); 
    }
    private void CheckLevelUp()
    {
        bool leveledUp = false;

        while (CurrentPlayerData.experience >= CurrentPlayerData.ExperienceRequiredForNextLevel)
        {
            CurrentPlayerData.experience -= CurrentPlayerData.ExperienceRequiredForNextLevel;
            CurrentPlayerData.level++;
            CurrentPlayerData.availableSkillPoints += 3;
            leveledUp = true;

            Debug.Log($"¡Nuevo nivel! Ahora eres nivel {CurrentPlayerData.level}");
        }

        if (leveledUp)
        {
            OnDataUpdated?.Invoke();
        }
    }
    public async void UpdatePlayerName(string newName)
    {
        if (!IsDataLoaded || string.IsNullOrEmpty(newName)) return;

        CurrentPlayerData.playerName = newName;
        await SavePlayerData();
    }
    public async void UseSkillPoint(string stat)
    {
        if (!IsDataLoaded || CurrentPlayerData.availableSkillPoints <= 0) return;

        switch (stat.ToLower())
        {
            case "strength":
                CurrentPlayerData.strength++;
                break;
            case "defense":
                CurrentPlayerData.defense++;
                break;
            case "agility":
                CurrentPlayerData.agility++;
                break;
            default:
                return;
        }

        CurrentPlayerData.availableSkillPoints--;
        await SavePlayerData();
    }
    private async Task SaveData(string key, string value)
    {
        var data = new Dictionary<string, object> { { key, value } };
        await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
    private async Task<string> LoadData(string key)
    {
        try
        {
            var data = await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
            return data.TryGetValue(key, out var value) ? value.Value.GetAs<string>() : null;
        }
        catch (System.Exception)
        {
            return null;
        }
    }
    public async void ResetPlayerData()
    {
        CurrentPlayerData = new PlayerData();
        await SavePlayerData();
    }
}