using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "data.json");
    
    public static SaveData SaveData { get; private set; }

    private void Awake()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            SaveData = new SaveData();
            return;
        }
        var saveJson = File.ReadAllText(saveFilePath);
        SaveData = JsonUtility.FromJson<SaveData>(saveJson) ?? new SaveData();
        
        var saveables = GetSaveables();
        foreach (var saveable in saveables)
        {
            saveable.LoadData(SaveData);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        var saveables = GetSaveables();
        foreach (var saveable in saveables)
        {
            saveable.SaveData(SaveData);
        }
        
        var saveJson = JsonUtility.ToJson(SaveData);
        File.WriteAllText(saveFilePath, saveJson);
    }

    private IEnumerable<ISaveable> GetSaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>();
    }
}
