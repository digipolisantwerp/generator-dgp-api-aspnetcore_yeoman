using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StarterKit.Api.Controllers;
using StarterKit.Api.Models.Status;
using StarterKit.Business.Monitoring;
using Xunit;
using Monitoring = StarterKit.Business.Monitoring.Monitoring;
using Status = StarterKit.Business.Monitoring.Status;

namespace StarterKit.UnitTests.Controllers
{
  public class StatusControllerTest
  {
    [Fact]
    public void CtrThrowsExceptionIfIStatusReaderIsNull()
    {
      var logger = new Mock<ILogger<StatusController>>().Object;
      var mapper = new Mock<IMapper>().Object;

      Assert.Throws<ArgumentException>(() => new StatusController(null, logger, mapper));
    }

    [Fact]
    public void CtrThrowsExceptionIfLoggerIsNull()
    {
      var statusReader = new Mock<IStatusReader>().Object;
      var mapper = new Mock<IMapper>().Object;

      Assert.Throws<ArgumentException>(() => new StatusController(statusReader, null, mapper));
    }

    [Fact]
    public async Task GetStatusUsesIStatusReader()
    {
      var logger = new Mock<ILogger<StatusController>>().Object;
      var mapper = new Mock<IMapper>().Object;
      var statusReaderMock = new Mock<IStatusReader>();
      statusReaderMock.Setup(x => x.GetStatus()).Returns(Task.FromResult(new Monitoring() { Status = Status.warning })).Verifiable();

      var controller = new StatusController(statusReaderMock.Object, logger,mapper);

      var result = (Monitoring)(await controller.GetMonitoring() as OkObjectResult)?.Value;

      statusReaderMock.Verify(x => x.GetStatus(), Times.Once());
    }


    [Fact]
    public void GetPingReturnsStatusOk()
    {
      var mapper = new Mock<IMapper>().Object;
      var logger = new Mock<ILogger<StatusController>>().Object;
      var statusReaderMock = new Mock<IStatusReader>();

      var controller = new StatusController(statusReaderMock.Object, logger,mapper);

      var result = (StatusResponse)(controller.GetPing() as OkObjectResult)?.Value;

      Assert.NotNull(result);
      Assert.Equal(Api.Models.Status.Status.ok, result.Status);
    }
  }
}
