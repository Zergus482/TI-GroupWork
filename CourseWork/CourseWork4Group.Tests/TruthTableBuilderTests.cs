using CourseWork4Group.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CourseWork4Group.Tests
{
    /// <summary>
    /// Юнит-тесты для построителя таблицы истинности, ДНФ и КНФ
    /// Переделаны из оригинальных тестов в Logic/TruthTableTests.cs
    /// </summary>
    public class TruthTableBuilderTests
    {
        private readonly TruthTableBuilder _builder;

        public TruthTableBuilderTests()
        {
            _builder = new TruthTableBuilder();
        }

        /// <summary>
        /// Тест 1: Проверка корректности построения таблицы истинности
        /// </summary>
        [Fact]
        public void TestTruthTableCorrectness()
        {
            // Arrange & Act
            var table = _builder.BuildTruthTable();

            // Assert
            // Проверка 1: Количество строк должно быть 16
            Assert.Equal(16, table.Count);

            // Проверка 2: Все комбинации должны быть уникальными
            var combinations = new HashSet<string>();
            foreach (var row in table)
            {
                string key = $"{row.LilsymbolsValue}{row.LargesymbolsValue}{row.NumbersValue}{row.SecSymbolsValue}";
                combinations.Add(key);
            }
            Assert.Equal(16, combinations.Count);

            // Проверка 3: Проверка правильности вычисления формулы для конкретных случаев
            // Случай 1: Все false -> результат false
            var allFalseRow = table.FirstOrDefault(r =>
                !r.Lilsymbols && !r.Largesymbols && !r.Numbers && !r.SecSymbols);
            Assert.NotNull(allFalseRow);
            Assert.False(allFalseRow.GreatPass);

            // Случай 2: lilsymbols=true, largesymbols=true, numbers=true -> результат true
            var case2Row = table.FirstOrDefault(r =>
                r.Lilsymbols && r.Largesymbols && r.Numbers && !r.SecSymbols);
            Assert.NotNull(case2Row);
            Assert.True(case2Row.GreatPass);

            // Случай 3: lilsymbols=true, largesymbols=true, SecSymbols=true -> результат true
            var case3Row = table.FirstOrDefault(r =>
                r.Lilsymbols && r.Largesymbols && !r.Numbers && r.SecSymbols);
            Assert.NotNull(case3Row);
            Assert.True(case3Row.GreatPass);

            // Случай 4: lilsymbols=false -> результат false
            var case4Row = table.FirstOrDefault(r =>
                !r.Lilsymbols && r.Largesymbols && r.Numbers && r.SecSymbols);
            Assert.NotNull(case4Row);
            Assert.False(case4Row.GreatPass);

            // Проверка 4: Проверка формулы для всех строк
            foreach (var row in table)
            {
                bool expected = row.Lilsymbols && row.Largesymbols && (row.Numbers || row.SecSymbols);
                Assert.Equal(expected, row.GreatPass);
            }
        }

        /// <summary>
        /// Тест 2: Проверка структуры и корректности ДНФ
        /// </summary>
        [Fact]
        public void TestDNFStructure()
        {
            // Arrange & Act
            var table = _builder.BuildTruthTable();
            string dnf = _builder.GetDNF(table);

            // Подсчет строк с greatPass = true
            int trueRowsCount = table.Count(r => r.GreatPass);

            // Assert
            // Проверка 1: ДНФ не должна быть пустой, если есть строки с true
            bool notEmpty = trueRowsCount > 0 ? !string.IsNullOrEmpty(dnf) : (dnf == "0" || string.IsNullOrEmpty(dnf));
            Assert.True(notEmpty);

            // Проверка 2: Подсчет термов в ДНФ (разделены символом "|")
            int termCount = dnf == "0" ? 0 : dnf.Split(new[] { " | " }, StringSplitOptions.None).Length;
            bool correctTermCount = termCount == trueRowsCount || (trueRowsCount == 0 && dnf == "0");
            Assert.True(correctTermCount);

            // Проверка 3: Каждый терм должен содержать 4 переменные
            if (dnf != "0" && !string.IsNullOrEmpty(dnf))
            {
                var terms = dnf.Split(new[] { " | " }, StringSplitOptions.None);
                foreach (var term in terms)
                {
                    string cleanTerm = term.Trim('(', ')');
                    int varCount = cleanTerm.Split(new[] { " & " }, StringSplitOptions.None).Length;
                    Assert.Equal(4, varCount);
                }
            }

            // Проверка 4: ДНФ должна содержать правильные переменные
            if (dnf != "0" && !string.IsNullOrEmpty(dnf))
            {
                Assert.Contains("lilsymbols", dnf);
                Assert.Contains("largesymbols", dnf);
                Assert.Contains("numbers", dnf);
                Assert.Contains("SecSymbols", dnf);
            }

            // Проверка 5: Структура ДНФ (термы в скобках, разделены "|")
            bool correctStructure = dnf == "0" ||
                (dnf.Contains("(") && dnf.Contains(")") && dnf.Contains(" | "));
            Assert.True(correctStructure);
        }

        /// <summary>
        /// Тест 3: Проверка структуры и корректности КНФ
        /// </summary>
        [Fact]
        public void TestCNFStructure()
        {
            // Arrange & Act
            var table = _builder.BuildTruthTable();
            string cnf = _builder.GetCNF(table);

            // Подсчет строк с greatPass = false
            int falseRowsCount = table.Count(r => !r.GreatPass);

            // Assert
            // Проверка 1: КНФ не должна быть пустой, если есть строки с false
            bool notEmpty = falseRowsCount > 0 ? !string.IsNullOrEmpty(cnf) : (cnf == "1" || string.IsNullOrEmpty(cnf));
            Assert.True(notEmpty);

            // Проверка 2: Подсчет термов в КНФ (разделены символом " & ")
            int termCount = cnf == "1" ? 0 : cnf.Split(new[] { " & " }, StringSplitOptions.None).Length;
            bool correctTermCount = termCount == falseRowsCount || (falseRowsCount == 0 && cnf == "1");
            Assert.True(correctTermCount);

            // Проверка 3: Каждый терм должен содержать 4 переменные
            if (cnf != "1" && !string.IsNullOrEmpty(cnf))
            {
                var terms = cnf.Split(new[] { " & " }, StringSplitOptions.None);
                foreach (var term in terms)
                {
                    string cleanTerm = term.Trim('(', ')');
                    int varCount = cleanTerm.Split(new[] { " | " }, StringSplitOptions.None).Length;
                    Assert.Equal(4, varCount);
                }
            }

            // Проверка 4: КНФ должна содержать правильные переменные
            if (cnf != "1" && !string.IsNullOrEmpty(cnf))
            {
                Assert.Contains("lilsymbols", cnf);
                Assert.Contains("largesymbols", cnf);
                Assert.Contains("numbers", cnf);
                Assert.Contains("SecSymbols", cnf);
            }

            // Проверка 5: Структура КНФ (термы в скобках, разделены " & ")
            bool correctStructure = cnf == "1" ||
                (cnf.Contains("(") && cnf.Contains(")") && cnf.Contains(" & "));
            Assert.True(correctStructure);
        }

        /// <summary>
        /// Тест 4: Проверка эквивалентности ДНФ и КНФ таблице истинности
        /// </summary>
        [Fact]
        public void TestDNFCNFEquivalence()
        {
            // Arrange & Act
            var table = _builder.BuildTruthTable();
            string dnf = _builder.GetDNF(table);
            string cnf = _builder.GetCNF(table);

            int trueRowsCount = table.Count(r => r.GreatPass);
            int falseRowsCount = table.Count(r => !r.GreatPass);

            // Assert
            // Проверка 1: Сумма термов ДНФ и КНФ должна равняться 16
            int dnfTerms = dnf == "0" ? 0 : dnf.Split(new[] { " | " }, StringSplitOptions.None).Length;
            int cnfTerms = cnf == "1" ? 0 : cnf.Split(new[] { " & " }, StringSplitOptions.None).Length;
            Assert.Equal(16, dnfTerms + cnfTerms);

            // Проверка 2: Количество термов ДНФ должно равняться количеству строк с true
            bool dnfCountMatches = dnfTerms == trueRowsCount || (trueRowsCount == 0 && dnf == "0");
            Assert.True(dnfCountMatches);

            // Проверка 3: Количество термов КНФ должно равняться количеству строк с false
            bool cnfCountMatches = cnfTerms == falseRowsCount || (falseRowsCount == 0 && cnf == "1");
            Assert.True(cnfCountMatches);

            // Проверка 4: Проверка конкретных примеров
            // Пример 1: Строка где все true -> должна быть в ДНФ (если greatPass = true)
            var allTrueRow = table.FirstOrDefault(r =>
                r.Lilsymbols && r.Largesymbols && r.Numbers && r.SecSymbols);
            
            if (allTrueRow != null && allTrueRow.GreatPass)
            {
                var dnfTermsArray = dnf == "0" ? new string[0] : dnf.Split(new[] { " | " }, StringSplitOptions.None);
                bool found = false;
                
                foreach (var term in dnfTermsArray)
                {
                    string cleanTerm = term.Trim('(', ')');
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
                
                Assert.True(found);
            }

            // Пример 2: Строка где все false -> должна быть в КНФ (если greatPass = false)
            var allFalseRow = table.FirstOrDefault(r =>
                !r.Lilsymbols && !r.Largesymbols && !r.Numbers && !r.SecSymbols);
            
            if (allFalseRow != null && !allFalseRow.GreatPass)
            {
                var cnfTermsArray = cnf == "1" ? new string[0] : cnf.Split(new[] { " & " }, StringSplitOptions.None);
                bool found = false;
                
                foreach (var term in cnfTermsArray)
                {
                    string cleanTerm = term.Trim('(', ')');
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
                
                Assert.True(found);
            }
        }
    }
}
