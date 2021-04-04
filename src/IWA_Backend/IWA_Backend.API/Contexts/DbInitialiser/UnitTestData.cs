using System;
using System.Collections.Generic;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Contexts.DbInitialiser
{
    public class UnitTestData : ISeedData
    {
        public List<User> Users =>
            new()
            {
                new()
                {
                    UserName = "contractor1",
                    Email = "contractor1@example.com",
                    Name = "Kézműves Károly",
                    ContractorPage = new ContractorPage
                    {
                        Title = "Kézműves mester",
                        Bio = "Kézműves Károly, 46 éves, nagyon érti a dolgát",
                    },
                },
                new()
                {
                    UserName = "contractor2",
                    Email = "contractor2@example.com",
                    Name = "Angoltanár András",
                    ContractorPage = new ContractorPage
                    {
                        Title = "Angol tanár",
                        Bio = "Nyelvizsgára felkészítést, magán angol órákat vállalok",
                    },
                },
                new()
                {
                    UserName = "customer1",
                    Email = "customer1@example.com",
                    Name = "Fogyasztó Feri",
                    ContractorPage = null
                },
                new()
                {
                    UserName = "customer2",
                    Email = "customer2@example.com",
                    Name = "Konszúmer Konrád",
                    ContractorPage = null
                },
            };
        
        public List<Category> Categories =>
            new()
            {
                new()
                {
                    //Id = 1,
                    Name = "Kézműves Kisegítés",
                    Description = "Karcsi segít mindenféle kézműves dologban!",
                    AllowedUsers = new List<User>(),
                    EveryoneAllowed = true,
                    MaxAttendees = 10,
                    Price = 3000,
                    Owner = Users[0],
                },
                new()
                {
                    //Id = 2,
                    Name = "Korai Kőműves Kajakozás",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedUsers = new List<User> {Users[2]},
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = Users[0],
                },
                new()
                {
                    //Id = 3,
                    Name = "Angol C1 felkészítés",
                    Description = "Felkészítés az Angol C1 nyelvvizsgára",
                    AllowedUsers = new List<User>(),
                    EveryoneAllowed = true,
                    MaxAttendees = 1,
                    Price = 3000,
                    Owner = Users[1],
                },
                new()
                {
                    //Id = 4,
                    Name = "Privát angol Konrádnak",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedUsers = new List<User> {Users[3]},
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = Users[1],
                },
            };
        
        public List<Appointment> Appointments =>
            new()
            {
                new()
                {
                    //Id = 1,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = Categories[0],
                    Attendees = new List<User> { },
                    MaxAttendees = Categories[0].MaxAttendees,
                },
                new()
                {
                    //Id = 2,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = Categories[0],
                    Attendees = new List<User> { },
                    MaxAttendees = Categories[0].MaxAttendees,
                },
                new()
                {
                    //Id = 3,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = Categories[1],
                    Attendees = new List<User> { },
                    MaxAttendees = Categories[1].MaxAttendees,
                },
                new()
                {
                    //Id = 4,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = Categories[1],
                    Attendees = new List<User> {Users[3]},
                    MaxAttendees = Categories[1].MaxAttendees,
                },
                new()
                {
                    //Id = 5,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = Categories[2],
                    Attendees = new List<User> { },
                    MaxAttendees = Categories[2].MaxAttendees,
                },
                new()
                {
                    //Id = 6,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = Categories[2],
                    Attendees = new List<User> { },
                    MaxAttendees = Categories[2].MaxAttendees,
                },
                new()
                {
                    //Id = 7,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = Categories[3],
                    Attendees = new List<User> { },
                    MaxAttendees = Categories[3].MaxAttendees,
                },
                new()
                {
                    //Id = 8,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = Categories[3],
                    Attendees = new List<User> { },
                    MaxAttendees = Categories[3].MaxAttendees,
                },
            };
    }
}