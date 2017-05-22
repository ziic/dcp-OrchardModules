using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.MediaLibrary.Models;
using Orchard.MediaLibrary.Services;

namespace AgileUploaderField.Controllers
{
    public class AgileUploaderController : Controller
    {
        private readonly IMediaServiceAdapter _mediaService;

        public AgileUploaderController(IMediaServiceAdapter mediaService)
        {
            _mediaService = mediaService;
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
                    if (postedFile == null)
                        continue;

                    if (_mediaService.FileAllowed(postedFile))
                    {
                        results.Add(_mediaService.UploadMediaFile(agileUploaderMediaFolder, postedFile));
                    }
                }
            }
            return results != null ? string.Join(";", results) : string.Empty;
            
        }
    }

    public class MediaServiceAdapter : IMediaServiceAdapter
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMediaLibraryService _mediaLibraryService;

        public MediaServiceAdapter(IOrchardServices orchardServices, IMediaLibraryService mediaLibraryService)
        {
            _orchardServices = orchardServices;
            _mediaLibraryService = mediaLibraryService;
        }

        public void CreateFolder(string relativePath, string folderName)
        {
            _mediaLibraryService.CreateFolder(relativePath, folderName);
        }

        public string UploadMediaFile(string folderPath, HttpPostedFileBase postedFile)
        {
            if (!FileAllowed(postedFile))
                throw new InvalidOperationException("Fiel type is not allowed");
            var uniqueFileName = _mediaLibraryService.GetUniqueFilename(folderPath, postedFile.FileName);

            return _mediaLibraryService.UploadMediaFile(folderPath, uniqueFileName, postedFile.InputStream);
        }

        public bool FileAllowed(HttpPostedFileBase postedFile)
        {
            var settings = _orchardServices.WorkContext.CurrentSite.As<MediaLibrarySettingsPart>();
            var allowedExtensions = (settings.UploadAllowedFileTypeWhitelist ?? "")
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.StartsWith(".")).ToList();

            return !(allowedExtensions.Any() && !allowedExtensions.Any(e => postedFile.FileName.EndsWith(e, StringComparison.OrdinalIgnoreCase)));

        }

       
    }

    public interface IMediaServiceAdapter : IDependency
    {
        bool FileAllowed(HttpPostedFileBase postedFile);
        string UploadMediaFile(string folderPath, HttpPostedFileBase postedFile);
       
    }
}
