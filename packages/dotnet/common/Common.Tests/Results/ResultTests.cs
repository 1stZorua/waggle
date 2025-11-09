using Waggle.Common.Results.Core;

namespace Waggle.Common.Tests.Results
{
    public class ResultTests
    {
        #region Map Tests
        [Fact]
        public void Map_WhenSuccessful_TransformsData()
        {
            // Arrange
            var result = Result<int>.Ok(123);

            // Act
            var mapped = result.Map(id => id + 1);

            // Assert
            mapped.Success.Should().BeTrue();
            mapped.Data.Should().Be(124);
        }

        [Fact]
        public async Task MapAsync_WhenSuccessful_TransformsDataAsynchronously()
        {
            // Arrange
            var result = Result<int>.Ok(10);

            // Act
            var mapped = await result.MapAsync(async value =>
            {
                await Task.Delay(1);
                return value * 2;
            });

            // Assert
            mapped.Success.Should().BeTrue();
            mapped.Data.Should().Be(20);
        }

        [Theory]
        [MemberData(nameof(TestData.ErrorCodeToMessage), MemberType = typeof(TestData))]
        public void Map_WhenFailed_PropagatesFailure(string errorCode, string errorMessage)
        {
            // Arrange
            var result = Result<int>.Fail(errorMessage, errorCode);

            // Act
            var mapped = result.Map(id => id + 1);

            // Assert
            mapped.Success.Should().BeFalse();
            mapped.Message.Should().Be(errorMessage);
            mapped.ErrorCode.Should().Be(errorCode);
        }

        [Theory]
        [MemberData(nameof(TestData.ErrorCodeToMessage), MemberType = typeof(TestData))]
        public async Task MapAsync_WhenFailed_PropagatesFailure(string errorCode, string errorMessage)
        {
            // Arrange
            var result = Result<int>.Fail(errorMessage, errorCode);

            // Act
            var mapped = await result.MapAsync(async value =>
            {
                await Task.Delay(1);
                return value * 2;
            });

            // Assert
            mapped.Success.Should().BeFalse();
            mapped.Message.Should().Be(errorMessage);
            mapped.ErrorCode.Should().Be(errorCode);
        }

        #endregion

        #region Match Tests

        [Fact]
        public void Match_ResultT_WhenSuccessful_InvokesSuccessFunc()
        {
            // Arrange
            var result = Result<int>.Ok(1);

            // Act
            var value = result.Match(
                onSuccess: val => val * 2,
                onFailure: _ => 0
            );

            // Assert
            value.Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestData.ErrorCodeToMessage), MemberType = typeof(TestData))]
        public void Match_ResultT_WhenFailed_InvokesFailureFunc(string errorCode, string errorMessage)
        {
            // Arrange
            var result = Result<int>.Fail(errorMessage, errorCode);

            // Act
            var value = result.Match(
                onSuccess: val => val * 2,
                onFailure: msg => msg.Length
            );

            // Assert
            value.Should().Be(errorMessage.Length);
        }

        [Fact]
        public void Match_Result_WhenSuccessful_InvokesSuccessFunc()
        {
            // Arrange
            var result = Result.Ok();

            // Act
            var value = result.Match(
                onSuccess: () => 1,
                onFailure: _ => 0
            );

            // Assert
            value.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(TestData.ErrorCodeToMessage), MemberType = typeof(TestData))]
        public void Match_Result_WhenFailed_InvokesFailureFunc(string errorCode, string errorMessage)
        {
            // Arrange
            var result = Result.Fail(errorMessage, errorCode);

            // Act
            var value = result.Match(
                onSuccess: () => 1,
                onFailure: msg => msg.Length
            );

            // Assert
            value.Should().Be(errorMessage.Length);
        }

        #endregion
    }
}
