using System; // Подключение пространства имен для основных классов .NET
using System.Collections.Generic; // Подключение пространства имен для работы с коллекциями
using System.Collections.ObjectModel; // Подключение пространства имен для работы с наблюдаемыми коллекциями
using System.ComponentModel; // Подключение пространства имен для реализации интерфейса INotifyPropertyChanged
using System.Data.SqlClient; // Подключение пространства имен для работы с SQL Server
using System.Linq; // Подключение пространства имен для LINQ-запросов
using System.Text; // Подключение пространства имен для работы со строками
using System.Threading.Tasks; // Подключение пространства имен для работы с асинхронными задачами

namespace CourseProject // Объявление пространства имен для проекта
{
    public class UserViewModel : INotifyPropertyChanged // Определение класса модели представления пользователя, реализующего интерфейс INotifyPropertyChanged
    {
        private User _user; // Поле для хранения экземпляра класса User

        public User User // Свойство User с механизмом уведомления об изменении
        {
            get { return _user; } // Геттер возвращает текущее значение _user
            set
            {
                if (_user != value) // Проверка на изменение значения
                {
                    _user = value; // Присвоение нового значения
                    OnPropertyChanged(nameof(User)); // Уведомление об изменении свойства
                }
            }
        }
        public int CurrentUserId { get; set; } // Свойство для хранения идентификатора текущего пользователя

        public UserViewModel() // Конструктор класса UserViewModel
        {
            User = new User(); // Инициализация нового экземпляра класса User
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое вызывается при изменении свойств
        protected virtual void OnPropertyChanged(string propertyName) // Метод для вызова события PropertyChanged
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Вызов события, если есть подписчики
        }
    }
}
