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
        public static IQueryable<QCEvent> Id(this IQueryable<QCEvent> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<QCEvent> IdOnly(this IQueryable<QCEvent> query)
        {
            return query.Select(o => new QCEvent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<QCEvent> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<QCEvent> Ids(this IQueryable<QCEvent> query, IEnumerable<int> ids)
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
            if (filter.defect_type != null)
                query = query.Where(o => o.DefectTypeCode == filter.defect_type);
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
