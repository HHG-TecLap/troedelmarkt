using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Diagnostics;
using System.Linq;

namespace TroedelMarkt
{
    public class ClientException : Exception
    {
        public readonly HttpStatusCode? code;

        public ClientException(HttpStatusCode? code) : this(code, null) { }
        public ClientException(HttpStatusCode? code, string? message) : this(code, message, null) { }
        public ClientException(HttpStatusCode? code, string? message, Exception? innerException) : base(message, innerException) {
            this.code = code;
        }
    }
    public class UnauthorizedException : ClientException
    {
        public UnauthorizedException() : this(null) { }
        public UnauthorizedException(string? message) : this(message, null) { }
        public UnauthorizedException(string? message, Exception? innerException) : base(HttpStatusCode.Unauthorized, message, innerException) { }
    }
    public class NotFoundException : ClientException
    {
        public NotFoundException() : this(null) { }
        public NotFoundException(string? message) : this(message,null) { }
        public NotFoundException(string? message, Exception? innerException) : base(HttpStatusCode.NotFound, message, innerException) { }
    }
    public class DuplicateException : ClientException
    {
        public DuplicateException() : this(null) { }
        public DuplicateException(string? message) : this(message, null) { }
        public DuplicateException(string? message, Exception? innerException) : base(HttpStatusCode.Conflict, message, innerException) { }
    }
    public class DeletionOrderException : ClientException
    {
        public DeletionOrderException() : base(null) { }
        public DeletionOrderException(string? message) : this(message,null) { }
        public DeletionOrderException(string? message, Exception? innerException) : base(HttpStatusCode.Conflict, message, innerException) { }
    }
    public class FailedDeserializeException : ClientException
    {
        public FailedDeserializeException() : this(null) { }
        public FailedDeserializeException(string? message) : this(message, null) { }
        public FailedDeserializeException(string? message, Exception? innerException) : base(null, message, innerException) { }
    }

    public class HTTPManager
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private Uri _baseURI;
        private AuthenticationHeaderValue? _authenticationKey;

        public Uri BaseUri { get { return _baseURI; } }

        private HTTPManager(Uri uri)
        {
            _baseURI = uri;
            _authenticationKey = new AuthenticationHeaderValue("Hello!");
        }

        /*
		 * This is an asynchronous method.
		 * It returns a new instance of this class that is already authenticated
		 * Raises:
		 *  UnauthorizedException: The password is incorrect.
		 *  NotFoundException: The server specified does not reply.
		 *  ClientException: The server returned a non-standard status code
		*/
        public async static Task<HTTPManager> NewAuthenticated(
            string ip_or_hostname,
            int? port,
            string password
        )
        {

            HTTPManager newObject = new HTTPManager(
                ConstructBaseUri(ip_or_hostname, port)
            );
            await newObject.Authenticate(password);

            return newObject;
        }


        private static Uri ConstructBaseUri(string ip_or_hostname, int? port)
        {
            UriBuilder builder = new UriBuilder(
                "http",
                ip_or_hostname,
                (port.HasValue) ? port.Value : 3080
            );

            return builder.Uri;
        }

