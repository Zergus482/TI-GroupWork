using System.Windows;
using CourseWork4Group.Models;

namespace CourseWork4Group.Views
{
    /// <summary>
    /// Interaction logic for SavePasswordDialog.xaml
    /// </summary>
    public partial class SavePasswordDialog : Window
    {
        public PasswordEntry? Result { get; private set; }

        public SavePasswordDialog(string? password = null)
        {
            InitializeComponent();
            
            if (!string.IsNullOrEmpty(password))
            {
                PasswordTextBox.Text = password;
            }
            
            // Фокус на поле сервиса
            ServiceTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполненности полей
            if (string.IsNullOrWhiteSpace(ServiceTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите название сервиса!", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                ServiceTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("Пароль не может быть пустым!", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            // Создаем объект пароля
            Result = new PasswordEntry
            {
                Service = ServiceTextBox.Text.Trim(),
                Login = LoginTextBox.Text.Trim(),
                Password = PasswordTextBox.Text
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

