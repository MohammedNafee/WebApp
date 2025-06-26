
namespace WebAppMVC.Data
{
    public interface IWebApiExecutor
    {
        Task InvokeDelete(string relativeUrl);
        Task<T?> InvokeGet<T>(string relativeUrl);
        Task InvokePost<T>(string relativeUrl, T obj);
        Task InvokePut<T>(string relativeUrl, T obj);
    }
}