using BookieWookie.API.Helpers;
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

        /// <summary>
        /// Initialize FileController with dependency injection.
        /// </summary>
        /// <param name="configuration"></param>
        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Upload an image.
        /// </summary>
        /// <param name="file">File sent via HTTP request.</param>
        /// <returns>File model.</returns>
        [HttpPost("UploadImage")]
        public ActionResult UploadImage(IFormFile file)
        {
            if (file.ContentType.Split('/').FirstOrDefault() != "image")
            {
                return BadRequest("uploaded file is not an image.");
            }

            // we can put rest of upload logic here.
            return Ok(file.FileName);
        }
    }
}
