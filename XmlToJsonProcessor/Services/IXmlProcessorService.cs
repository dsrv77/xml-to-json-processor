namespace XmlToJsonProcessor.Services
{
    public interface IXmlProcessorService
    {
        Task<string> ProcessXmlToJson(IFormFile file, string filename);
        Task SaveJsonToFile(string jsonContent, string originalFilename, string directoryPath);
    }
}
