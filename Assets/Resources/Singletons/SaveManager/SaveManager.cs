using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "save.bin");
    
    public static SaveData SaveData { get; private set; }

    private void Awake()
    {
        LoadGame();
    }

    public static void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            SaveData = new SaveData();
            return;
        }
        var saveBytes = File.ReadAllBytes(saveFilePath);
        SaveData = SerializationUtility.DeserializeValue<SaveData>(saveBytes, DataFormat.Binary) ?? new SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public static void SaveGame()
    {
        var saveables = GetSaveables();
        foreach (var saveable in saveables)
        {
            saveable.SaveData(SaveData);
        }
        var saveBytes = SerializationUtility.SerializeValue(SaveData, DataFormat.Binary);
        File.WriteAllBytes(saveFilePath, saveBytes);
    }

    private static IEnumerable<ISaveable> GetSaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>();
    }
}
