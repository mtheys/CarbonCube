using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarbonCubeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPIController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<ValuesController>/5
        [HttpGet("api/GetAsync")]
        public async Task<string> Get(int id)
        {
            string responseObj = string.Empty;

            await GetInfo();


            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public static async Task<string> GetInfo()
        {

            var clientId = "3be91816-9367-4018-98f3-c559d29aff4c_7ffdf4bb-e922-4f4a-9809-a5d24f463eab";
            var clientSecret = "SNc6qmaqoRGIBu/Px1femmn2fYyJhHQTTiMPPKkZYcc=";

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://icdaccessmanagement.who.int");
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = "icdapi_access",
                GrantType = "client_credentials",
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            HttpRequestMessage request;


            Console.WriteLine();
            Console.WriteLine("****************************************************************");
            Console.WriteLine("Requesting the root foundation URI...");
            request = new HttpRequestMessage(HttpMethod.Get, "https://id.who.int/icd/entity");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en"));
            request.Headers.Add("API-Version", "v2");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }

            var resultJson = response.Content.ReadAsStringAsync().Result;
            var prettyJson = JValue.Parse(resultJson).ToString(Formatting.Indented); //convert json to a more human readable fashion

            var term = "tube";
            request = new HttpRequestMessage(HttpMethod.Get, "https://id.who.int/icd/entity/search?q=" + term);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en"));
            request.Headers.Add("API-Version", "v2");
            response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }

            resultJson = response.Content.ReadAsStringAsync().Result; //Now resultJson has the resulting json string
          

            prettyJson = JValue.Parse(resultJson).ToString(Formatting.Indented); //convert json to a more human readable fashion
        

            //Now trying to parse and get titles from the search result

          
            dynamic searchResult = JsonConvert.DeserializeObject(resultJson);



            foreach (var de in searchResult.objectList)
            {
                Console.WriteLine(de.TheCode + " " + de.Title);
            }
        

            return "";
        }

        public static async Task<string> GetAuthorization(string authorizeToken)
        {
            string responseObj = string.Empty;

            // HTTP GET.  
            using (var client = new HttpClient())
            {
                // Initialization  
                string authorization = authorizeToken;

                // Setting Authorization.  
                client.DefaultRequestHeaders.Add("Client ID", "3be91816-9367-4018-98f3-c559d29aff4c_7ffdf4bb-e922-4f4a-9809-a5d24f463eab");
                client.DefaultRequestHeaders.Add("Client Secret", "SNc6qmaqoRGIBu/Px1femmn2fYyJhHQTTiMPPKkZYcc=");


                // Setting Base address.  
                client.BaseAddress = new Uri("https://icdaccessmanagement.who.int/");

                // Setting content type.  
             
                // Initialization.  
                HttpResponseMessage response = new HttpResponseMessage();

                // HTTP GET  
                response = await client.GetAsync("connect/token").ConfigureAwait(false);

                // Verification  
                if (response.IsSuccessStatusCode)
                {
                    // Reading Response.  

                }


            }

            return responseObj;
        }
    }

    public class MatchingPV
    {
        public string propertyId { get; set; }
        public string label { get; set; }
        public double score { get; set; }
        public bool important { get; set; }
    }

    public class DestinationEntity
    {
        public string id { get; set; }
        public string title { get; set; }
        public string stemId { get; set; }
        public bool isLeaf { get; set; }
        public int postcoordinationAvailability { get; set; }
        public bool hasCodingNote { get; set; }
        public bool hasMaternalChapterLink { get; set; }
        public List<MatchingPV> matchingPVs { get; set; }
        public bool propertiesTruncated { get; set; }
        public bool isResidualOther { get; set; }
        public bool isResidualUnspecified { get; set; }
        public string chapter { get; set; }
        public object theCode { get; set; }
        public double score { get; set; }
        public bool titleIsASearchResult { get; set; }
        public bool titleIsTopScore { get; set; }
        public int entityType { get; set; }
        public bool important { get; set; }
        public List<object> descendants { get; set; }
    }

    public class Root
    {
        public bool error { get; set; }
        public object errorMessage { get; set; }
        public bool resultChopped { get; set; }
        public bool wordSuggestionsChopped { get; set; }
        public int guessType { get; set; }
        public string uniqueSearchId { get; set; }
        public object words { get; set; }
        public List<DestinationEntity> destinationEntities { get; set; }
    }

}
