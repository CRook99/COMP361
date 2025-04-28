namespace Utility.Serialization
{

    public interface IGameSerializable
    {
        bool Validate();

        string Serialize();

        void Deserialize(string json);
    }
}
