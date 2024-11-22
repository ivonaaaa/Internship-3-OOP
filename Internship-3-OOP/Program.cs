using Internship_3_OOP.Classes;
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
        private static Dictionary<Project, List<Task>> projects = new Dictionary<Project, List<Task>>();
        static Dictionary<string, Action> MainMenu = new Dictionary<string, Action>();
        static Dictionary<string, Action> ProjectMenu = new Dictionary<string, Action>();

        static void Main(string[] args)
        {
            //InitalizedData();

            MainMenu = new Dictionary<string, Action>
            {
                //planirane funkcije
                { "1", ViewAllProjects},
                { "2", AddProject},
                { "3", DeleteProject},
                { "4", ViewTasksDueNext7Days},
                { "5", ViewProjectsByStatus},
                { "6", ManageProject},
                { "7", Exit}
            };

            while (true) {
                Console.WriteLine("\n---APLIKACIJA ZA UPRAVLJANJE PROJEKTIMA---");
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

        //u nastavku funkcije


    }//class Program
}
