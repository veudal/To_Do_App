using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using To_Do_App.Controllers;
using To_Do_App.Entities;

namespace To_Do_App.Pages
{
    public partial class TaskEditorPage : Page
    {

        readonly TaskItem item;

        public TaskEditorPage(TaskItem item_)
        {
            InitializeComponent();
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            this.item = item_;
            if (item != null)
            {
                ID_Block.Text = "Task ID: " + item.ID;
                ContentBox.Text = item.Content;
                CompleteStatus.IsChecked = item.Completed;
                FlagStatus.IsChecked = item.Flagged;
                CreationDate_Block.Text = item.CreationDate.ToString();
                if (item.DueDate != DateOnly.MinValue)
                    DueDate_Box.Text = item.DueDate.ToString();
            }

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            AcceptButton.IsEnabled = false;
            try
            {
                if (string.IsNullOrEmpty(ContentBox.Text))
                {
                    AcceptButton.IsEnabled = true;
                    MessageBox.Show("Content cannot be empty.");
                    return;
                }
                DateOnly dateOnly = DateOnly.MinValue;
                if (!string.IsNullOrEmpty(DueDate_Box.Text))
                    dateOnly = DateOnly.Parse(DueDate_Box.Text);

                TaskItem taskItem = new TaskItem
                {
                    Content = ContentBox.Text,
                    Completed = (bool)CompleteStatus.IsChecked,
                    Flagged = (bool)FlagStatus.IsChecked,
                    DueDate = dateOnly
                };

                if (item == null)
                    TaskController.CreateTask(taskItem);
                else
                {
                    taskItem.ID = item.ID;
                    TaskController.UpdateTask(taskItem);
                }

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                AcceptButton.IsEnabled = true;
            }

        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteButton.IsEnabled = false;
            if (this.item != null)
            {
                if (!TaskController.Delete(item.ID))
                    MessageBox.Show("Task could not be deleted.");
            }
            NavigationService.GoBack();

        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid.Focus();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
