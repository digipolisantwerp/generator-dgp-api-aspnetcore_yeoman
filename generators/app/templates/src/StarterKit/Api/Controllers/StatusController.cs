using AutoMapper;
using Digipolis.Errors;
using Digipolis.Web.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarterKit.Api.Models;
using StarterKit.Business.Monitoring;
using StarterKit.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;

namespace StarterKit.Api.Controllers
{

  [Route("[controller]")]
  [ApiExplorerSettings(IgnoreApi = false)]
  [Authorize]
  public class StatusController : Controller
  {

    private readonly IStatusReader _statusreader;
    private readonly ILogger<StatusController> _logger;
    private readonly IMapper _mapper;

    public StatusController(IStatusReader statusReader, ILogger<StatusController> logger, IMapper mapper)
    {
      _statusreader = statusReader ?? throw new ArgumentException($"StatusController.Ctr parameter {nameof(statusReader)} cannot be null.");
      _logger = logger ?? throw new ArgumentException($"StatusController.Ctr parameter {nameof(logger)} cannot be null.");
      _mapper = mapper ?? throw new ArgumentException($"StatusController.Ctr parameter {nameof(mapper)} cannot be null.");
    }

    /// <summary>
    /// Get the global API status and the components statusses.
    /// </summary>
    /// <remarks>
    /// Get the global API status and the components statusses.
    /// </remarks>
    /// <returns></returns>
    [HttpGet("monitoring")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Models.Monitoring), 200)]
    [ProducesResponseType(typeof(Error), 500)]
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
    /// <remarks>
    /// Get the global API status.
    /// </remarks>
    /// <returns></returns>
    [HttpGet("ping")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(StatusResponse), 200)]
    [ProducesResponseType(typeof(Error), 500)]
    [Versions(Versions.V1)]
    [AllowAnonymous]
    public IActionResult GetPing()
    {
      return Ok(new StatusResponse()
      {
        Status = Models.Status.ok
      });
    }

    /// <summary>
    /// Get the runtime configuration.
    /// </summary>
    /// <remarks>
    /// Get the runtime configuration.
    /// </remarks>
    /// <returns></returns>
    [HttpGet("runtime")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IDictionary<string, Object>), 200)]
    [ProducesResponseType(typeof(Error), 500)]
    [Versions(Versions.V1)]
    [AllowAnonymous]
    public IActionResult GetRuntimeValues()
    {
      dynamic values = new ExpandoObject();

      Process curProces = System.Diagnostics.Process.GetCurrentProcess();

      if (curProces != null)
      {
        values.machineName = Environment.MachineName;
        values.hostName = System.Net.Dns.GetHostName();
        values.startTime = curProces.StartTime;
        values.threadCount = curProces.Threads?.Count ?? -1;
        values.processorTime = new
        {
          user = curProces.UserProcessorTime.ToString(),
          total = curProces.TotalProcessorTime.ToString()
        };
      }

      return Ok(values);
    }
  }
}
