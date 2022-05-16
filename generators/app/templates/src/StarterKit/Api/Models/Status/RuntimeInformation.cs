using System;

namespace StarterKit.Api.Models.Status
{
	public class RuntimeInformation
	{
		public string ReleaseVersion { get; set; }
		public string MachineName { get; set; }
		public string HostName { get; set; }
		public DateTime StartTime { get; set; }
		public string OperatingSystem { get; set; }
		public float ProcessorCount { get; set; }
		public int ThreadCount { get; set; }
		public TimeSpan UserProcessorTime { get; set; }
		public TimeSpan TotalProcessorTime { get; set; }
	}
}