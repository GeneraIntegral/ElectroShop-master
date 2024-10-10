using System.Data;  // Подключение пространства имен для работы с данными
using System.Data.SqlClient;  // Подключение пространства имен для работы с SQL Server
using System.Windows;  // Подключение пространства имен для работы с окнами WPF
using System.Windows.Controls;  // Подключение пространства имен для работы с элементами управления WPF
using System.Windows.Media;  // Подключение пространства имен для работы с графикой и цветами в WPF

namespace CourseProject // Определение пространства имен для проекта
{ 
    public partial class WindowCatalog : Window // Определение класса окна каталога, наследующего от класса Window
    {
        private CatalogViewModel _catalogViewModel; // Поле для хранения модели представления каталога
        public WindowCatalog(UserViewModel userViewModel) // Конструктор класса WindowCatalog, принимающий модель представления пользователя
        {
            InitializeComponent(); // Инициализация компонентов окна
            _catalogViewModel = new CatalogViewModel(userViewModel); // Создание экземпляра модели представления каталога с передачей модели пользователя
            DataContext = _catalogViewModel; // Установка контекста данных для привязки данных в XAML
        }

        private void AddToFavorites_Click(object sender, RoutedEventArgs e) // Обработчик события нажатия кнопки "Добавить в избранное"
        {
            DataGrid dataGrid = ProductDataGrid; // Получение ссылки на элемент управления DataGrid с продуктами

            if (dataGrid.SelectedItem is Product selectedProduct) // Проверка, выбран ли продукт в DataGrid
            {
                
                if (_catalogViewModel.UserViewModel != null) // Проверка, установлен ли UserViewModel
                {
                    AddProductToFavoritesAsync(selectedProduct, _catalogViewModel.UserViewModel.CurrentUserId); // Вызов асинхронного метода для добавления продукта в избранное
                }
                else
                {
                    errors.Text = "UserViewModel is not set."; // Вывод сообщения об ошибке, если UserViewModel не установлен
                    errors.Foreground = Brushes.Red; // Установка красного цвета текста для сообщения об ошибке
                }
            }
            else
            {
                errors.Text = "Please select a product first."; // Вывод сообщения об ошибке, если продукт не выбран
                errors.Foreground = Brushes.Red; // Установка красного цвета текста для сообщения об ошибке
            }
        }
        private async Task<bool> AddProductToFavoritesAsync(Product product, int userId) // Асинхронный метод для добавления продукта в избранное
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; // Получение строки подключения из конфигурации

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) // Создание подключения к базе данных
                {
                    await connection.OpenAsync(); // Асинхронное открытие подключения к базе данных
                    // SQL-запрос для проверки, есть ли уже продукт в избранном у данного пользователя
                    string checkQuery = @"SELECT COUNT(*) FROM Favorites WHERE ProductsId = @ProductsId AND UsersId = @UsersId";
                    // Создание команды SQL для выполнения запроса
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        // Добавление параметров к команде проверки наличия продукта в избранном
                        checkCommand.Parameters.AddWithValue("@ProductsId", product.Id); // Установка ID продукта в параметр запроса
                        checkCommand.Parameters.AddWithValue("@UsersId", userId); // Установка ID пользователя в параметр запроса
                        // Выполнение асинхронного запроса для получения количества записей, соответствующих условиям
                        int exists = (int)await checkCommand.ExecuteScalarAsync(); // Получение результата выполнения запроса
                        // Проверка, существует ли уже продукт в избранном
                        if (exists > 0)
                        {
                            errors.Text = "His product is already in favorites.."; // Сообщение об ошибке, если продукт уже в избранном
                            errors.Foreground = Brushes.Red; // Установка красного цвета текста для сообщения об ошибке
                            return false; // Завершение метода с возвращением значения false
                        }
                    }

                    // SQL-запрос для добавления продукта в избранное
                    string insertQuery = @"INSERT INTO Favorites (ProductsId, UsersId) VALUES (@ProductsId, @UsersId)";
                    // Создание команды SQL для выполнения вставки
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        // Добавление параметров к команде вставки
                        insertCommand.Parameters.AddWithValue("@ProductsId", product.Id); // Установка ID продукта
                        insertCommand.Parameters.AddWithValue("@UsersId", userId); // Установка ID пользователя
                        // Выполнение асинхронной команды вставки и получение количества затронутых строк
                        int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                        errors.Text = "The product has been added to favorites"; // Сообщение об успешном добавлении продукта
                        errors.Foreground = Brushes.GreenYellow; // Установка зеленого цвета текста для сообщения об успехе
                        return rowsAffected > 0; // Возвращение true, если вставка прошла успешно
                    }
                }
            }

            catch (SqlException sqlEx) // Обработка исключений SQL
            {
                errors.Text = "SQL error when adding a product to favorites"; // Сообщение об ошибке SQL
                errors.Foreground = Brushes.Red; // Установка красного цвета текста для сообщения об ошибке
                return false; // Завершение метода с возвращением значения false
            }
            catch (Exception ex) // Обработка других исключений
            {
                errors.Text = "Error when adding a product to favorites"; // Общее сообщение об ошибке
                errors.Foreground = Brushes.Red; // Установка красного цвета текста для сообщения об ошибке
                return false; // Завершение метода с возвращением значения false
            }
        }
        private void CategorySelectionChanged(object sender, SelectionChangedEventArgs e) // Обработчик события изменения выбора категории в элементе управления
        {
            if (DataContext is CatalogViewModel viewModel) // Проверка, является ли DataContext экземпляром CatalogViewModel
            {
                viewModel.LoadProducts(); // Вызов метода для загрузки продуктов в зависимости от выбранной категории
            }
        }
        private void Button_Click_Exit(object sender, RoutedEventArgs e) // Обработчик события нажатия кнопки "Выход"
        {
            WindowMainMenu windowMainMenu = new WindowMainMenu(_catalogViewModel.UserViewModel); // Создание нового окна главного меню с передачей модели представления пользователя
            windowMainMenu.Show(); // Отображение нового окна главного меню
            this.Close(); // Закрытие текущего окна  
        }
  
    }
}
