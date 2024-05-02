using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using To_Do_App.Entities;

namespace To_Do_App.Controllers
{
    internal class TaskController
    {
        private static readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:7043/TaskList/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        public static List<TaskItem> GetTaskItems()
        {
            try
            {
                var res = client.GetStringAsync("Tasks").Result;
                var tasks = JsonConvert.DeserializeObject<List<TaskItem>>(res);
                return tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        public static TaskItem GetTaskByID(int id)
        {
            try
            {
                var res = client.GetStringAsync("TaskByID?ID=" + id).Result;
                var task = JsonConvert.DeserializeObject<TaskItem>(res);
                return task;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        public static List<TaskItem> GetTasksByKeyword(string keyword)
        {
            try
            {
                var res = client.GetStringAsync("TaskByKeyword?keyword=" + keyword).Result;
                var tasks = JsonConvert.DeserializeObject<List<TaskItem>>(res);
                return tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        public static bool CreateTask(TaskItem item)
        {
            try
            {
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                client.PostAsync("Create", httpContent);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        public static bool UpdateTask(TaskItem item)
        {
            try
            {
                client.PutAsync($"Update?ID={item.ID}&content={item.Content}&completed={item.Completed}&flagged={item.Flagged}&dueDate={item.DueDate}", null);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        public static bool EditContent(int ID, string content)
        {
            try
            {
                var updateData = new
                {
                    ID = ID,
                    content = content,
                };
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");
                client.PutAsync("EditContent", httpContent);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        private static bool Toggle(int ID, string value)
        {
            try
            {
                client.PutAsync(value + "?ID=" + ID, null);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        public static bool ToggleCompletion(int ID)
        {
            return Toggle(ID, "ToggleCompletion");
        }

        public static bool ToggleFlag(int ID)
        {
            return Toggle(ID, "ToggleFlag");
        }

        public static bool Delete(int ID)
        {
            try
            {
                client.DeleteAsync("Delete?ID=" + ID);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static bool BatchDelete(int startID, int endID)
        {
            try
            {
                client.DeleteAsync("Delete?startID=" + startID + "&endID=" + endID);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
