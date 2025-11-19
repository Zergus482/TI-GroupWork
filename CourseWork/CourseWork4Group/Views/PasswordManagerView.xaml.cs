using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CourseWork4Group.Models;
using CourseWork4Group.Services;

namespace CourseWork4Group.Views
{
    /// <summary>
    /// Interaction logic for PasswordManagerView.xaml
    /// </summary>
    public partial class PasswordManagerView : UserControl
    {
        private readonly PasswordService _passwordService;
        private readonly ObservableCollection<PasswordEntry> _passwords;

        public PasswordManagerView()
        {
            InitializeComponent();
            _passwordService = new PasswordService();
            _passwords = new ObservableCollection<PasswordEntry>();
        }

        public void RefreshPasswords()
        {
            LoadPasswords();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPasswords();
        }

        private void LoadPasswords()
        {
            _passwords.Clear();
            
            // Перезагружаем данные из файла, создавая новый экземпляр сервиса
            // чтобы получить актуальные данные
            var service = new PasswordService();
            foreach (var entry in service.GetPasswords())
            {
                _passwords.Add(entry);
            }

            PasswordsItemsControl.ItemsSource = _passwords;
            UpdateUI();
        }

        private void UpdateUI()
        {
            int count = _passwords.Count;
            PasswordsCountText.Text = $"Сохранено паролей: {count}";

            if (count == 0)
            {
                EmptyStateBorder.Visibility = Visibility.Visible;
                PasswordsItemsControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyStateBorder.Visibility = Visibility.Collapsed;
                PasswordsItemsControl.Visibility = Visibility.Visible;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string id)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот пароль?", 
                                           "Подтверждение удаления", 
                                           MessageBoxButton.YesNo, 
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Используем новый экземпляр сервиса для удаления, чтобы он работал с актуальными данными
                    var service = new PasswordService();
                    service.RemovePassword(id);
                    LoadPasswords(); // Перезагружаем данные из файла
                    
                    MessageBox.Show("Пароль успешно удален!", 
                                  "Успешно", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }
            }
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Находим родительский Grid (контейнер пароля)
                var grid = FindVisualParent<Grid>(button);
                if (grid != null)
                {
                    // Находим StackPanel с паролями
                    var stackPanel = FindVisualChild<StackPanel>(grid);
                    if (stackPanel != null)
                    {
                        TextBlock? passwordMaskTextBlock = null;
                        TextBlock? passwordTextBlock = null;

                        // Ищем TextBlock элементы по имени
                        foreach (var child in GetVisualChildren(stackPanel))
                        {
                            if (child is TextBlock textBlock)
                            {
                                if (textBlock.Name == "PasswordMaskTextBlock")
                                    passwordMaskTextBlock = textBlock;
                                else if (textBlock.Name == "PasswordTextBlock")
                                    passwordTextBlock = textBlock;
                            }
                        }

                        if (passwordMaskTextBlock != null && passwordTextBlock != null)
                        {
                            // Переключаем видимость
                            if (passwordTextBlock.Visibility == Visibility.Visible)
                            {
                                // Скрываем пароль
                                passwordTextBlock.Visibility = Visibility.Collapsed;
                                passwordMaskTextBlock.Visibility = Visibility.Visible;
                                button.Content = "👁️";
                            }
                            else
                            {
                                // Показываем пароль
                                passwordTextBlock.Visibility = Visibility.Visible;
                                passwordMaskTextBlock.Visibility = Visibility.Collapsed;
                                button.Content = "🙈";
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject parent)
        {
            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                yield return System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            }
        }

        private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T result)
                    return result;
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                {
                    return result;
                }
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

    }
}

