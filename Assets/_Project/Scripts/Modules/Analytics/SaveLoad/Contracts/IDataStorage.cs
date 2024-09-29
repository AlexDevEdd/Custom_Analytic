using Cysharp.Threading.Tasks;

namespace Analytics
{
    public interface IDataStorage
    {
        public UniTask<string> ReadAsync<TData>(string key);
        public UniTask WriteAsync(string key, string data);
        public void Remove(string key);
    }
}