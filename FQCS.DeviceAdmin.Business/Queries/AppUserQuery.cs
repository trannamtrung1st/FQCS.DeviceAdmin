using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.DeviceAdmin.Business.Queries
{
    public static class AppUserQuery
    {
        public static IQueryable<AppUser> Id(this IQueryable<AppUser> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<AppUser> IdOnly(this IQueryable<AppUser> query)
        {
            return query.Select(o => new AppUser { Id = o.Id });
        }

        public static bool Exists(this IQueryable<AppUser> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<AppUser> Ids(this IQueryable<AppUser> query, IEnumerable<string> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<AppUser>
        public static IQueryable<AppUser> Sort(this IQueryable<AppUser> query,
            AppUserQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case AppUserQuerySort.USERNAME:
                        {
                            query = asc ? query.OrderBy(o => o.UserName) :
                                query.OrderByDescending(o => o.UserName);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<AppUser> Filter(
            this IQueryable<AppUser> query, AppUserQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.uname_contains != null)
                query = query.Where(o => o.UserName.Contains(filter.uname_contains));
            if (filter.ids != null)
                query = query.Where(o => filter.ids.Contains(o.Id));
            return query;
        }

        public static IQueryable<AppUser> Project(
            this IQueryable<AppUser> query, AppUserQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (AppUserQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in AppUserQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
