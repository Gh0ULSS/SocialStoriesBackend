using AutoMapper;
using SocialStoriesBackend.DbContext;
using Microsoft.AspNetCore.Mvc;

namespace SocialStoriesBackend.Controllers;

[Controller]
public class BaseController<T> : ControllerBase
{
    public BaseController(ILogger<T> logger, DbContextService dbContextService, IMapper mapper)
    {
        _logger = logger;
        _dbContextService = dbContextService;
        _mapper = mapper;
    }
    
    protected readonly ILogger<T> _logger;
    protected readonly DbContextService _dbContextService;
    protected readonly IMapper _mapper;
}