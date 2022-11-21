namespace BookieWookie.API.Services
{
    using BookieWookie.API.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Service for CRUD file operations.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Upload an image file and log who/when it was uploaded.
        /// </summary>
        /// <param name="file">File sent via HTTP request.</param>
        /// <param name="userId">Id of the currently authenticated user.</param>
        /// <returns>Model of the uploaded file.</returns>
        Task<Entities.File> Create(IFormFile file, int userId);

        /// <summary>
        /// Gets an uploaded file using the file id.
        /// </summary>
        /// <param name="fileId">Uniques integer identifier of file</param>
        /// <returns>Byte array of the file read from disk.</returns>
        Task<FileContentResult> Get(int fileId);

        /// <summary>
        /// Update file information.
        /// </summary>
        /// <param name="updateFile"><see cref="Models.UpdateFileRequest"/></param>
        /// <param name="userId">Unique identifier of current user.</param>
        /// <returns><see cref="Entities.File"/></returns>
        Task<Entities.File> Update(Models.UpdateFileRequest updateFile, int userId);

        /// <summary>
        /// Deletes a file (users can only delete their own files).
        /// </summary>
        /// <param name="fileId">Unique identifier of file to delete.</param>
        /// <param name="userId">Unique identifier of current user.</param>
        /// <returns><see cref="Entities.File"/></returns>
        Task<Entities.File> Delete(int fileId, int userId);

    }

    /// <inheritdoc/>
    public class FileService : IFileService
    {
        private IWebHostEnvironment hostingEnvironment;

        private BookieWookieContext context;

        /// <summary>
        /// Restrict the types of files uploaded by what can be mapped back to a FileContentResult.
        /// </summary>
        private static Dictionary<string, string> contentMap
        {
            get
            {
                return new Dictionary<string, string> 
                {
                    { "jpg", "image"},
                    { "png", "image"},
                    { "gif", "image"},
                };
            }
        }

        /// <summary>
        /// Initialize file service with dependency injection.
        /// </summary>
        /// <param name="_hostingEnvironment">Hosting enviroment so files can be saved.</param>
        /// <param name="_configuration">Inject configuration settings.</param>
        /// <param name="_context">Inject db context for entity framework.</param>
        public FileService(
            IWebHostEnvironment _hostingEnvironment, 
            BookieWookieContext _context)
        {
            this.hostingEnvironment = _hostingEnvironment;
            this.context = _context;
        }

        /// <inheritdoc/>
        public async Task<Entities.File> Create(IFormFile file, int userId)
        {
            if (file.Length == 0)
            {
                throw new FileLoadException($"Invalid file {file.Name} with size 0 KB");
            }

            if (file.FileName.Contains('.') == false)
            {
                throw new FileLoadException($"Invalid filename: '{file.FileName}' must have an extension.");
            }

            string extension = Path.GetExtension(file.FileName).TrimStart('.');
            if (contentMap.ContainsKey(extension) == false)
            {
                throw new FileLoadException($"Cannot upload files of type {extension}");
            }

            string folder = Path.Combine(
                hostingEnvironment.ContentRootPath,
                "uploads");
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }

            string filePath = Path.Combine(
                folder,
                Guid.NewGuid().ToString() + '.' + extension);

            // just in case a GUID happens to overlap //
            while (System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(
                    folder,
                    Guid.NewGuid().ToString() + '.' + extension);
            }

            var fileEntity = new Entities.File()
            {
                Path = filePath,
                FileName = file.FileName,
                Uploaded = DateTime.Now,
                UserId = userId,
            };

            // TODO: writing file and saving to database can happen at the same time //
            //Task[] tasks = new Task[2];
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            this.context.Files.Add(fileEntity);
            await this.context.SaveChangesAsync();
            
            //await Task.WhenAll(tasks);
            return fileEntity;
        }

        /// <inheritdoc/>
        public async Task<FileContentResult> Get(int fileId)
        {
            var file = await this.context.Files.Where(f => f.FileId == fileId).SingleAsync();
            if (System.IO.File.Exists(file.Path) == false)
            {
                throw new FileNotFoundException($"Unable to locate file: {file.FileName}");
            }

            string contentType;
            string extension = Path.GetExtension(file.FileName).TrimStart('.');
            if (contentMap.ContainsKey(extension))
            {
                contentType = $"{contentMap[extension]}/{extension}";
            }
            else
            {
                throw new FileLoadException($"Cannot get files of type {extension}");
            }

            byte[] bytes = await System.IO.File.ReadAllBytesAsync(file.Path);
            return new FileContentResult(bytes, contentType);
        }

        /// <inheritdoc/>
        public async Task<Entities.File> Update(Models.UpdateFileRequest updateFile, int userId)
        {
            var file = await this.context.Files
                .Where(f => f.FileId == updateFile.FileId)
                .SingleAsync();
            if (file.UserId != userId)
            {
                throw new UnauthorizedAccessException($"Users can only update files they uploaded.");
            }

            file.Purpose = updateFile.Purpose;
            await this.context.SaveChangesAsync();
            return file;
        }

        /// <inheritdoc/>
        public async Task<Entities.File> Delete(int fileId, int userId)
        {
            var file = await this.context.Files.Where(f => f.FileId == fileId)
                .SingleAsync();
            if (file.UserId != userId)
            {
                throw new UnauthorizedAccessException($"Users can only delete files they uploaded.");
            }
                
            // if the file does not exist it does not need to be deleted.
            if (System.IO.File.Exists(file.Path))
            {
                try
                {
                    System.IO.File.Delete(file.Path);
                }
                catch (Exception)
                {
                    throw new IOException($"Unable to delete {file.FileName}");
                }
            }
                
            this.context.Remove(file);
            await this.context.SaveChangesAsync();
            return file;
        }
    }
}
