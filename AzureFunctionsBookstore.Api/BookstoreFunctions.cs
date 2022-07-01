using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureFunctionsBookstore.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace AzureFunctionsBookstore.Api
{
    public class BookstoreFunctions
    {
        private readonly ILogger<BookstoreFunctions> _logger;

        public BookstoreFunctions(ILogger<BookstoreFunctions> log)
        {
            _logger = log;
        }

        [FunctionName("GetBooks")]
        [OpenApiOperation(operationId: "GetBooks", tags: new[] {"Books"})]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "No parameters")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> GetBooks(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "book")] HttpRequest req)
        {
            var books = new List<Book>();

            // Get books from database...

            _logger.LogInformation($"Getting all books from the database.");

            return new OkObjectResult(books);
        }

        [FunctionName("GetBookById")]
        [OpenApiOperation(operationId: "GetBookById", tags: new[] { "Books" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> GetBookById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "book/{id}")] HttpRequest req, string id)
        {
            var book = new Book();
            // string id = req.Query["id"];

            // Get book by id from database..

            _logger.LogInformation($"Get book {book.Title} from database.");

            return new OkObjectResult(book);
        }

        [FunctionName("AddBook")]
        [OpenApiOperation(operationId: "AddBook", tags: new[] { "Books" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "Book ", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "{Id: int, Title: string, Author: string}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> AddBook(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "book")] HttpRequest req)
        {
            // getting book to add from request body
            // var requestBody = req.Body;
            // var streamReader = new StreamReader(requestBody);
            // var book = JsonSerializer.Deserialize<Book>(streamReader.ReadToEnd());

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic book = JsonConvert.DeserializeObject<Book>(requestBody);

            // Add new book to database...

            _logger.LogInformation($"Add new book with Title {book.Title} and id = {book.Id} to database.");

            return new OkObjectResult(book);
        }

        [FunctionName("UpdateBook")]
        [OpenApiOperation(operationId: "AddBook", tags: new[] { "Books" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "Book ", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "{Id: int, Title: string, Author: string}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateBook(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "book/{id}")] HttpRequest req, string id)
        {
            // getting book to update from request body
            // var requestBody = req.Body;
            // var streamReader = new StreamReader(requestBody);
            // var book = JsonSerializer.Deserialize<Book>(streamReader.ReadToEnd());

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic book = JsonConvert.DeserializeObject<Book>(requestBody);

            // Update book with given id in database...

            _logger.LogInformation($"Update book Title {book.Title} and id = {book.Id} in database.");

            return new OkObjectResult(book);
        }

        [FunctionName("DeleteBook")]
        [OpenApiOperation(operationId: "AddBook", tags: new[] { "Books" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "Book ", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Id")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteBook(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "book/{id}")] HttpRequest req, string id)
        {
            // getting book to update from request body
            // var book = new Book();
            // string id = req.Query["id"];

            try
            {
                // Try delete book from database...

                _logger.LogInformation($"Deleted book with id {id} from database.");
            }
            catch (StorageException exception) when (exception.RequestInformation.HttpStatusCode == 404)
            {
                return new NotFoundResult();
            }
            

            return new OkObjectResult(id);
        }
    }
}

/*
 The original
namespace AzureFunctionsBookstore.Api
{
    public class BookstoreFunctions
    {
        private readonly ILogger<BookstoreFunctions> _logger;

        public BookstoreFunctions(ILogger<BookstoreFunctions> log)
        {
            _logger = log;
        }

        [FunctionName("Function1")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
 */

