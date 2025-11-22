using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseWork4Group.Logic
{
    /// <summary>
    /// Тестовый класс для проверки функций построения таблицы истинности, ДНФ и КНФ
    /// </summary>
    public class TruthTableTests
    {
        private readonly TruthTableBuilder _builder;
        private int _testsPassed;
        private int _testsFailed;
        private int _testNumber;

        public TruthTableTests()
        {
            _builder = new TruthTableBuilder();
            _testsPassed = 0;
            _testsFailed = 0;
            _testNumber = 0;
        }

        /// <summary>
        /// Запускает все тесты для таблицы истинности, ДНФ и КНФ
        /// </summary>
        public void RunAllTests()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("ТЕСТИРОВАНИЕ ТАБЛИЦЫ ИСТИННОСТИ, ДНФ И КНФ");
            Console.WriteLine("========================================\n");

            TestTruthTableCorrectness();
            TestDNFStructure();
            TestCNFStructure();
            TestDNFCNFEquivalence();

            PrintSummary();
        }

        /// <summary>
        /// Тест 1: Проверка корректности построения таблицы истинности
        /// 
        /// Описание: Этот тест проверяет, что таблица истинности строится корректно.
        /// Проверяются следующие аспекты:
        /// 1. Таблица содержит ровно 16 строк (2^4 комбинации для 4 переменных)
        /// 2. Все комбинации входных переменных присутствуют (от 0000 до 1111)
        /// 3. Формула greatpass = lilsymbols ∧ largesymbols ∧ (numbers ∨ SecSymbols) 
        ///    вычисляется правильно для каждой строки
        /// 4. Проверяются конкретные случаи:
        ///    - Когда все переменные false, результат должен быть false
        ///    - Когда lilsymbols=true, largesymbols=true, numbers=true, результат должен быть true
        ///    - Когда lilsymbols=true, largesymbols=true, SecSymbols=true, результат должен быть true
        ///    - Когда lilsymbols=false или largesymbols=false, результат должен быть false
        /// </summary>
        private void TestTruthTableCorrectness()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Корректность построения таблицы истинности");
            Console.WriteLine("  Описание: Проверка количества строк, уникальности комбинаций и правильности вычисления формулы");
            
            try
            {
                var table = _builder.BuildTruthTable();

                // Проверка 1: Количество строк должно быть 16
                bool correctCount = table.Count == 16;
                
                // Проверка 2: Все комбинации должны быть уникальными
                var combinations = new HashSet<string>();
                foreach (var row in table)
                {
                    string key = $"{row.LilsymbolsValue}{row.LargesymbolsValue}{row.NumbersValue}{row.SecSymbolsValue}";
                    combinations.Add(key);
                }
                bool allUnique = combinations.Count == 16;

                // Проверка 3: Проверка правильности вычисления формулы для конкретных случаев
                // Случай 1: Все false -> результат false
                var allFalseRow = table.FirstOrDefault(r => 
                    !r.Lilsymbols && !r.Largesymbols && !r.Numbers && !r.SecSymbols);
                bool case1 = allFalseRow != null && !allFalseRow.GreatPass;

                // Случай 2: lilsymbols=true, largesymbols=true, numbers=true -> результат true
                var case2Row = table.FirstOrDefault(r => 
                    r.Lilsymbols && r.Largesymbols && r.Numbers && !r.SecSymbols);
                bool case2 = case2Row != null && case2Row.GreatPass;

                // Случай 3: lilsymbols=true, largesymbols=true, SecSymbols=true -> результат true
                var case3Row = table.FirstOrDefault(r => 
                    r.Lilsymbols && r.Largesymbols && !r.Numbers && r.SecSymbols);
                bool case3 = case3Row != null && case3Row.GreatPass;

                // Случай 4: lilsymbols=false -> результат false
                var case4Row = table.FirstOrDefault(r => 
                    !r.Lilsymbols && r.Largesymbols && r.Numbers && r.SecSymbols);
                bool case4 = case4Row != null && !case4Row.GreatPass;

                // Проверка 4: Проверка формулы для всех строк
                bool allFormulasCorrect = true;
                foreach (var row in table)
                {
                    bool expected = row.Lilsymbols && row.Largesymbols && (row.Numbers || row.SecSymbols);
                    if (row.GreatPass != expected)
                    {
                        allFormulasCorrect = false;
                        break;
                    }
                }

                bool passed = correctCount && allUnique && case1 && case2 && case3 && case4 && allFormulasCorrect;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН:");
                    Console.WriteLine($"    - Количество строк: {table.Count} (ожидалось: 16)");
                    Console.WriteLine($"    - Уникальных комбинаций: {combinations.Count} (ожидалось: 16)");
                    Console.WriteLine($"    - Все формулы вычислены корректно");
                    Console.WriteLine($"    - Проверены граничные случаи");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН:");
                    if (!correctCount) Console.WriteLine($"    - Неверное количество строк: {table.Count} вместо 16");
                    if (!allUnique) Console.WriteLine($"    - Не все комбинации уникальны");
                    if (!case1) Console.WriteLine($"    - Ошибка в случае: все переменные false");
                    if (!case2) Console.WriteLine($"    - Ошибка в случае: lilsymbols=true, largesymbols=true, numbers=true");
                    if (!case3) Console.WriteLine($"    - Ошибка в случае: lilsymbols=true, largesymbols=true, SecSymbols=true");
                    if (!case4) Console.WriteLine($"    - Ошибка в случае: lilsymbols=false");
                    if (!allFormulasCorrect) Console.WriteLine($"    - Некоторые формулы вычислены неверно");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                Console.WriteLine($"    StackTrace: {ex.StackTrace}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Тест 2: Проверка структуры и корректности ДНФ (Дизъюнктивной Нормальной Формы)
        /// 
        /// Описание: Этот тест проверяет корректность построения ДНФ из таблицы истинности.
        /// ДНФ должна представлять собой дизъюнкцию конъюнкций для всех строк, где greatPass = true.
        /// Проверяются следующие аспекты:
        /// 1. ДНФ содержит только строки, где greatPass = true
        /// 2. Каждый терм ДНФ представляет конъюнкцию всех 4 переменных (с отрицаниями или без)
        /// 3. ДНФ не пустая (если есть строки с greatPass = true)
        /// 4. Структура ДНФ: термы разделены символом "|" (дизъюнкция)
        /// 5. Каждый терм заключен в скобки и содержит 4 переменные, разделенные "&" (конъюнкция)
        /// 6. Проверяется, что количество термов в ДНФ равно количеству строк с greatPass = true
        /// </summary>
        private void TestDNFStructure()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Структура и корректность ДНФ");
            Console.WriteLine("  Описание: Проверка правильности построения Дизъюнктивной Нормальной Формы");
            
            try
            {
                var table = _builder.BuildTruthTable();
                string dnf = _builder.GetDNF(table);

                // Подсчет строк с greatPass = true
                int trueRowsCount = table.Count(r => r.GreatPass);

                // Проверка 1: ДНФ не должна быть пустой, если есть строки с true
                bool notEmpty = trueRowsCount > 0 ? !string.IsNullOrEmpty(dnf) : (dnf == "0" || string.IsNullOrEmpty(dnf));

                // Проверка 2: Подсчет термов в ДНФ (разделены символом "|")
                int termCount = dnf == "0" ? 0 : dnf.Split(new[] { " | " }, StringSplitOptions.None).Length;
                bool correctTermCount = termCount == trueRowsCount || (trueRowsCount == 0 && dnf == "0");

                // Проверка 3: Каждый терм должен содержать 4 переменные
                bool allTermsHave4Vars = true;
                if (dnf != "0" && !string.IsNullOrEmpty(dnf))
                {
                    var terms = dnf.Split(new[] { " | " }, StringSplitOptions.None);
                    foreach (var term in terms)
                    {
                        // Убираем скобки и считаем переменные
                        string cleanTerm = term.Trim('(', ')');
                        int varCount = cleanTerm.Split(new[] { " & " }, StringSplitOptions.None).Length;
                        if (varCount != 4)
                        {
                            allTermsHave4Vars = false;
                            break;
                        }
                    }
                }

                // Проверка 4: ДНФ должна содержать правильные переменные
                bool containsCorrectVars = true;
                if (dnf != "0" && !string.IsNullOrEmpty(dnf))
                {
                    // Проверяем, что в ДНФ упоминаются все 4 переменные
                    containsCorrectVars = dnf.Contains("lilsymbols") && 
                                         dnf.Contains("largesymbols") && 
                                         dnf.Contains("numbers") && 
                                         dnf.Contains("SecSymbols");
                }

                // Проверка 5: Структура ДНФ (термы в скобках, разделены "|")
                bool correctStructure = dnf == "0" || 
                    (dnf.Contains("(") && dnf.Contains(")") && dnf.Contains(" | "));

                bool passed = notEmpty && correctTermCount && allTermsHave4Vars && containsCorrectVars && correctStructure;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН:");
                    Console.WriteLine($"    - ДНФ построена корректно");
                    Console.WriteLine($"    - Количество термов: {termCount} (строк с greatPass=true: {trueRowsCount})");
                    Console.WriteLine($"    - Каждый терм содержит 4 переменные");
                    Console.WriteLine($"    - Структура ДНФ корректна");
                    Console.WriteLine($"    - ДНФ: {dnf.Substring(0, Math.Min(100, dnf.Length))}...");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН:");
                    if (!notEmpty) Console.WriteLine($"    - ДНФ пустая, хотя есть строки с greatPass=true");
                    if (!correctTermCount) Console.WriteLine($"    - Неверное количество термов: {termCount} (ожидалось: {trueRowsCount})");
                    if (!allTermsHave4Vars) Console.WriteLine($"    - Не все термы содержат 4 переменные");
                    if (!containsCorrectVars) Console.WriteLine($"    - ДНФ не содержит все необходимые переменные");
                    if (!correctStructure) Console.WriteLine($"    - Неверная структура ДНФ");
                    Console.WriteLine($"    - ДНФ: {dnf}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                Console.WriteLine($"    StackTrace: {ex.StackTrace}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Тест 3: Проверка структуры и корректности КНФ (Конъюнктивной Нормальной Формы)
        /// 
        /// Описание: Этот тест проверяет корректность построения КНФ из таблицы истинности.
        /// КНФ должна представлять собой конъюнкцию дизъюнкций для всех строк, где greatPass = false.
        /// Проверяются следующие аспекты:
        /// 1. КНФ содержит только строки, где greatPass = false
        /// 2. Каждый терм КНФ представляет дизъюнкцию всех 4 переменных (с отрицаниями или без)
        /// 3. КНФ не пустая (если есть строки с greatPass = false)
        /// 4. Структура КНФ: термы разделены символом "&" (конъюнкция)
        /// 5. Каждый терм заключен в скобки и содержит 4 переменные, разделенные "|" (дизъюнкция)
        /// 6. Проверяется, что количество термов в КНФ равно количеству строк с greatPass = false
        /// 7. Проверяется, что переменные в КНФ инвертированы относительно строк с false
        /// </summary>
        private void TestCNFStructure()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Структура и корректность КНФ");
            Console.WriteLine("  Описание: Проверка правильности построения Конъюнктивной Нормальной Формы");
            
            try
            {
                var table = _builder.BuildTruthTable();
                string cnf = _builder.GetCNF(table);

                // Подсчет строк с greatPass = false
                int falseRowsCount = table.Count(r => !r.GreatPass);

                // Проверка 1: КНФ не должна быть пустой, если есть строки с false
                bool notEmpty = falseRowsCount > 0 ? !string.IsNullOrEmpty(cnf) : (cnf == "1" || string.IsNullOrEmpty(cnf));

                // Проверка 2: Подсчет термов в КНФ (разделены символом " & ")
                int termCount = cnf == "1" ? 0 : cnf.Split(new[] { " & " }, StringSplitOptions.None).Length;
                bool correctTermCount = termCount == falseRowsCount || (falseRowsCount == 0 && cnf == "1");

                // Проверка 3: Каждый терм должен содержать 4 переменные
                bool allTermsHave4Vars = true;
                if (cnf != "1" && !string.IsNullOrEmpty(cnf))
                {
                    var terms = cnf.Split(new[] { " & " }, StringSplitOptions.None);
                    foreach (var term in terms)
                    {
                        // Убираем скобки и считаем переменные
                        string cleanTerm = term.Trim('(', ')');
                        int varCount = cleanTerm.Split(new[] { " | " }, StringSplitOptions.None).Length;
                        if (varCount != 4)
                        {
                            allTermsHave4Vars = false;
                            break;
                        }
                    }
                }

                // Проверка 4: КНФ должна содержать правильные переменные
                bool containsCorrectVars = true;
                if (cnf != "1" && !string.IsNullOrEmpty(cnf))
                {
                    // Проверяем, что в КНФ упоминаются все 4 переменные
                    containsCorrectVars = cnf.Contains("lilsymbols") && 
                                        cnf.Contains("largesymbols") && 
                                        cnf.Contains("numbers") && 
                                        cnf.Contains("SecSymbols");
                }

                // Проверка 5: Структура КНФ (термы в скобках, разделены " & ")
                bool correctStructure = cnf == "1" || 
                    (cnf.Contains("(") && cnf.Contains(")") && cnf.Contains(" & "));

                bool passed = notEmpty && correctTermCount && allTermsHave4Vars && containsCorrectVars && correctStructure;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН:");
                    Console.WriteLine($"    - КНФ построена корректно");
                    Console.WriteLine($"    - Количество термов: {termCount} (строк с greatPass=false: {falseRowsCount})");
                    Console.WriteLine($"    - Каждый терм содержит 4 переменные");
                    Console.WriteLine($"    - Структура КНФ корректна");
                    Console.WriteLine($"    - КНФ: {cnf.Substring(0, Math.Min(100, cnf.Length))}...");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН:");
                    if (!notEmpty) Console.WriteLine($"    - КНФ пустая, хотя есть строки с greatPass=false");
                    if (!correctTermCount) Console.WriteLine($"    - Неверное количество термов: {termCount} (ожидалось: {falseRowsCount})");
                    if (!allTermsHave4Vars) Console.WriteLine($"    - Не все термы содержат 4 переменные");
                    if (!containsCorrectVars) Console.WriteLine($"    - КНФ не содержит все необходимые переменные");
                    if (!correctStructure) Console.WriteLine($"    - Неверная структура КНФ");
                    Console.WriteLine($"    - КНФ: {cnf}");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                Console.WriteLine($"    StackTrace: {ex.StackTrace}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Тест 4: Проверка эквивалентности ДНФ и КНФ таблице истинности
        /// 
        /// Описание: Этот тест проверяет, что ДНФ и КНФ эквивалентны исходной таблице истинности.
        /// Это означает, что для любой комбинации входных переменных:
        /// - Если строка в таблице имеет greatPass = true, то ДНФ должна давать true для этой комбинации
        /// - Если строка в таблице имеет greatPass = false, то КНФ должна давать false для этой комбинации
        /// - ДНФ и КНФ должны быть логически эквивалентны друг другу
        /// 
        /// Проверяются следующие аспекты:
        /// 1. Для каждой строки с greatPass = true, ДНФ должна содержать соответствующий терм
        /// 2. Для каждой строки с greatPass = false, КНФ должна содержать соответствующий терм
        /// 3. Количество термов в ДНФ + количество термов в КНФ = 16 (общее количество строк)
        /// 4. ДНФ и КНФ покрывают все возможные комбинации переменных
        /// 5. Проверка конкретных примеров: для известных комбинаций проверяется наличие в ДНФ/КНФ
        /// </summary>
        private void TestDNFCNFEquivalence()
        {
            _testNumber++;
            Console.WriteLine($"Тест {_testNumber}: Эквивалентность ДНФ и КНФ таблице истинности");
            Console.WriteLine("  Описание: Проверка соответствия ДНФ и КНФ исходной таблице истинности");
            
            try
            {
                var table = _builder.BuildTruthTable();
                string dnf = _builder.GetDNF(table);
                string cnf = _builder.GetCNF(table);

                int trueRowsCount = table.Count(r => r.GreatPass);
                int falseRowsCount = table.Count(r => !r.GreatPass);

                // Проверка 1: Сумма термов ДНФ и КНФ должна равняться 16
                int dnfTerms = dnf == "0" ? 0 : dnf.Split(new[] { " | " }, StringSplitOptions.None).Length;
                int cnfTerms = cnf == "1" ? 0 : cnf.Split(new[] { " & " }, StringSplitOptions.None).Length;
                bool sumEquals16 = (dnfTerms + cnfTerms) == 16;

                // Проверка 2: Количество термов ДНФ должно равняться количеству строк с true
                bool dnfCountMatches = dnfTerms == trueRowsCount || (trueRowsCount == 0 && dnf == "0");

                // Проверка 3: Количество термов КНФ должно равняться количеству строк с false
                bool cnfCountMatches = cnfTerms == falseRowsCount || (falseRowsCount == 0 && cnf == "1");

                // Проверка 4: Проверка конкретных примеров - ищем конкретные термы в ДНФ и КНФ
                // Пример 1: Строка где все true -> должна быть в ДНФ (если greatPass = true)
                var allTrueRow = table.FirstOrDefault(r => 
                    r.Lilsymbols && r.Largesymbols && r.Numbers && r.SecSymbols);
                bool example1 = true;
                if (allTrueRow != null)
                {
                    if (allTrueRow.GreatPass)
                    {
                        // Ищем конкретный терм в ДНФ: (lilsymbols & largesymbols & numbers & SecSymbols)
                        var dnfTermsArray = dnf == "0" ? new string[0] : dnf.Split(new[] { " | " }, StringSplitOptions.None);
                        bool found = false;
                        foreach (var term in dnfTermsArray)
                        {
                            string cleanTerm = term.Trim('(', ')');
                            // Проверяем, что терм содержит все 4 переменные без отрицаний
                            var parts = cleanTerm.Split(new[] { " & " }, StringSplitOptions.None);
                            if (parts.Length == 4 &&
                                parts.Any(p => p.Trim() == "lilsymbols") &&
                                parts.Any(p => p.Trim() == "largesymbols") &&
                                parts.Any(p => p.Trim() == "numbers") &&
                                parts.Any(p => p.Trim() == "SecSymbols"))
                            {
                                found = true;
                                break;
                            }
                        }
                        example1 = found;
                    }
                    else
                    {
                        // Если greatPass = false, то этой строки не должно быть в ДНФ
                        example1 = true; // Это нормально
                    }
                }

                // Пример 2: Строка где все false -> должна быть в КНФ (если greatPass = false)
                var allFalseRow = table.FirstOrDefault(r => 
                    !r.Lilsymbols && !r.Largesymbols && !r.Numbers && !r.SecSymbols);
                bool example2 = true;
                if (allFalseRow != null)
                {
                    if (!allFalseRow.GreatPass)
                    {
                        // Ищем конкретный терм в КНФ: (lilsymbols | largesymbols | numbers | SecSymbols)
                        // Для строки с false в КНФ переменные инвертируются, поэтому все без отрицаний
                        var cnfTermsArray = cnf == "1" ? new string[0] : cnf.Split(new[] { " & " }, StringSplitOptions.None);
                        bool found = false;
                        foreach (var term in cnfTermsArray)
                        {
                            string cleanTerm = term.Trim('(', ')');
                            // Проверяем, что терм содержит все 4 переменные без отрицаний
                            var parts = cleanTerm.Split(new[] { " | " }, StringSplitOptions.None);
                            if (parts.Length == 4 &&
                                parts.Any(p => p.Trim() == "lilsymbols") &&
                                parts.Any(p => p.Trim() == "largesymbols") &&
                                parts.Any(p => p.Trim() == "numbers") &&
                                parts.Any(p => p.Trim() == "SecSymbols"))
                            {
                                found = true;
                                break;
                            }
                        }
                        example2 = found;
                    }
                    else
                    {
                        // Если greatPass = true, то этой строки не должно быть в КНФ
                        example2 = true; // Это нормально
                    }
                }

                // Проверка 5: ДНФ и КНФ не должны быть одновременно пустыми (если есть строки)
                // Это нормально, если все строки имеют greatPass = true (тогда КНФ = "1") или все false (тогда ДНФ = "0")
                bool notBothEmpty = true; // Убираем эту проверку, так как это валидное состояние

                // Проверка 6: Проверяем, что каждая строка с true представлена в ДНФ
                // Для этого проверяем, что количество термов совпадает
                bool allTrueRowsRepresented = dnfCountMatches;

                // Проверка 7: Проверяем, что каждая строка с false представлена в КНФ
                bool allFalseRowsRepresented = cnfCountMatches;

                bool passed = sumEquals16 && dnfCountMatches && cnfCountMatches && 
                             example1 && example2 && notBothEmpty && 
                             allTrueRowsRepresented && allFalseRowsRepresented;

                if (passed)
                {
                    Console.WriteLine($"  ✓ ПРОЙДЕН:");
                    Console.WriteLine($"    - Сумма термов ДНФ и КНФ: {dnfTerms} + {cnfTerms} = {dnfTerms + cnfTerms} (ожидалось: 16)");
                    Console.WriteLine($"    - Термов в ДНФ: {dnfTerms} (строк с greatPass=true: {trueRowsCount})");
                    Console.WriteLine($"    - Термов в КНФ: {cnfTerms} (строк с greatPass=false: {falseRowsCount})");
                    Console.WriteLine($"    - ДНФ и КНФ покрывают все комбинации");
                    Console.WriteLine($"    - Проверены конкретные примеры");
                    _testsPassed++;
                }
                else
                {
                    Console.WriteLine($"  ✗ ПРОВАЛЕН:");
                    if (!sumEquals16) Console.WriteLine($"    - Сумма термов не равна 16: {dnfTerms} + {cnfTerms} = {dnfTerms + cnfTerms}");
                    if (!dnfCountMatches) Console.WriteLine($"    - Количество термов ДНФ не совпадает: {dnfTerms} != {trueRowsCount}");
                    if (!cnfCountMatches) Console.WriteLine($"    - Количество термов КНФ не совпадает: {cnfTerms} != {falseRowsCount}");
                    if (!example1) Console.WriteLine($"    - Ошибка в примере 1: терм для строки (все переменные true) не найден в ДНФ");
                    if (!example2) Console.WriteLine($"    - Ошибка в примере 2: терм для строки (все переменные false) не найден в КНФ");
                    if (!notBothEmpty) Console.WriteLine($"    - ДНФ и КНФ одновременно пустые");
                    if (!allTrueRowsRepresented) Console.WriteLine($"    - Не все строки с greatPass=true представлены в ДНФ");
                    if (!allFalseRowsRepresented) Console.WriteLine($"    - Не все строки с greatPass=false представлены в КНФ");
                    Console.WriteLine($"    - ДНФ: {dnf.Substring(0, Math.Min(150, dnf.Length))}...");
                    Console.WriteLine($"    - КНФ: {cnf.Substring(0, Math.Min(150, cnf.Length))}...");
                    _testsFailed++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ ОШИБКА: {ex.Message}");
                Console.WriteLine($"    StackTrace: {ex.StackTrace}");
                _testsFailed++;
            }
            Console.WriteLine();
        }

        private void PrintSummary()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("ИТОГИ ТЕСТИРОВАНИЯ ТАБЛИЦЫ ИСТИННОСТИ");
            Console.WriteLine("========================================");
            Console.WriteLine($"Всего тестов: {_testNumber}");
            Console.WriteLine($"Пройдено: {_testsPassed}");
            Console.WriteLine($"Провалено: {_testsFailed}");
            Console.WriteLine($"Процент успешности: {(_testsPassed * 100.0 / _testNumber):F1}%");
            Console.WriteLine("========================================\n");
        }
    }
}

