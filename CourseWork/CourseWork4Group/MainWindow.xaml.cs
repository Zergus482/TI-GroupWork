using System;
using System.Windows;
using System.Windows.Controls;
using CourseWork4Group.Logic;
using CourseWork4Group.Views;

namespace CourseWork4Group
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PasswordGeneratorView? _passwordGeneratorView;
        private PasswordManagerView? _passwordManagerView;

        public MainWindow()
        {
            InitializeComponent();
            NavigateToGenerator();
        }

        private void GeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToGenerator();
        }

        private void ManagerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToManager();
        }

        private void NavigateToGenerator()
        {
            // Создаем или переиспользуем экземпляр генератора паролей
            if (_passwordGeneratorView == null)
            {
                _passwordGeneratorView = new PasswordGeneratorView();
            }
            
            ContentArea.Content = _passwordGeneratorView;
            
            // Обновляем стили кнопок
            GeneratorButton.Style = (Style)FindResource("ActiveNavigationButtonStyle");
            ManagerButton.Style = (Style)FindResource("NavigationButtonStyle");
        }

        private void NavigateToManager()
        {
            // Создаем или переиспользуем экземпляр менеджера паролей
            if (_passwordManagerView == null)
            {
                _passwordManagerView = new PasswordManagerView();
            }
            
            ContentArea.Content = _passwordManagerView;
            
            // Обновляем стили кнопок
            GeneratorButton.Style = (Style)FindResource("NavigationButtonStyle");
            ManagerButton.Style = (Style)FindResource("ActiveNavigationButtonStyle");
            
            // Обновляем список паролей при открытии менеджера
            _passwordManagerView.RefreshPasswords();
        }

        public void RefreshPasswordManager()
        {
            if (_passwordManagerView != null && ContentArea.Content == _passwordManagerView)
            {
                _passwordManagerView.RefreshPasswords();
            }
        }

        private void TruthTableButton_Click(object sender, RoutedEventArgs e)
        {
            // Вывод таблицы истинности в консоль
            var truthTableBuilder = new TruthTableBuilder();
            
            try
            {
                // Открываем консоль для вывода
                AllocConsole();
                truthTableBuilder.PrintTruthTable();
            }
            catch (Exception ex)
            {
                // Если не удалось открыть консоль, показываем ошибку
                MessageBox.Show($"Не удалось открыть консоль: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что генератор паролей открыт и пароль сгенерирован
            if (_passwordGeneratorView == null || ContentArea.Content != _passwordGeneratorView)
            {
                MessageBox.Show("Сначала откройте генератор паролей и сгенерируйте пароль!", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                NavigateToGenerator();
                return;
            }

            // Получаем параметры из генератора паролей
            var generatorView = _passwordGeneratorView;
            
            if (string.IsNullOrEmpty(generatorView.GetGeneratedPassword()))
            {
                MessageBox.Show("Сначала сгенерируйте пароль!", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            // Открываем диалоговое окно формальной верификации
            var dialog = new VerificationDialog(
                hasLowercase: generatorView.GetIncludeLowercase(),
                hasUppercase: generatorView.GetIncludeUppercase(),
                hasNumbers: generatorView.GetIncludeNumbers(),
                hasSpecial: generatorView.GetIncludeSpecial(),
                length: generatorView.GetPasswordLength(),
                generatedPassword: generatorView.GetGeneratedPassword())
            {
                Owner = this
            };

            dialog.ShowDialog();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool AllocConsole();
    }
}