using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CourseWork4Group.Logic;
using CourseWork4Group.Models;
using CourseWork4Group.Services;

namespace CourseWork4Group.Views
{
    /// <summary>
    /// Interaction logic for PasswordGeneratorView.xaml
    /// </summary>
    public partial class PasswordGeneratorView : UserControl
    {
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumberChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        private readonly PasswordService _passwordService;
        private readonly PasswordGenerator _passwordGenerator;

        public PasswordGeneratorView()
        {
            InitializeComponent();
            _passwordService = new PasswordService();
            _passwordGenerator = new PasswordGenerator();
            UpdateSecurityIndicator();
            UpdatePreIndicator();
            UpdatePostIndicator();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Обновляем индикаторы при загрузке
            UpdatePreIndicator();
            UpdatePostIndicator();
            
            // Обновляем индикатор надежности после загрузки окна для правильного расчета ширины
            if (!string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                UpdateSecurityIndicator(PasswordTextBox.Text);
            }
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePreIndicator();
        }

        private void UpdatePreIndicator()
        {
            // Проверяем, что элементы инициализированы
            if (IncludeLowercaseCheckBox == null || 
                IncludeUppercaseCheckBox == null || 
                IncludeNumbersCheckBox == null || 
                IncludeSpecialCharsCheckBox == null ||
                PreIndicatorBrush == null)
            {
                return;
            }

            // Проверяем, выбрана ли хотя бы одна опция
            bool hasSelection = IncludeLowercaseCheckBox.IsChecked == true ||
                               IncludeUppercaseCheckBox.IsChecked == true ||
                               IncludeNumbersCheckBox.IsChecked == true ||
                               IncludeSpecialCharsCheckBox.IsChecked == true;

            // Устанавливаем цвет: зеленый если выбрано, красный если ничего не выбрано
            Color indicatorColor = hasSelection 
                ? Color.FromRgb(76, 175, 80)  // Зеленый
                : Color.FromRgb(244, 67, 54);  // Красный

            PreIndicatorBrush.Color = indicatorColor;
        }

        private void UpdatePostIndicator()
        {
            // Проверяем, что элементы инициализированы
            if (PostIndicatorBrush == null || PasswordTextBox == null)
            {
                return;
            }

            // Проверяем, был ли сгенерирован пароль и соответствует ли он выбранным требованиям
            string password = PasswordTextBox.Text;
            
            if (string.IsNullOrEmpty(password))
            {
                // Если пароль не сгенерирован - красный
                PostIndicatorBrush.Color = Color.FromRgb(244, 67, 54);  // Красный
                return;
            }

            // Проверяем, соответствует ли пароль выбранным требованиям (пост-условие)
            bool meetsRequirements = true;

            // Проверяем, есть ли в пароле символы выбранных типов
            if (IncludeLowercaseCheckBox.IsChecked == true)
            {
                meetsRequirements = meetsRequirements && password.Any(c => char.IsLower(c));
            }

            if (IncludeUppercaseCheckBox.IsChecked == true)
            {
                meetsRequirements = meetsRequirements && password.Any(c => char.IsUpper(c));
            }

            if (IncludeNumbersCheckBox.IsChecked == true)
            {
                meetsRequirements = meetsRequirements && password.Any(c => char.IsDigit(c));
            }

            if (IncludeSpecialCharsCheckBox.IsChecked == true)
            {
                meetsRequirements = meetsRequirements && password.Any(c => !char.IsLetterOrDigit(c));
            }

            // Устанавливаем цвет: зеленый если соответствует требованиям, красный если нет
            Color indicatorColor = meetsRequirements 
                ? Color.FromRgb(76, 175, 80)  // Зеленый
                : Color.FromRgb(244, 67, 54);  // Красный

            PostIndicatorBrush.Color = indicatorColor;
        }

        private void PasswordLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PasswordLengthLabel != null)
            {
                PasswordLengthLabel.Text = ((int)e.NewValue).ToString();
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, что хотя бы один тип символов выбран
            if (!(IncludeLowercaseCheckBox.IsChecked == true) &&
                !(IncludeUppercaseCheckBox.IsChecked == true) &&
                !(IncludeNumbersCheckBox.IsChecked == true) &&
                !(IncludeSpecialCharsCheckBox.IsChecked == true))
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один тип символов для генерации пароля!", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            int length = (int)PasswordLengthSlider.Value;
            string password = GeneratePassword(length);
            PasswordTextBox.Text = password;
            UpdateSecurityIndicator(password);
            UpdatePostIndicator(); // Обновляем Post индикатор после генерации пароля
            
            // Показываем кнопку формального доказательства
            if (VerifyButton != null)
            {
                VerifyButton.Visibility = Visibility.Visible;
            }
        }

        private string GeneratePassword(int length)
        {
            bool includeLowercase = IncludeLowercaseCheckBox.IsChecked == true;
            bool includeUppercase = IncludeUppercaseCheckBox.IsChecked == true;
            bool includeNumbers = IncludeNumbersCheckBox.IsChecked == true;
            bool includeSpecialChars = IncludeSpecialCharsCheckBox.IsChecked == true;

            return _passwordGenerator.GeneratePassword(length, includeLowercase, includeUppercase, includeNumbers, includeSpecialChars);
        }

