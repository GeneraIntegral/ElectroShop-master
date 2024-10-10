using System; // Подключение пространства имен для работы с основными типами данных
using System.Collections.Generic; // Подключение для работы с коллекциями
using System.Collections.ObjectModel; // Подключение для работы с изменяемыми коллекциями
using System.Linq; // Подключение для использования LINQ (языка интегрированных запросов)
using System.Text; // Подключение для работы с текстом
using System.Threading.Tasks; // Подключение для работы с асинхронными операциями

namespace CourseProject // Определение пространства имен для классификации классов и других типов
{
    public class Category // Определение класса Category, представляющего категорию товаров
    {
        public int Id { get; set; } // Свойство для хранения уникального идентификатора категории
        public string Name { get; set; } // Свойство для хранения названия категории
    }
}
