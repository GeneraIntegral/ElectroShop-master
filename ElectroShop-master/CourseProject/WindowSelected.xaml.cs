using System.Data.SqlClient; // Подключение пространства имен для работы с SQL Server
using System.Data; // Подключение пространства имен для работы с данными
using System.Windows; // Подключение пространства имен для работы с окнами WPF
using System.Windows.Controls; // Подключение пространства имен для управления контролами WPF
using System.Windows.Media; // Подключение пространства имен для работы с графикой и цветами в WPF

namespace CourseProject
{
    public partial class WindowSelected : Window // Определение класса окна
    {
        private UserViewModel userViewModel; // Переменная для хранения модели пользователя
        private Product _selectedProduct; // Переменная для хранения выбранного продукта

        public WindowSelected(UserViewModel userViewModel) // Конструктор класса
        {
            InitializeComponent(); // Инициализация компонентов окна
            this.userViewModel = userViewModel; // Сохранение модели пользователя
            FavoritesViewModel viewModel = new FavoritesViewModel(userViewModel.User.Id); // Создание модели избранного
            DataContext = viewModel; // Установка контекста данных для привязки
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e) // Метод обработки нажатия кнопки выхода
        {
            WindowMainMenu windowMainMenu = new WindowMainMenu(userViewModel); // Создание нового окна главного меню
            windowMainMenu.Show(); // Отображение нового окна
            this.Close(); // Закрытие текущего окна
        }

        private void ProductDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) // Обработка изменения выбора в DataGrid
        {
            _selectedProduct = ProductDataGrid.SelectedItem as Product; // Присвоение выбранного продукта
        }

        private async void Button_Click_Delete(object sender, RoutedEventArgs e) // Метод обработки нажатия кнопки удаления
        {
            if (_selectedProduct != null) // Если продукт выбран
            {
                await DeleteProductFromFavoritesAsync(_selectedProduct.Id); // Удаление продукта из избранного
                RefreshFavoritesList(); // Обновление списка избранного
            }
            else
            {
                errors.Text = "Please select the product to remove!"; // Вывод сообщения об ошибке
                errors.Foreground = Brushes.Red; // Установка цвета текста ошибки
            }
        }

        private async Task DeleteProductFromFavoritesAsync(int productId) // Асинхронный метод удаления продукта из избранного
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; // Получение строки подключения к базе данных
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) // Создание подключения к базе данных
                {
                    await connection.OpenAsync(); // Асинхронное открытие соединения
                    string query = @"DELETE FROM Favorites WHERE ProductsId = @ProductId"; // SQL-запрос для удаления продукта
                    using (SqlCommand command = new SqlCommand(query, connection)) // Создание SQL-команды
                    {
                        command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId }); // Добавление параметра
                        int rowsAffected = await command.ExecuteNonQueryAsync(); // Выполнение команды и получение количества затронутых строк
                        if (rowsAffected > 0) // Если строка успешно удалена
                        {
                            errors.Text = "The product has been successfully deleted!"; // Успешное сообщение
                            errors.Foreground = Brushes.GreenYellow; // Установка цвета успешного сообщения
                        }
                        else
                        {
                            errors.Text = "The product could not be deleted from favorites!"; // Сообщение об ошибке
                            errors.Foreground = Brushes.Red; // Установка цвета текста ошибки
                        }
                    }
                }
            }
            catch (SqlException sqlEx) // Обработка ошибок SQL
            {
                errors.Text = "SQL error when deleting a product from favorites!"; // Сообщение об ошибке
                errors.Foreground = Brushes.Red; // Установка цвета текста ошибки
            }
            catch (Exception ex) // Обработка других ошибок
            {
                errors.Text = "An error occurred when deleting a product from favorites!"; // Сообщение об ошибке
                errors.Foreground = Brushes.Red; // Установка цвета текста ошибки
            }
        }

        private void RefreshFavoritesList() // Метод для обновления списка избранного
        {
            if (DataContext is FavoritesViewModel viewModel) // Если контекст данных является моделью избранного
            {
                viewModel.LoadFavorites(); // Загрузка списка избранных продуктов
            }
        }
    }
}
