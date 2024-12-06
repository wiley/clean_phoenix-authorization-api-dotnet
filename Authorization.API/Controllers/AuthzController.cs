using Authorization.API.Requests;
using Authorization.API.Responses;
using Authorization.API.Responses.NonSuccessfullResponses;
using Authorization.Domain.User;
using Authorization.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WLS.Log.LoggerTransactionPattern;

namespace Authorization.API.Controllers
{
    [Route("v{version:apiVersion}/authz/users")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class AuthzController : ControllerBase
    {
        private readonly ILogger<AuthzController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly ICacheService _service;

        private readonly Mapper _mapper_response;
        private readonly Mapper _mapper_request;

        public AuthzController(ILogger<AuthzController> logger, ILoggerStateFactory loggerStateFactory, ICacheService service)
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;

            /*_mapper_request = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<EntitlementCreateRequest, Entitlement>();
                cfg.CreateMap<EntitlementUpdateRequest, Entitlement>();
                cfg.AllowNullCollections = true;
            }));
            */
            _mapper_response = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<User, UserResponse>()));
        }

        [Authorize]
        [HttpGet("{Id}", Name = "GetCachedUser")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<ActionResult> GetCachedUser(
            [Required]
            [FromRoute] int Id
        )
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    User user = await _service.GetUser(Id);
                    if (user == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    UserResponse response = _mapper_response.Map<UserResponse>(user);
                    response._links.Self.Href = Url.Link("GetCachedUser", new { Id = response.UserId.ToString() });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetCachedUser - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GenerateUserCache(
            [FromBody] AuthzUserCreateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }
                    await _service.CreateUser(request.UserId);

                    User response = await _service.GetUser(request.UserId);
                    if (response == null)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }
                    var routeValues = new { Id = response.UserId };
                    return CreatedAtRoute("GetCachedUser", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GenerateUserCache - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
