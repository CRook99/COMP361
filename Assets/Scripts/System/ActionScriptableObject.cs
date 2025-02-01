using Entities;
using UnityEngine;

namespace System
{
    [CreateAssetMenu(fileName = "Action", menuName = "ScriptableObjects/Action")]
    public class ActionScriptableObject : ScriptableObject
    {
        public Sprite Icon;
        public ActionType Type;
    }
}