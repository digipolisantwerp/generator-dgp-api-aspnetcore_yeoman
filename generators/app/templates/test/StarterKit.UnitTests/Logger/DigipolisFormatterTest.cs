using System;
using System.IO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Parsing;
using StarterKit.Api.Controllers;
using StarterKit.Framework.Logging;
using Xunit;

namespace StarterKit.UnitTests.Logger
{
  public class DigipolisFormatterTest
  {
    private readonly DigipolisFormatter _formatter;
    private readonly StringWriter _output;

    public DigipolisFormatterTest()
    {
      _formatter = new DigipolisFormatter();
      _output = new StringWriter();
    }

    [Fact]
    public void ShouldLogMessageAsJson()
    {
      var log = new LogEvent(
        DateTimeOffset.Now,
        LogEventLevel.Information,
        null,
        new MessageTemplate(new [] { new TextToken("Test") } ),
        new LogEventProperty[0]);

      _formatter.Format(log, _output);

      //check if we have a log
      var jsonResult = _output.GetStringBuilder().ToString();
      Assert.NotNull(jsonResult);

      //JSON deserialize and check if we see what we expect
      var result = JsonConvert.DeserializeObject<LogResult>(jsonResult);

      Assert.Null(result.Exception);
      Assert.Equal("Test", result.Message);
      Assert.Equal(LogEventLevel.Information, result.Level);

    }

    [Fact]
    public void ShouldOutputException()
    {
      var log = new LogEvent(
        DateTimeOffset.Now,
        LogEventLevel.Information,
        new Exception("Test exception"),
        MessageTemplate.Empty,
        new LogEventProperty[0]);

      _formatter.Format(log, _output);

      //check if we have a log
      var jsonResult = _output.GetStringBuilder().ToString();
      Assert.NotNull(jsonResult);

      //JSON deserialize and check if we see what we expect
      var result = JsonConvert.DeserializeObject<LogResult>(jsonResult);

      Assert.NotNull(result.Exception);
      Assert.Equal("System.Exception: Test exception", result.Exception);
    }

    [Fact]
    public void ShouldNotOutputAdditionalProperties()
    {
      var log = new LogEvent(
        DateTimeOffset.Now,
        LogEventLevel.Information,
        null,
        MessageTemplate.Empty,
        new LogEventProperty[]
        {
          new LogEventProperty("Additional", new ScalarValue("TestValue")),
        });

      _formatter.Format(log, _output);

      //check if we have a log
      var jsonResult = _output.GetStringBuilder().ToString();
      Assert.NotNull(jsonResult);

      //JSON deserialize and check if we see what we expect
      var result = JsonConvert.DeserializeObject<LogResult>(jsonResult);

      Assert.Null(result.Additional);
    }

    [Fact]
    public void ShouldOutputLoggingProperties()
    {
      var log = new LogEvent(
        DateTimeOffset.Now,
        LogEventLevel.Information,
        null,
        MessageTemplate.Empty,
        new LogEventProperty[]
        {
          new LogEventProperty("CorrelationId", new ScalarValue("TestValue")),
          new LogEventProperty("ApplicationId", new ScalarValue("TestValue")),
          new LogEventProperty("Host", new ScalarValue("TestValue")),
          new LogEventProperty("Headers", new ScalarValue("TestValue")),
          new LogEventProperty("Path", new ScalarValue("TestValue")),
          new LogEventProperty("Payload", new ScalarValue("TestValue")),
          new LogEventProperty("Protocol", new ScalarValue("TestValue")),
          new LogEventProperty("Method", new ScalarValue("TestValue")),
          new LogEventProperty("Status", new ScalarValue("TestValue")),
          new LogEventProperty("Duration", new ScalarValue("TestValue")),
          new LogEventProperty("Type", new ScalarValue("TestValue")),
          new LogEventProperty("MessageUser", new ScalarValue("TestValue")),
          new LogEventProperty("MessageUserIsAuthenticated", new ScalarValue("TestValue")),
        });

      _formatter.Format(log, _output);

      //check if we have a log
      var jsonResult = _output.GetStringBuilder().ToString();
      Assert.NotNull(jsonResult);

      //JSON deserialize and check if we see what we expect
      var result = JsonConvert.DeserializeObject<LogResult>(jsonResult);

      Assert.Equal("TestValue", result.CorrelationId);
      Assert.Equal("TestValue", result.ApplicationId);
      Assert.Equal("TestValue", result.Host);
      Assert.Equal("TestValue", result.Headers);
      Assert.Equal("TestValue", result.Path);
      Assert.Equal("TestValue", result.Payload);
      Assert.Equal("TestValue", result.Protocol);
      Assert.Equal("TestValue", result.Method);
      Assert.Equal("TestValue", result.Status);
      Assert.Equal("TestValue", result.Duration);
      Assert.Equal("TestValue", result.Type);
      Assert.Equal("TestValue", result.MessageUser);
      Assert.Equal("TestValue", result.MessageUserIsAuthenticated);
    }

    [Fact]
    public void ShouldLogMessagePropertiesAsSeperateObject()
    {
      var log = new LogEvent(
        DateTimeOffset.Now,
        LogEventLevel.Information,
        null,
        new MessageTemplate("{Additional} Test {Additional2}",
          new MessageTemplateToken[]
          {
            new PropertyToken("Additional", "{Additional}"),
            new TextToken(" Test "),
            new PropertyToken("Additional2", "{Additional2}")
          } ),
        new LogEventProperty[]
        {
          new LogEventProperty("Additional", new ScalarValue("TestValue")),
          new LogEventProperty("Additional2", new ScalarValue("TestValue2")),
        });

      _formatter.Format(log, _output);

      //check if we have a log
      var jsonResult = _output.GetStringBuilder().ToString();
      Assert.NotNull(jsonResult);

      //JSON deserialize and check if we see what we expect
      var result = JsonConvert.DeserializeObject<LogResult>(jsonResult);

      Assert.Null(result.Additional);
      Assert.Equal("TestValue", result.MessageProperties.Additional);
      Assert.Equal("TestValue2", result.MessageProperties.Additional2);


    }

  }
}

public class LogResult
{
  public DateTime Timestamp { get; set; }
  public LogEventLevel Level { get; set; }
  public string Message { get; set; }
  public string Exception { get; set; }
  public string Additional { get; set; }
  public string CorrelationId { get; set; }
  public string ApplicationId { get; set; }
  public string Host { get; set; }
  public string Headers { get; set; }
  public string Path { get; set; }
  public string Payload { get; set; }
  public string Protocol { get; set; }
  public string Method { get; set; }
  public string Status { get; set; }
  public string Duration { get; set; }
  public string Type { get; set; }
  public string MessageUser { get; set; }
  public string MessageUserIsAuthenticated { get; set; }

  public MessageProperties MessageProperties { get; set; }

}

public class MessageProperties
{
  public string Additional { get; set; }
  public string Additional2 { get; set; }
}
