namespace BookProject.Interface
{
    public interface IRedisCacheService
    {
        Task SaveAsync(string key, object value);
        Task<T> GetAsync<T>(string key);
        Task<bool> DeleteAsync(string key);

    }
}
