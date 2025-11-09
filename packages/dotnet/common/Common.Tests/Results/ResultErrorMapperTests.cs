using System.Net;
using Waggle.Common.Results.Helpers;

namespace Waggle.Common.Tests.Results
{
    public class ResultErrorMapperTests
    {
        [Theory]
        [MemberData(nameof(TestData.StatusCodeToErrorCode), MemberType = typeof(TestData))]
        public void FromStatusCode_MapsHttpStatusToErrorCode(HttpStatusCode statusCode, string expectedErrorCode)
        {
            // Arrange & Act
            var errorCode = ResultErrorMapper.FromStatusCode(statusCode);

            // Assert
            errorCode.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(TestData.ErrorCodeToStatusCode), MemberType = typeof(TestData))]
        public void ToStatusCode_MapsErrorCodeToHttpStatus(string errorCode, HttpStatusCode expectedStatusCode)
        {
            // Arrange & Act
            var statusCode = ResultErrorMapper.ToStatusCode(errorCode);

            // Assert
            statusCode.Should().Be(expectedStatusCode);
        }

        [Fact]
        public void ToStatusCode_WhenUnknownErrorCode_ReturnsInternalServerError()
        {
            // Arrange
            var unknownCode = "UNKNOWN_ERROR";

            // Act
            var statusCode = ResultErrorMapper.ToStatusCode(unknownCode);

            // Assert
            statusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
