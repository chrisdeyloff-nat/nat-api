using System;
using System.Net;

namespace nat_api.core.results
{
    public interface IResult<T>
    {
        /// <summary>
        /// The payload object
        /// </summary>
        T Payload { get; }
        /// <summary>
        /// Success state uf the result
        /// </summary>
        bool IsSuccess { get; }
        /// <summary>
        /// Timestamp of result
        /// </summary>
        DateTime TimeStampUtc { get; }
        /// <summary>
        /// Reason for failure
        /// </summary>
        string FailureReason { get; }
        /// <summary>
        /// An Http status code intended to be returned as an API response.
        /// </summary>
        HttpStatusCode StatusCode { get; }
    }
}
