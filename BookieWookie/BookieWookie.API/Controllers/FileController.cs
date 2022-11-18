using BookieWookie.API.Helpers;
using BookieWookie.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookieWookie.API.Controllers
{
    /// <summary>
    /// Controller for uploading and linking files.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        private IConfiguration _configuration;

        private IFileService _fileService;

        /// <summary>
        /// Initialize FileController with dependency injection.
        /// </summary>
        /// <param name="configuration"></param>
        public FileController(IConfiguration configuration, IFileService fileService)
        {
            _configuration = configuration;
            _fileService = fileService;
        }

        /// <summary>
        /// Upload an image.
        /// </summary>
        /// <param name="file">File sent via HTTP request.</param>
        /// <returns>File model.</returns>
        [HttpPost("UploadImage")]
        [AuthorizeOwner]
        public async Task<ActionResult> TaskUploadImage(IFormFile file)
        {
            Entities.File fileEntity = new();
            try
            {
                int userId = this.ParseUserIdFromContext();
                fileEntity = await _fileService.UploadImage(file, userId);
            }
            catch (FileLoadException ex)
            {
                BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }

            return Ok(fileEntity);
        }
    }
}
