using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StarterKit.Api.Controllers;
using StarterKit.Business.Monitoring;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StarterKit.UnitTests
{
  public class StatusControllerTest
  {
    [Fact]
    public void CtrThrowsExceptionIfIStatusReaderIsNull()
    {
      var logger = new Moq.Mock<ILogger<StatusController>>().Object;
      var mapper = new Moq.Mock<IMapper>().Object;

      Assert.Throws<ArgumentException>(() => new StatusController(null, logger, mapper));
    }

    [Fact]
    public void CtrThrowsExceptionIfLoggerIsNull()
    {
      var statusReader = new Moq.Mock<IStatusReader>().Object;
      var mapper = new Moq.Mock<IMapper>().Object;

      Assert.Throws<ArgumentException>(() => new StatusController(statusReader, null, mapper));
    }

    [Fact]
    public async Task GetStatusUsesIStatusReader()
    {
      var logger = new Moq.Mock<ILogger<StatusController>>().Object;
      var mapper = new Moq.Mock<IMapper>().Object;
      var statusReaderMock = new Moq.Mock<IStatusReader>();
      statusReaderMock.Setup(x => x.GetStatus()).Returns(Task.FromResult(new Monitoring() { Status = Status.warning })).Verifiable();

      var controller = new StatusController(statusReaderMock.Object, logger,mapper);

      var result = (Monitoring)(await controller.GetMonitoring() as OkObjectResult).Value;

      statusReaderMock.Verify(x => x.GetStatus(), Times.Once());
    }


    [Fact]
    public void GetPingReturnsStatusOk()
    {
      var mapper = new Moq.Mock<IMapper>().Object;
      var logger = new Moq.Mock<ILogger<StatusController>>().Object;
      var statusReaderMock = new Moq.Mock<IStatusReader>();

      var controller = new StatusController(statusReaderMock.Object, logger,mapper);

      var result = (Api.Models.StatusResponse)(controller.GetPing() as OkObjectResult).Value;

      Assert.Equal(Api.Models.Status.ok, result.Status);
    }
  }
}
