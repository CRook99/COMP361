using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject toggleObject;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;
        
        private PlayerInput _playerInput;
        
        private void Start()
        {
            toggleObject.SetActive(false);
            
            _playerInput = InputManager.Instance.PlayerInput;
            _playerInput.Combat.Pause.performed += _ => Pause();
            resumeButton.onClick.AddListener(Unpause);
            quitButton.onClick.AddListener(OnSaveAndQuitButtonClicked);
        }

        private void Pause()
        {
            EventManager.TriggerEvent(EventTypes.OnPause);
            toggleObject.SetActive(true);
            Time.timeScale = 0f;
        }

        private void Unpause()
        {
            EventManager.TriggerEvent(EventTypes.OnUnpause);
            toggleObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public void OnSaveAndQuitButtonClicked()
        {
            GameState.Instance.SaveGameState(overwriteSave:false);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

