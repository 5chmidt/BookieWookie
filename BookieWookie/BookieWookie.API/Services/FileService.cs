namespace BookieWookie.API.Services
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using System.Threading.Tasks;

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
        Task<byte[]> Get(int fileId);

    }

    /// <inheritdoc/>
    public class FileService : IFileService
    {
        private IWebHostEnvironment _hostingEnvironment;

        private IConfiguration _configuration;

        /// <summary>
        /// Initialize file service with dependency injection.
        /// </summary>
        /// <param name="hostingEnvironment">Hosting enviroment so files can be saved.</param>
        /// <param name="configuration"></param>
        public FileService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
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

            string folder = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                "uploads");
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }

            string filePath = Path.Combine(
                folder,
                Guid.NewGuid().ToString() + '.' + file.FileName.Split('.').Last());

            // just in case a GUID happens to overlap //
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(
                    folder,
                    Guid.NewGuid().ToString() + '.' + file.FileName.Split('.').Last());
            }

            var fileEntity = new Entities.File()
            {
                Path = Path.GetFileName(filePath),
                FileName = file.FileName,
                Uploaded = DateTime.Now,
                UserId = userId,
            };

            // TODO: writing file and saving to database can happen at the same time //
            Task[] tasks = new Task[2];
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                tasks[0] = file.CopyToAsync(fileStream);
            }

            using (var db = new Entities.BookieWookieContext(_configuration))
            {
                db.Files.Add(fileEntity);
                tasks[1] = db.SaveChangesAsync();
            }
            
            await Task.WhenAll(tasks);
            return fileEntity;
        }

        /// <inheritdoc/>
        public async Task<byte[]> Get(int fileId)
        {
            using(var db = new Entities.BookieWookieContext(_configuration))
            {
                var file = await db.Files.Where(f => f.FileId == fileId).SingleAsync();
                if (File.Exists(file.Path) == false)
                {
                    throw new FileNotFoundException($"Unable to locate file: {file.FileName}");
                }

                return await File.ReadAllBytesAsync(file.Path);
            }
        }

        public async Task<Entities.File> Update(Entities.File file)
        {
            using (var db = new Entities.BookieWookieContext(_configuration))
            {

            }

            return file;
        }
    }
}
