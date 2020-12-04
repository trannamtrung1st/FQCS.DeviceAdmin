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
    [Route(Business.Constants.ApiEndpoint.DEVICE_CONFIG_API)]
    [ApiController]
    [InjectionFilter]
    public class DeviceConfigsController : BaseController
    {
        [Inject]
        private readonly IDeviceConfigService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]DeviceConfigQueryFilter filter,
            [FromQuery]DeviceConfigQuerySort sort,
            [FromQuery]DeviceConfigQueryProjection projection,
            [FromQuery]DeviceConfigQueryPaging paging,
            [FromQuery]DeviceConfigQueryOptions options)
        {
            var validationData = _service.ValidateGetDeviceConfigs(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryDeviceConfigDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize(Roles = Data.Constants.RoleName.ADMIN)]
        [HttpPost("")]
        public IActionResult Create(CreateDeviceConfigModel model)
        {
            var validationData = _service.ValidateCreateDeviceConfig(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateDeviceConfig(model);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize(Roles = Data.Constants.RoleName.ADMIN)]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateDeviceConfigModel model)
        {
            var entity = _service.DeviceConfigs.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateDeviceConfig(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateDeviceConfig(entity, model);
            context.SaveChanges();
            if (entity.IsCurrent)
                Startup.SetCurrentConfig(entity);
            return NoContent();
        }

        [Authorize(Roles = Data.Constants.RoleName.ADMIN)]
        [HttpPost("current")]
        public IActionResult ChangeCurrentDeviceConfig(ChangeCurrentDeviceConfigModel model)
        {
            var entity = _service.DeviceConfigs.Id(model.ConfigId).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateChangeCurrentDeviceConfig(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var oldCurrent = _service.DeviceConfigs.IsCurrent().FirstOrDefault();
            _service.ChangeCurrentDeviceConfig(entity, oldCurrent);
            context.SaveChanges();
            Startup.SetCurrentConfig(entity);
            return NoContent();
        }

        [Authorize(Roles = Data.Constants.RoleName.ADMIN)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var entity = _service.DeviceConfigs.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteDeviceConfig(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteDeviceConfig(entity);
                context.SaveChanges();
                if (entity.IsCurrent)
                    Startup.SetCurrentConfig(null);
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