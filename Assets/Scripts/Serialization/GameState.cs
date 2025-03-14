using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Utility.Serialization;
[Serializable]
public class GameState : MonoBehaviour
{
    private static GameState _instance;
    public static GameState Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameState");
                _instance = go.AddComponent<GameState>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void SaveGameState(bool overwriteSave = true)
    {
        IGameSerializable[] serializables = FindObjectsOfType<MonoBehaviour>().OfType<IGameSerializable>().ToArray();

        Dictionary<string, string> stateData = new Dictionary<string, string>();
        foreach (IGameSerializable serializable in serializables)
        {
            string key = serializable.GetType().Name + "_" + serializable.GetHashCode();
            if (serializable.Validate())
            {
                stateData[key] = serializable.Serialize();
            }else{
                Debug.LogWarning($"Failed to validate {key}");
            }
        }

        // Wrap the dict into a serializable for JsonUtility
        SerializationContainer container = new SerializationContainer(stateData);
        string json = JsonUtility.ToJson(container, true);

        string folderPath = System.IO.Path.Combine(Application.dataPath, "Scripts", "Serialization");
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        string filePath = overwriteSave ?
            System.IO.Path.Combine(folderPath, "Save_File.json") :
            System.IO.Path.Combine(folderPath, $"Save_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("Game saved successfully to " + filePath);
    }

    public void LoadGameState(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Save file not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        SerializationContainer container = JsonUtility.FromJson<SerializationContainer>(json);
        Dictionary<string, string> loadedData = container.stateData;

        /*
        foreach (var kvp in loadedData)
        {
            Debug.Log($"Loaded {kvp.Key}: {kvp.Value}");
        }
        */

        IGameSerializable[] serializables =
            UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IGameSerializable>().ToArray();

        // deserialize
        foreach (IGameSerializable serializable in serializables)
        {
            string key = serializable.GetType().Name + "_" + serializable.GetHashCode();
            if (loadedData.ContainsKey(key))
            {
                serializable.Deserialize(loadedData[key]);
            }
        }

        Debug.Log("Game state applied from loaded file.");
    }
   
}

