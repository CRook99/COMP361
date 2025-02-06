using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MainMenuScript : MonoBehaviour
    {
        public void OnClickNewGameButton()
        {
            Debug.Log("New game");
            // Scene transition logic
        }

        public void OnClickLoadGameButton()
        {
            Debug.Log("Load game");
            // Scene transition logic
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

