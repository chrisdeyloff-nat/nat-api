using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace nat_api.core.results
{
    public class Result<T> : IResult<T>
    {
        public T Payload { get; protected set; }
        public bool IsSuccess { get; protected set; }
        public DateTime TimeStampUtc { get; protected set; }
        public string FailureReason { get; protected set; }
        public int FailureCode { get; protected set; }
        public HttpStatusCode StatusCode { get; protected set; }

        /// <summary>
        /// Generate new failing Result
        /// </summary>
        /// <param name="reason">Failure reason.</param>
        /// <returns>A Result with IsSuccess set to false and a specified failure reason.</returns>
        public static Result<T> Fail(int code, string reason, HttpStatusCode statusCode)
        {
            if ((int)statusCode < 400)
            {
                throw new InvalidOperationException($"The status {statusCode} is not a valid failure code.");
            }
            return new Result<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                FailureCode = code,
                FailureReason = reason,
                TimeStampUtc = DateTime.UtcNow
            };
        }

        public static Result<T> Fail(string reason, HttpStatusCode statusCode)
        {
            return Result<T>.Fail(-1, reason, statusCode);
        }

        public static Result<T> Fail(int code, string reason)
        {
            return Result<T>.Fail(code, reason, HttpStatusCode.InternalServerError);
        }

        public static Result<T> Fail(string reason)
        {
            return Result<T>.Fail(reason, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Generate new success Result
        /// </summary>
        /// <param name="payload">Payload response.</param>
        /// <returns>A Result with IsSuccess set to true and a payload.</returns>
        public static Result<T> Success(T payload, HttpStatusCode statusCode)
        {
            if ((int)statusCode >= 400)
            {
                throw new InvalidOperationException($"The status {statusCode} is not a valid success code.");
            }
            return new Result<T>
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Payload = payload,
                TimeStampUtc = DateTime.UtcNow
            };
        }

        public static Result<T> Success(T payload)
        {
            return Result<T>.Success(payload, HttpStatusCode.OK);
        }

        /// <summary>
        /// Implicit conversion treating a Result object's IsSucesss property as indicator.
        /// </summary>
        /// <param name="result">Result object to operate on.</param>
        public static implicit operator bool(Result<T> result) => result.IsSuccess;

        public IActionResult ResultOrError(Controller controller)
        {
            return this.IsSuccess 
                        ? controller.Ok(this.Payload)
                        : controller.StatusCode((int)StatusCode, this);
        }
    }
}