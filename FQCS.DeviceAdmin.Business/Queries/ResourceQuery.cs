using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.DeviceAdmin.Business.Queries
{
    public static class ResourceQuery
    {
        public static IQueryable<Resource> Id(this IQueryable<Resource> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Resource> IdOnly(this IQueryable<Resource> query)
        {
            return query.Select(o => new Resource { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Resource> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Resource> Ids(this IQueryable<Resource> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<Resource>
        public static IQueryable<Resource> Sort(this IQueryable<Resource> query,
            ResourceQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ResourceQuerySort.NAME:
                        {
                            query = asc ? query.OrderBy(o => o.Name) :
                                query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Resource> Filter(
            this IQueryable<Resource> query, ResourceQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.name_contains != null)
                query = query.Where(o => o.Name.Contains(filter.name_contains));
            return query;
        }

        public static IQueryable<Resource> Project(
            this IQueryable<Resource> query, ResourceQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (ResourceQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in ResourceQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
