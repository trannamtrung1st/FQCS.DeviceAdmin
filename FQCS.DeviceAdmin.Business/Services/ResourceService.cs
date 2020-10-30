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

namespace FQCS.DeviceAdmin.Business.Services
{
    public class ResourceService : Service
    {
        public ResourceService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Resource
        public IQueryable<Resource> Resources
        {
            get
            {
                return context.Resource;
            }
        }

        public IDictionary<string, object> GetResourceDynamic(
            Resource row, ResourceQueryProjection projection,
            ResourceQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ResourceQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetResourceDynamic(
            IEnumerable<Resource> rows, ResourceQueryProjection projection,
            ResourceQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetResourceDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryResourceDynamic(
            ResourceQueryProjection projection,
            ResourceQueryOptions options,
            ResourceQueryFilter filter = null,
            ResourceQuerySort sort = null,
            ResourceQueryPaging paging = null)
        {
            var query = Resources;
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
                if (paging != null && (!options.load_all || !ResourceQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetResourceDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetResourceDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create Resource
        protected void PrepareCreate(Resource entity)
        {
        }

        public Resource CreateResource(CreateResourceModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Resource.Add(entity).Entity;
        }
        #endregion

        #region Update Resource
        public void UpdateResource(Resource entity, UpdateResourceModel model)
        {
            model.CopyTo(entity);
        }
        #endregion

        #region Delete Resource
        public Resource DeleteResource(Resource entity)
        {
            return context.Resource.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetResources(
            ClaimsPrincipal principal,
            ResourceQueryFilter filter,
            ResourceQuerySort sort,
            ResourceQueryProjection projection,
            ResourceQueryPaging paging,
            ResourceQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateResource(ClaimsPrincipal principal,
            CreateResourceModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateResource(ClaimsPrincipal principal,
            Resource entity, UpdateResourceModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteResource(ClaimsPrincipal principal,
            Resource entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
