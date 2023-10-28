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
    public class StoryController : BaseController<StoryController>
    {
        public StoryController(ILogger<StoryController> logger,
                              DbContextService dbContextService,
                              IMapper mapper,
                              UserManager<User> userManager)
                              : base(logger, dbContextService, mapper) 
        {
            _userManager = userManager;
        }
        
        /// <summary>
        /// Endpoint to fetch a users stories
        /// </summary>
        [HttpGet("Stories")]
        [ValidateModel]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(StoriesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetStoriesAsync() {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimValueTypes.Email));

            var userStories = _dbContextService.Stories.Include(x => x.Pages)
                                                                     .Where(x => x.User.Id == user.Id);
            return Ok(new StoriesDto
            {
                Stories = _mapper.ProjectTo<StoryDto>(userStories).ToArray()
            });
        }
        
        /// <summary>
        /// Endpoint to add a user story
        /// </summary>
        [HttpPost("Upload")]
        [ValidateModel]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadStoryAsync([FromBody] UploadStoryRequest newStory) {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimValueTypes.Email));

            var userStory = new Story {
                Id = Guid.NewGuid(),
                User = user,
                FontType = newStory.FontType,
                TitleStoryPage = _mapper.Map<StoryPage>(newStory.TitleStoryPage),
                Pages = _mapper.ProjectTo<StoryPage>(newStory.Pages.AsQueryable()).ToList()
            };
            
            _dbContextService.Stories.Add(userStory);
            
            await _dbContextService.SaveChangesAsync();
            
            return Ok(new SuccessDto
            {
                Message = new[]
                {
                    "Successfully stored stored user story"
                }
            });
        }
        
        /// <summary>
        /// Endpoint to delete a user story
        /// </summary>
        [HttpDelete("Delete")]
        [ValidateModel]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteStoryAsync([FromQuery][Required] Guid storyId) {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimValueTypes.Email));

            var userStory = _dbContextService.Stories.Include(x => x.Pages)
                                                     .FirstOrDefault(x => x.Id == storyId && x.User.Id == user.Id);
            if (userStory is null) {
                return NotFound(new ErrorDto
                {
                    Message = new[]
                    {
                        $"User story doesn't exist: {storyId}"
                    }
                });
            }
            
            _dbContextService.Stories.Remove(userStory);
            await _dbContextService.SaveChangesAsync();
            
            return Ok(new SuccessDto
            {
                Message = new[]
                {
                    "Successfully deleted user story"
                }
            });
        }
        
        /// <summary>
        /// Endpoint to update a user story
        /// </summary>
        [HttpPost("Update")]
        [ValidateModel]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateStoryAsync([FromBody] StoryDto newStory) {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimValueTypes.Email));
            
            // Find the product you want to update
            var storyToUpdate = _dbContextService.Stories.AsNoTracking()
                .Include(x => x.Pages)
                .FirstOrDefault(x => x.Id == newStory.Id && x.User.Id == user.Id);
            
            if (storyToUpdate is null) {
                return NotFound(new ErrorDto
                {
                    Message = new[]
                    {
                        $"User story doesn't exist: {newStory.Id}"
                    }
                });
            }

            storyToUpdate = _mapper.Map<Story>(newStory);
            storyToUpdate.User = user;
            
            _dbContextService.Stories.Update(storyToUpdate);
            
            await _dbContextService.SaveChangesAsync();
            
            return Ok(new SuccessDto
            {
                Message = new[]
                {
                    "Successfully stored updated user story"
                }
            });
        }
        
        private readonly UserManager<User> _userManager;
    }
}