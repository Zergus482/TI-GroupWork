using System.Windows;
using System.Windows.Controls;
using CourseWork4Group.Views;

namespace CourseWork4Group
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            ContentArea.Content = new PasswordGeneratorView();
            
            // Обновляем стили кнопок
            GeneratorButton.Style = (Style)FindResource("ActiveNavigationButtonStyle");
            ManagerButton.Style = (Style)FindResource("NavigationButtonStyle");
        }

        private PasswordManagerView? _passwordManagerView;

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
    }
}