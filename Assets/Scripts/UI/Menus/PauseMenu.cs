using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject toggleObject;
        
        private PlayerInput _playerInput;
        
        private void Start()
        {
            toggleObject.SetActive(false);
            
            _playerInput = InputManager.Instance.PlayerInput;
            _playerInput.Combat.Pause.performed += OnPause;
        }

        public void OnPause(InputAction.CallbackContext _)
        {
            EventManager.TriggerEvent(EventTypes.OnPause);
            toggleObject.SetActive(true);
            Time.timeScale = 0f;
        }

        public void OnResumeButtonClicked()
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

