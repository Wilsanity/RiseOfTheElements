using System.Collections.Generic;

namespace Kibo.Data
{
    public interface IDataService<T> where T : class
    {
        IEnumerable<string> Saves { get; }

        void Save(T data, bool overwrite = true);

        T Load(string name);

        void Delete(string name);

        void DeleteAll();
    }
}