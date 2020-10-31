using Microsoft.EntityFrameworkCore;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Queries;
using FQCS.DeviceAdmin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;
using FQCS.DeviceAdmin.Business.Helpers;

namespace FQCS.DeviceAdmin.Business.Services
{
    public class DeviceConfigService : Service
    {
        public DeviceConfigService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query DeviceConfig
        public IQueryable<DeviceConfig> DeviceConfigs
        {
            get
            {
                return context.DeviceConfig;
            }
        }

        public IDictionary<string, object> GetDeviceConfigDynamic(
            DeviceConfig row, DeviceConfigQueryProjection projection,
            DeviceConfigQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case DeviceConfigQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["identifier"] = entity.Identifier;
                            obj["is_current"] = entity.IsCurrent;
                            obj["kafka_server"] = entity.KafkaServer;
                            obj["kafka_username"] = entity.KafkaUsername;
                            var time = entity.CreatedTime
                                .ToDefaultTimeZone();
                            var timeStr = time.ToString(options.date_format);
                            obj["created_time"] = new
                            {
                                display = timeStr,
                                iso = $"{time.ToUniversalTime():s}Z"
                            };
                            time = entity.LastUpdated
                                .ToDefaultTimeZone();
                            timeStr = time.ToString(options.date_format);
                            obj["last_updated"] = new
                            {
                                display = timeStr,
                                iso = $"{time.ToUniversalTime():s}Z"
                            };
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetDeviceConfigDynamic(
            IEnumerable<DeviceConfig> rows, DeviceConfigQueryProjection projection,
            DeviceConfigQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetDeviceConfigDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryDeviceConfigDynamic(
            DeviceConfigQueryProjection projection,
            DeviceConfigQueryOptions options,
            DeviceConfigQueryFilter filter = null,
            DeviceConfigQuerySort sort = null,
            DeviceConfigQueryPaging paging = null)
        {
            var query = DeviceConfigs;
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
                if (paging != null && (!options.load_all || !DeviceConfigQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetDeviceConfigDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetDeviceConfigDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create DeviceConfig
        protected void PrepareCreate(DeviceConfig entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdated = entity.CreatedTime;
        }

        public DeviceConfig CreateDeviceConfig(CreateDeviceConfigModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.DeviceConfig.Add(entity).Entity;
        }
        #endregion

        #region Update DeviceConfig
        public DeviceConfig ChangeCurrentDeviceConfig(DeviceConfig entity, DeviceConfig oldCurrent)
        {
            entity.IsCurrent = true;
            PrepareUpdate(entity);
            if (oldCurrent != null)
            {
                oldCurrent.IsCurrent = false;
                PrepareUpdate(oldCurrent);
            }
            return entity;
        }

        protected void PrepareUpdate(DeviceConfig entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public void UpdateDeviceConfig(DeviceConfig entity, UpdateDeviceConfigModel model)
        {
            model.CopyTo(entity);
            PrepareUpdate(entity);
        }
        #endregion

        #region Delete DeviceConfig
        public DeviceConfig DeleteDeviceConfig(DeviceConfig entity)
        {
            return context.DeviceConfig.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetDeviceConfigs(
            ClaimsPrincipal principal,
            DeviceConfigQueryFilter filter,
            DeviceConfigQuerySort sort,
            DeviceConfigQueryProjection projection,
            DeviceConfigQueryPaging paging,
            DeviceConfigQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateDeviceConfig(ClaimsPrincipal principal,
            CreateDeviceConfigModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateDeviceConfig(ClaimsPrincipal principal,
            DeviceConfig entity, UpdateDeviceConfigModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateChangeCurrentDeviceConfig(ClaimsPrincipal principal,
            DeviceConfig entity, ChangeCurrentDeviceConfigModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteDeviceConfig(ClaimsPrincipal principal,
            DeviceConfig entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
