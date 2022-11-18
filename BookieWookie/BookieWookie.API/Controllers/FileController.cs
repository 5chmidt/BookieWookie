using BookieWookie.API.Helpers;
using BookieWookie.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpPost("Create")]
        [AuthorizeOwner]
        public async Task<ActionResult> Create(IFormFile file)
        {
            Entities.File fileEntity = new();
            try
            {
                int userId = this.ParseUserIdFromContext();
                fileEntity = await _fileService.Upload(file, userId);
            }
            catch (FileLoadException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                return BadRequest(ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(fileEntity);
        }
    }
}
