using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Media.Models;
using Orchard.Media.Services;

namespace AgileUploaderField.Controllers
{
    public class AgileUploaderController : Controller
    {
        private readonly IMediaService _mediaService;

        public AgileUploaderController(IMediaServiceAdapter mediaService)
        {
            _mediaService = (IMediaService)mediaService;
        }

        [HttpPost]
        public string Upload(string agileUploaderMediaFolder)
        {
            var postedFiles = Request.Files;
            List<string> results = null;
            if (postedFiles.Count > 0)
            {
                results = new List<string>(postedFiles.Count);
                foreach (string keyPostedFile in postedFiles)
                {
                    var postedFile = postedFiles[keyPostedFile];
                    if (_mediaService.FileAllowed(postedFile))
                    {
                        var fileName = Path.GetFileName(postedFile.FileName);
                        var uniqueFileName = fileName;
                        try
                        {
                            // try to create the folder before uploading a file into it
                            _mediaService.CreateFolder(null, agileUploaderMediaFolder);
                        }
                        catch
                        {
                            // the folder can't be created because it already exists, continue
                        }
                        var filesInFolder = _mediaService.GetMediaFiles(agileUploaderMediaFolder).ToList();
                        var found =
                            filesInFolder.Any(
                                f => 0 == String.Compare(fileName, f.Name, StringComparison.OrdinalIgnoreCase));
                        var index = 0;
                        while (found)
                        {
                            index++;
                            uniqueFileName = String.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(fileName),
                                                           index,
                                                           Path.GetExtension(fileName));
                            found =
                                filesInFolder.Any(
                                    f => 0 == String.Compare(uniqueFileName, f.Name, StringComparison.OrdinalIgnoreCase));
                        }
                        results.Add(_mediaService.UploadMediaFile(agileUploaderMediaFolder, uniqueFileName,
                                                                  postedFile.InputStream, false));
                    }
                }
            }
            return results != null ? string.Join(";", results) : string.Empty;
            
        }
    }
    
    public class MediaServiceAdapter: IMediaServiceAdapter, IMediaService
    {
        private readonly MediaService _mediaService;

        public MediaServiceAdapter(IStorageProvider storageProvider, IOrchardServices orchardServices)
        {
            _mediaService = new MediaService(storageProvider, orchardServices);
        }

        public string GetPublicUrl(string relativePath)
        {
            return _mediaService.GetPublicUrl(relativePath);
        }

        public string GetMediaPublicUrl(string mediaPath, string fileName)
        {
            return _mediaService.GetMediaPublicUrl(mediaPath, fileName);
        }

        public IEnumerable<MediaFolder> GetMediaFolders(string relativePath)
        {
            return _mediaService.GetMediaFolders(relativePath);
        }

        public IEnumerable<MediaFile> GetMediaFiles(string relativePath)
        {
            return _mediaService.GetMediaFiles(relativePath);
        }

        public void CreateFolder(string relativePath, string folderName)
        {
            _mediaService.CreateFolder(relativePath, folderName);
        }

        public void DeleteFolder(string folderPath)
        {
            _mediaService.DeleteFolder(folderPath);
        }

        public void RenameFolder(string folderPath, string newFolderName)
        {
            _mediaService.RenameFolder(folderPath, newFolderName);
        }

        public void DeleteFile(string folderPath, string fileName)
        {
            _mediaService.DeleteFile(folderPath, fileName);
        }

        public void RenameFile(string folderPath, string currentFileName, string newFileName)
        {
            _mediaService.RenameFile(folderPath, currentFileName, newFileName);
        }

        public void MoveFile(string fileName, string currentPath, string newPath)
        {
            _mediaService.MoveFile(fileName, currentPath, newPath);
        }

        public string UploadMediaFile(string folderPath, HttpPostedFileBase postedFile, bool extractZip)
        {
            return _mediaService.UploadMediaFile(folderPath, postedFile, extractZip);
        }

        public string UploadMediaFile(string folderPath, string fileName, byte[] bytes, bool extractZip)
        {
            return _mediaService.UploadMediaFile(folderPath, fileName, bytes, extractZip);
        }

        public string UploadMediaFile(string folderPath, string fileName, Stream inputStream, bool extractZip)
        {
            return _mediaService.UploadMediaFile(folderPath, fileName, inputStream, extractZip);
        }

        public bool FileAllowed(HttpPostedFileBase postedFile)
        {
            return _mediaService.FileAllowed(postedFile);
        }

        public bool FileAllowed(string fileName, bool allowZip)
        {
            return _mediaService.FileAllowed(fileName, allowZip);
        }
    }

    public interface IMediaServiceAdapter : IDependency
    {
    }
}
