namespace Kibo.Data
{
    public interface ISerializer
    {
        string Serialize<T>(T dataObject);

        T Deserialize<T>(string stringData);
    }
}