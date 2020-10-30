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
using static FQCS.DeviceAdmin.Business.Constants;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FQCS.DeviceAdmin.Business.Services
{
    public class FileService : Service
    {
        public FileService(ServiceInjection inj) : base(inj)
        {
        }

        public async Task SaveFile(byte[] file, string fullPath)
        {
            var folderPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            await File.WriteAllBytesAsync(fullPath, file);
        }

        public async Task SaveFile(IFormFile file, string fullPath)
        {
            var folderPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }
        }

        public (string, string) GetFilePath(string folderPath, string rootPath = null,
            string fileName = null, string ext = null)
        {
            if (fileName == null)
            {
                if (ext != null && !ext.Contains("."))
                    ext = "." + ext;
                fileName = Path.GetRandomFileName() + ext ?? "";
            }
            var filePath = Path.Combine(folderPath, fileName);
            var absPath = Path.GetFullPath(filePath);
            var relativePath = rootPath == null ? absPath : absPath.Replace(rootPath, "").Substring(1);
            return (relativePath, absPath);
        }

        public void DeleteFile(string filePath, string rootPath)
        {
            var fullPath = Path.Combine(rootPath, filePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public void DeleteDirectory(string folderPath, string rootPath, bool recursive = true)
        {
            var fullPath = Path.Combine(rootPath, folderPath);
            if (Directory.Exists(fullPath))
                Directory.Delete(fullPath, recursive);
        }
    }
}