        /*
		 * This is an asynchronous method.
		 * This method authenticates the user and raises an UnauthorizedError if failed
		 * Raises:
		 *  UnauthorizedException: The password is incorrect.
		 *  NotFoundException: The server specified does not reply.
		 *  ClientException: The server returned a non-standard status code
		*/
        protected async Task Authenticate(string password)
        {
            HttpResponseMessage response = await httpClient.PostAsync(
                new Uri(_baseURI, "/login"),
                new StringContent(password)
            );
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        throw new UnauthorizedException();
                    default:
                        {
                            throw new ClientException(response.StatusCode, "Authentication returned non-standard status code.");
                        }
                }
            }

            _authenticationKey = AuthenticationHeaderValue.Parse(
                await response.Content.ReadAsStringAsync()
            );
        }


        private async Task<JsonObject> CreateNewTraderRequestHandler(
            string traderID,
            string name,
            decimal? rate
        )
        {
            JsonObject requestBody = new JsonObject{
              {"id", traderID},
              {"name", name}
            };
            if (rate is not null)
            {
                requestBody["rate"] = rate.ToString();
            }
            try
            {
                return await RawPostRequest<JsonObject>(
                    "/seller",
                    typeof(JsonObject),
                    requestBody
                );
            }
            catch (ClientException e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.Conflict:
                        throw new DuplicateException($"A trader with the id {traderID} already exists.", e);
                    case HttpStatusCode.BadRequest:
                        throw new ClientException(HttpStatusCode.BadRequest,"The request was not formatted correctly. This is on some level always an error in this class.", e);
                    default:
                        throw new ClientException(e.code, $"The server sent an unknown response code {e.code}", e);
                }
            }
        }

        /*
		 * This is an asynchronous method.
		 * It returns an updated instance of the trader that was passed in
		 * Raises:
		 *  UnauthorizedException: The client is not authenticated.
		 *  DuplicateException: A trader with this trader.TraderID already exists.
		*/
        public async Task<Trader> CreateNewTrader(string newTraderID, Trader trader)
        {
            return trader.UpdateFromJson(
                await CreateNewTraderRequestHandler(
                    trader.TraderID,
                    trader.Name,
                    trader.ProvisionRate
                )
            );
        }
        public async Task<Trader> CreateNewTrader(
            string traderID,
            string name,
            decimal? rate
        )
        {
            return Trader.FromJson(await CreateNewTraderRequestHandler(traderID, name, rate));
        }

        /*
		 * This is an asynchronous method.
		 * It returns an array of all traders currently registered with the server
		 * Raises:
		 *  UnauthorizedException: The client is not authenticated.
		*/
        public async Task<Trader[]> GetAllTraders()
        {
            JsonArray responseObject = await RawGetRequest<JsonArray>("/sellers");

            Trader[] traders = new Trader[responseObject.Count];
            int i = 0;
            foreach (JsonObject? singleJsonObject in responseObject)
            {
                traders[i] = Trader.FromJson(singleJsonObject!);
                i++;
            }

            return traders;
        }

        /*
		 * This is an asynchronous method.
		 * It returns a newly created Trader instance with all up-to-date information
		 * Raises:
		 *  UnauthorizedException: The client is not authenticated.
		 *  NotFoundException: No trader with this id exists.
		*/
        public async Task<Trader> GetTrader(string traderID)
        {
            // NOTE: traderID are assumed to be alphanumeric. This is enforced by the UI
            JsonObject responseObject;
            try
            {
                responseObject = await RawGetRequest<JsonObject>($"/seller/{traderID}");
            }
            catch (ClientException e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException($"No seller with the id {traderID} exists", e);
                    default:
                        throw new ClientException(e.code, $"Server responded with unexpected status code {e.code}", e);
                }
            }

            return Trader.FromJson(responseObject);
        }


        private async Task<JsonObject> DeleteTraderRequestHandler(string traderID)
        {
            try
            {
                // NOTE: traderID are assumed to be alphanumeric. This is enforced by the UI
                return await RawDeleteRequest<JsonObject>($"/seller/{traderID}");
            }
            catch (ClientException e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.Forbidden:
                        throw new DeletionOrderException($"The trader with id {traderID} has a non-null balance and may not be deleted.");
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException($"No trader with id {traderID} exists.");
                    default:
                        throw new ClientException(e.code, $"Server responded with unexpected status code {e.code}", e);
                }
            }
        }
        /*
         * This is an asynchronous method.
         * It returns the trader that was just deleted with all the previously existing information.
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
         *  NotFoundException: No trader with the specified id exists.
         *  DeletionOrderException: The selected trader has stored balance and cannot be deleted.
        */
        public async Task<Trader> DeleteTrader(string traderID)
        {
            return Trader.FromJson(await DeleteTraderRequestHandler(traderID));
        }
        public async Task<Trader> DeleteTrader(Trader trader)
        {
            return Trader.FromJson(await DeleteTraderRequestHandler(trader.TraderID));
        }

        /*
         * This is an asynchronous method.
         * It returns the Trader instance passed in with all applied updates
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
         *  NotFoundException: No trader with trader.TraderID exists.
        */
        public async Task<Trader> UpdateTrader(Trader trader)
        {
            JsonObject responseObject;
            try
            {
                // NOTE: traderID are assumed to be alphanumeric. This is enforced by the UI
                responseObject = await RawPatchRequest<JsonObject>(
                    $"/seller/{trader.TraderID}",
                    typeof(JsonObject),
                    trader.ToJson()
                );
            }
            catch (ClientException e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException($"No trader with the id {trader.TraderID} exists.", e);
                    case HttpStatusCode.BadRequest:
                        throw new ClientException(HttpStatusCode.BadRequest,"The request was formatted incorrectly. This is always an error within this class structure");
                    default:
                        throw new ClientException(e.code, $"The server responded with an unexpected status code {e.code}", e);
                }
            }

            return trader.UpdateFromJson(responseObject);
        }

        /*
         * This is an asynchronous method.
         * It returns an array of all the Traders that have been changed.
         * These are new objects, not old ones being updated.
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
        */
        public async Task<Trader[]> SellItems(TransactionItem[] items)
        {
            JsonArray content = new JsonArray();
            foreach (TransactionItem item in items)
            {
                content.Add<JsonObject>(item.ToJson());
            }

            JsonArray responseObject;
            try
            {
                responseObject = await RawPostRequest<JsonArray>(
                    "/sell",
                    typeof(JsonArray),
                    content
                );
            }
            catch (ClientException e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.BadRequest:
                        throw new ClientException(HttpStatusCode.BadRequest, $"The server received an invaild URI format. This is always an internal error", e);
                    default:
                        throw new ClientException(e.code, $"The server responded with an unexpected status code {e.code}", e);
                }
            }

            System.Collections.Generic.List<Trader> traderList = new System.Collections.Generic.List<Trader>();
            foreach (JsonNode? traderJson in responseObject)
            {
                traderList.Add(
                    Trader.FromJson((JsonObject)traderJson!)
                );
            }
            return traderList.ToArray();
        }
        public async Task<Trader[]> SellItems(System.Collections.Generic.List<TransactionItem> items)
        {
            return await SellItems(items.ToArray());
        }

        public async Task<System.IO.Stream> ExportCsv()
        {
            HttpContent responseContent = await RawSendNoDecode(HttpMethod.Get, "/exportcsv");
            return responseContent.ReadAsStream();
        }


        // Raw
        private async Task<responseType> RawGetRequest<responseType>(string subpath)
        {
            return await RawSend<responseType>(HttpMethod.Get, subpath);
        }

        private async Task<responseType> RawPostRequest<responseType>(string subpath, Type contentType, object content)
        {
            return await RawSend<responseType>(HttpMethod.Post, subpath, contentType, content);
        }

        private async Task<responseType> RawPutRequest<responseType>(string subpath, Type contentType, object content)
        {
            return await RawSend<responseType>(HttpMethod.Put, subpath, contentType, content);
        }

        private async Task<responseType> RawPatchRequest<responseType>(string subpath, Type contentType, object content)
        {
            return await RawSend<responseType>(HttpMethod.Patch, subpath, contentType, content);
        }

        private async Task<responseType> RawDeleteRequest<responseType>(string subpath)
        {
            return await RawSend<responseType>(HttpMethod.Delete, subpath);
        }

        private HttpRequestMessage PrepareRequest(HttpMethod method, string subpath)
        {
            HttpRequestMessage request = new HttpRequestMessage(
                method,
                new Uri(_baseURI, subpath)
            );
            if (_authenticationKey is null)
            {
                throw new UnauthorizedException();
            }
            request.Headers.Authorization = _authenticationKey;
            return request;
        }

        private async Task<HttpContent> RawSendNoDecode(
            HttpMethod method,
            string subpath,
            Type? contentType = null,
            object? content = null
        )
        {
            HttpRequestMessage request = PrepareRequest(method, subpath);
            if (contentType is not null && content is not null)
            {
                request.Content = JsonContent.Create(content, contentType);
            }
            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(request);
            }
            catch (HttpRequestException e)
            {
                throw new NotFoundException("The server does not respond", e);
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new ClientException(response.StatusCode);
            }
            return response.Content;
        }
        private async Task<responseType> RawSend<responseType>(
            HttpMethod method,
            string subpath,
            Type? contentType = null,
            object? content = null
        )
        {
            HttpContent responseContent = await RawSendNoDecode(
                method,
                subpath,
                contentType,
                content
            );

            responseType? decoded = await responseContent.ReadFromJsonAsync<responseType>();
            if (decoded is null)
                throw new FailedDeserializeException();
            return decoded;
        }
    }
}