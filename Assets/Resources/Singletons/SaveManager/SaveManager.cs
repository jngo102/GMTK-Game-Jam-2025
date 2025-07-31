using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "data.bin");
    
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
        var saveBytes = File.ReadAllBytes(saveFilePath);
        SaveData = SerializationUtility.DeserializeValue<SaveData>(saveBytes, DataFormat.Binary) ?? new SaveData();
        
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
        
        var saveBytes = SerializationUtility.SerializeValue(SaveData, DataFormat.Binary);
        File.WriteAllBytes(saveFilePath, saveBytes);
    }

    private IEnumerable<ISaveable> GetSaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>();
    }
}
