namespace CACHINGWEBAPI.Services
{
    public interface ICacheService  
    {
        T GetData<T>(string key);             // three methods in our interfae
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

        object RemoveData(string key);
    }
}
