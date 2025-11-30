namespace Spotless.Application.Interfaces
{
    public interface ICacheInvalidator
    {
        string[] CacheKeys { get; }
    }
}
