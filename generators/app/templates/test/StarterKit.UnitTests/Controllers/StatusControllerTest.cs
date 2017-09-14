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
    public void CtrThrowsExceptionIfIStatusProviderIsNull()
    {
      var logger = new Moq.Mock<ILogger<StatusController>>().Object;
      var mapper = new Moq.Mock<IMapper>().Object;

      Assert.Throws<ArgumentException>(() => new StatusController(null, logger, mapper));
    }

    [Fact]
    public void CtrThrowsExceptionIfLoggerIsNull()
    {
      var statusProvider = new Moq.Mock<IStatusProvider>().Object;
      var mapper = new Moq.Mock<IMapper>().Object;

      Assert.Throws<ArgumentException>(() => new StatusController(statusProvider, null, mapper));
    }

    [Fact]
    public async Task GetStatusUsesIStatusProvider()
    {
      var logger = new Moq.Mock<ILogger<StatusController>>().Object;
      var mapper = new Moq.Mock<IMapper>().Object;
      var statusProviderMock = new Moq.Mock<IStatusProvider>();
      statusProviderMock.Setup(x => x.GetStatus()).Returns(Task.FromResult(new Monitoring() { Status = Status.warning })).Verifiable();

      var controller = new StatusController(statusProviderMock.Object, logger,mapper);

      var result = (Monitoring)(await controller.GetMonitoring() as OkObjectResult).Value;

      statusProviderMock.Verify(x => x.GetStatus(), Times.Once());
    }


    [Fact]
    public async Task GetPingReturnsStatusOk()
    {
      var mapper = new Moq.Mock<IMapper>().Object;
      var logger = new Moq.Mock<ILogger<StatusController>>().Object;
      var statusProviderMock = new Moq.Mock<IStatusProvider>();

      var controller = new StatusController(statusProviderMock.Object, logger,mapper);

      var result = (Api.Models.StatusResponse)(await controller.GetPing() as OkObjectResult).Value;

      Assert.Equal(Api.Models.Status.ok, result.Status);
    }
  }
}
