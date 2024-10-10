using System; // Подключение пространства имен для основных классов .NET
using System.Collections.Generic; // Подключение пространства имен для работы с коллекциями
using System.Collections.ObjectModel; // Подключение пространства имен для работы с наблюдаемыми коллекциями
using System.ComponentModel; // Подключение пространства имен для реализации интерфейса INotifyPropertyChanged
using System.Data.SqlClient; // Подключение пространства имен для работы с SQL Server
using System.Windows; // Подключение пространства имен для работы с WPF

// Объявление пространства имен для проекта
namespace CourseProject
{
    // Определение класса FavoritesViewModel, который управляет избранными продуктами пользователя
    public class FavoritesViewModel
    {
        // Коллекция избранных продуктов, которая уведомляет об изменениях в UI
        public ObservableCollection<Product> Favorites { get; set; } = new ObservableCollection<Product>();
        // Строка подключения к базе данных
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        // Идентификатор текущего пользователя
        private int _currentUserId;

        // Конструктор класса, принимающий идентификатор пользователя
        public FavoritesViewModel(int userId)
        {
            _currentUserId = userId; // Сохранение идентификатора пользователя
            LoadFavorites(); // Вызов метода для загрузки избранных продуктов
        }

        // Асинхронный метод для загрузки избранных продуктов из базы данных
        public async void LoadFavorites()
        {
            try
            {
                // Создание подключения к базе данных
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Асинхронное открытие соединения
                    await connection.OpenAsync();
                    string query = @"SELECT p.Id, p.Name, p.ImagePath, p.Price, p.Weight, p.Description, p.CategoryId 
                                 FROM Favorites f
                                 JOIN Products p ON f.ProductsId = p.Id
                                 WHERE f.UsersId = @UserId";
                    // SQL-запрос для получения избранных продуктов текущего пользователя

                    // Создание команды SQL с заданным запросом и соединением
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Добавление параметра для запроса
                        command.Parameters.AddWithValue("@UserId", _currentUserId);
                        // Асинхронное выполнение запроса и получение результата
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            // Очистка коллекции перед загрузкой новых данных
                            Favorites.Clear();
                            // Чтение данных построчно
                            while (await reader.ReadAsync())
                            {
                                // Создание нового объекта Product из считанных данных
                                Product product = new Product
                                {
                                    Id = reader.GetInt32(0),
                                    // Получение идентификатора продукта
                                    Name = reader.GetString(1),
                                    // Получение названия продукта
                                    ImagePath = reader.GetString(2),
                                    // Получение пути к изображению продукта
                                    Price = reader.GetDecimal(3),
                                    // Получение цены продукта
                                    Weight = reader.GetDouble(4),
                                    // Получение веса продукта
                                    Description = reader.GetString(5),
                                    // Получение описания продукта
                                    CategoryId = reader.GetInt32(6)
                                    // Получение идентификатора категории продукта
                                };
                               
                                // Добавление продукта в коллекцию избранных
                                Favorites.Add(product);
                            }
                        }
                    }
                }
            }
            // Обработка исключений при загрузке данных
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке пользователю
                MessageBox.Show($"Получена ошибка загрузки избранного: {ex.Message}");
            }
        }
        // Событие, которое вызывается при изменении свойств
        public event PropertyChangedEventHandler PropertyChanged;
        // Метод для вызова события PropertyChanged
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
