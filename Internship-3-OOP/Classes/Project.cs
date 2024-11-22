﻿using Internship_3_OOP.Enum;
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
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Ime projekta ne može biti prazno.");
                return;
            }
            if (name.All(char.IsDigit))
            {
                Console.WriteLine("Krivi tip podatka pri unosu.");
                return;
            }
            this.Name = name;

            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Opis projekta ne može biti prazan.");
                return;
            }
            if (description.All(char.IsDigit))
            {
                Console.WriteLine("Krivi tip podatka pri unosu.");
                return;
            }
            this.Description = description;

            if (StartDate > EndDate)
            {
                Console.WriteLine("Datum početka ne može biti nakon datuma završetka.");
                return;
            }
            this.StartDate = startDate;

            if (EndDate > StartDate)
            {
                Console.WriteLine("Datum završetka ne može biti prije datuma početka.");
                return;
            }
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
