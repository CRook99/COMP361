using System;
using DG.Tweening;
using Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class AllyStatusActionWidget : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Color inactiveColor;
        
        public ActionScriptableObject Data;

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void Activate()
        {
            icon.color = Color.white;
        }
        
        public void Deactivate()
        {
            icon.color = inactiveColor;
        }
    }
}