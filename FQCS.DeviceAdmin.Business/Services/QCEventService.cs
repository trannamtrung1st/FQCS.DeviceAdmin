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
using System.IO;
using FQCS.DeviceAdmin.Business.Helpers;
using System.Globalization;
using FQCS.DeviceAdmin.Kafka;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace FQCS.DeviceAdmin.Business.Services
{
    public class QCEventService : Service
    {
        public QCEventService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query QCEvent
        public IQueryable<QCEvent> QCEvents
        {
            get
            {
                return context.QCEvent;
            }
        }

        public IDictionary<string, object> GetQCEventDynamic(
            QCEvent row, QCEventQueryProjection projection,
            QCEventQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case QCEventQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["defect_type_code"] = entity.DefectTypeCode;
                            obj["left_image"] = entity.LeftImage;
                            obj["right_image"] = entity.RightImage;
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

        public List<IDictionary<string, object>> GetQCEventDynamic(
            IEnumerable<QCEvent> rows, QCEventQueryProjection projection,
            QCEventQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetQCEventDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryQCEventDynamic(
            QCEventQueryProjection projection,
            QCEventQueryOptions options,
            QCEventQueryFilter filter = null,
            QCEventQuerySort sort = null,
            QCEventQueryPaging paging = null)
        {
            var query = QCEvents;
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
                if (paging != null && (!options.load_all || !QCEventQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetQCEventDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetQCEventDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create QCEvent
        protected void PrepareCreate(QCEvent entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public void ProduceEventToKafkaServer(IProducer<Null, string> producer, QCEvent entity,
            DeviceConfig currentConfig, string dataFolder)
        {
            var mess = new Message<Null, string>();
            string leftImgB64 = null; string rightImgB64 = null;
            var imgPath = Path.Combine(dataFolder, entity.LeftImage);
            if (File.Exists(imgPath))
            {
                var img = File.ReadAllBytes(imgPath);
                leftImgB64 = Convert.ToBase64String(img);
            }
            imgPath = Path.Combine(dataFolder, entity.RightImage);
            if (File.Exists(imgPath))
            {
                var img = File.ReadAllBytes(imgPath);
                rightImgB64 = Convert.ToBase64String(img);
            }
            mess.Value = JsonConvert.SerializeObject(new QCEventMessage
            {
                CreatedTime = entity.CreatedTime,
                QCDefectCode = entity.DefectTypeCode,
                Identifier = currentConfig.Identifier,
                LeftB64Image = leftImgB64,
                RightB64Image = rightImgB64,
            });
            producer.Produce(Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT, mess);
        }

        public QCEvent CreateQCEvent(CreateQCEventModel model)
        {
            var entity = model.ToDest();
            entity.CreatedTime = DateTime.ParseExact(
                model.CreatedTimeStr, model.DateFormat, CultureInfo.InvariantCulture);
            PrepareCreate(entity);
            return context.QCEvent.Add(entity).Entity;
        }
        #endregion

        #region Update QCEvent
        #endregion

        #region Delete QCEvent
        public QCEvent DeleteQCEvent(QCEvent entity)
        {
            return context.QCEvent.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetAllImages(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateGetQCEvents(
            ClaimsPrincipal principal,
            QCEventQueryFilter filter,
            QCEventQuerySort sort,
            QCEventQueryProjection projection,
            QCEventQueryPaging paging,
            QCEventQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateQCEvent(ClaimsPrincipal principal,
            CreateQCEventModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteQCEvent(ClaimsPrincipal principal,
            QCEvent entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
