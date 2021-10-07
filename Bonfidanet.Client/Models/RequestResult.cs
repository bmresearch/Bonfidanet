using System.Net;
using System.Net.Http;

namespace Bonfida.Client.Models
{
    /// <summary>
    /// Represents a result to a request to the Bonfida API.
    /// </summary>
    /// <typeparam name="T">The type of the resulting data.</typeparam>
    public class RequestResponse<T>
    {
        /// <summary>
        /// Returns <c>true</c> if the API request was successful.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The underlying data associated with the request.
        /// </summary>
        public T Data { get; set; }
    }
    
    /// <summary>
    /// Represents a result to a request to the Bonfida API.
    /// </summary>
    /// <typeparam name="T">The type of the resulting data.</typeparam>
    public class RequestResult<T>
    {
        /// <summary>
        /// Returns <c>true</c> if the request was successfully handled and parsed.
        /// </summary>
        public bool WasSuccessful => WasHttpRequestSuccessful && WasRequestSuccessfullyHandled;

        /// <summary>
        /// <summary>
        /// Returns <c>true</c> if the HTTP request was successful (e.g. Code 200).
        /// </summary>
        public bool WasHttpRequestSuccessful { get; set; }
        
        /// <summary>
        /// Returns <c>true</c> if the request was successfully handled by the server and no error parameters are found in the result.
        /// </summary>
        public bool WasRequestSuccessfullyHandled { get; set; }
        
        
        /// <summary>
        /// Returns the <see cref="HttpStatusCode"/> of the request.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// The error reason.
        /// </summary>
        public string Reason { get; set; }
        
        /// <summary>
        /// The underlying data associated with the request.
        /// </summary>
        public T Data { get; set; }
        
        /// <summary>
        /// Initialize the request result.
        /// <param name="resultMsg">An http request result.</param>
        /// <param name="result">The type of the request result.</param>
        /// </summary>
        public RequestResult(HttpResponseMessage resultMsg, T result = default(T))
        {
            HttpStatusCode = resultMsg.StatusCode;
            WasHttpRequestSuccessful = resultMsg.IsSuccessStatusCode;
            Reason = resultMsg.ReasonPhrase;
            Data = result;
            if (Data != null)
                WasRequestSuccessfullyHandled = true;
        }
    }
}