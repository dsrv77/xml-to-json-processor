using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System.Net.Http.Json;
using XmlToJsonProcessor.Controllers;
using XmlToJsonProcessor.Services;
using XmlToJsonProcessor.Settings;

namespace XmlToJsonProcessorTests.Controllers
{
    [TestFixture]
    public class XmlProcessorControllerTests
    {
        private ILogger<XmlProcessorController> _logger;
        private IXmlProcessorService _xmlProcessorService;
        private IOptions<FileSaveSettings> _fileSaveSettings;
        private IWebHostEnvironment _env;

        [SetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger<XmlProcessorController>>();
            _xmlProcessorService = Substitute.For<IXmlProcessorService>();
            _fileSaveSettings = Substitute.For<IOptions<FileSaveSettings>>();
            _env = Substitute.For<IWebHostEnvironment>();

            _env.ContentRootPath.Returns("C:\\randomPath");
            _fileSaveSettings.Value.Returns(new FileSaveSettings { OutputPath = "output" });
        }

        [Test]
        public async Task Should_Return_BadRequest_When_File_Is_Null()
        {
            var controller = new XmlProcessorController(_logger, _xmlProcessorService, _fileSaveSettings, _env);
            var result = await controller.Post(null, "test.xml");

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Should_Return_BadRequest_When_File_Is_Empty()
        {
            var stream = new MemoryStream();
            var file = new FormFile(stream, 0, 0, "test","test.xml");

            var controller = new XmlProcessorController(_logger, _xmlProcessorService, _fileSaveSettings, _env);
            var result = await controller.Post(file, "test.xml");

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Should_Return_BadRequest_When_File_Has_Errors()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("<xml>content");
            writer.Flush();
            stream.Position = 0;

            var file = new FormFile(stream, 0, stream.Length, "test", "test.xml");
            _xmlProcessorService.ProcessXmlToJson(file, "test.xml").Throws(new Exception("Missing closing tag"));

            var controller = new XmlProcessorController(_logger, _xmlProcessorService, _fileSaveSettings, _env);
            var result = await controller.Post(file, "test.xml");

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Should_Process_And_Save_File()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("<xml>content</xml>");
            writer.Flush();
            stream.Position = 0;

            var file = new FormFile(stream, 0, stream.Length, "file", "filename.xml");
            var jsonContent = "{\"key\":\"value\"}";

            _xmlProcessorService.ProcessXmlToJson(file, "filename.xml")
                .Returns(Task.FromResult(jsonContent));

            var controller = new XmlProcessorController(_logger, _xmlProcessorService, _fileSaveSettings, _env);
            var result = await controller.Post(file, "filename.xml");

            await _xmlProcessorService.Received().ProcessXmlToJson(file, "filename.xml");
            await _xmlProcessorService.Received().SaveJsonToFile(jsonContent, "filename.xml", Arg.Any<string>());

            result.ShouldBeOfType<OkResult>();
        }
    }
}
