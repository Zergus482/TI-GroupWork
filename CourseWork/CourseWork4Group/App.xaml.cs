using System.Configuration;
using System.Data;
using System.Windows;
using CourseWork4Group.Logic;

namespace CourseWork4Group
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Запуск тестов, если передан аргумент --run-tests или -t
            // Пример запуска: CourseWork4Group.exe --run-tests
            if (e.Args.Length > 0 && (e.Args[0] == "--run-tests" || e.Args[0] == "-t"))
            {
                TestRunner.RunTests();
            }
        }

        /// <summary>
        /// Запускает тесты генерации паролей и выводит результаты в консоль
        /// Можно вызвать из любого места в коде: App.RunPasswordGeneratorTests()
        /// </summary>
        public static void RunPasswordGeneratorTests()
        {
            TestRunner.RunTests();
        }
    }

}
