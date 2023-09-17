using Newtonsoft.Json;
using System.Net.Security;
using System.Xml;

namespace XmlToJsonProcessor.Services
{
    public class XmlProcessorService: IXmlProcessorService
    {
        private readonly ILogger<XmlProcessorService> _logger;
        public XmlProcessorService(ILogger<XmlProcessorService> logger)
        {
            _logger = logger;
        }
        public async Task<string> ProcessXmlToJson(IFormFile file, string filename)
        {
            try
            {
                string jsonContent = "";

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    _logger.LogInformation($"{DateTime.UtcNow} Thread: {Thread.CurrentThread.ManagedThreadId} Reading XML content for {filename}", filename);
                    string xmlContent = await reader.ReadToEndAsync();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlContent);
                    jsonContent = JsonConvert.SerializeXmlNode(doc);
                    _logger.LogInformation($"{DateTime.UtcNow} Thread: {Thread.CurrentThread.ManagedThreadId} Converted XML to JSON for {filename}", filename);
                }

                return jsonContent;
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message, $"Error processing XML to JSON for {filename}");
                throw;
            }

        }

        public async Task SaveJsonToFile(string jsonContent, string originalFilename, string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string jsonFilename = Path.GetFileNameWithoutExtension(originalFilename) + ".json";
                string fullPath = Path.Combine(directoryPath, jsonFilename);

                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    _logger.LogInformation($"{DateTime.UtcNow} Thread: {Thread.CurrentThread.ManagedThreadId} Saving JSON to {fullPath}", fullPath);
                    await writer.WriteAsync(jsonContent);
                    _logger.LogInformation($"{DateTime.UtcNow} Thread: {Thread.CurrentThread.ManagedThreadId}  Saved JSON to {fullPath}", fullPath);
                }
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message, $"Error saving JSON to file for {originalFilename}");
                throw;
            }

        }
    }
}
