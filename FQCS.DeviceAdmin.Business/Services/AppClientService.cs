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
    public interface IAppClientService
    {
        IQueryable<AppClient> AppClients { get; }

        AppClient CreateAppClient(CreateAppClientModel model);
        AppClient DeleteAppClient(AppClient entity);
        IDictionary<string, object> GetAppClientDynamic(AppClient row, AppClientQueryProjection projection, AppClientQueryOptions options);
        List<IDictionary<string, object>> GetAppClientDynamic(IEnumerable<AppClient> rows, AppClientQueryProjection projection, AppClientQueryOptions options);
        Task<QueryResult<IDictionary<string, object>>> QueryAppClientDynamic(AppClientQueryProjection projection, AppClientQueryOptions options, AppClientQueryFilter filter = null, AppClientQuerySort sort = null, AppClientQueryPaging paging = null);
        void UpdateAppClient(AppClient entity, UpdateAppClientModel model);
        ValidationData ValidateCreateAppClient(ClaimsPrincipal principal, CreateAppClientModel model);
        ValidationData ValidateDeleteAppClient(ClaimsPrincipal principal, AppClient entity);
        ValidationData ValidateGetAppClients(ClaimsPrincipal principal, AppClientQueryFilter filter, AppClientQuerySort sort, AppClientQueryProjection projection, AppClientQueryPaging paging, AppClientQueryOptions options);
        ValidationData ValidateUpdateAppClient(ClaimsPrincipal principal, AppClient entity, UpdateAppClientModel model);
    }

    public class AppClientService : Service, IAppClientService
    {
        public AppClientService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query AppClient
        public IQueryable<AppClient> AppClients
        {
            get
            {
                return context.AppClient;
            }
        }

        public IDictionary<string, object> GetAppClientDynamic(
            AppClient row, AppClientQueryProjection projection,
            AppClientQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case AppClientQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["client_name"] = entity.ClientName;
                            obj["description"] = entity.Description;
                            obj["secret_key"] = entity.SecretKey;
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

        public List<IDictionary<string, object>> GetAppClientDynamic(
            IEnumerable<AppClient> rows, AppClientQueryProjection projection,
            AppClientQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetAppClientDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryAppClientDynamic(
            AppClientQueryProjection projection,
            AppClientQueryOptions options,
            AppClientQueryFilter filter = null,
            AppClientQuerySort sort = null,
            AppClientQueryPaging paging = null)
        {
            var query = AppClients;
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
                if (paging != null && (!options.load_all || !AppClientQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetAppClientDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetAppClientDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create AppClient
        protected void PrepareCreate(AppClient entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdated = entity.CreatedTime;
        }

        public AppClient CreateAppClient(CreateAppClientModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.AppClient.Add(entity).Entity;
        }
        #endregion

        #region Update AppClient
        protected void PrepareUpdate(AppClient entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public void UpdateAppClient(AppClient entity, UpdateAppClientModel model)
        {
            model.CopyTo(entity);
            PrepareUpdate(entity);
        }
        #endregion

        #region Delete AppClient
        public AppClient DeleteAppClient(AppClient entity)
        {
            return context.AppClient.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetAppClients(
            ClaimsPrincipal principal,
            AppClientQueryFilter filter,
            AppClientQuerySort sort,
            AppClientQueryProjection projection,
            AppClientQueryPaging paging,
            AppClientQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateAppClient(ClaimsPrincipal principal,
            CreateAppClientModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.ClientName))
                validationData = validationData.Fail("Client name must not be null", code: Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.Id))
                validationData = validationData.Fail("Client ID must not be null", code: Constants.AppResultCode.FailValidation);
            else if (AppClients.Exists(model.Id))
                validationData = validationData.Fail("Client ID existed", code: Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.SecretKey))
                validationData = validationData.Fail("Client secret must not be null", code: Constants.AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateUpdateAppClient(ClaimsPrincipal principal,
            AppClient entity, UpdateAppClientModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.ClientName))
                validationData = validationData.Fail("Client name must not be null", code: Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.SecretKey))
                validationData = validationData.Fail("Client secret must not be null", code: Constants.AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateDeleteAppClient(ClaimsPrincipal principal,
            AppClient entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
