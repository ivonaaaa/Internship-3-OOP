using Internship_3_OOP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_3_OOP.Classes
{
    internal class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Enum.TaskStatus Status { get; set; }
        public int ExpectedDuration { get; set; }

        public Task(string name, string description, DateTime dueDate, Enum.TaskStatus status, int expectedDuration)
        {
            this.Name = name;
            this.Description = description;
            this.DueDate = dueDate;
            this.Status = status;
            this.ExpectedDuration = expectedDuration;
        }

        public void SetStatus(Enum.TaskStatus newStatus)
        {
            if (Status != Enum.TaskStatus.Completed)
            {
                Status = newStatus;
            }
            else
            {
                Console.WriteLine("Nije moguće promijeniti status završenog zadatka.");
            }
        }
    }
}
