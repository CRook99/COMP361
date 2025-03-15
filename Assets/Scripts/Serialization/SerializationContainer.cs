using System;
using System.Collections.Generic;

[Serializable]
public class SerializationContainer
{
    public List<string> keys;
    public List<string> values;

    public SerializationContainer(Dictionary<string, string> dict)
    {
        keys = new List<string>(dict.Keys);
        values = new List<string>(dict.Values);
    }

    // To rebuild the dictionary:
    public Dictionary<string, string> stateData
    {
        get
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < keys.Count; i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }
    }
}
