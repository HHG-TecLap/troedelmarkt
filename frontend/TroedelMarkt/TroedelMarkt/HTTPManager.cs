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
    /// <summary>
    /// An exeption thrown, when a HttpError occures 
    /// </summary>
    public class ClientException : Exception
    {
        public readonly HttpStatusCode? code;

        public ClientException(HttpStatusCode? code) : this(code, null) { }
        public ClientException(HttpStatusCode? code, string? message) : this(code, message, null) { }
        public ClientException(HttpStatusCode? code, string? message, Exception? innerException) : base(message, innerException)
        {
            this.code = code;
        }
    }
    /// <summary>
    /// An exception thrown, when a HttpUnouthorisedError occures
    /// </summary>
    public class UnauthorizedException : ClientException
    {
        public UnauthorizedException() : this(null) { }
        public UnauthorizedException(string? message) : this(message, null) { }
        public UnauthorizedException(string? message, Exception? innerException) : base(HttpStatusCode.Unauthorized, message, innerException) { }
    }
    /// <summary>
    ///  An exception thrown, when a HttpNotFoundError occures
    /// </summary>
    public class NotFoundException : ClientException
    {
        public NotFoundException() : this(null) { }
        public NotFoundException(string? message) : this(message, null) { }
        public NotFoundException(string? message, Exception? innerException) : base(HttpStatusCode.NotFound, message, innerException) { }
    }
    /// <summary>
    ///  An exception thrown, when an HttpConflictError occures due to an duplicate error
    /// </summary>
    public class DuplicateException : ClientException
    {
        public DuplicateException() : this(null) { }
        public DuplicateException(string? message) : this(message, null) { }
        public DuplicateException(string? message, Exception? innerException) : base(HttpStatusCode.Conflict, message, innerException) { }
    }
    /// <summary>
    ///  An exception thrown, when a deletion odrer has an error
    /// </summary>
    public class DeletionOrderException : ClientException
    {
        public DeletionOrderException() : base(null) { }
        public DeletionOrderException(string? message) : this(message, null) { }
        public DeletionOrderException(string? message, Exception? innerException) : base(HttpStatusCode.Conflict, message, innerException) { }
    }
    /// <summary>
    /// An exception thrown, when a a deserialisation failded 
    /// </summary>
    public class FailedDeserializeException : ClientException
    {
        public FailedDeserializeException() : this(null) { }
        public FailedDeserializeException(string? message) : this(message, null) { }
        public FailedDeserializeException(string? message, Exception? innerException) : base(null, message, innerException) { }
    }

    /// <summary>
    /// A class for managing Http communications with the server
    /// </summary>
    public class HTTPManager
    {
        private static System.Globalization.CultureInfo InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;
        private class HttpStatusCarrier : Exception
        {
            public readonly HttpStatusCode code;
            public readonly HttpContent content;
            public HttpStatusCarrier(HttpStatusCode code, HttpContent content)
            {
                this.code = code;
                this.content = content;
            }
        }


        private static readonly HttpClient httpClient = new HttpClient();

        private Uri _baseURI;
        private AuthenticationHeaderValue? _authenticationKey;

        /// <summary>
        /// The <see cref="Uri"/> used to connect to the server
        /// </summary>
        public Uri BaseUri { get { return _baseURI; } }

        /// <summary>
        /// The constructor for creation a <see cref="HTTPManager"/>
        /// </summary>
        /// <param name="uri"> The <see cref="Uri"/> used to connect to the server</param>
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
        /// <summary>
        /// Creates a authenticated copy of the currend <see cref="HTTPManager"/>
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="ip_or_hostname">Adress used to connect to the server</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="password">The password for authenticating on the server </param>
        /// <exception cref="UnauthorizedException">The password is incorrect</exception>
        /// <exception cref="NotFoundException">The server specified does not reply</exception>
        /// <exception cref="ClientException">The server returned a non-standard status code</exception>
        /// <returns>A already authenticated copy of the <see cref="HTTPManager"/></returns>
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

        /// <summary>
        /// Generates a <see cref="Uri"/> from ip or hostname and port of the server
        /// </summary>
        /// <param name="ip_or_hostname">Ipadress or hostname of the server</param>
        /// <param name="port">The port the server is running on</param>
        /// <returns>The <see cref="Uri"/> constructed from the given input</returns>
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
        /// <summary>
        /// A method used for authenticating th <see cref="HTTPManager"/>
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="password">The password to authenticate with</param>
        /// <exception cref="UnauthorizedException">The password is incorrect</exception>
        /// <exception cref="NotFoundException">The server specified does not reply</exception>
        /// <exception cref="ClientException">The server returned a non-standard status code</exception>
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
                            throw new ClientException(response.StatusCode,  "Authentication returned non-standard status code.");
                        }
                }
            }

            _authenticationKey = AuthenticationHeaderValue.Parse(
                await response.Content.ReadAsStringAsync()
            );
        }

        /// <summary>
        /// Crates a request for creating a new <see cref="Trader"/> on the server
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="traderID">The id of the new <see cref="Trader"/></param>
        /// <param name="name">The name of the new <see cref="Trader"/></param>
        /// <param name="rate">The provision rate of the new <see cref="Trader"/></param>
        /// <returns></returns>
        /// <exception cref="DuplicateException"></exception>
        /// <exception cref="ClientException"></exception>
        private async Task<JsonObject> CreateNewTraderRequestHandler(
            string traderID,
            string name,
            decimal? rate,
            decimal? startingFee
        )
        {
            JsonObject requestBody = new JsonObject{
              {"id", traderID},
              {"name", name}
            };
            if (rate is not null)
            {
                requestBody["rate"] = ((decimal)rate!).ToString(InvariantCulture);
            }
            if (startingFee is not null)
            {
                requestBody["starting_fee"] = ((decimal)startingFee!).ToString(InvariantCulture);
            }
            try
            {
                return await RawPostRequest<JsonObject>(
                    "/seller",
                    typeof(JsonObject),
                    requestBody
                );
            }
            catch (HttpStatusCarrier e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.Conflict:
                        throw new DuplicateException($"A trader with the id {traderID} already exists.", e);
                    case HttpStatusCode.BadRequest:
                        throw new ClientException(HttpStatusCode.BadRequest, "The request was not formatted correctly. This is on some level always an error in this class.", e);
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
        /// <summary>
        /// Creates a new <see cref="Trader"/> on the server
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="trader">The <see cref="Trader"/> to add to the server</param>
        /// <returns></returns>
        public async Task<Trader> CreateNewTrader(string newTraderID, Trader trader)
        {
            return trader.UpdateFromJson(
                await CreateNewTraderRequestHandler(
                    trader.TraderID,
                    trader.Name,
                    trader.ProvisionRate,
                    trader.StartingFee
                )
            );
        }
        /// <summary>
        /// Creates a new <see cref="Trader"/> on the server
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="traderID">The <see cref="Trader.TraderID"/> of the new trader</param>
        /// <param name="name">The <see cref="Trader.Name"/> of the new trader</param>
        /// <param name="rate">The <see cref="Trader.ProvisionRate"/> of the new trader</param>
        /// <returns></returns>
        public async Task<Trader> CreateNewTrader(
            string traderID,
            string name,
            decimal? rate,
            decimal? startingFee
        )
        {
            return Trader.FromJson(await CreateNewTraderRequestHandler(traderID, name, rate,startingFee));
        }

        /*
		 * This is an asynchronous method.
		 * It returns an array of all traders currently registered with the server
		 * Raises:
		 *  UnauthorizedException: The client is not authenticated.
		*/
        /// <summary>
        /// A request for getting all <see cref="Trader"/>s from the server
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <returns>All <see cref="Trader"/>s on the server</returns>
        public async Task<Trader[]> GetAllTraders()
        {
            JsonArray responseObject;
            try
            {
                responseObject = await RawGetRequest<JsonArray>("/sellers");
            }
            catch (HttpStatusCarrier e)
            {
                switch (e.code)
                {
                    default:
                        throw new ClientException(e.code, $"Server responded with unexpected status code {e.code}", e);
                }
            }

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
        /// <summary>
        /// A request for getting a <see cref="Trader"/> ny its <see cref="Trader.TraderID"/>
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="traderID">The <see cref="Trader.TraderID"/> of the <see cref="Trader"/></param>
        /// <returns>Tht <see cref="Trader"/> with the given <see cref="Trader.TraderID"/></returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ClientException"></exception>
        public async Task<Trader> GetTrader(string traderID)
        {
            // NOTE: traderID are assumed to be alphanumeric. This is enforced by the UI
            JsonObject responseObject;
            try
            {
                responseObject = await RawGetRequest<JsonObject>($"/seller/{traderID}");
            }
            catch (HttpStatusCarrier e)
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

        /// <summary>
        ///Creates a request for deleting a <see cref="Trader"/>
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="traderID">The <see cref="Trader.TraderID"/> of the <see cref="Trader"/> to delete</param>
        /// <returns></returns>
        /// <exception cref="DeletionOrderException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ClientException"></exception>
        private async Task<JsonObject> DeleteTraderRequestHandler(string traderID)
        {
            try
            {
                // NOTE: traderID are assumed to be alphanumeric. This is enforced by the UI
                return await RawDeleteRequest<JsonObject>($"/seller/{traderID}");
            }
            catch (HttpStatusCarrier e)
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
        /// <summary>
        /// Deletes a <see cref="Trader"/> on the server
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="traderID">The <see cref="Trader.TraderID"/> of the <see cref="Trader"/> to delete</param>
        /// <exception cref="DeletionOrderException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ClientException"></exception>
        /// <returns>The trader that was just deleted with all the previously existing information</returns>
        public async Task<Trader> DeleteTrader(string traderID)
        {
            return Trader.FromJson(await DeleteTraderRequestHandler(traderID));
        }
        /// <inheritdoc cref="DeleteTrader(string)"/>
        /// <param name="trader">The <see cref="Trader"/> to delete</param>
        /// <returns>The trader that was just deleted with all the previously existing information</returns>
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
        /// <summary>
        /// Updates a <see cref="Trader"/> on the server
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="trader">The <see cref="Trader"/> to update</param>
        /// <returns>The Trader instance passed in with all applied updates</returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ClientException"></exception>
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
            catch (HttpStatusCarrier e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException($"No trader with the id {trader.TraderID} exists.", e);
                    case HttpStatusCode.BadRequest:
                        throw new ClientException(HttpStatusCode.BadRequest, "The request was formatted incorrectly. This is always an error within this class structure");
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
        /// <summary>
        /// Handels selling <see cref="TransactionItem"/>s
        /// </summary>
        /// <remarks>This is an asynchronous method</remarks>
        /// <param name="items">The <see cref="TransactionItem"/>s to sell</param>
        /// <returns>
        /// It returns an array of all the Traders that have been changed.
        /// These are new objects, not old ones being updated
        /// </returns>
        /// <exception cref="ClientException"></exception>
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
            catch (HttpStatusCarrier e)
            {
                switch (e.code)
                {
                    case HttpStatusCode.BadRequest:
                        throw new ClientException(HttpStatusCode.BadRequest, $"The server received an invaild URI format. This is always an internal error", e);
                    default:
                        throw new ClientException(e.code, $"The server responded with an unexpected status code {e.code}\n{await e.content.ReadAsStringAsync()}", e);
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
                throw new HttpStatusCarrier(response.StatusCode, response.Content);
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