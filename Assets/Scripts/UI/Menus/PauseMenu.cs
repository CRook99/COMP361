using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
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
            Debug.Log("Save and quit");
            // Saving logic
            // Scene transition logic
        }
    }
}

