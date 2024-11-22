using Internship_3_OOP.Classes;
using Internship_3_OOP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Internship_3_OOP
{
    public class Program
    {
        static Dictionary<Project, List<Task>> projects = new Dictionary<Project, List<Task>>();
        static Dictionary<string, Action> MainMenu = new Dictionary<string, Action>();
        static Dictionary<string, Action> ProjectMenu = new Dictionary<string, Action>();
        static Dictionary<string, Action> TaskMenu = new Dictionary<string, Action>();

        static void Main(string[] args)
        {
            InitializeData();

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
                Console.WriteLine("\n--- GLAVNI IZBORNIK ---");
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
                    MainMenu[choice].Invoke();
                else Console.WriteLine("Nevazeca opcija. Pokusajte ponovno:");
            }

        }//Main

        //funkcije
        static void ViewAllProjects()
        {
            Console.Clear();
            Console.WriteLine("\n--- SVI PROJEKTI S PRIPADAJUCIM ZADACIMA ---");
            if (projects.Count == 0)
            {
                Console.WriteLine("Nema projekata u sustavu.");
                return;
            }
            foreach (var project in projects)
            {
                Console.WriteLine($"Projekt: {project.Key.Name} - Status: {project.Key.Status}");
                foreach (var task in project.Value)
                    Console.WriteLine($"\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.DueDate.ToShortDateString()}");
            }
        }

        public static void AddProject()
        {
            Console.Clear();
            while (true)
            {
                try
                {
                    Console.WriteLine("Unesite ime projekta:");
                    string name;
                    while (true)
                    {
                        name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Ime projekta ne moze biti prazno. Pokusajte ponovno:");
                            continue;
                        }
                        if (projects.Keys.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("To ime je zauzeto. Pokusajte ponovno:");
                            continue;
                        }
                        break;
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
                        else Console.WriteLine("Neispravan format datuma. Molimo unesite datum u formatu yyyy-MM-dd:");
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
                Console.WriteLine($"Projekt: {project.Key.Name} - Status: {project.Key.Status}");

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
                else Console.WriteLine("Brisanje projekta otkazano.");
            }
            else Console.WriteLine("Projekt nije pronaden.");
            Console.Clear();
        }

        static void ViewTasksDueNext7Days()
        {
            Console.Clear();
            Console.WriteLine("\n--- ZADACI S ROKOM U SLJEDECIH 7 DANA ---");
            var today = DateTime.Today;
            var sevenDaysLater = today.AddDays(7);
            bool tasksFound = false;
            foreach (var project in projects)
            {
                foreach (var task in project.Value)
                {
                    if (task.DueDate >= today && task.DueDate <= sevenDaysLater)
                    {
                        Console.WriteLine($"Zadatak: {task.Name} - Rok: {task.DueDate.ToShortDateString()} - Projekt: {project.Key.Name}");
                        tasksFound = true;
                    }
                }
            }
            if (!tasksFound) Console.WriteLine("Nema zadataka s rokom u sljedećih 7 dana.");
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
                            Console.WriteLine($"\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.DueDate.ToShortDateString()}");
                    }
                }
                else Console.WriteLine("Nema projekata s odabranim statusom.");
            }
            else Console.WriteLine("Nevazeci status. Odaberite jedan od sljedecih: Active, Pending, Completed.");
        }

        static void ManageProject()
        {
            Console.Clear();
            Console.WriteLine("\n--- UPRAVLJANJE POJEDIIM PROJEKTOM ---");
            Console.WriteLine("Trenutni projekti:");
            int index = 1;
            foreach (var project in projects)
                Console.WriteLine($"{index++}. {project.Key.Name} - Status: {project.Key.Status}");

            Console.WriteLine("\nOdaberite broj projekta za upravljanje:");
            int projectIndex;
            while (!int.TryParse(Console.ReadLine(), out projectIndex) || projectIndex < 1 || projectIndex > projects.Count)
                Console.WriteLine("Neispravan odabir. Pokusajte ponovno.");

            var selectedProject = projects.Keys.ElementAt(projectIndex - 1);
            ProjectMenu = new Dictionary<string, Action>
            {
                { "1", () => ViewTasks(selectedProject) },
                { "2", () => ViewTasksSortedByDuration(selectedProject) },
                { "3", () => ViewProjectDetails(selectedProject) },
                { "4", () => EditProjectStatus(selectedProject) },
                { "5", () => AddTaskToProject(selectedProject) },
                { "6", () => RemoveTaskFromProject(selectedProject) },
                { "7", () => CalculateTotalTimeForActiveTasks(selectedProject) },
                { "8", () => ManageTask(selectedProject) }
            };

            while (true)
            {
                Console.WriteLine($"\n--- IZBORNIK ZA UPRAVLJANJE PROJEKTOM {selectedProject.Name} ---");
                Console.WriteLine("1 - Ispis svih zadataka unutar odabranog projekta");
                Console.WriteLine("2 - Ispis zadataka sortiranih po duljini trajanja");
                Console.WriteLine("3 - Prikaz detalja odabranog projekta");
                Console.WriteLine("4 - Uredivanje statusa projekta");
                Console.WriteLine("5 - Dodavanje zadatka unutar projekta");
                Console.WriteLine("6 - Brisanje zadatka iz projekta");
                Console.WriteLine("7 - Prikaz ukupno ocekivanog vremena za sve aktivne zadatke u projektu");
                Console.WriteLine("8 - Upravljanje pojedinim zadatkom");
                Console.WriteLine("9 - Povratak na glavni izbornik");
                Console.WriteLine("\nOdaberite opciju:");

                string choice = Console.ReadLine();
                if (choice == "9")
                {
                    Console.Clear();
                    break;
                }
                else if (ProjectMenu.ContainsKey(choice))
                    ProjectMenu[choice].Invoke();
                else
                    Console.WriteLine("Nevazeca opcija. Pokusajte ponovno.");
            }
        }

        static void ViewTasks(Project project)
        {
            Console.Clear();
            Console.WriteLine($"\n--- ZADACI ZA PROJEKT: {project.Name} ---");
            if (!projects[project].Any())
            {
                Console.WriteLine("Nema zadataka za ovaj projekt.");
                return;
            }
            else
            {
                foreach (var task in projects[project])
                    Console.WriteLine($"Task: {task.Name} - Status: {task.Status} - Due: {task.DueDate.ToShortDateString()}");
            }
        }

        static void ViewTasksSortedByDuration(Project project)
        {
            Console.Clear();
            Console.WriteLine($"\n--- ZADACI SORTIRANI OD NAJKRAĆEG DO NAJDULJEG ZA PROJEKT: {project.Name} ---");
            if (projects[project].Count == 0)
            {
                Console.WriteLine($"Nema zadataka za ovaj projekt.");
                return;
            }

            var sortedTasks = projects[project].OrderBy(t => t.ExpectedDuration).ToList();
            foreach (var task in sortedTasks)
                Console.WriteLine($"Zadatak: {task.Name} - Trajanje: {task.ExpectedDuration} dan(a) - Rok: {task.DueDate.ToShortDateString()}");
        }

        static void ViewProjectDetails(Project project)
        {
            Console.Clear();
            Console.WriteLine($"\n--- DETALJI PROJEKTA: {project.Name} ---");
            Console.WriteLine($"Opis: {project.Description}");
            Console.WriteLine($"Pocetak: {project.StartDate.ToShortDateString()}");
            Console.WriteLine($"Zavrsetak: {project.EndDate.ToShortDateString()}");
            Console.WriteLine($"Status: {project.Status}");
            Console.WriteLine("\nPritisni bilo koju tipku za nastavak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void EditProjectStatus(Project project)
        {
            Console.Clear();
            Console.WriteLine($"Trenutni status projekta: {project.Status}");
            if (project.Status == ProjectStatus.Completed)
            {
                Console.WriteLine("Promjena zavrsenog projekta nije moguca.");
                return;
            }

            Console.WriteLine("Unesite novi status projekta (Active, Pending, Completed):");
            string statusInput = Console.ReadLine();
            if (Enum.ProjectStatus.TryParse(statusInput, true, out ProjectStatus newStatus))
            {
                project.Status = newStatus;
                Console.WriteLine($"Status projekta '{project.Name}' uspjesno promijenjen u: {newStatus}");
            }
            else Console.WriteLine("Nevazeci status. Pokusajte ponovno.");
        }

        static void AddTaskToProject(Project project)
        {
            Console.Clear();
            while (true)
            {
                try
                {
                    Console.WriteLine("Unesite ime zadatka:");
                    string name;
                    while (true)
                    {
                        name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Ime zadatka ne moze biti prazno. Pokusajte ponovno:");
                            continue;
                        }
                        if (projects.Values.SelectMany(taskList => taskList).Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("To ime je zauzeto. Pokusajte ponovno:");
                            continue;
                        }
                        break;
                    }

                    Console.WriteLine("Unesite opis zadatka:");
                    string description;
                    while (true)
                    {
                        description = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(description))
                            break;
                        Console.WriteLine("Opis zadatka ne moze biti prazan. Pokusajte ponovno:");
                    }

                    Console.WriteLine("Unesite datum roka zadatka (yyyy-MM-dd):");
                    DateTime dueDate;
                    while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dueDate))
                        Console.WriteLine("Neispravan format datuma. Molimo unesite datum u formatu yyyy-MM-dd.");

                    Console.WriteLine("Unesite ocekivano trajanje zadatka (u danima):");
                    int expectedDays;
                    while (!int.TryParse(Console.ReadLine(), out expectedDays) || expectedDays < 1)
                        Console.WriteLine("Nevazeci unos. Pokusajte ponovno.");

                    TaskStatus taskStatus;
                    while (true)
                    {
                        Console.WriteLine("Unesite status zadatka (Active, Completed, Delayed):");
                        string taskStatusInput = Console.ReadLine();
                        if (Enum.TaskStatus.TryParse(taskStatusInput, true, out taskStatus))
                            break;
                        else Console.WriteLine("Nevazeci status. Pokusajte ponovno.");
                    }

                    Task newTask = new Task(name, description, dueDate, taskStatus, expectedDays);
                    projects[project].Add(newTask);
                    Console.WriteLine("Zadatak uspjesno dodan! Pritisnite bilo koju tipku za nastavak...");
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

        static void RemoveTaskFromProject(Project project)
        {
            Console.Clear();
            Console.WriteLine("Trenutni zadaci:");
            var tasks = projects[project];
            if (tasks.Count > 0)
            {
                foreach (var task in tasks)
                    Console.WriteLine($"\tZadatak: {task.Name} - Status: {task.Status} - Rok: {task.DueDate.ToShortDateString()}");
            }
            else
            {
                Console.WriteLine("\tNema zadataka za ovaj projekt.");
                return;
            }

            Console.WriteLine("\nUnesite ime zadatka kojeg zelite izbrisati:");

            string taskName = Console.ReadLine();
            var taskToRemove = projects[project].FirstOrDefault(t => t.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase));
            if (taskToRemove != null)
            {
                Console.WriteLine($"Jeste li sigurni? (y/n)");
                string confirm = Console.ReadLine();
                if (confirm.ToLower() == "y")
                {
                    projects[project].Remove(taskToRemove);
                    Console.WriteLine($"Zadatak '{taskName}' uspjesno uklonjen iz projekta '{project.Name}'.");
                }
                else Console.WriteLine("Brisanje zadatka otkazano.");
            }
            else Console.WriteLine("Zadatak nije pronaden.");
        }

        static void CalculateTotalTimeForActiveTasks(Project project)
        {
            Console.Clear();
            TimeSpan totalTime = TimeSpan.Zero;
            foreach (var task in projects[project])
            {
                if (task.Status == TaskStatus.Active || task.Status == TaskStatus.Delayed)
                    totalTime += task.DueDate - DateTime.Today;
            }
            Console.WriteLine($"\nUkupno ocekivano vrijeme za sve aktivne zadatke u projektu '{project.Name}': {totalTime.Days} dana.");
        }

        static void ManageTask(Project project)
        {
            Console.Clear();
            Console.WriteLine($"\n--- UPRAVLJANJE ZADACIMA PROJEKTA: {project.Name} ---");
            List<Task> taskList = projects[project];
            if (!taskList.Any())
            {
                Console.WriteLine("Projekt nema zadataka za upravljanje.");
                return;
            }
            Console.WriteLine("Odaberite broj zadatka kojim zelite upravljati:");
            for (int i = 0; i < taskList.Count; i++)
                Console.WriteLine($"{i + 1} - {taskList[i].Name}");

            int selectedTaskIndex;
            while (!int.TryParse(Console.ReadLine(), out selectedTaskIndex) || selectedTaskIndex < 1 || selectedTaskIndex > taskList.Count)
                Console.WriteLine("Nevazeci odabir. Pokusajte ponovno:");

            Task selectedTask = taskList[selectedTaskIndex - 1];
            TaskMenu = new Dictionary<string, Action>
            {
                { "1", () => ViewTaskDetails(selectedTask) },
                { "2", () => EditTaskStatus(selectedTask) }
            };
            while (true)
            {
                Console.WriteLine($"\n--- ZADATAK: {selectedTask.Name} ---");
                Console.WriteLine("1 - Prikaz detalja zadatka");
                Console.WriteLine("2 - Uređivanje statusa zadatka");
                Console.WriteLine("3 - Povratak na prethodni izbornik");
                Console.Write("Odaberite opciju:");
                string choice = Console.ReadLine();

                if (choice == "3")
                {
                    Console.Clear();
                    break;
                }
                else if (TaskMenu.ContainsKey(choice))
                    TaskMenu[choice].Invoke();
                else Console.WriteLine("Nevazeca opcija. Pokusajte ponovno.");
            }
        }

        static void ViewTaskDetails(Task task)
        {
            Console.Clear();
            Console.WriteLine($"\n--- DETALJI ZADATKA: {task.Name} ---");
            Console.WriteLine($"Opis: {task.Description}");
            Console.WriteLine($"Rok zavrsetka: {task.DueDate:yyyy-MM-dd}");
            Console.WriteLine($"Ocekivano trajanje: {task.ExpectedDuration} dana");
            Console.WriteLine($"Status: {task.Status}");
            Console.WriteLine("\nPritisnite bilo koju tipku za nastavak...");
        }

        static void EditTaskStatus(Task task)
        {
            Console.Clear();
            Console.WriteLine($"Trenutni status zadatka: {task.Status}");
            if (task.Status == TaskStatus.Completed)
            {
                Console.WriteLine("Promjena zavrsenog zadatka nije moguca.");
                return;
            }
            Console.WriteLine("Unesite novi status zadatka (Active, Completed, Delayed):");
            string statusInput = Console.ReadLine();
            if (Enum.TaskStatus.TryParse(statusInput, true, out TaskStatus newStatus))
            {
                task.Status = newStatus;
                Console.WriteLine($"Status projekta '{task.Name}' uspjesno promijenjen u: {newStatus}");
            }
            else Console.WriteLine("Nevazeci status. Pokusajte ponovno.");
        }

        static void Exit()
        {
            Console.Clear();
            Console.WriteLine("Izlazak iz aplikacije...");
            Environment.Exit(0);
        }

        //inicijalni podaci
        static void InitializeData()
        {
            DateTime startDate1 = DateTime.ParseExact("2020-06-01", "yyyy-MM-dd", null);
            DateTime startDate2 = DateTime.ParseExact("2023-02-18", "yyyy-MM-dd", null);
            DateTime startDate3 = DateTime.ParseExact("2024-11-25", "yyyy-MM-dd", null);
            DateTime startDate4 = DateTime.ParseExact("2022-04-13", "yyyy-MM-dd", null);

            DateTime endDate1 = DateTime.ParseExact("2021-01-01", "yyyy-MM-dd", null);
            DateTime endDate2 = DateTime.ParseExact("2023-05-16", "yyyy-MM-dd", null);
            DateTime endDate3 = DateTime.ParseExact("2025-01-06", "yyyy-MM-dd", null);
            DateTime endDate4 = DateTime.ParseExact("2024-12-07", "yyyy-MM-dd", null);

            Project project1 = new Project("Database", "Baza podataka za zooloski vrt", startDate1, endDate1, ProjectStatus.Completed);
            Project project2 = new Project("FullStack App", "Aplikacija za kvizove", startDate2, endDate2, ProjectStatus.Completed);
            Project project3 = new Project("Dump internship", "Edukacijski program", startDate3, endDate3, ProjectStatus.Active);
            Project project4 = new Project("Game App", "Prototip igre Super Mario", startDate4, endDate4, ProjectStatus.Pending);

            project1.AddTask(new Task("Zadatak 1", "Izrada osnovne strukture baze podataka i povezivanje tablica", startDate1.AddDays(20), TaskStatus.Completed, 15));
            project1.AddTask(new Task("Zadatak 2", "Implementacija naprednih funkcionalnosti za pretragu podataka", startDate1.AddDays(50), TaskStatus.Completed, 20));
            project1.AddTask(new Task("Zadatak 3", "Testiranje i optimizacija performansi baze podataka", startDate1.AddDays(60), TaskStatus.Delayed, 7));

            project2.AddTask(new Task("Zadatak 1", "Razvoj korisničkog sučelja aplikacije za kvizove", startDate2.AddDays(10), TaskStatus.Completed, 9));
            project2.AddTask(new Task("Zadatak 2", "Implementacija backend sustava za pohranu rezultata korisnika", startDate2.AddDays(20), TaskStatus.Completed, 20));

            project3.AddTask(new Task("Zadatak 1", "Odradivanje backend lekcija", startDate3.AddDays(2), TaskStatus.Active, 2));
            project3.AddTask(new Task("Zadatak 2", "Odradivanje frontend lekcija", startDate3.AddDays(140), TaskStatus.Delayed, 132));

            project4.AddTask(new Task("Zadatak 1", "Razvoj osnovne igre s kontrolama i jednostavnim sučeljem", startDate4.AddDays(180), TaskStatus.Completed, 179));
            project4.AddTask(new Task("Zadatak 2", "Dodavanje novih likova i razina u igri", startDate4.AddDays(365), TaskStatus.Delayed, 270));
            project4.AddTask(new Task("Zadatak 3", "Testiranje igre i ispravljanje bugova prije lansiranja", startDate4.AddDays(600), TaskStatus.Active, 567));

            projects.Add(project1, new List<Task> { project1.Tasks[0], project1.Tasks[1], project1.Tasks[2] });
            projects.Add(project2, new List<Task> { project2.Tasks[0], project2.Tasks[1] });
            projects.Add(project3, new List<Task> { project3.Tasks[0], project3.Tasks[1] });
            projects.Add(project4, new List<Task> { project4.Tasks[0], project4.Tasks[1], project4.Tasks[2] });
        }
    }
}
