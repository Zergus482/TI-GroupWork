using System;
using System.Runtime.InteropServices;

namespace CourseWork4Group.Logic
{
    /// <summary>
    /// Класс для запуска тестов генерации паролей
    /// </summary>
    public static class TestRunner
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        /// <summary>
        /// Запускает все тесты генерации паролей и выводит результаты в консоль
        /// </summary>
        public static void RunTests()
        {
            // Открываем консоль для вывода
            AllocConsole();
            
            // Запускаем тесты генерации паролей
            var passwordTests = new PasswordGeneratorTests();
            passwordTests.RunAllTests();
            
            // Запускаем тесты таблицы истинности, ДНФ и КНФ
            var truthTableTests = new TruthTableTests();
            truthTableTests.RunAllTests();
            
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}

