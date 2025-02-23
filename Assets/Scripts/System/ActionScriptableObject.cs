using Entities;
using UI.BottomWidgets;
using UnityEngine;

namespace System
{
    [CreateAssetMenu(fileName = "Action", menuName = "ScriptableObjects/Action")]
    public class ActionScriptableObject : ScriptableObject
    {
        public Sprite Icon;
        public ActionType Type;
        public string DisplayName;
        public EBottomWidget WidgetType;
    }
}