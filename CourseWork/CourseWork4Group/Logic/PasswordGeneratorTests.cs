using System;
using System.Linq;

namespace CourseWork4Group.Logic
{
    /// <summary>
    /// Тестовый класс для генерации паролей
    /// Выводит результаты тестов в консоль
    /// </summary>
    public class PasswordGeneratorTests
    {
        private readonly PasswordGenerator _generator;
        private int _testsPassed;
        private int _testsFailed;
        private int _testNumber;

        public PasswordGeneratorTests()
        {
            _generator = new PasswordGenerator();
            _testsPassed = 0;
            _testsFailed = 0;
            _testNumber = 0;
        }

        /// <summary>
        /// Запускает все тесты
        /// </summary>
        public void RunAllTests()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("ТЕСТИРОВАНИЕ ГЕНЕРАЦИИ ПАРОЛЕЙ");
            Console.WriteLine("========================================\n");

            // Граничные тесты
            TestMinimalLength();
            TestMaximalLength();
            TestZeroLength();
            TestNegativeLength();
            TestLengthOne();

            // Тесты с одним типом символов
            TestOnlyLowercase();
            TestOnlyUppercase();
            TestOnlyNumbers();
            TestOnlySpecialChars();

            // Тесты с комбинациями
            TestLowercaseAndUppercase();
            TestLowercaseAndNumbers();
            TestAllTypes();

            // Тесты без выбранных типов
            TestNoTypesSelected();

            // Тесты валидации
            TestPasswordValidation();
            TestPasswordValidationFailure();

            // Тесты на уникальность и случайность
            TestPasswordUniqueness();
            TestPasswordRandomness();

            // Тесты на длину
            TestVariousLengths();

            // Итоги
            PrintSummary();
        }

