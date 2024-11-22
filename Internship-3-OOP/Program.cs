using Internship_3_OOP.Classes;
using Internship_3_OOP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Internship_3_OOP
{
    public class Program
    {
        static Dictionary<Project, List<Task>> projects = new Dictionary<Project, List<Task>>();
        static Dictionary<string, Action> MainMenu = new Dictionary<string, Action>();
        static Dictionary<string, Action> ProjectMenu = new Dictionary<string, Action>();

        static void Main(string[] args)
        {
            //InitalizedData();

            MainMenu = new Dictionary<string, Action>
            {
                { "1", ViewAllProjects},
                { "2", AddProject},
                { "3", DeleteProject},
                { "4", ViewTasksDueNext7Days},
                { "5", ViewProjectsByStatus},
                { "6", ManageProject},
                { "7", Exit}
            };

            while (true) {
                Console.WriteLine("\n--- APLIKACIJA ZA UPRAVLJANJE PROJEKTIMA ---");
                Console.WriteLine("1 - Ispis svih projekata s pripadajucim zadacima");
                Console.WriteLine("2 - Dodavanje novog projekta");
                Console.WriteLine("3 - Brisanje projekta");
                Console.WriteLine("4 - Prikaz svih zadataka s rokom u sljedecih 7 dana");
                Console.WriteLine("5 - Prikaz projekata filtriranih po statusu");
                Console.WriteLine("6 - Upravljanje pojedinim projektom");
                Console.WriteLine("7 - Izlaz");
                Console.WriteLine("\nOdaberite opciju:");
                string choice = Console.ReadLine();

                if (MainMenu.ContainsKey(choice))
                {
                    MainMenu[choice].Invoke();
                }
                else
                {
                    Console.WriteLine("Nevazeca opcija. Pokusajte ponovno:");
                }
            }

        }//Main

        //funkcije
        static void ViewAllProjects()
        {
            Console.Clear();
            Console.WriteLine("\n--- SVI PROJEKTI S PRIPADAJUCIM ZADACIMA ---");
            foreach (var project in projects)
            {
                Console.WriteLine($"Projekt: {project.Key.Name} - Status: {project.Key.Status}");
                foreach (var task in project.Value)
                {
                    Console.WriteLine("$\"\\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.DueDate.ToShortDateString()}\"");
                }
            }
        }

        public static void AddProject()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Unesite ime projekta:");
                    string name;
                    while (true)
                    {
                        name = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(name))
                            break;

                        Console.WriteLine("Ime projekta ne moze biti prazno. Pokusajte ponovno:");
                    }

                    Console.WriteLine("Unesite opis projekta:");
                    string description;
                    while (true)
                    {
                        description = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(description))
                            break;

                        Console.WriteLine("Opis projekta ne moze biti prazan. Pokusajte ponovno:");
                    }

                    Console.WriteLine("Unesite datum pocetka projekta (yyyy-MM-dd):");
                    DateTime startDate;
                    while (true)
                    {
                        if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out startDate))
                            break;

                        Console.WriteLine("Neispravan format datuma. Molimo unesite datum u formatu yyyy-MM-dd:");
                    }

                    Console.WriteLine("Unesite datum zavrsetka projekta (yyyy-MM-dd):");
                    DateTime endDate;
                    while (true)
                    {
                        if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out endDate))
                        {
                            if (startDate < endDate)
                                break;

                            Console.WriteLine("Datum pocetka mora biti prije datuma zavrsetka. Unesite ponovo datum zavrsetka:");
                        }
                        else
                        {
                            Console.WriteLine("Neispravan format datuma. Molimo unesite datum u formatu yyyy-MM-dd:");
                        }
                    }

                    Console.WriteLine("Unesite status projekta (Active, Pending, Completed):");
                    ProjectStatus status;
                    while (true)
                    {
                        if (Enum.ProjectStatus.TryParse(Console.ReadLine(), true, out status))
                            break;

                        Console.WriteLine("Nevazeci status. Unesite Active, Pending ili Completed:");
                    }


                    var newProject = new Project(name, description, startDate, endDate, status);
                    projects.Add(newProject, new List<Task>());

                    Console.WriteLine("Projekt uspjesno dodan! Pritisni bilo koju tipku za nastavak...");
                    Console.ReadKey();
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Greska: {ex.Message}");
                    Console.WriteLine("Molimo pokusajte ponovno.\n");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Pogresan format datuma. Molimo unesite datum u formatu yyyy-MM-dd.");
                }
            }
            Console.Clear();
        }

        static void DeleteProject()
        {
            Console.Clear();
            Console.WriteLine("Trenutni projekti:");
            foreach (var project in projects)
            {
                Console.WriteLine($"Projekt: {project.Key.Name} - Status: {project.Key.Status}");
            }
            Console.WriteLine("Unesite ime projekta kojeg zelite izbrisati:");
            string projectName = Console.ReadLine();
            var projectToDelete = projects.Keys.FirstOrDefault(p => p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));

            if (projectToDelete != null)
            {
                Console.WriteLine("Jeste li sigurni? (y/n):");
                string confirm = Console.ReadLine();
                if (confirm.ToLower() == "y")
                {
                    projects.Remove(projectToDelete);
                    Console.WriteLine("Projekt izbrisan uspjesno.");
                }
                else
                {
                    Console.WriteLine("Neuspjesno.");
                }
            }
            else
            {
                Console.WriteLine("Projekt nije pronaden.");
            }
        }

        static void ViewTasksDueNext7Days()
        {
            Console.Clear();
            Console.WriteLine("\n--- ZADACI S ROKOM U SLJEDECIH 7 DANA ---");
            var today = DateTime.Today;
            var sevenDaysLater = today.AddDays(7);
            foreach (var project in projects)
            {
                foreach (var task in project.Value)
                {
                    if (task.DueDate >= today && task.DueDate <= sevenDaysLater)
                    {
                        Console.WriteLine($"Zadatak: {task.Name} - Rok: {task.DueDate.ToShortDateString()} - Projekt: {project.Key.Name}");
                    }
                }
            }
        }

        static void ViewProjectsByStatus()
        {
            Console.Clear();
            Console.WriteLine("\n--- PRIKAZ PROJEKATA FILTRIRANIH PO STATUSU ---");
            Console.WriteLine("Odaberite status projekta (Active, Pending, Completed):");
            string statusInput = Console.ReadLine();

            if (Enum.ProjectStatus.TryParse<ProjectStatus>(statusInput, true, out ProjectStatus status))
            {
                var filteredProjects = projects.Where(p => p.Key.Status == status).ToList();
                if (filteredProjects.Any())
                {
                    foreach (var project in filteredProjects)
                    {
                        Console.WriteLine($"Projekt: {project.Key.Name} - Status: {project.Key.Status}");
                        foreach (var task in project.Value)
                        {
                            Console.WriteLine($"\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.DueDate.ToShortDateString()}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Nema projekata s odabranim statusom.");
                }
            }
            else
            {
                Console.WriteLine("Nevazeci status. Odaberite jedan od sljedecih: Active, Pending, Completed.");
            }
        }

        static void ManageProject()
        {
            Console.Clear();
            //ovdje ce ic podizbornikm
        }

        static void Exit()
        {
            Console.Clear();
            Console.WriteLine("Izlazak iz aplikacije...");
            Environment.Exit(0);
        }

        //u nastavku dodati inicijalne podatke...


    }//class Program
}
