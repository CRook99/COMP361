using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    // Flag to control whether the save file should be loaded on scene load.
    private bool _loadGameStateOnSceneLoad = false;

    // Call this method to request that the game state is loaded on the next combat scene load.
    public void PrepareForLoadGameState()
    {
        _loadGameStateOnSceneLoad = true;
    }

    private const string TARGET_SCENE = "Combat";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only load the game state if we're in the target scene and the flag is set.
        if (scene.name == TARGET_SCENE && _loadGameStateOnSceneLoad)
        {
            Debug.Log("Scene '" + scene.name + "' loaded. Loading game state from save file...");
            LoadGameState("Save_File.json");
            _loadGameStateOnSceneLoad = false; // Reset the flag after loading
        }
    }

    public void SaveGameState(bool overwriteSave = true)
    {
        IGameSerializable[] serializables = FindObjectsOfType<MonoBehaviour>()
            .OfType<IGameSerializable>().ToArray();

        Dictionary<string, string> stateData = new Dictionary<string, string>();
        foreach (IGameSerializable serializable in serializables)
        {
            string key;
            if (serializable is Entities.Entity entity)
            {
                key = serializable.GetType().Name + "_" + entity.UniqueId;
            }
            else
            {
                key = serializable.GetType().Name;
            }
            if (serializable.Validate())
            {
                stateData[key] = serializable.Serialize();
            }
            else
            {
                Debug.LogWarning($"Failed to validate {key}");
            }
        }

        // Wrap the dictionary into a serializable container for JsonUtility
        SerializationContainer container = new SerializationContainer(stateData);
        string json = JsonUtility.ToJson(container, true);

        // Use Application.dataPath for saving files (original folder)
        string folderPath = Path.Combine(Application.dataPath, "Scripts", "Serialization");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = overwriteSave ?
            Path.Combine(folderPath, "Save_File.json") :
            Path.Combine(folderPath, $"Save_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        File.WriteAllText(filePath, json);
        Debug.Log("Game saved successfully to " + filePath);
    }

    // For loading, pass in just the file name (e.g., "Save_File.json")
    public void LoadGameState(string fileName)
    {
        // Use the original folder for loading
        string folderPath = Path.Combine(Application.dataPath, "Scripts", "Serialization");
        string filePath = Path.Combine(folderPath, fileName);
        Debug.Log("Loading game state from: " + filePath);

        if (!File.Exists(filePath))
        {
            Debug.LogError("Save file not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        SerializationContainer container = JsonUtility.FromJson<SerializationContainer>(json);
        Dictionary<string, string> loadedData = container.stateData;

        IGameSerializable[] serializables =
            UnityEngine.Object.FindObjectsOfType<MonoBehaviour>()
            .OfType<IGameSerializable>().ToArray();

        foreach (IGameSerializable serializable in serializables)
        {
            string key;
            if (serializable is Entities.Entity entity)
            {
                key = serializable.GetType().Name + "_" + entity.UniqueId;
            }
            else
            {
                key = serializable.GetType().Name;
            }
            if (loadedData.ContainsKey(key))
            {
                serializable.Deserialize(loadedData[key]);
            }
        }

        Debug.Log("Game state applied from loaded file.");
    }

}
