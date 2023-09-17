# xml-to-json-processor

.NET Core 6.0 project for parsing xml files to json

# Running the API with Docker:

 - go into XmlToJsonProcessor folder
 - execute docker build -t xml-to-json .
 - execute docker run -p 8080:80 xml-to-json

  API Endpoint listens on:

  POST http://localhost:8080/xml-to-json


# Running the API in local terminal

   - go into XmlToJsonProcessor folder
   - execute dotnet run

API Endpoint listens on:

POST https://localhost:5000/xml-to-json


The endpoint expects:
  
  Headers: 
  
  Content-Type: multipart/form-data

  With Body:
  
  file - the .xml file
  
  filename - string 

# Testing & Concurrency

In XmlToJsonProcessor/test-files there are several files which I have used to test the API.

The application saves the resulting json files in a directory specified in appsettings. Currently set to relative path output.

There also can be found a bash script concurrent-requests.sh which fires multiple requests at the same time.

To demonstrate concurrency logs have been added which identify the ThreadId processing the file.
