using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProject
{
    // Класс, представляющий модель представления каталога
    public class CatalogViewModel : INotifyPropertyChanged
    {
        // Коллекция категорий для отображения в интерфейсе
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        // Коллекция продуктов для отображения в интерфейсе
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        // Выбранная категория
        public Category SelectedCategory { get; set; }
        // Выбранный продукт
        public Product SelectedProduct { get; set; }
        // Модель представления пользователя
        public UserViewModel UserViewModel { get; }

        // Конструктор класса, принимающий модель представления пользователя
        public CatalogViewModel(UserViewModel userViewModel)
        {
            // Проверка на null для объекта userViewModel
            UserViewModel = userViewModel ?? throw new ArgumentNullException(nameof(userViewModel));
            // Загрузка категорий из базы данных
            LoadCategories();
        }

        // Метод для загрузки категорий из базы данных
        private void LoadCategories()
        {
            // Получаем строку подключения
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            // Создаем новое соединение с базой данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Открываем соединение
                connection.Open();
                // SQL-запрос для выбора категорий
                string query = "SELECT Id, Name FROM Categories";
                SqlCommand command = new SqlCommand(query, connection);
                // Создаем команду SQL
                SqlDataReader reader = command.ExecuteReader();
                // Выполняем запрос

                // Читаем данные из результата
                while (reader.Read())
                {
                    // Добавляем категории в коллекцию
                    Categories.Add(new Category
                    {
                        Id = reader.GetInt32(0),
                        // Читаем Id категории
                        Name = reader.GetString(1)
                        // Читаем название категории
                    });
                }
            }
        }

        // Метод для загрузки продуктов выбранной категории
        public void LoadProducts()
        {
            // Проверка, выбрана ли категория
            if (SelectedCategory == null) return;

            Products.Clear(); // Очищаем предыдущие продукты

            // Получаем строку подключения
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            // Создаем новое соединение с базой данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Открываем соединение
                string query = "SELECT Id, Name, ImagePath, Price, Weight, Description FROM Products WHERE CategoryId = @CategoryId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", SelectedCategory.Id);
                // Добавляем параметр для категории
                SqlDataReader reader = command.ExecuteReader();
                // Выполняем запрос

                // Читаем данные из результата
                while (reader.Read()) 
                {
                    // Добавляем продукты в коллекцию
                    Products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        // Читаем Id продукта
                        Name = reader.GetString(1),
                        // Читаем название продукта
                        ImagePath = reader.GetString(2),
                        // Читаем путь к изображению
                        Price = reader.GetDecimal(3),
                        // Читаем цену
                        Weight = reader.GetDouble(4),
                        // Читаем вес
                        Description = reader.GetString(5)
                        // Читаем описание
                    });
                }
            }
        }

        // Событие изменения свойства
        public event PropertyChangedEventHandler PropertyChanged;
        // Метод для вызова события изменения свойства
        protected void OnPropertyChanged(string name)
        {
            // Вызываем событие, если есть подписчики
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

