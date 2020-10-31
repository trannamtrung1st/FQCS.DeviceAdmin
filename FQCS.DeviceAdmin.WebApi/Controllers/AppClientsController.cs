using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TNT.Core.Http.DI;
using TNT.Core.Helpers.DI;
using System.Data.SqlClient;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Queries;
using Microsoft.EntityFrameworkCore;
using FQCS.DeviceAdmin.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using FQCS.DeviceAdmin.Business.Helpers;

namespace FQCS.DeviceAdmin.WebApi.Controllers
{
    [Route(Business.Constants.ApiEndpoint.APP_CLIENT_API)]
    [ApiController]
    [InjectionFilter]
    public class AppClientsController : BaseController
    {
        [Inject]
        private readonly AppClientService _service;
        [Inject]
        private readonly IdentityService _identityService;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]AppClientQueryFilter filter,
            [FromQuery]AppClientQuerySort sort,
            [FromQuery]AppClientQueryProjection projection,
            [FromQuery]AppClientQueryPaging paging,
            [FromQuery]AppClientQueryOptions options)
        {
            var validationData = _service.ValidateGetAppClients(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryAppClientDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

#if !RELEASE
        [HttpGet("test-client-info")]
        public IActionResult GetTestClientInfo(string clientId)
        {
            var client = _service.AppClients.Id(clientId).First();
            var now = DateTime.UtcNow;
            var df = "ddMMyyyyHHmmss";
            var dtStr = now.ToString(df);
            var hashed = _identityService.ComputeHash(dtStr, df, client.SecretKey);
            return Ok(AppResult.Success($"AppClient {clientId}!{dtStr}!{df}!{hashed}"));
        }

        [Authorize(Policy = Constants.Policy.And.APP_CLIENT)]
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            var clientId = HttpContext.Items[Constants.RequestItemKey.CLIENT_ID] as string;
            var secret = _service.AppClients.Id(clientId).First().SecretKey;
            return Ok(AppResult.Success(secret));
        }
#endif

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateAppClientModel model)
        {
            var validationData = _service.ValidateCreateAppClient(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateAppClient(model);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(string id, UpdateAppClientModel model)
        {
            var entity = _service.AppClients.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateAppClient(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateAppClient(entity, model);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var entity = _service.AppClients.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteAppClient(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteAppClient(entity);
                context.SaveChanges();
                return NoContent();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
                return BadRequest(AppResult.DependencyDeleteFail());
            }
        }

    }
}