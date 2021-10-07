namespace Bonfida.Client.Models
{
    /// <summary>
    /// Represents a result to a request to the Bonfida API.
    /// </summary>
    /// <typeparam name="T">The type of the resulting data.</typeparam>
    public class RequestResult<T>
    {
        /// <summary>
        /// Represents if the request was successful.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The underlying data associated with the request.
        /// </summary>
        public T Data { get; set; }
    }
}