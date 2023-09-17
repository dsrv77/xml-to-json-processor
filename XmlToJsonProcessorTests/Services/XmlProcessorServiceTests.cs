using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using System.Text;
using XmlToJsonProcessor.Services;

namespace XmlToJsonProcessorTests.Services
{
    [TestFixture]
    public class XmlProcessorServiceTests
    {
        private XmlProcessorService _service;
        private ILogger<XmlProcessorService> _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger<XmlProcessorService>>();
            _service = new XmlProcessorService(_logger);
        }

        [Test]
        public async Task Should_Convert_Xml_To_Json()
        {
            var xmlContent = "<root><child>text</child></root>";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            var formFile = new FormFile(stream, 0, stream.Length, "Data", "dummy.xml");

            var jsonResult = await _service.ProcessXmlToJson(formFile, "dummy.xml");

            jsonResult.ShouldNotBeEmpty();
            var jsonEquivalent = JsonConvert.SerializeObject(new { root = new { child = "text" } });
            jsonResult.ShouldBe(jsonEquivalent);
        }

        [Test]
        public async Task Should_Save_Json_To_File()
        {
            var jsonContent = "{\"root\": {\"child\": \"text\"}}";
            var directoryPath = "outputDirectory";
            var originalFilename = "dummy.xml";

            await _service.SaveJsonToFile(jsonContent, originalFilename, directoryPath);
            var savedFilePath = Path.Combine(directoryPath, "dummy.json");
            File.Exists(savedFilePath).ShouldBeTrue();
            var savedContent = await File.ReadAllTextAsync(savedFilePath);
            savedContent.ShouldBe(jsonContent);
        }

        [Test]
        public async Task Should_Throw_Exception_On_Invalid_Xml()
        {
            var invalidXmlContent = "this is not xml";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidXmlContent));
            var formFile = new FormFile(stream, 0, stream.Length, "Data", "invalid.xml");

            await Should.ThrowAsync<Exception>(async () =>
            {
                await _service.ProcessXmlToJson(formFile, "invalid.xml");
            });
        }

    }
}
