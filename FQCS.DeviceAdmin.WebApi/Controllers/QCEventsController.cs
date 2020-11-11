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
using System.IO.Compression;
using System.IO;
using elFinder.NetCore.Models;
using System.Net.Mime;
using FQCS.DeviceAdmin.Data.Models;

namespace FQCS.DeviceAdmin.WebApi.Controllers
{
    [Route(Business.Constants.ApiEndpoint.QC_EVENT_API)]
    [ApiController]
    [InjectionFilter]
    public class QCEventsController : BaseController
    {
        [Inject]
        private readonly QCEventService _service;
        [Inject]
        private readonly FileService _fileService;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize(Policy = Constants.Policy.Or.APP_CLIENT)]
        [Authorize(Policy = Constants.Policy.Or.ADMIN_USER)]
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
                projection, options, filter, sort, paging, Settings.Instance.QCEventImageFolderPath);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }


        [Authorize(Policy = Constants.Policy.Or.APP_CLIENT)]
        [Authorize(Policy = Constants.Policy.Or.ADMIN_USER)]
        [HttpGet("count")]
        public IActionResult Count([FromQuery][QueryObject]QCEventQueryFilter filter,
            [FromQuery]QCEventQuerySort sort,
            [FromQuery]QCEventQueryPaging paging,
            [FromQuery]QCEventQueryOptions options)
        {
            var validationData = _service.ValidateCountQCEvents(
                User, filter, sort, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var query = _service.QueryQCEvent(options, filter, sort, paging);
            var count = query.Count();
            return Ok(AppResult.Success(count));
        }

        [Authorize(Policy = Constants.Policy.And.APP_CLIENT)]
        [HttpPut("sent-status")]
        public IActionResult UpdateSentStatus([FromQuery][QueryObject]QCEventQueryFilter filter,
            [FromQuery]QCEventQuerySort sort,
            [FromQuery]QCEventQueryPaging paging,
            [FromQuery]QCEventQueryOptions options)
        {
            var validationData = _service.ValidateUpdateSentStatus(
                User, filter, sort, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var query = _service.GetQueryableQCEventForUpdate(options, filter, sort, paging);
            var updatedIds = query.Select(o => o.Id).ToList();
            var updated = _service.UpdateEventsSentStatus(query, true);
            foreach (var id in updatedIds)
                QCEvent.CheckedEvents.Remove(id);
            return Ok(AppResult.Success(updated));
        }

        [Authorize(Policy = Constants.Policy.Or.APP_CLIENT)]
        [Authorize(Policy = Constants.Policy.Or.ADMIN_USER)]
        [HttpGet("images")]
        public IActionResult GetAllImages()
        {
            var validationData = _service.ValidateGetAllImages(User);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var tempPath = _fileService.GetLocalTempFilePath(ext: ".zip");
            try
            {
                _fileService.ZipFolderToDir(Settings.Instance.QCEventImageFolderPath, tempPath);
                return PhysicalFile(tempPath, MediaTypeNames.Application.Zip, "images.zip");
            }
            catch (Exception e)
            {
                _fileService.DeleteFile(tempPath, "");
                throw e;
            }
        }

        [Authorize(Policy = Constants.Policy.Or.APP_CLIENT)]
        [Authorize(Policy = Constants.Policy.Or.ADMIN_USER)]
        [HttpPost("send-events")]
        public async Task<IActionResult> SendUnsentEvents()
        {
            var validationData = _service.ValidateSendEvents(User);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var dt = await Startup.Scheduler.TriggerSendUnsentEventsJob(null);
            return Ok(AppResult.Success(dt));
        }


        [Authorize(Policy = Constants.Policy.Or.APP_CLIENT)]
        [Authorize(Policy = Constants.Policy.Or.ADMIN_USER)]
        [HttpPost("clear")]
        public async Task<IActionResult> ClearAllEvents()
        {
            var validationData = _service.ValidateClearAllEvents(User);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var task = _service.ClearAllQCEvents();
            _service.ClearAllQCEventImages(Settings.Instance.QCEventImageFolderPath);
            var deleted = await task;
            return Ok(AppResult.Success(deleted));
        }

        [Authorize(Policy = Constants.Policy.Or.APP_CLIENT)]
        [Authorize(Policy = Constants.Policy.Or.ADMIN_USER)]
        [HttpPost("")]
        public IActionResult Create(CreateQCEventModel model)
        {
            var validationData = _service.ValidateCreateQCEvent(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            DateTime createdTime;
            createdTime = validationData.GetTempData<DateTime>(nameof(createdTime));
            var entity = _service.CreateQCEvent(model, createdTime);
            context.SaveChanges();
            if (Startup.KafkaProducer != null)
                _service.ProduceEventToKafkaServer(Startup.KafkaProducer,
                    entity, Startup.CurrentConfig, Settings.Instance.QCEventImageFolderPath,
                    Startup.ConnStr);
            return Created($"/{Business.Constants.ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

    }
}