using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            // to convert object to json
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };
            // add the header to the response
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            // allow the header to be exposed
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}