using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseWork4Group.Logic
{
    public class TruthTableBuilder
    {
        public class TruthTableRow
        {
            public bool Lilsymbols { get; set; }
            public bool Largesymbols { get; set; }
            public bool Numbers { get; set; }
            public bool SecSymbols { get; set; }
            public bool GreatPass { get; set; }

            // Свойства для отображения в виде 0/1
            public int LilsymbolsValue => Lilsymbols ? 1 : 0;
            public int LargesymbolsValue => Largesymbols ? 1 : 0;
            public int NumbersValue => Numbers ? 1 : 0;
            public int SecSymbolsValue => SecSymbols ? 1 : 0;
            public int GreatPassValue => GreatPass ? 1 : 0;
        }

        public List<TruthTableRow> BuildTruthTable()
        {
            var table = new List<TruthTableRow>();

            // Перебираем все возможные комбинации (2^4 = 16 строк)
            for (int i = 0; i < 16; i++)
            {
                bool lilsymbols = (i & 1) != 0;
                bool largesymbols = (i & 2) != 0;
                bool numbers = (i & 4) != 0;
                bool secSymbols = (i & 8) != 0;

                // Вычисляем формулу: greatpass = lilsymbols ∧ largesymbols ∧ (numbers ∨ SecSymbols)
                bool greatPass = lilsymbols && largesymbols && (numbers || secSymbols);

                table.Add(new TruthTableRow
                {
                    Lilsymbols = lilsymbols,
                    Largesymbols = largesymbols,
                    Numbers = numbers,
                    SecSymbols = secSymbols,
                    GreatPass = greatPass
                });
            }

            return table;
        }

        public string GetDNF(List<TruthTableRow> table)
        {
            // ДНФ = дизъюнкция конъюнкций для строк, где greatPass = true
            var terms = new List<string>();

            foreach (var row in table.Where(r => r.GreatPass))
            {
                var parts = new List<string>();

                if (row.Lilsymbols)
                    parts.Add("lilsymbols");
                else
                    parts.Add("¬lilsymbols");

                if (row.Largesymbols)
                    parts.Add("largesymbols");
                else
                    parts.Add("¬largesymbols");

                if (row.Numbers)
                    parts.Add("numbers");
                else
                    parts.Add("¬numbers");

                if (row.SecSymbols)
                    parts.Add("SecSymbols");
                else
                    parts.Add("¬SecSymbols");

                terms.Add("(" + string.Join(" & ", parts) + ")");
            }

            if (terms.Count == 0)
                return "0"; // Константа ложь

            return string.Join(" | ", terms);
        }

        public string GetCNF(List<TruthTableRow> table)
        {
            // КНФ = конъюнкция дизъюнкций для строк, где greatPass = false
            var terms = new List<string>();

            foreach (var row in table.Where(r => !r.GreatPass))
            {
                var parts = new List<string>();

                if (!row.Lilsymbols)
                    parts.Add("lilsymbols");
                else
                    parts.Add("¬lilsymbols");

                if (!row.Largesymbols)
                    parts.Add("largesymbols");
                else
                    parts.Add("¬largesymbols");

                if (!row.Numbers)
                    parts.Add("numbers");
                else
                    parts.Add("¬numbers");

                if (!row.SecSymbols)
                    parts.Add("SecSymbols");
                else
                    parts.Add("¬SecSymbols");

                terms.Add("(" + string.Join(" | ", parts) + ")");
            }

            if (terms.Count == 0)
                return "1"; // Константа истина

            return string.Join(" & ", terms);
        }

        public void PrintTruthTable()
        {
            var table = BuildTruthTable();

            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("ТАБЛИЦА ИСТИННОСТИ");
            Console.WriteLine("Формула: greatpass = lilsymbols ∧ largesymbols ∧ (numbers ∨ SecSymbols)");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine();

            // Заголовок таблицы
            Console.WriteLine($"{"lilsymbols",-12} | {"largesymbols",-12} | {"numbers",-8} | {"SecSymbols",-11} | {"greatpass",-10}");
            Console.WriteLine("-".PadRight(80, '-'));

            // Строки таблицы (0 и 1 вместо True/False)
            foreach (var row in table)
            {
                int lilsymbolsVal = row.Lilsymbols ? 1 : 0;
                int largesymbolsVal = row.Largesymbols ? 1 : 0;
                int numbersVal = row.Numbers ? 1 : 0;
                int secSymbolsVal = row.SecSymbols ? 1 : 0;
                int greatPassVal = row.GreatPass ? 1 : 0;
                
                Console.WriteLine($"{lilsymbolsVal,-12} | {largesymbolsVal,-12} | {numbersVal,-8} | {secSymbolsVal,-11} | {greatPassVal,-10}");
            }

            Console.WriteLine();
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine();

            // Вывод ДНФ
            string dnf = GetDNF(table);
            Console.WriteLine("ДНФ (Дизъюнктивная Нормальная Форма):");
            Console.WriteLine(dnf);
            Console.WriteLine();

            // Вывод КНФ
            string cnf = GetCNF(table);
            Console.WriteLine("КНФ (Конъюнктивная Нормальная Форма):");
            Console.WriteLine(cnf);
            Console.WriteLine();
            Console.WriteLine("=".PadRight(80, '='));
        }
    }
}

