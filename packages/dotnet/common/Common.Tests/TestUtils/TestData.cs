using Microsoft.AspNetCore.Http;
using System.Net;
using Waggle.Common.Constants;
using Waggle.Common.Pagination.Models;

namespace Waggle.Common.Tests.TestUtils
{
    public static class TestData
    {
        public static TheoryData<string, string> ErrorCodeToMessage => new()
        {
            { ErrorCodes.NotFound, ErrorMessages.Resource.NotFound },
            { ErrorCodes.Unauthorized, ErrorMessages.Authentication.Unauthorized },
            { ErrorCodes.ValidationFailed, ErrorMessages.Validation.InvalidInput },
            { ErrorCodes.ServiceFailed, ErrorMessages.Service.Failed }
        };

        public static TheoryData<HttpStatusCode, string> StatusCodeToErrorCode => new()
        {
            { HttpStatusCode.BadRequest, ErrorCodes.InvalidInput },
            { HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized },
            { HttpStatusCode.Forbidden, ErrorCodes.Forbidden },
            { HttpStatusCode.NotFound, ErrorCodes.NotFound },
            { HttpStatusCode.Conflict, ErrorCodes.AlreadyExists },
            { HttpStatusCode.InternalServerError, ErrorCodes.ServiceFailed },
            { HttpStatusCode.ServiceUnavailable, ErrorCodes.ServiceUnavailable }
        };

        public static TheoryData<string, HttpStatusCode> ErrorCodeToStatusCode => new()
        {
            { ErrorCodes.InvalidInput, HttpStatusCode.BadRequest },
            { ErrorCodes.ValidationFailed, HttpStatusCode.UnprocessableEntity },
            { ErrorCodes.Unauthorized, HttpStatusCode.Unauthorized },
            { ErrorCodes.Forbidden, HttpStatusCode.Forbidden },
            { ErrorCodes.NotFound, HttpStatusCode.NotFound },
            { ErrorCodes.AlreadyExists, HttpStatusCode.Conflict },
            { ErrorCodes.ServiceFailed, HttpStatusCode.InternalServerError },
            { ErrorCodes.ServiceUnavailable, HttpStatusCode.ServiceUnavailable }
        };

        public static TheoryData<string, string, int> ErrorCodeMessageAndStatus => new()
        {
            { ErrorCodes.NotFound, ErrorMessages.Resource.NotFound, StatusCodes.Status404NotFound },
            { ErrorCodes.Unauthorized, ErrorMessages.Authentication.Unauthorized, StatusCodes.Status401Unauthorized },
            { ErrorCodes.ValidationFailed, ErrorMessages.Validation.InvalidInput, StatusCodes.Status400BadRequest },
            { ErrorCodes.ServiceFailed, ErrorMessages.Service.Failed, StatusCodes.Status502BadGateway }
        };
    }
}
