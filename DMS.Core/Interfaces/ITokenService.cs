using DMS.Core.Entities;

namespace DMS.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
