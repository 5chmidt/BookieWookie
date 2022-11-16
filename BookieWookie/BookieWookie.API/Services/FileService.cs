namespace BookieWookie.API.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Upload an image file and log who/when it was uploaded.
        /// </summary>
        /// <param name="file">File sent via HTTP request.</param>
        /// <param name="userId">Id of the currently authenticated user.</param>
        /// <returns>Model of the uploaded file.</returns>
        Entities.File UploadImage(IFormFile file, int userId);

    }

    /// <inheritdoc/>
    public class FileService : IFileService
    {
        /// <inheritdoc/>
        public Entities.File UploadImage(IFormFile file, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
