using Microsoft.AspNetCore.Http;
using Npgsql;
using Microsoft.Extensions.Logging;
using nat_api.core.serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace nat_api.api.middleware
{
    public class ErrorHandlerMiddleware
    {
        public enum ErrorCodes
        {
            ValidationError = 1000,
            AlreadyLoggedOut = 1002
        }

        public class Result
        {
            public bool IsSuccess { get; set; }
            public DateTime TimeStampUtc { get; set; }
            public string FailureReason { get; set; }
            public int FailureCode { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public string Payload { get; set; }

            public Result()
            {
                IsSuccess = false;
                TimeStampUtc = DateTime.UtcNow;
                StatusCode = HttpStatusCode.InternalServerError;
                FailureReason = "Something went wrong. Please try again.\n If problem persists, please contact support for assistance. ";
            }
        }

        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ErrorHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }

            catch (NpgsqlException sqlException)
            {
                if (sqlException.Message.Contains("timeout"))
                {
                    logger.LogError(sqlException, sqlException.Message);
                    await BuildResponse(context.Response, new Result()
                    {
                        FailureReason = sqlException.Message,
                        StatusCode = HttpStatusCode.GatewayTimeout,
                    });
                }

                else
                {
                    logger.LogError(sqlException, $"An unhandled exception was found while trying to process '{context.Request.Path}' endpoint");
                    await BuildResponse(context.Response, new Result());
                }
            }

            catch (FluentValidation.ValidationException validationException)
            {
                logger.LogError(validationException, $"A valitation exception was found while trying to process '{context.Request.Path}' endpoint");
                await BuildResponse(context.Response, FluentValidationExceptionHandler(validationException));
            }

            catch (InvalidOperationException ex)
            {
                if (ex.StackTrace.Trim().Split(" ")[1].Contains("SingleAsync"))
                {
                    var failureReason = "Requested object not found or more than one exists ";
                    logger.LogError(ex, $"{failureReason} on '{context.Request.Path}' endpoint");
                    await BuildResponse(context.Response, new Result()
                    {
                        FailureReason = failureReason,
                        StatusCode = HttpStatusCode.NotFound,
                    });
                }

                else
                {
                    logger.LogError(ex, $"An invalid operation was found while trying to process '{context.Request.Path}' endpoint");
                    await BuildResponse(context.Response, new Result());
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"An unhandled exception was found while trying to process '{context.Request.Path}' endpoint");
                await BuildResponse(context.Response, new Result());
            }
        }

        private void AddContent(Dictionary<string, object> dictionary, string propertyName, string errorMessage)
        {
            if (!dictionary.ContainsKey(propertyName))
            {
                dictionary[propertyName] = new List<string>();
            }

            var list = dictionary[propertyName] as List<string>;

            if (list != null)
            {
                list.Add(errorMessage);
            }
        }

        private void AddInnerContent(Dictionary<string, object> tree, string propertyName, string propertyTree, string errorMessage)
        {
            if (!tree.ContainsKey(propertyName))
            {
                tree[propertyName] = new Dictionary<string, object>();
            }

            var dictionary = tree[propertyName] as Dictionary<string, object>;

            AddContent(dictionary, propertyTree, errorMessage);
        }

        public Result FluentValidationExceptionHandler(FluentValidation.ValidationException validationException)
        {
            var failureReasons = validationException
                .Errors
                .Select( error => error.ErrorMessage )
                .Distinct();

            var specificErrorList = new Dictionary<string, object>();
            
            var lowerCamelCase = new LowerCamelCase();

            validationException.Errors
                .Select( error => new { error.PropertyName, error.ErrorMessage })
                .Distinct()
                .ToList()
                .ForEach( specificError => {

                    var tree = specificError
                        .PropertyName
                        .Split('.')
                        .ToArray();

                    var propertyName = lowerCamelCase.ConvertName(specificError.PropertyName);

                    if (tree.Length <= 2)
                    {
                        AddContent(specificErrorList, propertyName, specificError.ErrorMessage);
                    }

                    else
                    {
                        AddInnerContent(specificErrorList, lowerCamelCase.ConvertName($"{tree[0]}.{tree[1]}"), lowerCamelCase.ConvertName(tree[2]), specificError.ErrorMessage);
                    }
                });

            var payload = JsonSerializer.Serialize(specificErrorList);

            return new Result()
            {
                StatusCode = HttpStatusCode.UnprocessableEntity,
                FailureReason = string.Join("\n", failureReasons),
                FailureCode = (int)ErrorCodes.ValidationError,
                Payload = payload,
            };
        }

        private async Task BuildResponse(HttpResponse response, Result result)
        {
            response.ContentType = "application/json";
            response.StatusCode = (int) result.StatusCode;
            var contentAsJson = JsonSerializer.Serialize(result);

            await response.WriteAsync(contentAsJson);
        }
    }
}