using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Transform = UnityEngine.Transform;

namespace UI
{
    public class MainMenuScript : MonoBehaviour
    {
        public GameObject SavePopup;
        public SaveItem SaveItemPrefab;
        public GameObject SavePopupContent;
        private static readonly string COMBAT_SCENE = "Combat";

        private void Awake()
        {
            SavePopup.SetActive(false);

            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                string json = File.ReadAllText(file);
                
                SerializationContainer container = JsonUtility.FromJson<SerializationContainer>(json);
                int enemyCount = 0;
                int turnNumber = -1;

                for (int i = 0; i < container.keys.Count; i++)
                {
                    string key = container.keys[i];

                    if (key.StartsWith("Enemy"))
                    {
                        enemyCount++;
                    }
                    else if (key == "TurnManager")
                    {
                        TurnDTO dto = JsonUtility.FromJson<TurnDTO>(container.values[i]);
                        turnNumber = dto.turnNumber;
                    }
                }

                SaveItem saveItem = Instantiate(SaveItemPrefab, SavePopupContent.transform);
                saveItem.LoadData(filename, turnNumber, enemyCount);
                saveItem.Button.onClick.AddListener(() => OnClickSaveItem(file));
            }
        }

        public void OnClickNewGameButton()
        {
            // Scene transition logic
            SceneManager.LoadSceneAsync("Equipment");
        }

        public void OnClickLoadGameButton()
        {
            SavePopup.SetActive(true);
        }

        public void CloseSavePopup()
        {
            SavePopup.SetActive(false);
        }

        public void ClearSaves()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");

            foreach (string file in files)
            {
                File.Delete(file);
            }

            foreach (Transform t in SavePopupContent.transform)
            {
                if (t == SavePopupContent.transform) continue;
                
                Destroy(t.gameObject);
            }
        }

        public void OnClickSaveItem(string saveName)
        {
            GameState.Instance.PrepareForLoadGameState(saveName);
            
            SceneManager.LoadSceneAsync(COMBAT_SCENE);
        }

        public void OnClickExitButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

