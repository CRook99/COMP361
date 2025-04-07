using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuScript : MonoBehaviour
    {

        private static readonly string COMBAT_SCENE = "Combat"; 


        public void OnClickNewGameButton()
        {
            Debug.Log("New game");
            // Scene transition logic
            SceneManager.LoadSceneAsync("Equipment");
        }

        public void OnClickLoadGameButton()
        {
            Debug.Log("Load game");
            
            GameState.Instance.PrepareForLoadGameState();

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

