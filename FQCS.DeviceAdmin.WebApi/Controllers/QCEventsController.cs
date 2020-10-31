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
    [Route(Business.Constants.ApiEndpoint.QC_EVENT_API)]
    [ApiController]
    [InjectionFilter]
    public class QCEventsController : BaseController
    {
        [Inject]
        private readonly QCEventService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize(Policy = Constants.Policy.Or.APP_CLIENT)]
        [Authorize(Policy = Constants.Policy.Or.AUTH_USER)]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]QCEventQueryFilter filter,
            [FromQuery]QCEventQuerySort sort,
            [FromQuery]QCEventQueryProjection projection,
            [FromQuery]QCEventQueryPaging paging,
            [FromQuery]QCEventQueryOptions options)
        {
            var validationData = _service.ValidateGetQCEvents(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryQCEventDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateQCEventModel model)
        {
            var validationData = _service.ValidateCreateQCEvent(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateQCEvent(model);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

    }
}