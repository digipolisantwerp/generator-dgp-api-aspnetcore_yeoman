using System;
using Digipolis.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Digipolis.Errors;
using StarterKit.Business.Monitoring;
using StarterKit.Api.Models;
using AutoMapper;
using Digipolis.Web.Api;
using StarterKit.Shared.Constants;

namespace StarterKit.Api.Controllers
{

  [Route("[controller]")]
  [ApiExplorerSettings(IgnoreApi = false)]
  [Authorize]
  public class StatusController : Controller
  {

    private readonly IStatusProvider _statusreader;
    private readonly ILogger<StatusController> _logger;
    private readonly IMapper _mapper;

    public StatusController(IStatusProvider statusProvider, ILogger<StatusController> logger, IMapper mapper)
    {
      _statusreader = statusProvider ?? throw new ArgumentException($"StatusController.Ctr parameter {nameof(statusProvider)} cannot be null.");
      _logger = logger ?? throw new ArgumentException($"StatusController.Ctr parameter {nameof(logger)} cannot be null.");
      _mapper = mapper ?? throw new ArgumentException($"StatusController.Ctr parameter {nameof(mapper)} cannot be null.");
    }

    /// <summary>
    /// Get the global API status and the components statusses.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Models.Monitoring), 200)]
    [ProducesResponseType(typeof(Error), 500)]
    [Route("monitoring")]
    [Versions(Versions.V1)]
    public async Task<IActionResult> GetMonitoring()
    {
      var status = await _statusreader.GetStatus();

      var result = _mapper.Map<Api.Models.Monitoring>(status);

      return Ok(result);
    }


    /// <summary>
    /// Get the global API status.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(StatusResponse), 200)]
    [ProducesResponseType(typeof(Error), 500)]
    [Route("ping")]
    [Versions(Versions.V1)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPing()
    {
      return Ok(new StatusResponse()
      {
        Status = Models.Status.ok
      });
    }


  }
}
