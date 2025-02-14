using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Controller
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            PlayerInput = new PlayerInput();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnPause, DisableInput);
            EventManager.Subscribe(EventTypes.OnUnpause, EnableInput);

        }

        private void DisableInput()
        {
            PlayerInput.Disable();
        }
        
        private void EnableInput()
        {
            PlayerInput.Enable();
        }
    }
}
