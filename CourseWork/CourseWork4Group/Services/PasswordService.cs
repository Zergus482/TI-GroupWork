using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using CourseWork4Group.Models;

namespace CourseWork4Group.Services
{
    public class PasswordService
    {
        private static readonly string StorageFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "passwords.json");
        private ObservableCollection<PasswordEntry> _passwords;

        public PasswordService()
        {
            _passwords = new ObservableCollection<PasswordEntry>();
            LoadPasswords();
        }

        public ObservableCollection<PasswordEntry> GetPasswords()
        {
            return _passwords;
        }

        public void AddPassword(PasswordEntry entry)
        {
            _passwords.Add(entry);
            SavePasswords();
        }

        public void RemovePassword(string id)
        {
            var entry = _passwords.FirstOrDefault(p => p.Id == id);
            if (entry != null)
            {
                _passwords.Remove(entry);
                SavePasswords();
            }
        }

        public void UpdatePassword(PasswordEntry updatedEntry)
        {
            var entry = _passwords.FirstOrDefault(p => p.Id == updatedEntry.Id);
            if (entry != null)
            {
                var index = _passwords.IndexOf(entry);
                _passwords[index] = updatedEntry;
                SavePasswords();
            }
        }

        private void LoadPasswords()
        {
            try
            {
                if (File.Exists(StorageFile))
                {
                    var json = File.ReadAllText(StorageFile);
                    var entries = JsonSerializer.Deserialize<List<PasswordEntry>>(json);
                    if (entries != null)
                    {
                        foreach (var entry in entries)
                        {
                            _passwords.Add(entry);
                        }
                    }
                }
            }
            catch
            {
                // Если не удалось загрузить, продолжаем с пустым списком
            }
        }

        private void SavePasswords()
        {
            try
            {
                var json = JsonSerializer.Serialize(_passwords.ToList(), new JsonSerializerOptions { WriteIndented = true });
                
                // Создаем временный файл для безопасной записи
                string tempFile = StorageFile + ".tmp";
                File.WriteAllText(tempFile, json);
                
                // Заменяем старый файл новым только после успешной записи
                if (File.Exists(StorageFile))
                {
                    File.Delete(StorageFile);
                }
                File.Move(tempFile, StorageFile);
            }
            catch (Exception ex)
            {
                // Выводим ошибку для отладки
                System.Windows.MessageBox.Show($"Ошибка сохранения паролей: {ex.Message}", 
                                              "Ошибка", 
                                              System.Windows.MessageBoxButton.OK, 
                                              System.Windows.MessageBoxImage.Error);
            }
        }
    }
}

