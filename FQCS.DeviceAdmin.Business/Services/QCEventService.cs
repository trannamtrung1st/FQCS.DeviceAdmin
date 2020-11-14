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
using EFCore.BulkExtensions;
using Microsoft.Extensions.DependencyInjection;

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
            QCEventQueryOptions options, string folderPath, FileService fileService)
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
                            var details = entity.Details.Select(o => new
                            {
                                id = o.Id,
                                defect_type_code = o.DefectTypeCode,
                            }).ToList();
                            obj["details"] = details;
                            obj["noti_sent"] = entity.NotiSent;
                            obj["left_image"] = entity.LeftImage;
                            obj["right_image"] = entity.RightImage;
                            obj["side_images"] = entity.SideImages == null ? null :
                                JsonConvert.DeserializeObject<IEnumerable<string>>(entity.SideImages);
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
                    case QCEventQueryProjection.IMAGE:
                        {
                            if (!options.single_only)
                                throw new Exception("Only single option can query image field");
                            var entity = row;
                            if (entity.LeftImage != null)
                            {
                                var fullPath = fileService.GetFilePath(folderPath, null, entity.LeftImage).Item2;
                                if (File.Exists(fullPath))
                                {
                                    var img = File.ReadAllBytes(fullPath);
                                    var img64 = Convert.ToBase64String(img);
                                    obj["left_image_64"] = img64;
                                }
                            }
                            if (entity.RightImage != null)
                            {
                                var fullPath = fileService.GetFilePath(folderPath, null, entity.RightImage).Item2;
                                if (File.Exists(fullPath))
                                {
                                    var img = File.ReadAllBytes(fullPath);
                                    var img64 = Convert.ToBase64String(img);
                                    obj["right_image_64"] = img64;
                                }
                            }
                            if (entity.SideImages != null)
                            {
                                var sideImages = JsonConvert.DeserializeObject<IEnumerable<string>>(entity.SideImages);
                                var sideImagesB64 = new List<string>();
                                obj["side_images_b64"] = sideImagesB64;
                                foreach (var iPath in sideImages)
                                {
                                    var fullPath = fileService.GetFilePath(folderPath, null, iPath).Item2;
                                    if (File.Exists(fullPath))
                                    {
                                        var img = File.ReadAllBytes(fullPath);
                                        var img64 = Convert.ToBase64String(img);
                                        sideImagesB64.Add(img64);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetQCEventDynamic(
            IEnumerable<QCEvent> rows, QCEventQueryProjection projection,
            QCEventQueryOptions options, string folderPath, FileService fileService)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetQCEventDynamic(o, projection, options, folderPath, fileService);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryQCEventDynamic(
            QCEventQueryProjection projection,
            QCEventQueryOptions options,
            QCEventQueryFilter filter = null,
            QCEventQuerySort sort = null,
            QCEventQueryPaging paging = null,
            string folderPath = null)
        {
            var fileService = provider.GetRequiredService<FileService>();
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
                var singleResult = GetQCEventDynamic(single, projection, options, folderPath, fileService);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetQCEventDynamic(entities, projection, options, folderPath, fileService);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }

        public IQueryable<QCEvent> GetQueryableQCEventForUpdate(
            QCEventQueryOptions options,
            QCEventQueryFilter filter = null,
            QCEventQuerySort sort = null,
            QCEventQueryPaging paging = null)
        {
            var query = QCEvents;
            if (filter != null) query = query.Filter(filter);
            if (!options.single_only)
            {
                if (paging != null && (!options.load_all || !QCEventQueryOptions.IsLoadAllAllowed))
                {
                    if (sort != null) query = query.Sort(sort);
                    query = query.SelectPage(paging.page, paging.limit);
                }
            }
            return query;
        }

        public IQueryable<QCEvent> GetQueryableQCEvent(
            QCEventQueryOptions options,
            QCEventQueryFilter filter = null,
            QCEventQuerySort sort = null,
            QCEventQueryPaging paging = null)
        {
            var query = QCEvents;
            if (filter != null) query = query.Filter(filter);
            if (!options.single_only)
            {
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !QCEventQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
            }
            return query;
        }
        #endregion

        #region Create QCEvent
        protected void PrepareCreate(QCEvent entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.LastUpdated = DateTime.UtcNow;
            foreach (var e in entity.Details)
                e.Id = Guid.NewGuid().ToString();
        }

        public void ProduceEventToKafkaServer(IProducer<Null, string> producer, QCEvent entity,
            DeviceConfig currentConfig, string dataFolder, string connStr)
        {
            var mess = new Message<Null, string>();
            string leftImgB64 = null; string rightImgB64 = null; List<string> sideImagesB64 = null;
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
            var sideImages = entity.SideImages == null ? null :
                JsonConvert.DeserializeObject<IEnumerable<string>>(entity.SideImages);
            if (sideImages != null)
            {
                sideImagesB64 = new List<string>();
                foreach (var iPath in sideImages)
                    imgPath = Path.Combine(dataFolder, iPath);
                if (File.Exists(imgPath))
                {
                    var img = File.ReadAllBytes(imgPath);
                    var b64 = Convert.ToBase64String(img);
                    sideImagesB64.Add(b64);
                }
            }
            var details = entity.Details.Select(o => new QCEventDetailMessage
            {
                Id = o.Id,
                QCDefectCode = o.DefectTypeCode
            }).ToList();
            mess.Value = JsonConvert.SerializeObject(new QCEventMessage
            {
                Id = entity.Id,
                CreatedTime = entity.CreatedTime,
                Identifier = currentConfig.Identifier,
                LeftB64Image = leftImgB64,
                RightB64Image = rightImgB64,
                SideB64Images = sideImagesB64,
                Details = details,
                LeftImage = entity.LeftImage,
                RightImage = entity.RightImage,
                SideImages = entity.SideImages == null ? null :
                    JsonConvert.DeserializeObject<IList<string>>(entity.SideImages)
            });
            QCEvent.CheckedEvents.Add(entity.Id);
            producer.Produce(Kafka.Constants.KafkaTopic.TOPIC_QC_EVENT, mess, report =>
            {
                if (report.Status == PersistenceStatus.Persisted)
                {
                    using var context = new DataContext(
                        new DbContextOptionsBuilder<DataContext>().UseSqlServer(connStr).Options);
                    entity = context.QCEvent.Id(entity.Id).First();
                    entity.NotiSent = true;
                    entity.LastUpdated = DateTime.UtcNow;
                    context.SaveChanges();
                    QCEvent.CheckedEvents.Remove(entity.Id);
                }
            });
        }

        public QCEvent CreateQCEvent(CreateQCEventModel model, DateTime createdTime)
        {
            var entity = model.ToDest();
            entity.CreatedTime = createdTime;
            entity.SideImages = model.SideImages == null ? null :
                JsonConvert.SerializeObject(model.SideImages);
            PrepareCreate(entity);
            return context.QCEvent.Add(entity).Entity;
        }
        #endregion

        #region Update QCEvent
        protected void PrepareUpdate(QCEvent entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public int UpdateEventsSentStatus(IQueryable<QCEvent> entities, bool val)
        {
            var updated = new QCEvent
            {
                NotiSent = val
            };
            PrepareUpdate(updated);
            return entities.BatchUpdate(updated, new List<string> {
                nameof(QCEvent.NotiSent), nameof(QCEvent.LastUpdated) });
        }
        #endregion

        #region Delete QCEvent
        public async Task<int> ClearAllQCEvents()
        {
            return await context.QCEvent.BatchDeleteAsync();
        }

        public void ClearAllQCEventImages(string dataFolder)
        {
            var fileService = provider.GetRequiredService<FileService>();
            fileService.DeleteDirectory(dataFolder, "");
            Directory.CreateDirectory(dataFolder);
        }

        public QCEvent DeleteQCEvent(QCEvent entity)
        {
            return context.QCEvent.Remove(entity).Entity;
        }

        public int DeleteQCEvents(IQueryable<QCEvent> entities)
        {
            return entities.BatchDelete();
        }
        #endregion

        #region Validation
        public ValidationData ValidateSendEvents(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateClearAllEvents(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateGetAllImages(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateSentStatus(
            ClaimsPrincipal principal,
            QCEventQueryFilter filter,
            QCEventQuerySort sort,
            QCEventQueryPaging paging,
            QCEventQueryOptions options)
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

        public ValidationData ValidateCountQCEvents(
            ClaimsPrincipal principal,
            QCEventQueryFilter filter,
            QCEventQuerySort sort,
            QCEventQueryPaging paging,
            QCEventQueryOptions options)
        {
            return new ValidationData();
        }


        public ValidationData ValidateCreateQCEvent(ClaimsPrincipal principal,
            CreateQCEventModel model)
        {
            var validationData = new ValidationData();
            if (model.Details != null)
                foreach (var d in model.Details)
                    if (!Data.Constants.DefectTypeCode.ALL.Contains(d.DefectTypeCode))
                        validationData = validationData.Fail("Invalid defect type",
                            code: Constants.AppResultCode.FailValidation);
            DateTime createdTime;
            if (!DateTime.TryParseExact(
                model.CreatedTimeStr, model.DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal, out createdTime))
                validationData = validationData.Fail("Invalid datetime", code: Constants.AppResultCode.FailValidation);
            else validationData.TempData[nameof(createdTime)] = createdTime;
            return validationData;
        }

        public ValidationData ValidateDeleteQCEvent(ClaimsPrincipal principal,
            QCEvent entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
