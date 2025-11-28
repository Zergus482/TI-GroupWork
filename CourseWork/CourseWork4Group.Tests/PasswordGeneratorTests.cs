using CourseWork4Group.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CourseWork4Group.Tests
{
    /// <summary>
    /// Юнит-тесты для генератора паролей
    /// Переделаны из оригинальных тестов в Logic/PasswordGeneratorTests.cs
    /// </summary>
    public class PasswordGeneratorTests
    {
        private readonly PasswordGenerator _generator;

        public PasswordGeneratorTests()
        {
            _generator = new PasswordGenerator();
        }

        [Fact]
        public void TestMinimalLength()
        {
            // Arrange
            int length = 8;

            // Act
            string password = _generator.GeneratePassword(length, true, true, true, true);

            // Assert
            Assert.Equal(length, password.Length);
            // При минимальной длине не гарантировано наличие всех типов символов
            // Проверяем только длину и что пароль создан
            Assert.NotEmpty(password);
        }

        [Fact]
        public void TestMaximalLength()
        {
            // Arrange
            int length = 64;

            // Act
            string password = _generator.GeneratePassword(length, true, true, true, true);

            // Assert
            Assert.Equal(length, password.Length);
        }

        [Fact]
        public void TestZeroLength()
        {
            // Arrange
            int length = 0;

            // Act
            string password = _generator.GeneratePassword(length, true, true, true, true);

            // Assert
            Assert.Empty(password);
        }

        [Fact]
        public void TestNegativeLength()
        {
            // Arrange
            int length = -5;

            // Act
            string password = _generator.GeneratePassword(length, true, true, true, true);

            // Assert
            Assert.Empty(password);
        }

        [Fact]
        public void TestLengthOne()
        {
            // Arrange
            int length = 1;

            // Act
            string password = _generator.GeneratePassword(length, true, true, true, true);

            // Assert
            Assert.Equal(length, password.Length);
        }

        [Fact]
        public void TestOnlyLowercase()
        {
            // Arrange
            int length = 16;

            // Act
            string password = _generator.GeneratePassword(length, true, false, false, false);

            // Assert
            Assert.Equal(length, password.Length);
            Assert.All(password, c => Assert.True(char.IsLower(c)));
            Assert.DoesNotContain(password, c => char.IsUpper(c));
            Assert.DoesNotContain(password, c => char.IsDigit(c));
            Assert.DoesNotContain(password, c => !char.IsLetterOrDigit(c));
        }

        [Fact]
        public void TestOnlyUppercase()
        {
            // Arrange
            int length = 16;

            // Act
            string password = _generator.GeneratePassword(length, false, true, false, false);

            // Assert
            Assert.Equal(length, password.Length);
            Assert.All(password, c => Assert.True(char.IsUpper(c)));
            Assert.DoesNotContain(password, c => char.IsLower(c));
            Assert.DoesNotContain(password, c => char.IsDigit(c));
            Assert.DoesNotContain(password, c => !char.IsLetterOrDigit(c));
        }

        [Fact]
        public void TestOnlyNumbers()
        {
            // Arrange
            int length = 16;

            // Act
            string password = _generator.GeneratePassword(length, false, false, true, false);

            // Assert
            Assert.Equal(length, password.Length);
            Assert.All(password, c => Assert.True(char.IsDigit(c)));
            Assert.DoesNotContain(password, c => char.IsLetter(c));
            Assert.DoesNotContain(password, c => !char.IsLetterOrDigit(c));
        }

        [Fact]
        public void TestOnlySpecialChars()
        {
            // Arrange
            int length = 16;

            // Act
            string password = _generator.GeneratePassword(length, false, false, false, true);

            // Assert
            Assert.Equal(length, password.Length);
            Assert.All(password, c => Assert.False(char.IsLetterOrDigit(c)));
            Assert.DoesNotContain(password, c => char.IsLetter(c));
            Assert.DoesNotContain(password, c => char.IsDigit(c));
        }

        [Fact]
        public void TestLowercaseAndUppercase()
        {
            // Arrange
            int length = 20;

            // Act
            string password = _generator.GeneratePassword(length, true, true, false, false);

            // Assert
            Assert.Equal(length, password.Length);
            Assert.Contains(password, c => char.IsLower(c));
            Assert.Contains(password, c => char.IsUpper(c));
            Assert.DoesNotContain(password, c => char.IsDigit(c));
            Assert.DoesNotContain(password, c => !char.IsLetterOrDigit(c));
        }

        [Fact]
        public void TestLowercaseAndNumbers()
        {
            // Arrange
            int length = 20;

            // Act
            string password = _generator.GeneratePassword(length, true, false, true, false);

            // Assert
            Assert.Equal(length, password.Length);
            Assert.Contains(password, c => char.IsLower(c));
            Assert.Contains(password, c => char.IsDigit(c));
            Assert.DoesNotContain(password, c => char.IsUpper(c));
            Assert.DoesNotContain(password, c => !char.IsLetterOrDigit(c));
        }

        [Fact]
        public void TestAllTypes()
        {
            // Arrange
            int length = 24;

            // Act
            string password = _generator.GeneratePassword(length, true, true, true, true);

            // Assert
            Assert.Equal(length, password.Length);
            Assert.Contains(password, c => char.IsLower(c));
            Assert.Contains(password, c => char.IsUpper(c));
            Assert.Contains(password, c => char.IsDigit(c));
            Assert.Contains(password, c => !char.IsLetterOrDigit(c));
        }

        [Fact]
        public void TestNoTypesSelected()
        {
            // Arrange
            int length = 16;

            // Act
            string password = _generator.GeneratePassword(length, false, false, false, false);

            // Assert
            Assert.Empty(password);
        }

        [Fact]
        public void TestPasswordValidation()
        {
            // Arrange
            string password = _generator.GeneratePassword(16, true, true, true, true);

            // Act
            bool isValid = _generator.ValidatePassword(password, true, true, true, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void TestPasswordValidationFailure()
        {
            // Arrange - генерируем пароль только из строчных букв, но требуем все типы
            string password = _generator.GeneratePassword(16, true, false, false, false);

            // Act
            bool isValid = _generator.ValidatePassword(password, true, true, true, true);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void TestPasswordUniqueness()
        {
            // Arrange
            var passwords = new HashSet<string>();
            int iterations = 10;

            // Act
            for (int i = 0; i < iterations; i++)
            {
                string password = _generator.GeneratePassword(16, true, true, true, true);
                passwords.Add(password);
            }

            // Assert
            Assert.Equal(iterations, passwords.Count);
        }

        [Fact]
        public void TestPasswordRandomness()
        {
            // Arrange
            int length = 100;

            // Act
            string password = _generator.GeneratePassword(length, true, true, true, true);

            // Assert
            int lowercaseCount = password.Count(c => char.IsLower(c));
            int uppercaseCount = password.Count(c => char.IsUpper(c));
            int digitCount = password.Count(c => char.IsDigit(c));
            int specialCount = password.Count(c => !char.IsLetterOrDigit(c));

            // Проверяем, что все типы символов присутствуют
            Assert.True(lowercaseCount > 0);
            Assert.True(uppercaseCount > 0);
            Assert.True(digitCount > 0);
            Assert.True(specialCount > 0);
        }

        [Fact]
        public void TestVariousLengths()
        {
            // Arrange
            int[] lengths = { 8, 12, 16, 32, 64 };

            // Act & Assert
            foreach (int length in lengths)
            {
                string password = _generator.GeneratePassword(length, true, true, true, true);
                Assert.Equal(length, password.Length);
            }
        }
    }
}
