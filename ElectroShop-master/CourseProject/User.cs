using System; // Подключение пространства имен для основных классов .NET
using System.Collections.Generic; // Подключение пространства имен для работы с коллекциями
using System.ComponentModel; // Подключение пространства имен для реализации INotifyPropertyChanged
using System.ComponentModel.DataAnnotations; // Подключение пространства имен для использования атрибутов валидации данных
using System.Linq; // Подключение пространства имен для использования LINQ (языка интегрированных запросов)
using System.Text; // Подключение пространства имен для работы с текстом и кодировками
using System.Threading.Tasks; // Подключение пространства имен для использования асинхронного программирования
using System.Windows.Controls; // Подключение пространства имен для работы с элементами управления WPF

namespace CourseProject
{
    // Класс, представляющий пользователя и реализующий интерфейс INotifyPropertyChanged
    public class User : INotifyPropertyChanged
    {
        // Приватные поля для хранения значений свойств
        private int _id;
        private string? _firstname;
        private string? _lastname;
        private string? _email;
        private string? _login;
        private string? _password;
        private DateTime _dateOfBirth;
        private string? _country;

        // Свойство для доступа к Id пользователя
        public int Id
        {
            get { return _id; } // Возвращает значение Id
            set
            {
                if (_id != value) // Проверка на изменение значения
                {
                    _id = value; // Присваивание нового значения
                    OnPropertyChanged(nameof(Id)); // Уведомление об изменении свойства Id
                }
            }
        }

        // Атрибут Required указывает, что свойство является обязательным, а ErrorMessage задает сообщение об ошибке
        [Required(ErrorMessage = "Enter a fitstname!")]
        // Свойство для доступа к имени пользователя
        public string FirstName
        {
            get { return _firstname; } // Возвращает значение имени
            set
            {
                if (_firstname != value) // Проверка на изменение значения
                {
                    _firstname = value; // Присваивание нового значения
                    OnPropertyChanged(nameof(FirstName)); // Уведомление об изменении свойства FirstName
                }
            }
        }

        // Атрибут Required указывает, что свойство является обязательным, а ErrorMessage задает сообщение об ошибке
        [Required(ErrorMessage = "Enter a lastname!")]
        // Свойство для доступа к фамилии пользователя
        public string LastName
        {
            get { return _lastname; } // Возвращает значение фамилии
            set
            {
                if (_lastname != value) // Проверка на изменение значения
                {
                    _lastname = value; // Присваивание нового значения
                    OnPropertyChanged(nameof(LastName)); // Уведомление об изменении свойства LastName
                }
            }
        }

        // Атрибуты Required и EmailAddress для валидации обязательного поля email и проверки его формата
        [Required(ErrorMessage = "Enter email!")]
        [EmailAddress]
        / Свойство для доступа к email пользователя
        public string Email
        {
            get { return _email; } // Возвращает значение email
            set
            {
                if (_email != value) // Проверка на изменение значения
                {
                    _email = value; // Присваивание нового значения
                    OnPropertyChanged(nameof(Email)); // Уведомление об изменении свойства Email
                }
            }
        }

        [Required(ErrorMessage = "Enter login!")] // Атрибут валидации: обязательное поле для логина
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Login 3-20 symbol!")] // Ограничение длины логина (3-20 символов)
        public string Login // Свойство для доступа к логину пользователя
        {
            get { return _login; } // Возврат значения логина
            set // Установка нового значения логина
            {
                if (_login != value) // Проверка на изменение значения
                {
                    _login = value; // Установка нового значения
                    OnPropertyChanged(nameof(Login)); // Уведомление об изменении свойства
                }
            }
        }

        [Required(ErrorMessage = "Enter password!")] // Атрибут валидации: обязательное поле для пароля
        [StringLength(16, MinimumLength = 4, ErrorMessage = "Password 4-16 symbol!")] // Ограничение длины пароля (4-16 символов)
        public string Password // Свойство для доступа к паролю пользователя
        {
            get { return _password; } // Возврат значения пароля
            set // Установка нового значения пароля
            {
                if (_password != value) // Проверка на изменение значения
                {
                    _password = value; // Установка нового значения
                    OnPropertyChanged(nameof(Password)); // Уведомление об изменении свойства
                }
            }
        }
        public DateTime DateOfBirth // Свойство для доступа к дате рождения пользователя
        {
            get { return _dateOfBirth; } // Возврат значения даты рождения
            set // Установка новой даты рождения
            {
                if (_dateOfBirth != value) // Проверка на изменение значения
                {
                    _dateOfBirth = value; // Установка нового значения
                    OnPropertyChanged(nameof(DateOfBirth)); // Уведомление об изменении свойства
                    OnPropertyChanged(nameof(Age)); // Уведомление об изменении свойства возраста (пересчитывается при изменении даты рождения)
                }
            }
        }
        public int Age // Свойство для вычисления возраста пользователя на основе даты рождения
        {
            get
            {
                var today = DateTime.Today; // Получение текущей даты
                int age = today.Year - DateOfBirth.Year; // Вычисление возраста по годам
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age; // Возврат вычисленного возраста
            }
        }

        [Required(ErrorMessage = "Enter a country!")] // Атрибут валидации: обязательное поле для страны пользователя
        public string Country // Свойство для доступа к стране пользователя
        {
            get { return _country; } // Возврат значения страны
            set // Установка нового значения страны
            {
                if (_country != value) // Проверка на изменение значения
                {
                    _country = value; // Установка нового значения
                    OnPropertyChanged(nameof(Country)); // Уведомление об изменении свойства
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged; // Событие, уведомляющее об изменениях свойств
        protected virtual void OnPropertyChanged(string propertyName) // Метод для вызова события PropertyChanged
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Вызов события с указанием имени измененного свойства
        }
    }
}
