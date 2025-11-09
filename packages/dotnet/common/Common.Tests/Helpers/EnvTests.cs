using Waggle.Common.Helpers;

namespace Waggle.Common.Tests.Helpers
{
    public class EnvTests : IDisposable
    {
        private const string TestKeyRequired = "TEST_REQUIRED_KEY";
        private const string TestKeyOptional = "TEST_OPTIONAL_KEY";

        public EnvTests()
        {
            Environment.SetEnvironmentVariable(TestKeyRequired, null);
            Environment.SetEnvironmentVariable(TestKeyOptional, null);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(TestKeyRequired, null);
            Environment.SetEnvironmentVariable(TestKeyOptional, null);
        }

        [Fact]
        public void GetRequired_ReturnsValue_WhenSet()
        {
            // Arrange
            var expected = "value123";
            Environment.SetEnvironmentVariable(TestKeyRequired, expected);

            // Act
            var result = Env.GetRequired(TestKeyRequired);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetRequired_Throws_WhenNotSet()
        {
            // Act
            Action act = () => Env.GetRequired(TestKeyRequired);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage($"{TestKeyRequired} environment variable is not set");
        }

        [Fact]
        public void GetOptional_ReturnsValue_WhenSet()
        {
            // Arrange
            var expected = "optionalValue";
            Environment.SetEnvironmentVariable(TestKeyOptional, expected);

            // Act
            var result = Env.GetOptional(TestKeyOptional);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetOptional_ReturnsNull_WhenNotSet()
        {
            // Act
            var result = Env.GetOptional(TestKeyOptional);

            // Assert
            result.Should().BeNull();
        }
    }
}
