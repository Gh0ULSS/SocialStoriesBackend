using System.ComponentModel.DataAnnotations;
using System.Net;
using SocialStoriesBackend.DbContext;
using SocialStoriesBackend.Entities;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Models.Users.PostRequests;

using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
using System.Net.Mime;
using System.Runtime.InteropServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SocialStoriesBackend.Mappings;
using SocialStoriesBackend.Models.File.PostRequests;
using SocialStoriesBackend.Services;
using Authorization = SocialStoriesBackend.Services.Authorization;

namespace SocialStoriesBackend.Controllers
{
    /// <summary>
    /// Controller providing file operations 
    /// </summary>
    [ApiController]
    [Authorize(Policy = Authorization.UserPolicy)]
    [Route("api/[controller]")]
    public class FileController : BaseController<FileController>
    {
        public FileController(ILogger<FileController> logger,
                              DbContextService dbContextService,
                              IMapper mapper,
                              IAmazonS3 s3Service,
                              IOptions<Settings.AWS> awsSettings)
                              : base(logger, dbContextService, mapper) 
        {
            _s3Service = s3Service;
            _awsSettings = awsSettings;
        }
        
        private async Task<bool> FileExists(string fileId, string fileExtension) {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _awsSettings.Value.BucketName,
                Key = $"{fileExtension}/{fileId}"
            };

            try {
                await _s3Service.GetObjectMetadataAsync(request);
            }
            catch(AmazonS3Exception ex) {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Endpoint to check if a file exists
        /// </summary>
        [HttpGet("Exists")]
        [ValidateModel]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> FileExistsAsync([FromQuery][Required] string fileId, [FromQuery][Required] string fileExtension) {
            if (! await FileExists(fileId, fileExtension)) {
                return NotFound(new ErrorDto { Message = new[]
                {
                    $"File: {fileId} was not found"
                }});
            }
            
            return Ok(new SuccessDto { Message = new[]
            {
                $"File: {fileId} exists"
            }});
        }
        
        /// <summary>
        /// Endpoint to upload a file encoded as Base64 to S3
        /// </summary>
        [HttpPost("Upload")]
        [ValidateModel]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadFileAsync([FromBody] UploadFileRequest request) {
            request.FileExtension = Helpers.StripFileExtension(request.FileExtension);
            
            var decodedData = Helpers.TryParseBase64(request.EncodedData);
            if (decodedData is null) {
                return BadRequest(new ErrorDto { Message = new[]
                {
                    "Failed to parse encoded file data"
                }});
            }

            var fileId = Guid.NewGuid();
            var objectRequest = new PutObjectRequest
            {
                BucketName = _awsSettings.Value.BucketName,
                Key = $"{request.FileExtension}/{fileId.ToString()}"
            };

            var ms = new MemoryStream(decodedData);
            objectRequest.InputStream = ms;
            var putObjectResponse = await _s3Service.PutObjectAsync(objectRequest);

            if (putObjectResponse.HttpStatusCode == HttpStatusCode.OK) {
                return Ok(new SuccessDto { Message = new[]
                {
                    $"File: {fileId} uploaded to S3",
                    fileId.ToString(),
                    request.FileExtension
                }});
            }

            return BadRequest(new ErrorDto
            {
                Message = new[]
                {
                    $"File: {fileId} failed upload to S3, error code: {putObjectResponse.HttpStatusCode}"
                }
            });
        }
        
        /// <summary>
        /// Endpoint to delete a file from S3
        /// </summary>
        [HttpDelete("Delete")]
        [ValidateModel]
        [Consumes(MediaTypeNames.Text.Plain)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteFileAsync([FromQuery][Required] string fileId, [FromQuery][Required] string fileExtension) {
            fileExtension = Helpers.StripFileExtension(fileExtension);
            
            var objectRequest = new DeleteObjectRequest
            {
                BucketName = _awsSettings.Value.BucketName,
                Key = $"{fileExtension}/{fileId}"
            };

            
            var deleteObjectResponse = await _s3Service.DeleteObjectAsync(objectRequest);
            if (deleteObjectResponse.HttpStatusCode == HttpStatusCode.OK) {
                return Ok(new SuccessDto { Message = new[]
                {
                    $"File: {objectRequest.Key} deleted from S3"
                }});
            }

            return BadRequest(new ErrorDto
            {
                Message = new[]
                {
                    $"File: {objectRequest.Key} failed to delete from S3, error code: {deleteObjectResponse.HttpStatusCode}"
                }
            });
        }
        
        /// <summary>
        /// Endpoint to download a file encoded as Base64 from S3
        /// </summary>
        [HttpGet("Download")]
        [ValidateModel]
        [Consumes(MediaTypeNames.Text.Plain)]
        [Produces(MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadFileAsync([FromQuery][Required] string fileId, [FromQuery][Required] string fileExtension) {
            fileExtension = Helpers.StripFileExtension(fileExtension);
            
            if (! await FileExists(fileId, fileExtension)) {
                return BadRequest(new ErrorDto { Message = new[]
                {
                    $"Failed to download file: {fileExtension}/{fileId}, does NOT exist"
                }});
            }
            
            
            var objectRequest = new GetObjectRequest
            {
                BucketName = _awsSettings.Value.BucketName,
                Key = $"{fileExtension}/{fileId}"
            };
            
            var getObjectResponse = await _s3Service.GetObjectAsync(objectRequest);

            if (getObjectResponse.HttpStatusCode != HttpStatusCode.OK) {
                return BadRequest(new ErrorDto
                {
                    Message = new[]
                    {
                        $"File: {objectRequest.Key} failed download from S3, error code: {getObjectResponse.HttpStatusCode}"
                    }
                });
            }


            using var ms = new MemoryStream();
            await getObjectResponse.ResponseStream.CopyToAsync(ms);
            var encodedFileData = Helpers.ToBase64(ms.ToArray());

            if (encodedFileData is null) {
                return BadRequest(new ErrorDto { Message = new[]
                    {
                        "Failed to encode file data as Base64"
                    }
                });
            }
                    
            return Ok(encodedFileData);
        }
        
        private readonly IAmazonS3 _s3Service;
        private readonly IOptions<Settings.AWS> _awsSettings;
    }
}