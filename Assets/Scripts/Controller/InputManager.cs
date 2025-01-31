using System.Collections;
using System.Collections.Generic;
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
    }
}
