using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface ILoginService<T> where T : class, IUser<T>
{
     Task<bool> LoginAsync(LoginViewModel credentialsVM);
}
