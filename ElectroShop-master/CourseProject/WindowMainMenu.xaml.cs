using System.Windows; // Подключение пространства имен для работы с окнами WPF
using System.Windows.Input; // Подключение пространства имен для работы с вводом

namespace CourseProject // Объявление пространства имен для проекта
{
    public partial class WindowMainMenu : Window // Определение класса главного меню, унаследованного от класса Window
    {
        private UserViewModel userViewModel; // Поле для хранения модели представления пользователя

        // Конструктор класса WindowMainMenu
        public WindowMainMenu(UserViewModel userViewModel)
        {
            InitializeComponent(); // Инициализация компонентов окна
            this.userViewModel = userViewModel; // Присвоение переданной модели представления локальному полю
            DataContext = this.userViewModel; // Установка контекста данных для привязки данных в XAML
        }

        // Обработчик события нажатия кнопки "Выйти"
        private void Button_Click_Logout(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); // Создание нового экземпляра главного окна
            mainWindow.Show(); // Отображение главного окна
            this.Close(); // Закрытие текущего окна
        }

        // Обработчик события нажатия кнопки "Аккаунт"
        private void Button_Click_Account(object sender, RoutedEventArgs e)
        {
            WindowAccount windowAccount = new WindowAccount(userViewModel); // Создание нового окна аккаунта с передачей модели представления
            windowAccount.Show(); // Отображение окна аккаунта
            this.Close(); // Закрытие текущего окна
        }

        // Обработчик события нажатия кнопки "Каталог"
        private void Button_Click_Catalog(object sender, RoutedEventArgs e)
        {
            WindowCatalog windowCatalog = new WindowCatalog(userViewModel); // Создание нового окна каталога с передачей модели представления
            windowCatalog.Show(); // Отображение окна каталога
            this.Close(); // Закрытие текущего окна
        }

        // Обработчик события нажатия кнопки "Выбранные"
        private void Button_Click_Selected(object sender, RoutedEventArgs e)
        {
            WindowSelected windowSelected = new WindowSelected(userViewModel); // Создание нового окна выбранных элементов с передачей модели представления
            windowSelected.Show(); // Отображение окна выбранных элементов
            this.Close(); // Закрытие текущего окна
        }
    }
}
