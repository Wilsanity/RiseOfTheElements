using UnityEngine;

namespace Kibo.Data
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T dataObject)
        {
            return JsonUtility.ToJson(dataObject);
        }

        public T Deserialize<T>(string stringData)
        {
            return JsonUtility.FromJson<T>(stringData);
        }
    }
}
