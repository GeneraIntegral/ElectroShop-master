using System.Configuration; // Подключение пространства имен для работы с конфигурационными файлами
using System.Windows; // Подключение пространства имен для работы с WPF
using System.Windows.Media; // Подключение пространства имен для работы с графикой и цветами в WPF
using System.Data.SqlClient; // Подключение пространства имен для работы с SQL Server
using System.Security.Cryptography; // Подключение пространства имен для работы с криптографией
using System.Text; // Подключение пространства имен для работы со строками
using System.ComponentModel.DataAnnotations; // Подключение пространства имен для валидации данных
using System.Windows.Controls; // Подключение пространства имен для работы с элементами управления WPF

namespace CourseProject // Объявление пространства имен для проекта
{
    public partial class WindowRegistration : Window // Объявление класса окна регистрации, наследующего от Window
    {
        public string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; // Получение строки подключения из конфигурационного файла

        public WindowRegistration() // Конструктор класса
        {
            InitializeComponent(); // Инициализация компонентов окна
            this.DataContext = new UserViewModel(); // Установка контекста данных для привязки данных к элементам управления
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e) // Обработчик события нажатия кнопки "Выход"
        {
            MainWindow mainWindow = new MainWindow(); // Создание нового экземпляра главного окна
            this.Close(); // Закрытие текущего окна регистрации
            mainWindow.Show(); // Отображение главного окна
        }

        private void Button_Click_Registration(object sender, RoutedEventArgs e) // Обработчик события нажатия кнопки "Регистрация"
        {
            UserViewModel viewModel = DataContext as UserViewModel; // Привязка контекста данных к переменной viewModel
            if (viewModel == null) // Проверка, что контекст данных успешно установлен
            {
                return; // Выход из метода, если контекст данных не установлен
            }

            var validationContext = new ValidationContext(viewModel.User, serviceProvider: null, items: null); // Создание контекста валидации для пользователя
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>(); // Список результатов валидации
            bool isValid = Validator.TryValidateObject(viewModel.User, validationContext, validationResults, validateAllProperties: true); // Проверка валидности объекта пользователя

            if (isValid) // Если объект валиден
            {
                UserSaveData(); // Сохранение данных пользователя
                ResetErrorColors(); // Сброс цветов ошибок
            }
            else // Если объект не валиден
            {
                ResetErrorColors(); // Сброс цветов ошибок перед выводом новых сообщений об ошибках
                foreach (var result in validationResults) // Перебор результатов валидации
                {
                    // Проверка каждого поля на наличие ошибок и установка соответствующего сообщения об ошибке и цвета текста
                    if (result.MemberNames.Contains(nameof(User.FirstName)))
                    {
                        userFirstNameError.Text = result.ErrorMessage; // Установка сообщения об ошибке для имени
                        userFirstNameError.Foreground = Brushes.Red; // Установка красного цвета текста для имени
                    }
                    if (result.MemberNames.Contains(nameof(User.LastName)))
                    {
                        userLastNameError.Text = result.ErrorMessage; // Установка сообщения об ошибке для фамилии
                        userLastNameError.Foreground = Brushes.Red; // Установка красного цвета текста для фамилии
                    }
                    if (result.MemberNames.Contains(nameof(User.Email)))
                    {
                        userEmailError.Text = result.ErrorMessage; // Установка сообщения об ошибке для email
                        userEmailError.Foreground = Brushes.Red; // Установка красного цвета текста для email
                    }
                    if (result.MemberNames.Contains(nameof(User.Country)))
                    {
                        userCountryError.Text = result.ErrorMessage; // Установка сообщения об ошибке для страны
                        userCountryError.Foreground = Brushes.Red; // Установка красного цвета текста для страны
                    }
                    if (result.MemberNames.Contains(nameof(User.Login)))
                    {
                        userLoginError.Text = result.ErrorMessage; // Установка сообщения об ошибке для логина
                        userLoginError.Foreground = Brushes.Red; // Установка красного цвета текста для логина
                    }
                    if (result.MemberNames.Contains(nameof(User.Password)))
                    {
                        userPasswordError.Text = result.ErrorMessage; // Установка сообщения об ошибке для пароля
                        userPasswordError.Foreground = Brushes.Red; // Установка красного цвета текста для пароля
                    }
                }
            }
        }

        private void userPassword_PasswordChanged(object sender, RoutedEventArgs e) // Обработчик события изменения пароля
        {
            PasswordBox passwordBox = sender as PasswordBox; // Привязка отправителя события к элементу управления PasswordBox
            UserViewModel viewModel = DataContext as UserViewModel; // Привязка контекста данных к переменной viewModel
            if (viewModel != null) // Проверка, что контекст данных успешно установлен
            {
                viewModel.User.Password = passwordBox.Password; // Сохранение введенного пароля в модель пользователя
            }
        }