        private void UpdateSecurityIndicator(string? password = null)
        {
            if (string.IsNullOrEmpty(password))
            {
                SecurityStatusText.Text = "Сгенерируйте пароль для оценки надежности";
                SecurityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));
                SecurityScoreText.Text = "";
                SecurityBarFill.Width = 0;
                SecurityBarFill.Background = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                SecurityIndicatorBorder.Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
                SecurityDetailsText.Text = "";
                SecurityDetailsText.Visibility = Visibility.Collapsed;
                return;
            }

            var security = EvaluatePasswordStrength(password);
            
            SecurityStatusText.Text = security.StatusText;
            SecurityStatusText.Foreground = new SolidColorBrush(security.StatusColor);
            SecurityScoreText.Text = security.ScoreText;
            SecurityScoreText.Foreground = new SolidColorBrush(security.StatusColor);
            
            // Обновление индикатора силы
            // Используем LayoutUpdated для правильного расчета ширины или применяем через анимацию
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                double barWidth = SecurityBarBackground.ActualWidth > 0 
                    ? SecurityBarBackground.ActualWidth 
                    : 500; // Fallback значение
                
                SecurityBarFill.Width = barWidth * security.Strength;
                SecurityBarFill.Background = new SolidColorBrush(security.StatusColor);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
            SecurityIndicatorBorder.Background = new SolidColorBrush(
                Color.FromArgb(30, security.StatusColor.R, security.StatusColor.G, security.StatusColor.B));
            SecurityIndicatorBorder.BorderBrush = new SolidColorBrush(security.StatusColor);

            // Дополнительная информация
            SecurityDetailsText.Text = security.Details;
            SecurityDetailsText.Visibility = Visibility.Visible;
        }

        private (string StatusText, Color StatusColor, double Strength, string ScoreText, string Details) EvaluatePasswordStrength(string password)
        {
            int score = 0;
            StringBuilder details = new StringBuilder();

            // Длина пароля
            if (password.Length >= 12)
            {
                score += 25;
                details.AppendLine("✓ Достаточная длина пароля (12+ символов)");
            }
            else if (password.Length >= 8)
            {
                score += 15;
                details.AppendLine("⚠ Минимальная длина пароля (8-11 символов)");
            }
            else
            {
                details.AppendLine("✗ Пароль слишком короткий");
            }

            // Разнообразие символов
            bool hasLower = password.Any(c => char.IsLower(c));
            bool hasUpper = password.Any(c => char.IsUpper(c));
            bool hasDigit = password.Any(c => char.IsDigit(c));
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            int charTypes = (hasLower ? 1 : 0) + (hasUpper ? 1 : 0) + 
                           (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);

            score += charTypes * 15;

            if (hasLower) details.AppendLine("✓ Содержит строчные буквы");
            if (hasUpper) details.AppendLine("✓ Содержит заглавные буквы");
            if (hasDigit) details.AppendLine("✓ Содержит цифры");
            if (hasSpecial) details.AppendLine("✓ Содержит специальные символы");

            // Энтропия
            int uniqueChars = password.Distinct().Count();
            if (uniqueChars >= password.Length * 0.7)
            {
                score += 15;
                details.AppendLine("✓ Высокая энтропия (хорошая случайность)");
            }

            // Определение уровня безопасности
            string statusText;
            Color statusColor;
            double strength;
            string scoreText;

            if (score >= 80)
            {
                statusText = "Очень надежный пароль";
                statusColor = Color.FromRgb(76, 175, 80); // Зеленый
                strength = 1.0;
                scoreText = "★★★★★";
            }
            else if (score >= 60)
            {
                statusText = "Надежный пароль";
                statusColor = Color.FromRgb(139, 195, 74); // Светло-зеленый
                strength = 0.75;
                scoreText = "★★★★☆";
            }
            else if (score >= 40)
            {
                statusText = "Средний уровень надежности";
                statusColor = Color.FromRgb(255, 193, 7); // Желтый
                strength = 0.5;
                scoreText = "★★★☆☆";
            }
            else if (score >= 20)
            {
                statusText = "Слабый пароль";
                statusColor = Color.FromRgb(255, 152, 0); // Оранжевый
                strength = 0.25;
                scoreText = "★★☆☆☆";
            }
            else
            {
                statusText = "Очень слабый пароль";
                statusColor = Color.FromRgb(244, 67, 54); // Красный
                strength = 0.1;
                scoreText = "★☆☆☆☆";
            }

            return (statusText, statusColor, strength, scoreText, details.ToString().TrimEnd());
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                Clipboard.SetText(PasswordTextBox.Text);
                
                // Визуальная обратная связь
                string originalContent = CopyButton.Content.ToString()!;
                CopyButton.Content = "✓";
                CopyButton.Background = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1.5)
                };
                timer.Tick += (s, args) =>
                {
                    CopyButton.Content = originalContent;
                    CopyButton.Background = new SolidColorBrush(Color.FromRgb(102, 126, 234));
                    timer.Stop();
                };
                timer.Start();

               
            }
        }

        private void SaveToManagerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                MessageBox.Show("Сначала сгенерируйте пароль!", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            // Открываем диалоговое окно для сохранения пароля
            var dialog = new SavePasswordDialog(PasswordTextBox.Text)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                // Сохраняем пароль через сервис
                _passwordService.AddPassword(dialog.Result);
                
                MessageBox.Show("Пароль успешно сохранен в менеджер паролей!", 
                              "Успешно", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Information);
                
                // Обновляем менеджер паролей, если он открыт
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.RefreshPasswordManager();
                }
            }
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                MessageBox.Show("Сначала сгенерируйте пароль!", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            // Открываем диалоговое окно формальной верификации
            var dialog = new VerificationDialog(
                hasLowercase: IncludeLowercaseCheckBox.IsChecked == true,
                hasUppercase: IncludeUppercaseCheckBox.IsChecked == true,
                hasNumbers: IncludeNumbersCheckBox.IsChecked == true,
                hasSpecial: IncludeSpecialCharsCheckBox.IsChecked == true,
                length: (int)PasswordLengthSlider.Value,
                generatedPassword: PasswordTextBox.Text)
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }

        private void ShowTruthTableButton_Click(object sender, RoutedEventArgs e)
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

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool AllocConsole();
    }
}

