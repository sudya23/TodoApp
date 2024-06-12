using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TodoApp
{
    public partial class MainWindow : Window
    {
        // список элементов фильтра - комбобокса
        public static string[] Filters { get; set; } = { "Все", "Актуальные", "Завершённые" };
        // наши туду
        public ObservableCollection<Todo> Data { get; set; } = new();

        // туду, которые мы привязываем к списку и к ним применяем фильтр
        public ObservableCollection<Todo> Filtered { get {
                var q = Data.Where(o => o.Text.Contains(searchField.Text, StringComparison.OrdinalIgnoreCase));
                if (filterBox.Text == "Актуальные") q = q.Where(o => !o.IsCompleted); 
                if (filterBox.Text == "Завершённые") q = q.Where(o => o.IsCompleted);
                return new(q);
            } }

        public MainWindow()
        {
            InitializeComponent();
            // таймер для обновления времени на экране
            DispatcherTimer timer = new DispatcherTimer { Interval = new(0, 0, 0,0,250) };
            timer.IsEnabled = true;
            timer.Tick += Timer_Tick;
            timer.Start();

            // загружаем данные с файла
            try
            {
                using (FileStream fs = new FileStream("data.json", FileMode.Open))
                    Data = JsonSerializer.Deserialize<ObservableCollection<Todo>>(fs) ?? new();
            }
            catch (Exception ex) { }

            // прикрепляем данные
            listBox.ItemsSource = Filtered;
            filterBox.ItemsSource = Filters;
            filterBox.SelectedIndex = 0;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // если есть туду у которого закончилось время, и они не завершено
            // то завершаем и показываем пользователю
            listBox.ItemsSource = Filtered;
            foreach (var item in Data)
            {
                if (item.TimeLeft == TimeSpan.Zero && !item.IsCompleted)
                {
                    MessageBox.Show(item.Text, "Время вышло:", MessageBoxButton.OK);
                    item.IsCompleted = true;
                }
            }
        }
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            // создаём туду, передаём в новое окно, и если успешно, то добавляем и обновляем
            var todo = new Todo { DateTime = DateTime.Now + new TimeSpan(0, 1, 0) };
            if (new TodoWindow(todo).ShowDialog() != true) return;
            Data.Add(todo);
            Save();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            // тоже самое что и в добавлении, но туду не создаём, а берём из кнопки
            var todo = ((Button)sender).DataContext as Todo;
            if (new TodoWindow(todo).ShowDialog() != true) return;
            todo.IsCompleted = false;
            Save();
        }


        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            // берём из кнопки, удаляем, обновляем
            var todo = ((Button)sender).DataContext as Todo;
            Data.Remove(todo);
            Save();
        }
        private void MarkCompleted(object sender, RoutedEventArgs e)
        {
            // берём из кнопки, помечаем как законченые, обновляем
            var todo = ((Button)sender).DataContext as Todo;
            todo.IsCompleted = true;
            Save();
        }
        void Save()
        {   // сохраняем данные в файл и обновляем список
            using (FileStream fs = new FileStream("data.json", FileMode.Create))
                JsonSerializer.Serialize(fs, Data);
            listBox.ItemsSource = Filtered;
        }
    }

    // перевести bool в видимость (для того чтобы спрятать кнопку)
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value == false ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();
    }
}