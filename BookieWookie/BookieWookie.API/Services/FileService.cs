namespace BookieWookie.API.Services
{
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
        Task<Entities.File> Upload(IFormFile file, int userId);

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
        public FileService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<Entities.File> Upload(IFormFile file, int userId)
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
                Uploaded = DateTime.Now,
                FileName = file.FileName,
            };


            Task[] tasks = new Task[2];
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                tasks[0] =  file.CopyToAsync(fileStream);
            }

            using (var db = new Entities.BookieWookieContext(_configuration))
            {
                db.Files.Add(fileEntity);
                tasks[1] = db.SaveChangesAsync();
            }

            // writing file and saving to database can happen at the same time //
            await Task.WhenAll();
            return fileEntity;
        }
    }
}
