using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_Do_App.Entities
{
    public class TaskItem
    {

        public TaskItem()
        {

        }

        public int ID { get; set; }

        public string Content { get; set; }

        public bool Completed { get; set; }

        public bool Flagged { get; set; }

        public DateTime CreationDate { get; set; }

        public DateOnly DueDate { get; set; }

    }
}
