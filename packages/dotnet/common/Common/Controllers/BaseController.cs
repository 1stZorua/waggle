using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Auth;

namespace Waggle.Common.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the current user from the ClaimsPrincipal
        /// </summary>
        protected UserInfoDto CurrentUser => User.ToUserInfo();
    }
}
