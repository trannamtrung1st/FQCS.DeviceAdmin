﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business.Helpers;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Queries;
using FQCS.DeviceAdmin.Data;
using FQCS.DeviceAdmin.Data.Models;
using TNT.Core.Helpers.DI;
using static FQCS.DeviceAdmin.Business.Constants;

namespace FQCS.DeviceAdmin.Business.Services
{
    public interface IIdentityService
    {
        IQueryable<AppRole> Roles { get; }
        IQueryable<AppUser> Users { get; }

        Task<IdentityResult> AddLoginToUserAsync(AppUser entity, ExternalLoginInfo info);
        Task<IdentityResult> AddRolesForUserAsync(AppUser entity, IEnumerable<string> roles);
        Task<AppUser> AuthenticateAsync(string username, string password);
        Task<AppUser> AuthenticateExternalAsync(string provider, string providerKey);
        string ComputeHash(string dateTimeStr, string dateFormat, string secret);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null);
        AppUser ConvertToUser(CreateAppUserModel model);
        AppUser ConvertToUser(RegisterModel model);
        AppUser CreateAppUser(CreateAppUserModel model);
        Task<IdentityResult> CreateRoleAsync(CreateRoleModel model);
        Task<IdentityResult> CreateUserAsync(AppUser entity, string password);
        Task<IdentityResult> CreateUserWithRolesTransactionAsync(AppUser entity, string password, IEnumerable<string> roles = null);
        AppUser DeleteAppUser(AppUser entity);
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor);
        string[] ExtractInfoFromClientAuthHeader(string authHeader);
        TokenResponseModel GenerateTokenResponse(ClaimsPrincipal principal, AuthenticationProperties properties, string scope = null);
        Task<ClaimsPrincipal> GetApplicationPrincipalAsync(AppUser entity);
        IDictionary<string, object> GetAppUserDynamic(AppUser row, AppUserQueryProjection projection, AppUserQueryOptions options);
        List<IDictionary<string, object>> GetAppUserDynamic(IEnumerable<AppUser> rows, AppUserQueryProjection projection, AppUserQueryOptions options);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null);
        List<Claim> GetExtraClaims(AppUser entity);
        Task<ClaimsIdentity> GetIdentityAsync(AppUser entity, string scheme);
        AppRole GetRoleByName(string name);
        Task<AppUser> GetUserByIdAsync(string id);
        Task<AppUser> GetUserByUserNameAsync(string username);
        object GetUserProfile(AppUser entity);
        bool IsValidAppClientScheme(string authHeader);
        Task<SignInResult> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure);
        void PrepareUpdate(AppUser entity);
        Task<QueryResult<IDictionary<string, object>>> QueryAppUserDynamic(AppUserQueryProjection projection, AppUserQueryOptions options, AppUserQueryFilter filter = null, AppUserQuerySort sort = null, AppUserQueryPaging paging = null);
        Task<IdentityResult> RemoveRoleAsync(AppRole entity);
        Task<IdentityResult> RemoveUserFromRolesAsync(AppUser entity, IEnumerable<string> roles);
        Task SignInAsync(AppUser user, AuthenticationProperties props);
        Task SignInWithExtraClaimsAsync(AppUser entity, bool isPersistent);
        Task SignOutAsync();
        void UpdateAppUser(AppUser entity, UpdateAppUserModel model);
        Task<IdentityResult> UpdatePasswordIfAvailable(AppUser entity, UpdateAppUserModel model);
        Task<IdentityResult> UpdateRoleAsync(AppRole entity, UpdateRoleModel model);
        Task<IdentityResult> UpdateUserAsync(AppUser entity);
        ValidationData ValidateClientRequest(string dateTimeStr, string dateFormat, string hashed, AppClient client);
        ValidationData ValidateCreateAppUser(ClaimsPrincipal principal, CreateAppUserModel model);
        ValidationData ValidateCreateRole(ClaimsPrincipal principal, CreateRoleModel model);
        ValidationData ValidateDeleteAppUser(ClaimsPrincipal principal, AppUser entity);
        ValidationData ValidateDeleteRole(ClaimsPrincipal principal, AppRole entity);
        ValidationData ValidateGetAppUsers(ClaimsPrincipal principal, AppUserQueryFilter filter, AppUserQuerySort sort, AppUserQueryProjection projection, AppUserQueryPaging paging, AppUserQueryOptions options);
        ValidationData ValidateGetProfile(ClaimsPrincipal principal);
        ValidationData ValidateGetRoles(ClaimsPrincipal principal);
        ValidationData ValidateLogin(ClaimsPrincipal principal, AuthorizationGrantModel model);
        ClaimsPrincipal ValidateRefreshToken(string tokenStr);
        ValidationData ValidateRegister(ClaimsPrincipal principal, RegisterModel model);
        ValidationData ValidateUpdateAppUser(ClaimsPrincipal principal, AppUser entity, UpdateAppUserModel model);
        ValidationData ValidateUpdateRole(ClaimsPrincipal principal, UpdateRoleModel model);
    }

    public class IdentityService : Service, IIdentityService
    {
        [Inject]
        private readonly UserManager<AppUser> _userManager;
        [Inject]
        private readonly SignInManager<AppUser> _signInManager;
        [Inject]
        private readonly RoleManager<AppRole> _roleManager;

        public IdentityService(ServiceInjection inj) : base(inj)
        {
        }

        #region App client
        public string[] ExtractInfoFromClientAuthHeader(string authHeader)
        {
            if (IsValidAppClientScheme(authHeader))
                return authHeader.Split(' ')[1]
                    .Split(Constants.Authorization.APP_CLIENT_INFO_SPLIT);
            throw new Exception("Invalid scheme");
        }
        public string ComputeHash(string dateTimeStr, string dateFormat, string secret)
        {
            return CryptoHelper.HMACSHA256(dateTimeStr + dateFormat, secret);
        }
        #endregion

        #region Query AppUser
        public IQueryable<AppUser> Users
        {
            get
            {
                return context.Users.AsQueryable();
            }
        }

        public IDictionary<string, object> GetAppUserDynamic(
            AppUser row, AppUserQueryProjection projection,
            AppUserQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case AppUserQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["username"] = entity.UserName;
                            obj["full_name"] = entity.FullName;
                        }
                        break;
                    case AppUserQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["username"] = entity.UserName;
                            obj["full_name"] = entity.FullName;
                        }
                        break;
                    case AppUserQueryProjection.ROLE:
                        {
                            var entity = row.UserRoles.FirstOrDefault()?.Role;
                            if (entity != null)
                                obj["role"] = new
                                {
                                    id = entity.Id,
                                    name = entity.Name
                                };
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetAppUserDynamic(
            IEnumerable<AppUser> rows, AppUserQueryProjection projection,
            AppUserQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetAppUserDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryAppUserDynamic(
            AppUserQueryProjection projection,
            AppUserQueryOptions options,
            AppUserQueryFilter filter = null,
            AppUserQuerySort sort = null,
            AppUserQueryPaging paging = null)
        {
            var query = Users;
            #region General
            if (filter != null) query = query.Filter(filter);
            query = query.Project(projection);
            int? totalCount = null;
            if (options.count_total) totalCount = query.Count();
            #endregion
            if (!options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !AppUserQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetAppUserDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetAppUserDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Role
        public IQueryable<AppRole> Roles
        {
            get
            {
                return _roleManager.Roles;
            }
        }

        public AppRole GetRoleByName(string name)
        {
            return Roles.FirstOrDefault(r => r.Name == name);
        }

        public async Task<IdentityResult> RemoveRoleAsync(AppRole entity)
        {
            return await _roleManager.DeleteAsync(entity);
        }

        protected void PrepareCreate(AppRole entity)
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        public async Task<IdentityResult> CreateRoleAsync(CreateRoleModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            var result = await _roleManager.CreateAsync(entity);
            return result;
        }

        public async Task<IdentityResult> UpdateRoleAsync(AppRole entity,
            UpdateRoleModel model)
        {
            model.CopyTo(entity);
            var result = await _roleManager.UpdateAsync(entity);
            return result;
        }

        public ValidationData ValidateGetProfile(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateGetRoles(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateRole(
            ClaimsPrincipal principal, CreateRoleModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateRole(
            ClaimsPrincipal principal, UpdateRoleModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteRole(
            ClaimsPrincipal principal, AppRole entity)
        {
            return new ValidationData();
        }
        #endregion

        #region User
        public AppUser ConvertToUser(RegisterModel model)
        {
            var entity = new AppUser { UserName = model.username, FullName = model.full_name };
            return entity;
        }

        public AppUser ConvertToUser(CreateAppUserModel model)
        {
            var entity = new AppUser { UserName = model.UserName, FullName = model.FullName };
            return entity;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<SignInResult> PasswordSignInAsync(
            string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var result = await _signInManager.PasswordSignInAsync(userName: username,
                   password,
                   isPersistent, lockoutOnFailure);
            return result;
        }

        public async Task SignInAsync(AppUser user, AuthenticationProperties props)
        {
            await _signInManager.SignInAsync(user: user, authenticationProperties: props);
        }

        public List<Claim> GetExtraClaims(AppUser entity)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(Constants.AppClaimType.UserName, entity.UserName));
            claims.Add(new Claim(Constants.AppClaimType.FullName, entity.FullName ?? ""));
            return claims;
        }

        public async Task SignInWithExtraClaimsAsync(AppUser entity, bool isPersistent)
        {
            var extraClaims = GetExtraClaims(entity);
            //SignInWithClaimsAsync: for additional claims
            await _signInManager.SignInWithClaimsAsync(user: entity,
                isPersistent: isPersistent, extraClaims);
        }

        protected void PrepareCreate(AppUser entity)
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public object GetUserProfile(AppUser entity)
        {
            return new
            {
                full_name = entity.FullName,
                email = entity.Email,
                id = entity.Id,
                phone = entity.PhoneNumber
            };
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser entity)
        {
            return await _userManager.UpdateAsync(entity);
        }

        public async Task<IdentityResult> AddRolesForUserAsync(AppUser entity, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(entity, roles);
        }

        public async Task<IdentityResult> RemoveUserFromRolesAsync(AppUser entity, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(entity, roles);
        }

        public async Task<IdentityResult> CreateUserAsync(AppUser entity, string password)
        {
            PrepareCreate(entity);
            var result = await _userManager.CreateAsync(entity, password);
            return result;
        }

        public async Task<IdentityResult> CreateUserWithRolesTransactionAsync(AppUser entity, string password,
            IEnumerable<string> roles = null)
        {
            PrepareCreate(entity);
            var result = await _userManager.CreateAsync(entity, password);
            if (!result.Succeeded)
                return result;
            if (roles != null)
                result = await _userManager.AddToRolesAsync(entity, roles);
            return result;
        }

        public ValidationData ValidateLogin(
            ClaimsPrincipal principal, AuthorizationGrantModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateRegister(
            ClaimsPrincipal principal, RegisterModel model)
        {
            var validationData = new ValidationData();
            if (Users.Any())
                validationData.Fail("Access denied", AppResultCode.AccessDenied);
            return validationData;
        }
        #endregion

        #region OAuth
        public TokenResponseModel GenerateTokenResponse(ClaimsPrincipal principal,
            AuthenticationProperties properties, string scope = null)
        {
            #region Generate Constants.JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.Default.GetBytes(Constants.JWT.SECRET_KEY);
            var issuer = Constants.JWT.ISSUER;
            var audience = Constants.JWT.AUDIENCE;
            var identity = principal.Identity as ClaimsIdentity;
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, principal.Identity.Name));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = properties.IssuedUtc?.UtcDateTime,
                Expires = properties.ExpiresUtc?.UtcDateTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                NotBefore = properties.IssuedUtc?.UtcDateTime
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            #endregion
            var resp = new TokenResponseModel();
            resp.user_id = identity.Name;
            resp.access_token = tokenString;
            resp.token_type = "bearer";
            if (properties.ExpiresUtc != null)
                resp.expires_utc = properties.ExpiresUtc?.ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (properties.IssuedUtc != null)
                resp.issued_utc = properties.IssuedUtc?.ToString("yyyy-MM-ddTHH:mm:ssZ");
            #region Handle scope
            resp.roles = identity.FindAll(identity.RoleClaimType)
                                .Select(c => c.Value).ToList();
            if (scope != null)
            {
                var scopes = scope.Split(' ');
                foreach (var s in scopes)
                {
                    //switch (s)
                    //{
                    //    case Constants.AppOAuthScope.ROLES:
                    //        resp.roles = identity.FindAll(identity.RoleClaimType)
                    //            .Select(c => c.Value).ToList();
                    //        break;
                    //}
                }
            }
            #endregion
            #region Refresh Token
            key = Encoding.Default.GetBytes(Constants.JWT.REFRESH_SECRET_KEY);
            issuer = Constants.JWT.REFRESH_ISSUER;
            audience = Constants.JWT.REFRESH_AUDIENCE;
            var id = identity.Name;
            identity = new ClaimsIdentity(
                identity.Claims.Where(c => c.Type == identity.NameClaimType),
                identity.AuthenticationType);

            var refresh_expires = (properties.Parameters["refresh_expires"]
                as DateTimeOffset?)?.UtcDateTime;
            tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = properties.IssuedUtc?.UtcDateTime,
                Expires = refresh_expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                NotBefore = properties.IssuedUtc?.UtcDateTime
            };

            token = tokenHandler.CreateToken(tokenDescriptor);
            tokenString = tokenHandler.WriteToken(token);
            resp.refresh_token = tokenString;
            #endregion
            return resp;
        }

        public ClaimsPrincipal ValidateRefreshToken(string tokenStr)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken secToken;
                var param = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Constants.JWT.REFRESH_ISSUER,
                    ValidAudience = Constants.JWT.REFRESH_AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(Constants.JWT.REFRESH_SECRET_KEY)),
                    ClockSkew = TimeSpan.Zero
                };
                return tokenHandler.ValidateToken(tokenStr, param, out secToken);
            }
            catch (Exception) { }
            return null;
        }

        public async Task<AppUser> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
                return null;
            return user;
        }

        public async Task<ClaimsIdentity> GetIdentityAsync(AppUser entity, string scheme)
        {
            var identity = new ClaimsIdentity(scheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, entity.Id));
            var claims = await _userManager.GetClaimsAsync(entity);
            var roles = await _userManager.GetRolesAsync(entity);
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));
            identity.AddClaims(claims);
            claims = GetExtraClaims(entity);
            identity.AddClaims(claims);
            return identity;
        }

        //for IdentityCookie
        public async Task<ClaimsPrincipal> GetApplicationPrincipalAsync(AppUser entity)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(entity);
            var identity = principal.Identity as ClaimsIdentity;
            identity.TryRemoveClaim(identity.FindFirst(ClaimTypes.Name));
            identity.AddClaim(new Claim(ClaimTypes.Name, entity.Id));
            var claims = GetExtraClaims(entity);
            var roles = await _userManager.GetRolesAsync(entity);
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));
            identity.AddClaims(claims);
            return principal;
        }
        #endregion

        #region External Login
        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            return await _signInManager.GetExternalLoginInfoAsync(expectedXsrf);
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            return _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
        }

        public async Task<AppUser> AuthenticateExternalAsync(string provider, string providerKey)
        {
            var user = await _userManager.FindByLoginAsync(provider, providerKey);
            return user;
        }

        public async Task<IdentityResult> AddLoginToUserAsync(AppUser entity, ExternalLoginInfo info)
        {
            var result = await _userManager.AddLoginAsync(entity, info);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Unexpected error occurred adding external login for user with ID '{entity.Id}'.");
            return result;
        }
        #endregion

        #region Create AppUser
        public AppUser CreateAppUser(CreateAppUserModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Users.Add(entity).Entity;
        }
        #endregion

        #region Update AppUser
        public void PrepareUpdate(AppUser entity)
        {
        }

        public void UpdateAppUser(AppUser entity, UpdateAppUserModel model)
        {
            model.CopyTo(entity);
            PrepareUpdate(entity);
        }

        public async Task<IdentityResult> UpdatePasswordIfAvailable(AppUser entity, UpdateAppUserModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.PasswordReset))
            {
                var resetPassToken = await _userManager.GeneratePasswordResetTokenAsync(entity);
                var result = await _userManager.ResetPasswordAsync(entity, resetPassToken, model.PasswordReset);
                return result;
            }
            return null;
        }
        #endregion

        #region Delete AppUser
        public AppUser DeleteAppUser(AppUser entity)
        {
            return context.Users.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public bool IsValidAppClientScheme(string authHeader)
        {
            return authHeader.StartsWith(Constants.Authorization.CLIENT_AUTH_SCHEME + " ");
        }

        public ValidationData ValidateClientRequest(string dateTimeStr, string dateFormat,
            string hashed, AppClient client)
        {
            var validationData = new ValidationData();
            DateTime dateTime;
            if (!dateTimeStr.TryConvertToDateTime(dateFormat, out dateTime))
                return validationData.Fail(mess: "Wrong date format", code: Constants.AppResultCode.FailValidation);
            var diffMins = Math.Abs(DateTime.UtcNow.Subtract(dateTime).TotalMinutes);
            if (diffMins >= Settings.Instance.RequestMinsDiffAllowed)
                return validationData.Fail(mess: "Invalid request time", code: Constants.AppResultCode.FailValidation);
            var serverHashed = ComputeHash(dateTimeStr, dateFormat, client.SecretKey);
            if (serverHashed != hashed)
                return validationData.Fail(mess: "Invalid hashed", code: Constants.AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateGetAppUsers(
            ClaimsPrincipal principal,
            AppUserQueryFilter filter,
            AppUserQuerySort sort,
            AppUserQueryProjection projection,
            AppUserQueryPaging paging,
            AppUserQueryOptions options)
        {
            return new ValidationData();
        }


        public ValidationData ValidateCreateAppUser(ClaimsPrincipal principal,
            CreateAppUserModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.Role)
                || !Data.Constants.RoleName.ALL.Contains(model.Role))
                validationData = validationData.Fail("Invalid role", code: Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password)
                || model.UserName.Length < 3 || model.Password.Length < 6)
                validationData = validationData.Fail("Invalid username or password", code: Constants.AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateUpdateAppUser(ClaimsPrincipal principal,
            AppUser entity, UpdateAppUserModel model)
        {
            var validationData = new ValidationData();
            if (entity.UserRoles.First().Role.Name == Data.Constants.RoleName.ADMIN
                && principal.Identity.Name != entity.Id)
                validationData = validationData.Fail(code: AppResultCode.AccessDenied);
            if (!string.IsNullOrWhiteSpace(model.PasswordReset) && model.PasswordReset.Length < 6)
                validationData = validationData.Fail("Invalid password reset", code: Constants.AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateDeleteAppUser(ClaimsPrincipal principal,
            AppUser entity)
        {
            if (entity.UserRoles.FirstOrDefault()?.Role.Name == Data.Constants.RoleName.ADMIN)
                return new ValidationData().Fail(code: AppResultCode.AccessDenied);
            return new ValidationData();
        }
        #endregion

    }
}
