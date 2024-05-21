using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
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
using To_Do_App.Controllers;
using To_Do_App.Entities;
using To_Do_App.Pages;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace To_Do_App
{

    public partial class MainPage : Page
    {
        List<TaskItem> taskItems = new List<TaskItem>();
        Timer timer;

        public MainPage()
        {
            InitializeComponent();
            FilterOption.Text = "All tasks";
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            var navigatedPage = e.Content as Page;

            if (navigatedPage is MainPage)
            {
                FilterTasks(SearchBox.Text);
                TaskList.Items.Refresh();
            }
        }

        private void FilterOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTasks(SearchBox.Text);
        }

        private void FilterTasks(string text)
        {
            List<TaskItem> list = FilterByKeyword(taskItems, text);
            if (list == null)
            {
                MessageBox.Show("Unable to retrieve data from API.");
                return;
            }

            string selectedItem = (string)((ComboBoxItem)FilterOption.SelectedItem).Content;

            if (selectedItem == "Complete tasks")
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Completed == false)
                        list.RemoveAt(i);
                }
            }
            else if (selectedItem == "Incomplete tasks")
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Completed)
                        list.RemoveAt(i);
                }
            }
            else if (selectedItem == "Flagged tasks")
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Flagged == false)
                        list.RemoveAt(i);
                }
            }
            else if (selectedItem == "Unflagged tasks")
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Flagged)
                        list.RemoveAt(i);
                }
            }
            else if (selectedItem == "With due date")
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (DateOnly.MinValue == list[i].DueDate)
                        list.RemoveAt(i);
                }
            }
            TaskList.ItemsSource = list;
        }

        private void TaskList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = (TaskItem)TaskList.SelectedItem;
            if (e.ChangedButton == MouseButton.Left)
            {
                if(item != null)
                {
                    item.Completed = !item.Completed;
                    TaskList.Items.Refresh();
                    TaskController.ToggleCompletion(item.ID);
                }
            }
            else if(e.ChangedButton == MouseButton.Right)
            {
                NavigationService.Navigate(new TaskEditorPage(item));
                GC.Collect();
            }
            TaskList.SelectedItem = null;
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineDown();
            }
            else
            {
                scrollViewer.LineUp();
            }
            e.Handled = true;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid.Focus();
        }


        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(timer != null)
                timer.Dispose();

            var text = ((TextBox)sender).Text;
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                FilterTasks(SearchBox.Text);
            }
            else
            {
                timer = new Timer(new TimerCallback(SearchKeyword), text, 500, Timeout.Infinite);
            }
        }

        private void SearchKeyword(object text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FilterTasks((string)text);
            }));
        }

        private List<TaskItem> FilterByKeyword(List<TaskItem> list, string keyword)
        {
            keyword = keyword.ToLower();
            return list.FindAll(t => t.Content.ToLower().Contains(keyword));
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TaskEditorPage(null));
            GC.Collect();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshButton_Click(sender, e);
            NavigationService.Navigated += NavigationService_Navigated;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            taskItems = TaskController.GetTaskItems();
            FilterTasks(SearchBox.Text);
        }
    }
}