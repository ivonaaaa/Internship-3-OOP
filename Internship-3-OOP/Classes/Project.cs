using Internship_3_OOP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_3_OOP.Classes
{
    internal class Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public List<Task> Tasks { get; set; }

        public Project(string name, string description, DateTime startDate, DateTime endDate, ProjectStatus status)
        {
            this.Name = name;
            this.Description = description;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Status = status;
            this.Tasks = new List<Task>();
        }

        public void SetStatus(ProjectStatus newStatus)
        {
            if (Status != ProjectStatus.Completed)
            {
                Status = newStatus;
            }
            else
            {
                Console.WriteLine("Nije moguće promijeniti status završenog projekta.");
            }
        }
    }
}
