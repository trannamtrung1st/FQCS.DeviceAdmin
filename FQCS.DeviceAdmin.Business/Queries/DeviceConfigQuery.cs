using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.DeviceAdmin.Business.Queries
{
    public static class DeviceConfigQuery
    {
        public static IQueryable<DeviceConfig> IsCurrent(this IQueryable<DeviceConfig> query, bool value = true)
        {
            return query.Where(o => o.IsCurrent == value);
        }

        public static IQueryable<DeviceConfig> Id(this IQueryable<DeviceConfig> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<DeviceConfig> IdOnly(this IQueryable<DeviceConfig> query)
        {
            return query.Select(o => new DeviceConfig { Id = o.Id });
        }

        public static bool Exists(this IQueryable<DeviceConfig> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<DeviceConfig> Ids(this IQueryable<DeviceConfig> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<DeviceConfig>
        public static IQueryable<DeviceConfig> Sort(this IQueryable<DeviceConfig> query,
            DeviceConfigQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case DeviceConfigQuerySort.TIME:
                        {
                            query = asc ? query.OrderBy(o => o.CreatedTime) :
                                query.OrderByDescending(o => o.CreatedTime);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<DeviceConfig> Filter(
            this IQueryable<DeviceConfig> query, DeviceConfigQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            return query;
        }

        public static IQueryable<DeviceConfig> Project(
            this IQueryable<DeviceConfig> query, DeviceConfigQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (DeviceConfigQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in DeviceConfigQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
