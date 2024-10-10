using System;  // Подключение пространства имен для работы с основными типами и функциями
using System.Collections.Generic;  // Подключение пространства имен для работы с коллекциями
using System.Configuration;  // Подключение пространства имен для работы с конфигурационными файлами
using System.Data.SqlClient;  // Подключение пространства имен для работы с SQL Server
using System.Data.SqlTypes;  // Подключение пространства имен для работы с SQL типами данных
using System.Linq;  // Подключение пространства имен для LINQ
using System.Text;  // Подключение пространства имен для работы со строками
using System.Threading.Tasks;  // Подключение пространства имен для асинхронного программирования
using System.Windows;  // Подключение пространства имен для WPF приложений
using System.Windows.Controls;  // Подключение пространства имен для управления элементами управления WPF
using System.Windows.Data;  // Подключение пространства имен для привязки данных в WPF
using System.Windows.Documents;  // Подключение пространства имен для работы с документами в WPF
using System.Windows.Input;  // Подключение пространства имен для обработки ввода в WPF
using System.Windows.Media;  // Подключение пространства имен для работы с графикой в WPF
using System.Windows.Media.Imaging;  // Подключение пространства имен для работы с изображениями в WPF
using System.Windows.Shapes;  // Подключение пространства имен для работы с фигурами в WPF

namespace CourseProject // Определение пространства имен для проекта
{
    public partial class WindowAccount : Window // Определение класса окна аккаунта, наследующего от Window
    {
        private UserViewModel userViewModel; // Хранит модель представления пользователя
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; // Получение строки подключения из конфигурационного файла

        public WindowAccount(UserViewModel userViewModel) // Конструктор класса, принимающий модель представления пользователя
        {
            InitializeComponent(); // Инициализация компонентов окна
            this.userViewModel = userViewModel; // Присвоение переданной модели представления полю класса
            DataContext = this.userViewModel; // Установка контекста данных для привязки данных в XAML
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e) // Обработчик события нажатия кнопки "Выход"
        {
            WindowMainMenu windowMainMenu = new WindowMainMenu(userViewModel); // Создание нового окна главного меню, передавая модель представления
            windowMainMenu.Show(); // Отображение нового окна
            this.Close(); // Закрытие текущего окна
        }

        private void Button_Click_ChangedTheData(object sender, RoutedEventArgs e) // Обработчик события нажатия кнопки "Изменить данные"
        {
            string newFirstName = userFirstName.Text; // Получение нового имени пользователя из текстового поля
            string newLastName = userLastName.Text; // Получение новой фамилии пользователя из текстового поля
            string newEmail = userEmail.Text; // Получение нового email из текстового поля
            string newCountry = userCountry.Text; // Получение новой страны из текстового поля
            DateTime newDateOfBirth = userViewModel.User.DateOfBirth; // Получение даты рождения пользователя из модели представления
            string newLogin = userLogin.Text;

            // Проверка, была ли выбрана дата в элементе выбора даты
            if (datePicker.SelectedDate.HasValue)
            {
                // Если выбрана, присваиваем новое значение даты рождения
                newDateOfBirth = datePicker.SelectedDate.Value;
            }
            else if (!string.IsNullOrWhiteSpace(userDateofBirth.Text)) // Если поле даты рождения не пустое
            {
                if (DateTime.TryParse(userDateofBirth.Text, out DateTime parsedDate)) // Пытаемся распарсить строку в дату
                {
                    // Проверяем, находится ли распарсенная дата в допустимом диапазоне SQL Server
                    if (parsedDate >= SqlDateTime.MinValue.Value && parsedDate <= SqlDateTime.MaxValue.Value)
                    {
                        newDateOfBirth = parsedDate; // Присваиваем новое значение даты рождения
                    }
                    else
                    {
                        MessageBox.Show("Указана неверная дата."); // Сообщаем о недопустимой дате
                        return; // Выход из метода, если дата недопустима
                    } 
                }
            }

            // Проверка, находится ли новая дата рождения в допустимом диапазоне SQL Server
            if (newDateOfBirth < SqlDateTime.MinValue.Value || newDateOfBirth > SqlDateTime.MaxValue.Value)
            {
                MessageBox.Show("Диапазон даты рождения должен быть от 1/1/1753 до 12/31/9999."); // Сообщаем о недопустимой дате рождения
                return; // Выход из метода, если дата недопустима
            }

            try // Начало блока обработки исключений
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) // Создание подключения к базе данных с использованием строки подключения
                {
                    connection.Open(); // Открытие соединения с базой данных
                    // SQL - запрос для обновления данных пользователя в таблице Users
                    string updateSql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email=@Email, Country=@Country, DateOfBirth=@DateOfBirth, Login=@Login WHERE ID = @Id";

                    using (SqlCommand command = new SqlCommand(updateSql, connection)) // Создание команды для выполнения SQL-запроса
                    {
                        // Добавление параметров к команде для предотвращения SQL-инъекций
                        command.Parameters.AddWithValue("@FirstName", newFirstName);  // Установка нового имени пользователя
                        command.Parameters.AddWithValue("@LastName", newLastName);  // Установка новой фамилии пользователя
                        command.Parameters.AddWithValue("@Email", newEmail);  // Установка нового email пользователя
                        command.Parameters.AddWithValue("@Country", newCountry);  // Установка новой страны пользователя
                        command.Parameters.AddWithValue("@DateOfBirth", newDateOfBirth);  // Установка новой даты рождения пользователя
                        command.Parameters.AddWithValue("@Login", newLogin);  // Установка нового логина пользователя
                        command.Parameters.AddWithValue("@Id", userViewModel.User.Id);  // Установка ID пользователя для обновления

                        // Выполнение команды и получение количества затронутых строк
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0) // Если были изменены строки
                        {
                            errorChangeData.Text = "The data has been successfully changed"; // Сообщение об успешном изменении данных
                            errorChangeData.Foreground = Brushes.GreenYellow; // Установка цвета текста на зеленый
                        }
                        else // Если строки не были изменены
                        {
                            errorChangeData.Text = "Data modification error"; // Сообщение об ошибке изменения данных
                            errorChangeData.Foreground = Brushes.Red; // Установка цвета текста на красный
                        }
                    }
                }
            }
            catch (Exception ex) // Обработка исключений
            {
                MessageBox.Show($"Возникла ошибка: {ex.Message}"); // Показ сообщения об ошибке с текстом исключения
            }
        }
    }
}
    

