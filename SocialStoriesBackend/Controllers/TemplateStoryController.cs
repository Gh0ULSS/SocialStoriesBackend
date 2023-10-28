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
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialStoriesBackend.Mappings;
using SocialStoriesBackend.Models.File.PostRequests;
using SocialStoriesBackend.Models.Story.PostRequests;
using SocialStoriesBackend.Services;
using Authorization = SocialStoriesBackend.Services.Authorization;

namespace SocialStoriesBackend.Controllers
{
    /// <summary>
    /// Controller providing Story operations 
    /// </summary>
    [ApiController]
    [Authorize(Policy = Authorization.UserPolicy)]
    [Route("api/[controller]")]
    public class TemplateStoryController : BaseController<TemplateStoryController>
    {
        public TemplateStoryController(ILogger<TemplateStoryController> logger,
                                       DbContextService dbContextService,
                                       IMapper mapper)
                                       : base(logger, dbContextService, mapper) 
        {}
        
        /// <summary>
        /// Endpoint to fetch template stories
        /// </summary>
        [HttpGet("Stories")]
        [ValidateModel]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(TemplateStoriesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public Task<IActionResult> GetStoriesAsync() {
            var templateUserStories = _dbContextService.TemplateStories;
            return Task.FromResult<IActionResult>(Ok(new TemplateStoriesDto
            {
                Stories = _mapper.ProjectTo<TemplateStoryDto>(templateUserStories).ToArray()
            }));
        }
    }
}