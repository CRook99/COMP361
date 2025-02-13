using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Utility.Serialization
{

    [Serializable]
    public abstract class GameSerializableBase : IGameSerializable
    {

        public abstract bool Validate();

        public string Serialize()
        {
            if (!Validate())
            {
                throw new InvalidOperationException("Object state is invalid for serialization.");
            }
            return JsonUtility.ToJson(this, true);
        }

        public void Deserialize(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
            if (!Validate())
            {
                throw new InvalidOperationException("Deserialized object state is invalid.");
            }
        }
    }
}
