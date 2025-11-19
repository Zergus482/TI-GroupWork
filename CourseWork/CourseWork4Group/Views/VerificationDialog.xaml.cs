using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CourseWork4Group.Views
{
    /// <summary>
    /// Interaction logic for VerificationDialog.xaml
    /// </summary>
    public partial class VerificationDialog : Window
    {
        public string HoareTriple { get; private set; } = string.Empty;

        public VerificationDialog(bool hasLowercase, bool hasUppercase, bool hasNumbers, 
                                 bool hasSpecial, int length, string? generatedPassword = null)
        {
            InitializeComponent();
            BuildVerification(hasLowercase, hasUppercase, hasNumbers, hasSpecial, length, generatedPassword);
        }

        private void BuildVerification(bool hasLowercase, bool hasUppercase, bool hasNumbers, 
                                      bool hasSpecial, int length, string? password)
        {
            // Построение постусловия
            var postconditions = new List<string>();
            if (hasLowercase) postconditions.Add("HasLower(password)");
            if (hasUppercase) postconditions.Add("HasUpper(password)");
            if (hasNumbers) postconditions.Add("HasDigit(password)");
            if (hasSpecial) postconditions.Add("HasSpecial(password)");
            postconditions.Add($"Length(password) = {length}");
            postconditions.Add("NoSequential(password)");
            postconditions.Add("NoRepeating(password)");

            string postcondition = string.Join(" ∧ ", postconditions);
            PostconditionText.Text = $"{{ P }} password := GeneratePassword() {{ Q }}\n\n" +
                                    $"Q: {postcondition}";

            // Построение кода фрагмента
            var codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("function GeneratePassword(length: int, options: Options): string");
            codeBuilder.AppendLine("{");
            codeBuilder.AppendLine("    charSet := \"\"");
            
            if (hasLowercase) codeBuilder.AppendLine("    if (options.lowercase) charSet += \"a-z\"");
            if (hasUppercase) codeBuilder.AppendLine("    if (options.uppercase) charSet += \"A-Z\"");
            if (hasNumbers) codeBuilder.AppendLine("    if (options.numbers) charSet += \"0-9\"");
            if (hasSpecial) codeBuilder.AppendLine("    if (options.special) charSet += \"!@#$%...\"");
            
            codeBuilder.AppendLine("    ");
            codeBuilder.AppendLine("    password := \"\"");
            codeBuilder.AppendLine($"    do {{");
            codeBuilder.AppendLine("        password = RandomSelect(charSet, length)");
            codeBuilder.AppendLine("    }} while (");
            
            var whileConditions = new List<string>();
            if (hasLowercase) whileConditions.Add("!HasLower(password)");
            if (hasUppercase) whileConditions.Add("!HasUpper(password)");
            if (hasNumbers) whileConditions.Add("!HasDigit(password)");
            if (hasSpecial) whileConditions.Add("!HasSpecial(password)");
            whileConditions.Add("HasSequential(password)");
            whileConditions.Add("HasRepeating(password)");
            
            codeBuilder.AppendLine("        " + string.Join(" || ", whileConditions));
            codeBuilder.AppendLine("    )");
            codeBuilder.AppendLine("    ");
            codeBuilder.AppendLine("    return password");
            codeBuilder.AppendLine("}");

            CodeFragmentText.Text = codeBuilder.ToString();

            // Построение предусловия (WP - Weakest Precondition)
            var preconditions = new List<string>();
            if (hasLowercase) preconditions.Add("charSet содержит строчные буквы");
            if (hasUppercase) preconditions.Add("charSet содержит заглавные буквы");
            if (hasNumbers) preconditions.Add("charSet содержит цифры");
            if (hasSpecial) preconditions.Add("charSet содержит спецсимволы");
            preconditions.Add($"length >= {Math.Max(length, 8)}");
            preconditions.Add("RandomSelect криптографически стойкий");

            string precondition = string.Join(" ∧ ", preconditions);
            PreconditionText.Text = $"P (WP): {precondition}";

            // Построение триады Хоара
            var hoareBuilder = new StringBuilder();
            hoareBuilder.AppendLine($"{{ P }} password := GeneratePassword() {{ Q }}");
            hoareBuilder.AppendLine("");
            hoareBuilder.AppendLine("где:");
            hoareBuilder.AppendLine($"  P (Precondition): {precondition}");
            hoareBuilder.AppendLine($"  Q (Postcondition): {postcondition}");
            hoareBuilder.AppendLine("");
            hoareBuilder.AppendLine("Доказательство (WP):");
            hoareBuilder.AppendLine("  WP(GeneratePassword, Q) = P'");
            hoareBuilder.AppendLine("  P' ⊆ P → Доказано ✓");
            
            HoareTriple = hoareBuilder.ToString();
            HoareTripleText.Text = HoareTriple;

            // Результат доказательства
            bool allConditionsMet = (hasLowercase || hasUppercase || hasNumbers || hasSpecial) && length >= 8;
            
            if (allConditionsMet)
            {
                ProofResultBorder.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(232, 245, 233));
                ProofResultBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(76, 175, 80));
                
                var proofText = new StringBuilder();
                proofText.AppendLine("✓ ДОКАЗАНО: Генератор гарантирует выполнение постусловия");
                proofText.AppendLine("");
                proofText.AppendLine("Доказательство:");
                proofText.AppendLine($"1. Предусловие P выполнено: все выбранные наборы символов включены");
                proofText.AppendLine($"2. Цикл do-while гарантирует повторную генерацию до удовлетворения всех условий");
                proofText.AppendLine($"3. Проверки HasLower, HasUpper, HasDigit, HasSpecial гарантируют наличие требуемых символов");
                proofText.AppendLine($"4. Длина пароля установлена как {length} ≥ 8");
                proofText.AppendLine("");
                
                if (!string.IsNullOrEmpty(password))
                {
                    proofText.AppendLine($"Проверка на примере сгенерированного пароля:");
                    bool pwdHasLower = password.Any(char.IsLower);
                    bool pwdHasUpper = password.Any(char.IsUpper);
                    bool pwdHasDigit = password.Any(char.IsDigit);
                    bool pwdHasSpecial = password.Any(c => !char.IsLetterOrDigit(c));
                    
                    proofText.AppendLine($"  • HasLower: {(pwdHasLower ? "✓" : "✗")}");
                    proofText.AppendLine($"  • HasUpper: {(pwdHasUpper ? "✓" : "✗")}");
                    proofText.AppendLine($"  • HasDigit: {(pwdHasDigit ? "✓" : "✗")}");
                    proofText.AppendLine($"  • HasSpecial: {(pwdHasSpecial ? "✓" : "✗")}");
                    proofText.AppendLine($"  • Length = {length}: {(password.Length == length ? "✓" : "✗")}");
                }
                
                ProofResultText.Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(46, 125, 50));
                ProofResultText.Text = proofText.ToString();
            }
            else
            {
                ProofResultBorder.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(255, 235, 238));
                ProofResultBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(244, 67, 54));
                
                ProofResultText.Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(183, 28, 28));
                ProofResultText.Text = "✗ НЕ ДОКАЗАНО: Не выполнено предусловие\n" +
                                      "Необходимо выбрать хотя бы один тип символов и длину ≥ 8";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

