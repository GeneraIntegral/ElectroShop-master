using System.Configuration;  // Подключение пространства имен для работы с конфигурационными файлами
using System.Data.SqlClient;  // Подключение пространства имен для работы с SQL Server
using System.Text;  // Подключение пространства имен для работы с текстовыми кодировками
using System.Windows;  // Подключение пространства имен для работы с WPF
using System.Windows.Media;  // Подключение пространства имен для работы с графикой в WPF
using System.Security.Cryptography;  // Подключение пространства имен для работы с криптографией

// Определение пространства имен для проекта
namespace CourseProject
{
    // Определение класса MainWindow, наследующего от класса Window
    public partial class MainWindow : Window 
    {
        // Получение строки подключения из конфигурационного файла
        public string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        // Конструктор класса MainWindow
        public MainWindow()
        {
            // Инициализация компонентов окна
            InitializeComponent();
        }
        // Обработчик события нажатия кнопки "Выход"
        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            // Закрытие текущего окна
            this.Close();
        }
        // Обработчик события нажатия кнопки "Регистрация"
        private void Button_Click_Registration(object sender, RoutedEventArgs e)
        {
            // Создание нового окна регистрации
            WindowRegistration windowRegistration = new WindowRegistration();
            // Закрытие текущего окна
            this.Close();
            // Отображение окна регистрации
            windowRegistration.Show();
        }
        // Обработчик события нажатия кнопки "Вход"
        private void Button_Click_Log_in(object sender, RoutedEventArgs e)
        {
            // Получение логина из текстового поля
            string loginToCheck = userLoginToCheck.Text;
            // Получение пароля из поля ввода пароля
            string passwordToCheck = userPasswordToCheck.Password;

            try
            {
                // Создание подключения к базе данных с использованием строки подключения
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Открытие подключения к базе данных
                    connection.Open();
                    // SQL-запрос для получения ID и пароля пользователя по логину
                    string query = "SELECT ID, Password FROM Users WHERE Login=@Login";
                    // Идентификатор пользователя (по умолчанию -1)
                    int userId = -1;
                    // Хранит хэшированный пароль из базы данных
                    string hashedPasswordFromDatabase = null;

                    // Создание команды SQL
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Добавление параметра логина в запрос
                        command.Parameters.AddWithValue("@Login", loginToCheck);

                        // Выполнение запроса и получение данных
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Если есть результаты запроса
                            if (reader.Read())
                            {
                                // Получение ID пользователя
                                userId = reader.GetInt32(0);
                                // Получение хэшированного пароля из базы данных
                                hashedPasswordFromDatabase = reader.GetString(1);
                            }
                        }
                    }

                    // Проверка, найден ли пользователь и есть ли хэшированный пароль
                    if (userId != -1 && hashedPasswordFromDatabase != null)
                    {
                        // Хранит хэшированный пароль, введенный пользователем
                        string hashedPasswordToCheck;
                        // Создание объекта SHA256 для хэширования пароля
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            // Хэширование введенного пароля
                            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordToCheck));
                            // Преобразование хэша в строку в нижнем регистре
                            hashedPasswordToCheck = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                        }
                        
                        // Сравнение хэшированного пароля, введенного пользователем, с хэшированным паролем из базы данных
                        if (hashedPasswordToCheck == hashedPasswordFromDatabase)
                        {
                            // SQL-запрос для получения данных пользователя по его ID
                            string query1 = "SELECT FirstName, LastName, Email, Login, Password, DateOfBirth, Country FROM Users WHERE ID=@UserId";
                            // Создание модели представления пользователя с текущим ID
                            UserViewModel userViewModel = new UserViewModel { CurrentUserId = userId };
                            // Создание команды SQL для выполнения запроса
                            using (SqlCommand command1 = new SqlCommand(query1, connection))
                            {
                                // Добавление параметра ID пользователя в запрос
                                command1.Parameters.AddWithValue("@UserId", userId);
                               
                                // Выполнение запроса и получение данных
                                using (SqlDataReader reader1 = command1.ExecuteReader())
                                {
                                    // Если есть результаты запроса
                                    if (reader1.Read())
                                    {
                                        // Заполнение модели пользователя данными из базы данных
                                        userViewModel.User = new User
                                        {
                                            Id = userId,
                                            FirstName = reader1["FirstName"].ToString(),
                                            // Получение имени пользователя
                                            LastName = reader1["LastName"].ToString(),
                                            // Получение фамилии пользователя
                                            Email = reader1["Email"].ToString(),
                                            // Получение email пользователя
                                            Login = reader1["Login"].ToString(),
                                            // Получение логина пользователя
                                            Password = reader1["Password"].ToString(),
                                            // Получение пароля пользователя
                                            DateOfBirth = Convert.ToDateTime(reader1["DateOfBirth"]),
                                            // Получение даты рождения пользователя
                                            Country = reader1["Country"].ToString()
                                            // Получение страны пользователя
                                        };
                                    }
                                    else
                                    {
                                        // Если данные пользователя не найдены, выбрасываем исключение
                                        throw new Exception("Получена ошибка при получении данных пользователя.");
                                    }
                                }
                            }

                            // Создание и отображение главного меню с моделью представления пользователя
                            WindowMainMenu windowMainMenu = new WindowMainMenu(userViewModel);
                            // Отображение главного меню
                            windowMainMenu.Show();
                            // Закрытие текущего окна входа
                            this.Close();
                        }
                        else
                        {
                            // Если логин или пароль неверные, выводим сообщение об ошибке
                            errorData.Text = "Введен неверный логин или пароль";
                            // Установка красного цвета текста для сообщения об ошибке
                            errorData.Foreground = Brushes.Red;
                        }
                    }
                    else
                    {
                        // Если логин или пароль неверные, выводим сообщение об ошибке
                        errorData.Text = "Введен неверный логин или пароль";
                        // Установка красного цвета текста для сообщения об ошибке
                        errorData.Foreground = Brushes.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений: вывод сообщения об ошибке в случае возникновения исключения
                MessageBox.Show($"Получена ошибка: {ex.Message}");
            }
        }
    }