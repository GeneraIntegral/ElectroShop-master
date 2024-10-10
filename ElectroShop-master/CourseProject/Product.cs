using System; // Подключение пространства имен для основных классов .NET
using System.Collections.Generic; // Подключение пространства имен для работы с коллекциями
using System.ComponentModel; // Подключение пространства имен для реализации интерфейса INotifyPropertyChanged
using System.Linq; // Подключение пространства имен для LINQ-запросов
using System.Text; // Подключение пространства имен для работы со строками
using System.Threading.Tasks; // Подключение пространства имен для работы с асинхронными задачами

namespace CourseProject // Объявление пространства имен для проекта
{
    // Определение класса Product, реализующего интерфейс INotifyPropertyChanged для уведомления об изменениях свойств
    public class Product : INotifyPropertyChanged
    {
        public int Id { get; set; } // Идентификатор продукта
        public string Name { get; set; } // Название продукта
        public string ImagePath { get; set; } // Путь к изображению продукта
        public decimal Price { get; set; } // Цена продукта
        public double Weight { get; set; } // Вес продукта
        public string Description { get; set; } // Описание продукта
        public int CategoryId { get; set; } // Идентификатор категории, к которой относится продукт

        private bool _isFavorite; // Поле для хранения состояния "избранного" продукта
        public bool IsFavorite // Свойство для доступа к состоянию "избранного"
        {
            get => _isFavorite; // Геттер возвращает текущее значение _isFavorite
            set // Сеттер для изменения значения IsFavorite
            {
                _isFavorite = value; // Присвоение нового значения
                OnPropertyChanged(nameof(IsFavorite)); // Уведомление об изменении свойства IsFavorite
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое вызывается при изменении свойств
        protected void OnPropertyChanged(string propertyName) // Метод для вызова события PropertyChanged
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Вызов события, если есть подписчики
        }
    }
}
