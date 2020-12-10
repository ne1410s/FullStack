using System.Linq;
using FullStack.Validity.Tests.Models;
using Xunit;

namespace FullStack.Validity.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var model = new TestModel1();

            // Act
            var result = model.Validate(out var errors);

            // Assert
            Assert.False(result);
            Assert.True(errors.Any());
            Assert.NotNull(errors.FirstOrDefault(e => e.PropertyName == nameof(TestModel1.MyRequiredString)));
        }

        [Fact]
        public void Test2()
        {
            // Arrange
            var model = new TestModel2();

            // Act
            var result = model.Validate(out var errors);

            // Assert
            Assert.False(result);
            Assert.Equal(2, errors.Count);
        }

        [Fact]
        public void Test3()
        {
            // Arrange
            var model = new TestModel3();

            // Act
            var result = model.Validate(out var errors);

            // Assert
            Assert.False(result);
            Assert.Equal(8, errors.Count);
        }
    }
}
