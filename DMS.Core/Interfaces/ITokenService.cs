using DMS.Core.Entities;

namespace DMS.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