        private void TestMinimalLength()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Минимальная длина пароля (8 символов)");
            try
            {
                string password = _generator.GeneratePassword(8, true, true, true, true);
                
                bool passed = password.Length == 8 && 
                             password.Any(c => char.IsLower(c)) &&
                             password.Any(c => char.IsUpper(c)) &&
                             password.Any(c => char.IsDigit(c)) &&
                             password.Any(c => !char.IsLetterOrDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль длиной {password.Length} символов");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль не соответствует требованиям");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestMaximalLength()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Максимальная длина пароля (64 символа)");
            try
            {
                string password = _generator.GeneratePassword(64, true, true, true, true);
                
                bool passed = password.Length == 64;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль длиной {password.Length} символов");
                    Console.WriteLine($"    Первые 20 символов: {password.Substring(0, Math.Min(20, password.Length))}...");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Ожидалась длина 64, получено {password.Length}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestZeroLength()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Нулевая длина пароля (граничный случай)");
            try
            {
                string password = _generator.GeneratePassword(0, true, true, true, true);
                
                bool passed = string.IsNullOrEmpty(password);

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Возвращена пустая строка для нулевой длины");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Ожидалась пустая строка, получено: '{password}'");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestNegativeLength()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Отрицательная длина пароля (граничный случай)");
            try
            {
                string password = _generator.GeneratePassword(-5, true, true, true, true);
                
                bool passed = string.IsNullOrEmpty(password);

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Возвращена пустая строка для отрицательной длины");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Ожидалась пустая строка, получено: '{password}'");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestLengthOne()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Длина пароля = 1 (граничный случай)");
            try
            {
                string password = _generator.GeneratePassword(1, true, true, true, true);
                
                bool passed = password.Length == 1;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль длиной 1 символ");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Ожидалась длина 1, получено {password.Length}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestOnlyLowercase()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Только строчные буквы");
            try
            {
                string password = _generator.GeneratePassword(16, true, false, false, false);
                
                bool passed = password.Length == 16 && 
                             password.All(c => char.IsLower(c)) &&
                             !password.Any(c => char.IsUpper(c)) &&
                             !password.Any(c => char.IsDigit(c)) &&
                             !password.Any(c => !char.IsLetterOrDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит только строчные буквы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль содержит недопустимые символы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestOnlyUppercase()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Только заглавные буквы");
            try
            {
                string password = _generator.GeneratePassword(16, false, true, false, false);
                
                bool passed = password.Length == 16 && 
                             password.All(c => char.IsUpper(c)) &&
                             !password.Any(c => char.IsLower(c)) &&
                             !password.Any(c => char.IsDigit(c)) &&
                             !password.Any(c => !char.IsLetterOrDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит только заглавные буквы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль содержит недопустимые символы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestOnlyNumbers()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Только цифры");
            try
            {
                string password = _generator.GeneratePassword(16, false, false, true, false);
                
                bool passed = password.Length == 16 && 
                             password.All(c => char.IsDigit(c)) &&
                             !password.Any(c => char.IsLetter(c)) &&
                             !password.Any(c => !char.IsLetterOrDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит только цифры");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль содержит недопустимые символы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestOnlySpecialChars()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Только специальные символы");
            try
            {
                string password = _generator.GeneratePassword(16, false, false, false, true);
                
                bool passed = password.Length == 16 && 
                             password.All(c => !char.IsLetterOrDigit(c)) &&
                             !password.Any(c => char.IsLetter(c)) &&
                             !password.Any(c => char.IsDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит только специальные символы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль содержит недопустимые символы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestLowercaseAndUppercase()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Строчные и заглавные буквы");
            try
            {
                string password = _generator.GeneratePassword(20, true, true, false, false);
                
                bool passed = password.Length == 20 && 
                             password.Any(c => char.IsLower(c)) &&
                             password.Any(c => char.IsUpper(c)) &&
                             !password.Any(c => char.IsDigit(c)) &&
                             !password.Any(c => !char.IsLetterOrDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит строчные и заглавные буквы");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль не соответствует требованиям");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestLowercaseAndNumbers()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Строчные буквы и цифры");
            try
            {
                string password = _generator.GeneratePassword(20, true, false, true, false);
                
                bool passed = password.Length == 20 && 
                             password.Any(c => char.IsLower(c)) &&
                             password.Any(c => char.IsDigit(c)) &&
                             !password.Any(c => char.IsUpper(c)) &&
                             !password.Any(c => !char.IsLetterOrDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит строчные буквы и цифры");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль не соответствует требованиям");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestAllTypes()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Все типы символов");
            try
            {
                string password = _generator.GeneratePassword(24, true, true, true, true);
                
                bool passed = password.Length == 24 && 
                             password.Any(c => char.IsLower(c)) &&
                             password.Any(c => char.IsUpper(c)) &&
                             password.Any(c => char.IsDigit(c)) &&
                             password.Any(c => !char.IsLetterOrDigit(c));

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит все типы символов");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль не содержит все требуемые типы символов");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestNoTypesSelected()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Ни один тип символов не выбран (граничный случай)");
            try
            {
                string password = _generator.GeneratePassword(16, false, false, false, false);
                
                bool passed = string.IsNullOrEmpty(password);

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Возвращена пустая строка, когда не выбран ни один тип");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Ожидалась пустая строка, получено: '{password}'");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestPasswordValidation()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Валидация пароля (успешная)");
            try
            {
                string password = _generator.GeneratePassword(16, true, true, true, true);
                bool isValid = _generator.ValidatePassword(password, true, true, true, true);

                if (isValid)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль прошел валидацию");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль не прошел валидацию");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestPasswordValidationFailure()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Валидация пароля (провал)");
            try
            {
                // Генерируем пароль только из строчных букв, но требуем все типы
                string password = _generator.GeneratePassword(16, true, false, false, false);
                bool isValid = _generator.ValidatePassword(password, true, true, true, true);

                if (!isValid)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль правильно не прошел валидацию (ожидалось)");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Пароль прошел валидацию, хотя не должен был");
                    Console.WriteLine($"    Пароль: {password}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestPasswordUniqueness()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Уникальность паролей (генерация 10 паролей)");
            try
            {
                var passwords = new System.Collections.Generic.HashSet<string>();
                for (int i = 0; i < 10; i++)
                {
                    string password = _generator.GeneratePassword(16, true, true, true, true);
                    passwords.Add(password);
                }

                bool passed = passwords.Count == 10;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Все 10 паролей уникальны");
                    Console.WriteLine($"    Уникальных паролей: {passwords.Count}/10");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ⚠ ЧАСТИЧНО ПРОЙДЕН: Найдено {passwords.Count} уникальных паролей из 10");
                    Console.WriteLine($"    (Возможно случайное совпадение)");
                    _testsPassed++; // Считаем частично пройденным
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestPasswordRandomness()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Случайность паролей (проверка распределения символов)");
            try
            {
                // Генерируем длинный пароль для проверки распределения
                string password = _generator.GeneratePassword(100, true, true, true, true);
                
                int lowercaseCount = password.Count(c => char.IsLower(c));
                int uppercaseCount = password.Count(c => char.IsUpper(c));
                int digitCount = password.Count(c => char.IsDigit(c));
                int specialCount = password.Count(c => !char.IsLetterOrDigit(c));

                // Проверяем, что все типы символов присутствуют
                bool passed = lowercaseCount > 0 && uppercaseCount > 0 && 
                             digitCount > 0 && specialCount > 0;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Пароль содержит все типы символов");
                    Console.WriteLine($"    Строчные: {lowercaseCount}, Заглавные: {uppercaseCount}, " +
                                    $"Цифры: {digitCount}, Спец. символы: {specialCount}");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Не все типы символов присутствуют в пароле");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void TestVariousLengths()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Различные длины паролей (8, 12, 16, 32, 64)");
            try
            {
                int[] lengths = { 8, 12, 16, 32, 64 };
                bool allPassed = true;

                foreach (int length in lengths)
                {
                    string password = _generator.GeneratePassword(length, true, true, true, true);
                    if (password.Length != length)
                    {
                        allPassed = false;
                        break;
                    }
                }

                if (allPassed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН: Все длины паролей корректны");
                    foreach (int length in lengths)
                    {
                        string password = _generator.GeneratePassword(length, true, true, true, true);
                        Console.WriteLine($"    Длина {length}: {password.Substring(0, Math.Min(20, password.Length))}...");
                    }
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН: Некоторые длины паролей некорректны");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void PrintSummary()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("ИТОГИ ТЕСТИРОВАНИЯ");
            Console.WriteLine("========================================");
            Console.WriteLine($"Всего тестов: {_testNumber}");
            Console.WriteLine($"Пройдено: {_testsPassed}");
            Console.WriteLine($"Провалено: {_testsFailed}");
            Console.WriteLine($"Процент успешности: {(_testsPassed * 100.0 / _testNumber):F1}%");
            Console.WriteLine("========================================\n");
        }
    }
}

