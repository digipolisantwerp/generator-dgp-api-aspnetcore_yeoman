using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Digipolis.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarterKit.Api.Models.Status;
using StarterKit.Business.Monitoring;
using StarterKit.Shared.Constants;
using Status = StarterKit.Api.Models.Status.Status;

namespace StarterKit.Api.Controllers
{
	[Route("v{version:apiVersion}/[controller]")]
	[ApiController, ApiVersion(Versions.V1)]
	[Authorize]
	public class StatusController : Controller
	{
		private readonly ILogger<StatusController> _logger;
		private readonly IMapper _mapper;

		private readonly IStatusReader _statusReader;

		public StatusController(IStatusReader statusReader, ILogger<StatusController> logger, IMapper mapper)
		{
			_statusReader = statusReader ??
			                throw new ArgumentException(
				                $"StatusController.Ctr parameter {nameof(statusReader)} cannot be null.");
			_logger = logger ??
			          throw new ArgumentException($"StatusController.Ctr parameter {nameof(logger)} cannot be null.");
			_mapper = mapper ??
			          throw new ArgumentException($"StatusController.Ctr parameter {nameof(mapper)} cannot be null.");
		}

		/// <summary>
		/// Get the global API health status.
		/// </summary>
		/// <returns></returns>
		[HttpGet("health")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(StatusResponse), 200)]
		[ProducesResponseType(typeof(Error), 500)]
		[AllowAnonymous]
		public IActionResult GetHealthStatus()
		{
			// TODO: run checks here on all critical services (fe: db, serviceAgent, logging etc)

			return Ok(new StatusResponse
			{
				Status = Status.ok
			});
		}

		/// <summary>
		/// Get the global API status and the components statuses.
		/// </summary>
		/// <returns></returns>
		[HttpGet("health/components")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(ComponentsHealthStatus), 200)]
		[ProducesResponseType(typeof(Error), 500)]
		public async Task<IActionResult> GetComponentsHealthStatus()
		{
			var status = await _statusReader.GetStatus();

			var result = _mapper.Map<ComponentsHealthStatus>(status);

			return Ok(result);
		}


		/// <summary>
		/// Get the runtime configuration
		/// </summary>
		/// <returns></returns>
		[HttpGet("runtime")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(IDictionary<string, object>), 200)]
		[ProducesResponseType(typeof(Error), 500)]
		[AllowAnonymous]
		public IActionResult GetRuntimeValues()
		{
			_logger.LogInformation("Getting runtime information");

			var runtimeInformation = new RuntimeInformation
			{
				ReleaseVersion = Environment.GetEnvironmentVariable("RELEASE_VERSION")
			};

			using (var currentProcess = Process.GetCurrentProcess())
			{
				runtimeInformation.MachineName = Environment.MachineName;
				runtimeInformation.HostName = System.Net.Dns.GetHostName();
				runtimeInformation.StartTime = currentProcess.StartTime;
				runtimeInformation.ProcessorCount = Environment.ProcessorCount;
				runtimeInformation.OperatingSystem = Environment.OSVersion.ToString();
				runtimeInformation.ThreadCount = currentProcess.Threads?.Count ?? -1;
				runtimeInformation.UserProcessorTime = currentProcess.UserProcessorTime;
				runtimeInformation.TotalProcessorTime = currentProcess.TotalProcessorTime;
			}

			return Ok(runtimeInformation);
		}

		/// <summary>
		/// Datetime timezone (for testing purposes)
		/// </summary>
		/// <returns></returns>
		[HttpGet("v{version:apiVersion}/[controller]/dateTimes/timezone")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(StatusResponse), 200)]
		[ProducesResponseType(typeof(Error), 500)]
		[ApiVersion(Versions.V1)]
		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		public IActionResult GetLocalTimezone()
		{
			_logger.LogInformation($"Container timezone: {TimeZoneInfo.Local.DisplayName}");
			return Ok(TimeZoneInfo.Local.DisplayName);
		}
	}
}
