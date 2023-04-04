using blog_api.models.DTOs.Http;
using System.Threading.Tasks;

namespace blog_api.services.services
{
    public interface IUserService
    {
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<UserResDto> CreateAsync(UserReqDto user);

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<UserResDto> UpdateAsync(UserUpdateReqpDto user);
    }
}
