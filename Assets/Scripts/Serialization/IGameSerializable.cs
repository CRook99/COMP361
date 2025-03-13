using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace Utility.Serialization
{

    public interface IGameSerializable
    {
        bool Validate();

        string Serialize();

        void Deserialize(string json);
    }
}