        private void UserSaveData() // Метод для сохранения данных пользователя
        {
            string firstName = userFirstName.Text; // Получение текста из поля имени
            string lastName = userLastName.Text; // Получение текста из поля фамилии
            string email = userEmail.Text; // Получение текста из поля email
            string country = userCountry.Text; // Получение текста из поля страны
            DateTime selectedDate = datePicker.SelectedDate ?? DateTime.MinValue; // Получение выбранной даты или минимального значения, если дата не выбрана
            string login = userLogin.Text; // Получение текста из поля логина
            string password = userPassword.Password; // Получение введенного пароля
            string passwordRepeat = userPasswordRepeat.Password; // Получение повторно введенного пароля

            // Проверка, что все поля заполнены
            if (string.IsNullOrWhiteSpace(firstName)
                || string.IsNullOrWhiteSpace(lastName)
                || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(login)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(passwordRepeat))
            {
                errorDataRepeat.Text = "All fields must be filled!"; // Установка сообщения об ошибке, если поля пустые
                errorDataRepeat.Foreground = Brushes.Red; // Установка красного цвета текста для сообщения об ошибке
                return; // Выход из метода
            }

            // Проверка, что пароли совпадают
            if (password != passwordRepeat)
            {
                errorDataRepeat.Text = "Passwords don't match!"; // Установка сообщения об ошибке, если пароли не совпадают
                errorDataRepeat.Foreground = Brushes.Red; // Установка красного цвета текста для сообщения об ошибке
                return; // Выход из метода
            }

            string hashedPassword = HashPassword(password); // Хеширование пароля перед сохранением

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) // Создание подключения к базе данных с использованием строки подключения
                {
                    connection.Open(); // Открытие подключения к базе данных
                                       // Здесь может быть код для выполнения SQL-запроса на сохранение данных пользователя в базе данных
                                       // SQL-запрос для вставки нового пользователя в таблицу Users
                    string query = "INSERT INTO Users (FirstName, LastName, Email, Login, Password, DateOfBirth, Country)" +
                                "VALUES (@FirstName, @LastName, @Email, @Login, @Password, @DateOfBirth, @Country)";

                    // Создание команды SQL с использованием запроса и подключения к базе данных
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Добавление параметров к команде для предотвращения SQL-инъекций
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", hashedPassword); // Хешированный пароль
                        command.Parameters.AddWithValue("@DateOfBirth", selectedDate); // Дата рождения
                        command.Parameters.AddWithValue("@Country", country); // Страна

                        // Выполнение команды и получение количества затронутых строк
                        int rowsAffected = command.ExecuteNonQuery();

                        // Проверка, был ли успешно добавлен пользователь
                        if (rowsAffected > 0)
                        {
                            successfulRegistration.Text = "Registration was successful!"; // Сообщение об успешной регистрации
                            successfulRegistration.Foreground = Brushes.GreenYellow; // Установка цвета текста на зеленый
                        }
                        else
                        {
                            successfulRegistration.Text = "Registration failed."; // Сообщение о неудачной регистрации
                            successfulRegistration.Foreground = Brushes.Red; // Установка цвета текста на красный
                        }
                    }
                }
            }
            catch (Exception ex) // Обработка исключений при работе с базой данных
            {
                MessageBox.Show($"Error: {ex.Message}"); // Вывод сообщения об ошибке
            }
        }
        // Метод для сброса цветов ошибок в полях ввода
        private void ResetErrorColors()
        {
            userFirstNameError.Text = string.Empty; // Сброс текста ошибки для имени
            userLastNameError.Text = string.Empty; // Сброс текста ошибки для фамилии
            userEmailError.Text = string.Empty; // Сброс текста ошибки для email
            userCountryError.Text = string.Empty; // Сброс текста ошибки для страны
            userLoginError.Text = string.Empty; // Сброс текста ошибки для логина
            userPasswordError.Text = string.Empty; // Сброс текста ошибки для пароля

            // Установка прозрачного цвета для текста ошибок
            userFirstNameError.Foreground = Brushes.Transparent;
            userLastNameError.Foreground = Brushes.Transparent;
            userEmailError.Foreground = Brushes.Transparent;
            userCountryError.Foreground = Brushes.Transparent;
            userLoginError.Foreground = Brushes.Transparent;
            userPasswordError.Foreground = Brushes.Transparent;
        }

        // Метод для хеширования пароля с использованием алгоритма SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create()) // Создание экземпляра SHA256
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password)); // Хеширование пароля в байты
                var builder = new StringBuilder(); // Строковый строитель для формирования хеш-строки

                // Преобразование байтов в шестнадцатеричную строку
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString(); // Возврат хешированного пароля как строки
            }
        }
    }
}
