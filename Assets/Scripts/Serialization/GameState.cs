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

    private bool _loadGameStateOnSceneLoad = false;
    private string _fileToLoad;

    public void PrepareForLoadGameState(string filename)
    {
        _loadGameStateOnSceneLoad = true;
        _fileToLoad = filename;
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
        if (scene.name == TARGET_SCENE && _loadGameStateOnSceneLoad && _fileToLoad.Length != 0)
        {
            Debug.Log("Scene '" + scene.name + "' loaded. Loading game state from save file...");
            LoadGameState(_fileToLoad);
            _loadGameStateOnSceneLoad = false;
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

        SerializationContainer container = new SerializationContainer(stateData);
        string json = JsonUtility.ToJson(container, true);

        // string folderPath = Path.Combine(Application.persistentDataPath, "Scripts", "Serialization");
        // if (!Directory.Exists(folderPath))
        // {
        //     Directory.CreateDirectory(folderPath);
        // }

        string filePath = overwriteSave ?
            Path.Combine(Application.persistentDataPath, "Autosave.json") :
            Path.Combine(Application.persistentDataPath, $"Save_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        File.WriteAllText(filePath, json);
        Debug.Log("Game saved successfully to " + filePath);
    }

    public void LoadGameState(string fileName)
    {
        //string folderPath = Path.Combine(Application.dataPath, "Scripts", "Serialization");
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
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
