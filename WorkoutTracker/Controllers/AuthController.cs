using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace WorkoutTracker.Controllers
{
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected readonly UserManager<IdentityUser> _userManager;

        public AuthController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected string? GetCurrentUser()
        {
            return _userManager.GetUserId(User);
        }

        // POST: /logout
        [Route("logout")]
        [HttpPost]
        public async Task<ActionResult<IdentityUser>> Logout(SignInManager<IdentityUser> signInManager, object empty)
        {
            if (empty != null)
            {
                await signInManager.SignOutAsync();
                return Ok();
            }
            return Unauthorized();
        }
    }
}

