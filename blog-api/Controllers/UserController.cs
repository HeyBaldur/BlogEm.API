using blog_api.models.DTOs.Http;
using blog_api.services.services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace blog_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService bookService)
        {
            _userService = bookService;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(UserReqDto user)
        {
            UserResDto response = await _userService.CreateAsync(user);

            return Ok(response);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("user")]
        public async Task<IActionResult> Update(UserUpdateReqpDto user)
        {
            var result = await _userService.UpdateAsync(user);

            return Ok(result);
        }
    }
}
