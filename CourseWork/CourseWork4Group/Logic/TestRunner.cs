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
            
            var tests = new PasswordGeneratorTests();
            tests.RunAllTests();
            
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}

