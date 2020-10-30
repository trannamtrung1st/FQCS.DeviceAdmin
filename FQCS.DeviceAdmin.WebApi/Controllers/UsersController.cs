using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using FQCS.DeviceAdmin.Business;
using FQCS.DeviceAdmin.Business.Helpers;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Helpers;
using FQCS.DeviceAdmin.Data.Models;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;

namespace FQCS.DeviceAdmin.WebApi.Controllers
{

    [Route(Business.Constants.ApiEndpoint.USER_API)]
    [ApiController]
    [InjectionFilter]
    public class UsersController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly IdentityService _service;

        #region OAuth
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromForm]AuthorizationGrantModel model)
        {
            var validationData = _service.ValidateLogin(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            AppUser entity = null;
            switch (model.grant_type)
            {
                case "password":
                case null:
                    {
                        entity = await
                            _service.AuthenticateAsync(model.username, model.password);
                        if (entity == null)
                        {
                            return Unauthorized(AppResult
                                .Unauthorized(mess: "Invalid username or password"));
                        }
                    }
                    break;
                case "refresh_token":
                    {
                        var validResult = _service.ValidateRefreshToken(model.refresh_token);
                        if (validResult == null)
                        {
                            return Unauthorized(AppResult
                                .Unauthorized(mess: "Invalid refresh token"));
                        }
                        entity = await _service.GetUserByIdAsync(validResult.Identity.Name);
                        if (entity == null)
                        {
                            return Unauthorized(AppResult
                                .Unauthorized(mess: "Invalid user identity"));
                        }
                    }
                    break;
                default:
                    return BadRequest(AppResult
                        .Unsupported("Unsupported grant type"));
            }
            var identity =
                await _service.GetIdentityAsync(entity, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var utcNow = DateTime.UtcNow;
            var props = new AuthenticationProperties()
            {
                IssuedUtc = utcNow,
                ExpiresUtc = utcNow.AddHours(WebApi.Settings.Instance.TokenValidHours)
            };
            props.Parameters["refresh_expires"] = utcNow.AddHours(
                WebApi.Settings.Instance.RefreshTokenValidHours);
            var resp = _service.GenerateTokenResponse(principal, props, model.scope);
            _logger.CustomProperties(entity).Info("Login user");
            return Ok(resp);
        }
        #endregion

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]AppUserQueryFilter filter,
            [FromQuery]AppUserQuerySort sort,
            [FromQuery]AppUserQueryProjection projection,
            [FromQuery]AppUserQueryPaging paging,
            [FromQuery]AppUserQueryOptions options)
        {
            var validationData = _service.ValidateGetAppUsers(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryAppUserDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [HttpGet("token-info")]
        [Authorize]
        public IActionResult GetTokenInfo()
        {
            var resp = new TokenInfo(User);
            return Ok(AppResult.Success(resp));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var validationData = _service.ValidateGetProfile(User);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = await _service.GetUserByIdAsync(UserId);
            var data = _service.GetUserProfile(entity);
            return Ok(AppResult.Success(data));
        }

        [HttpPost("")]
        public async Task<IActionResult> Register([FromForm]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var validationData = _service.ValidateRegister(User, model);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                IdentityResult result;
                using (var trans = context.Database.BeginTransaction())
                {
                    var entity = _service.ConvertToUser(model);
                    result = await _service
                        .CreateUserWithRolesTransactionAsync(entity, model.password);
                    if (result.Succeeded)
                    {
                        trans.Commit();
                        _logger.CustomProperties(entity).Info("Register new user");
                        return NoContent();
                    }
                }
                foreach (var err in result.Errors)
                    ModelState.AddModelError(err.Code, err.Description);
            }
            var appResult = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
            return BadRequest(appResult);
        }

#if DEBUG
        #region Administration
        [HttpPost("role")]
        public async Task<IActionResult> AddRole(AddRolesToUserModel model)
        {
            var entity = await _service.GetUserByUserNameAsync(model.username);
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var result = await _service.AddRolesForUserAsync(entity, model.roles);
            if (result.Succeeded)
                return NoContent();
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(AppResult.FailValidation(ModelState));
        }

        [HttpDelete("role")]
        public async Task<IActionResult> RemoveRole(RemoveRolesFromUserModel model)
        {
            var entity = await _service.GetUserByUserNameAsync(model.username);
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var result = await _service.RemoveUserFromRolesAsync(entity, model.roles);
            if (result.Succeeded)
                return NoContent();
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(AppResult.FailValidation(ModelState));
        }
        #endregion
#endif
    }
}
