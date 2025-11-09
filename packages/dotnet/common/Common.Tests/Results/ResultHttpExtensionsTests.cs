using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Results.Core;
using Waggle.Common.Results.Extensions;

namespace Waggle.Common.Tests.Results
{
    public class ResultHttpExtensionsTests
    {
        [Fact]
        public void ToActionResult_WhenSuccessful_Returns200()
        {
            // Arrange
            var result = Result.Ok();

            // Act
            var actionResult = result.ToActionResult();

            // Assert
            var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Theory]
        [MemberData(nameof(TestData.ErrorCodeMessageAndStatus), MemberType = typeof(TestData))]
        public void ToActionResult_WhenFailed_ReturnsCorrectStatusCode(string errorCode, string errorMessage, int expectedStatus)
        {
            // Arrange
            var result = Result<string>.Fail(errorMessage, errorCode);

            // Act
            var actionResult = result.ToActionResult();

            // Assert
            var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatus);
        }

        [Fact]
        public void ToCreatedResult_WhenSuccessful_ReturnsCreatedWithLocation()
        {
            // Arrange
            var result = Result<int>.Ok(42);
            var location = "/api/pets/42";

            // Act
            var actionResult = result.ToCreatedResult(location);

            // Assert
            var createdResult = actionResult.Should().BeOfType<CreatedResult>().Subject;
            createdResult.Location.Should().Be(location);
        }

        [Theory]
        [MemberData(nameof(TestData.ErrorCodeMessageAndStatus), MemberType = typeof(TestData))]
        public void ToCreatedResult_WhenFailed_ReturnsCorrectStatusCode(string errorCode, string errorMessage, int expectedStatus)
        {
            // Arrange
            var result = Result<int>.Fail(errorMessage, errorCode);
            var location = "/api/pets/42";

            // Act
            var actionResult = result.ToCreatedResult(location);

            // Assert
            var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatus);
        }

        [Fact]
        public void ToNoContentResult_WhenSuccessful_ReturnsNoContent()
        {
            // Arrange
            var result = Result.Ok();

            // Act
            var actionResult = result.ToNoContentResult();

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
        }
    }
}
