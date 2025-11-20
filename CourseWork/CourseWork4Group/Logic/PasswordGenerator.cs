using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CourseWork4Group.Logic
{
    /// <summary>
    /// Класс для генерации паролей
    /// </summary>
    public class PasswordGenerator
    {
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumberChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        /// <summary>
        /// Генерирует пароль с заданными параметрами
        /// </summary>
        /// <param name="length">Длина пароля</param>
        /// <param name="includeLowercase">Включать строчные буквы</param>
        /// <param name="includeUppercase">Включать заглавные буквы</param>
        /// <param name="includeNumbers">Включать цифры</param>
        /// <param name="includeSpecialChars">Включать специальные символы</param>
        /// <returns>Сгенерированный пароль</returns>
        public string GeneratePassword(int length, bool includeLowercase = true, bool includeUppercase = true, 
            bool includeNumbers = true, bool includeSpecialChars = true)
        {
            if (length <= 0)
                return string.Empty;

            StringBuilder charSet = new StringBuilder();

            if (includeLowercase)
                charSet.Append(LowercaseChars);
            if (includeUppercase)
                charSet.Append(UppercaseChars);
            if (includeNumbers)
                charSet.Append(NumberChars);
            if (includeSpecialChars)
                charSet.Append(SpecialChars);

            if (charSet.Length == 0)
                return string.Empty;

            StringBuilder password = new StringBuilder(length);
            char[] chars = charSet.ToString().ToCharArray();

            // Используем криптографически стойкий генератор случайных чисел
            byte[] data = new byte[length];
            RandomNumberGenerator.Fill(data);

            for (int i = 0; i < length; i++)
            {
                password.Append(chars[data[i] % chars.Length]);
            }

            return password.ToString();
        }

        /// <summary>
        /// Проверяет, соответствует ли пароль заданным требованиям
        /// </summary>
        public bool ValidatePassword(string password, bool requireLowercase, bool requireUppercase, 
            bool requireNumbers, bool requireSpecialChars)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (requireLowercase && !password.Any(c => char.IsLower(c)))
                return false;

            if (requireUppercase && !password.Any(c => char.IsUpper(c)))
                return false;

            if (requireNumbers && !password.Any(c => char.IsDigit(c)))
                return false;

            if (requireSpecialChars && !password.Any(c => !char.IsLetterOrDigit(c)))
                return false;

            return true;
        }
    }
}

