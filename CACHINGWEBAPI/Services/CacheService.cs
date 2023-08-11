using StackExchange.Redis;
using System.Text.Json;

namespace CACHINGWEBAPI.Services
{
    public class CacheService : ICacheService  //implementing the interface
    {
        private IDatabase _cacheDb;

        public CacheService()   //creating a constructor
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");   //connecting actual instance of redies or actual end point of redies 
            _cacheDb = redis.GetDatabase();  //here we are initializing
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if(!string.IsNullOrEmpty(value))  //its a key value pair so I need to refer to the key I need to get inside form redis
                return JsonSerializer.Deserialize<T>(value);  // if not Iam gonna return the value (if we are taking a list of objects we gonna make sure the string is not empty if empty or null we are not gonna serialize)
                                                              //Onece we make sure not empty or null and we are able to serialize it we gonna serialize the type of object that the user has passed and the we gonna be return it
            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key); // if we gonna check weather the key exist or not before we remove anything (_cacheDb.KeyExists just like a build in functionality)

            if (_exist)
                return _cacheDb.KeyDelete(key);  //basically we are deleting the data from there 
                                                 //At first we check if the key exist is basically then we initiate the key delete functionality if not we just return false
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);       //set data we need to get the expirytime actually when we are setting the data
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value),expirtyTime); //we need to convert any object that we get into a string because that's really important we cannot store the object we need to store them in a string 
        }                                                                                // once we store the string we pass the expiry time, to convert them into string we need to serialize them
    }
}
