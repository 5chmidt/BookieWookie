using BookieWookie.API.Entities;
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
    public class FileController : ControllerBase
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
        /// Upload a File.
        /// </summary>
        /// <param name="file">File sent via HTTP request.</param>
        /// <returns>File model.</returns>
        [HttpPost("Create")]
        [AuthorizeOwner]
        public async Task<ActionResult> Create(IFormFile file)
        {
            try
            {
                Entities.File fileEntity = new();
                int userId = this.ParseUserIdFromContext();
                fileEntity = await _fileService.Create(file, userId);
                return Ok(fileEntity);
            }
            catch (FileLoadException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += Environment.NewLine + ex.InnerException.Message;
                }

                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets an uploaded file.
        /// </summary>
        /// <param name="fileId">Unique identifier for uploaded file.</param>
        /// <returns><seealso cref="FileContentResult"/></returns>
        [HttpGet("{fileId}")]
        [AuthorizeOwner]
        public async Task<ActionResult> Get(int fileId)
        {
            try
            {
                int userId = this.ParseUserIdFromContext();
                FileContentResult file = await _fileService.Get(fileId);
                return file;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        /// <summary>
        /// Update the purpose tag for an image.
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [AuthorizeOwner]
        public async Task<ActionResult> Update(Models.UpdateFileRequest fileRequest)
        {
            try
            {
                int userId = this.ParseUserIdFromContext();
                Entities.File file = await _fileService.Update(fileRequest, userId);
                return Ok(file);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete and existing file, users can only delete files they uploaded.
        /// </summary>
        /// <param name="fileId">Id of the file to be deleted.</param>
        /// <returns>The object that was deleted.</returns>
        [AuthorizeOwner]
        [HttpDelete("{fileId}")]
        public async Task<IActionResult> Delete(int fileId)
        {
            try
            {
                int userId = this.ParseUserIdFromContext();
                var file = await _fileService.Delete(fileId, userId);
                return Ok(file);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
