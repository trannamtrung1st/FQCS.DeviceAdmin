using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.DeviceAdmin.Business.Queries
{
    public static class AppClientQuery
    {
        public static IQueryable<AppClient> Id(this IQueryable<AppClient> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<AppClient> IdOnly(this IQueryable<AppClient> query)
        {
            return query.Select(o => new AppClient { Id = o.Id });
        }

        public static bool Exists(this IQueryable<AppClient> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<AppClient> Ids(this IQueryable<AppClient> query, IEnumerable<string> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<AppClient>
        public static IQueryable<AppClient> Sort(this IQueryable<AppClient> query,
            AppClientQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case AppClientQuerySort.TIME:
                        {
                            query = asc ? query.OrderBy(o => o.CreatedTime) :
                                query.OrderByDescending(o => o.CreatedTime);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<AppClient> Filter(
            this IQueryable<AppClient> query, AppClientQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            return query;
        }

        public static IQueryable<AppClient> Project(
            this IQueryable<AppClient> query, AppClientQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (AppClientQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in AppClientQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
