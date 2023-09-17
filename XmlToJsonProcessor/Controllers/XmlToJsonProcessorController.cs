using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XmlToJsonProcessor.Services;
using XmlToJsonProcessor.Settings;
using Microsoft.AspNetCore.Hosting;

namespace XmlToJsonProcessor.Controllers
{
    [ApiController]
    [Route("xml-to-json")]
    public class XmlProcessorController : ControllerBase
    {


        private readonly ILogger<XmlProcessorController> _logger;
        private readonly IXmlProcessorService _xmlProcessorService;
        private readonly string _targetDirectory;

        public XmlProcessorController(ILogger<XmlProcessorController> logger, IXmlProcessorService xmlProcessorService, IOptions<FileSaveSettings> fileSaveSettings, IWebHostEnvironment env)
        {
            _logger = logger;
            _xmlProcessorService = xmlProcessorService;
            _targetDirectory = Path.Combine(env.ContentRootPath, fileSaveSettings.Value.OutputPath);

        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file, [FromForm] string filename)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("Empty file");
                return BadRequest("Empty file");
            }


            if (Path.GetExtension(file.FileName) != ".xml")
            {
                _logger.LogError("Not an XML file");
                return BadRequest("Not an XML file");
            }

            if (string.IsNullOrEmpty(filename))
            {
                _logger.LogError("Filename is required");
                return BadRequest("Filename is required");
            }
            try
            {
                string jsonContent = await _xmlProcessorService.ProcessXmlToJson(file, filename);

                await _xmlProcessorService.SaveJsonToFile(jsonContent, filename, _targetDirectory);
                return Ok(jsonContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error processing XML to JSON for {file.FileName}");
                return BadRequest(ex.Message);
            }
        }
    }
}