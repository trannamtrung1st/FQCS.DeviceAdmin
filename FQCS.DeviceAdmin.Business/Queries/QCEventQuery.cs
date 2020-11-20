using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.DeviceAdmin.Business.Queries
{
    public static class QCEventQuery
    {

        public static IQueryable<QCEvent> ExceptLast(this IQueryable<QCEvent> query, int days)
        {
            var lastKeepDate = DateTime.UtcNow.Date.Subtract(TimeSpan.FromDays(days));
            return query.Where(o => o.CreatedTime.Date < lastKeepDate);
        }

        public static IQueryable<QCEvent> Unsent(this IQueryable<QCEvent> query)
        {
            return query.Where(o => o.NotiSent == false);
        }

        public static IQueryable<QCEvent> Id(this IQueryable<QCEvent> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<QCEvent> IdOnly(this IQueryable<QCEvent> query)
        {
            return query.Select(o => new QCEvent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<QCEvent> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<QCEvent> Ids(this IQueryable<QCEvent> query, IEnumerable<string> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<QCEvent>
        public static IQueryable<QCEvent> Sort(this IQueryable<QCEvent> query,
            QCEventQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case QCEventQuerySort.TIME:
                        {
                            query = asc ? query.OrderBy(o => o.CreatedTime) :
                                query.OrderByDescending(o => o.CreatedTime);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<QCEvent> Filter(
            this IQueryable<QCEvent> query, QCEventQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.ids != null && filter.ids.Length > 0)
                query = query.Ids(filter.ids);
            if (filter.defect_type != null)
                query = query.Where(o => o.Details.Any(e => e.DefectTypeCode == filter.defect_type));
            if (filter.from_time != null)
                query = query.Where(o => o.CreatedTime >= filter.from_time);
            if (filter.to_time != null)
                query = query.Where(o => o.CreatedTime <= filter.to_time);
            if (filter.sent != null)
                query = query.Where(o => o.NotiSent == filter.sent);
            return query;
        }

        public static IQueryable<QCEvent> Project(
            this IQueryable<QCEvent> query, QCEventQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (QCEventQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in QCEventQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
