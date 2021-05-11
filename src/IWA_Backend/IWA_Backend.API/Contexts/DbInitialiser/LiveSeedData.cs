using System;
using System.Collections.Generic;
using System.Globalization;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Contexts.DbInitialiser
{
    public class LiveSeedData : ISeedData
    {
        public List<User> Users() =>
            new()
            {
                new() // Index: 0
                {
                    UserName = "contractor1",
                    Email = "contractor1@example.com",
                    Name = "Kézműves Károly",
                    ContractorPage = new ContractorPage
                    {
                        Title = "Kézműves mester",
                        Bio = "Kézműves Károly, 46 éves, nagyon érti a dolgát",
                        Avatar = "seed_karcsi.jpg",
                    },
                },
                new() // Index: 1
                {
                    UserName = "contractor2",
                    Email = "contractor2@example.com",
                    Name = "Jakubik Máté",
                    ContractorPage = new ContractorPage
                    {
                        Title = "Angol tanár",
                        Bio = "Nyelvizsgára felkészítést, magán angol órákat vállalok",
                        Avatar = "seed_angola.jpg",
                    },
                },
                new() // Index: 2
                {
                    UserName = "gyumi",
                    Email = "kristof@gyimesi.hu",
                    Name = "Gyimesi \"Gyümi\" Kristóf",
                    ContractorPage = new ContractorPage
                    {
                        Title = "Terapeuta",
                        Bio = "Ha baj van, Gyümi meggyógyítja lelked.",
                        Avatar = "seed_gyümi.jpg",
                    },
                },
                new() // Index: 3
                {
                    UserName = "fuzerdonat",
                    Email = "donat@fuzerdonat.hu",
                    Name = "Füzér Donát",
                    ContractorPage = new ContractorPage
                    {
                        Title = "Személyi edző",
                        Bio = "Személyi, csoportos vagy akár online edzés, mert az edzés öröm!",
                        Avatar = "seed_doni.jpg",
                    },
                },
                new() // Index: 4
                {
                    UserName = "ndsblnt",
                    Email = "best@hacker.man",
                    Name = "Nádas Bálint",
                    ContractorPage = null,
                },
                new() // Index: 5
                {
                    UserName = "hajdulord",
                    Email = "nefi@unity.com",
                    Name = "Hajdu Marcell ferenc",
                    ContractorPage = null
                },
                new() // Index: 6
                {
                    UserName = "customer1",
                    Email = "customer1@example.com",
                    Name = "Fogyasztó Feri",
                    ContractorPage = null
                },
                new() // Index: 7
                {
                    UserName = "customer2",
                    Email = "customer2@example.com",
                    Name = "Konszúmer Konrád",
                    ContractorPage = null
                },
                new() // Index: 8
                {
                    UserName = "wency",
                    Email = "twitchmaister@twitch.tv",
                    Name = "Zsolti",
                    ContractorPage = null
                },
                new() // Index: 9
                {
                    UserName = "petrosz",
                    Email = "s6alxc@inf.elte.hu",
                    Name = "Andi Péter",
                    ContractorPage = null
                },
                new() // Index: 10
                {
                    UserName = "geri902",
                    Email = "gergo@andigergo.hu",
                    Name = "Andi Gergő",
                    ContractorPage = null
                },
                new() // Index: 11
                {
                    UserName = "csuvi",
                    Email = "szalai@patrik.wookie",
                    Name = "Szalai Patrik",
                    ContractorPage = null
                },
                new() // Index: 12
                {
                    UserName = "andi_de_nem_peter",
                    Email = "andi@andikvarium.net",
                    Name = "Góga Andrea",
                    ContractorPage = null
                },
                new() // Index: 13
                {
                    UserName = "kajt",
                    Email = "kajt@dontbanme.ccp",
                    Name = "Yu Kai Te",
                    ContractorPage = null
                },
                new() // Index: 14
                {
                    UserName = "queen",
                    Email = "presidente@ikhok.elte.hu",
                    Name = "Deák Dalma",
                    ContractorPage = null
                },
                new() // Index: 15
                {
                    UserName = "twok",
                    Email = "kk@eva.elte.hu",
                    Name = "Károlyi Kristóf",
                    ContractorPage = null
                },
            };
        public List<Category> Categories(List<User> users) =>
            new()
            {
                new() // Index: 0
                {
                    Name = "Kézműves Kisegítés",
                    Description = "Karcsi segít mindenféle kézműves dologban!",
                    AllowedUsers = new (),
                    EveryoneAllowed = true,
                    MaxAttendees = 10,
                    Price = 3000,
                    Owner = users[0],
                },
                new() // Index: 1
                {
                    Name = "Korai Kőműves Kajakozás",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedUsers = new List<User>{ users[6], users[7] },
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = users[0],
                },
                new() // Index: 2
                {
                    Name = "Angol C1 felkészítés",
                    Description = "Felkészítés az Angol C1 nyelvvizsgára",
                    AllowedUsers = new List<User>{ users[6], users[7] },
                    EveryoneAllowed = true,
                    MaxAttendees = 1,
                    Price = 3000,
                    Owner = users[1],
                },
                new() // Index: 3
                {
                    Name = "Privát angol Konrádnak",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedUsers = new List<User>{ users[7] },
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = users[1],
                },
                new () // Index: 4
                {
                    Name = "Személyi edzés",
                    Description = "60 perces személyi edzés az Erőpont konditeremben",
                    AllowedUsers = new (),
                    EveryoneAllowed = true,
                    MaxAttendees = 1,
                    Price = 3500,
                    Owner = users[3],
                },
                new () // Index: 5
                {
                    Name = "Csoportos edzés",
                    Description = "Csoportos edzés a Margit szigeten a kondiparkban",
                    AllowedUsers = new (),
                    EveryoneAllowed = true,
                    MaxAttendees = 7,
                    Price = 2500,
                    Owner = users[3],
                },
                new () // Index: 6
                {
                    Name = "Családi edzés",
                    Description = "Edzés családtagoknak",
                    AllowedUsers = new List<User> { users[9], users[10] },
                    EveryoneAllowed = false,
                    MaxAttendees = 7,
                    Price = 1200,
                    Owner = users[3],
                },
                new () // Index: 7
                {
                    Name = "Kemény Nap",
                    Description = "😎😎😹👾🧐🙈🐯🐯",
                    AllowedUsers = new List<User>
                    {
                        users[4], users[5], users[8], users[9], users[11],
                        users[12], users[13], users[14], users[15],
                    },
                    EveryoneAllowed = false,
                    MaxAttendees = 10,
                    Price = 690,
                    Owner = users[2],
                },
                new () // Index: 8
                {
                    Name = "Tepertőkrémes Terápia",
                    Description = "Vagy nutellás, de gluténmentes opciók is vannak.",
                    AllowedUsers = new List<User>{},
                    EveryoneAllowed = true,
                    MaxAttendees = 2,
                    Price = 6999,
                    Owner = users[2],
                },
            };
        
        public List<Appointment> Appointments(List<Category> categories, List<User> users) =>
            new()
            {
                // Kézműves Károly
                new()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[0],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[0].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[0],
                    Attendees = new List<User>{ users[6] },
                    MaxAttendees = categories[0].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[1],
                    Attendees = new List<User>{ users[6] },
                    MaxAttendees = categories[1].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[1],
                    Attendees = new List<User>{ users[7], users[6] },
                    MaxAttendees = categories[1].MaxAttendees,
                },
                // Angoltanár András
                new()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[2],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[2].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[2],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[2].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[3],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[3].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[3],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[3].MaxAttendees,
                },
                // Füzér Donát
                new()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[4],
                    Attendees = new List<User>{ users[6] },
                    MaxAttendees = categories[4].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[4],
                    Attendees = new List<User>{ users[7] },
                    MaxAttendees = categories[4].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(2),
                    EndTime = DateTime.Now.AddHours(3),
                    Category = categories[4],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[4].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(3),
                    EndTime = DateTime.Now.AddHours(4),
                    Category = categories[4],
                    Attendees = new List<User>{ users[9] },
                    MaxAttendees = categories[4].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(4),
                    EndTime = DateTime.Now.AddHours(5),
                    Category = categories[4],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[4].MaxAttendees,
                },                
                new()
                {
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(1).AddHours(1).AddMinutes(30),
                    Category = categories[5],
                    Attendees = new List<User>{ users[8], users[9], users[10], users[11] },
                    MaxAttendees = categories[5].MaxAttendees,
                },                
                new()
                {
                    StartTime = DateTime.Now.AddDays(2),
                    EndTime = DateTime.Now.AddDays(2).AddHours(1).AddMinutes(30),
                    Category = categories[5],
                    Attendees = new List<User>{ users[8], users[9], users[10], users[11] },
                    MaxAttendees = categories[5].MaxAttendees,
                },                
                new()
                {
                    StartTime = DateTime.Now.AddDays(3),
                    EndTime = DateTime.Now.AddDays(3).AddHours(1),
                    Category = categories[6],
                    Attendees = new List<User>{ users[9], users[10] },
                    MaxAttendees = categories[6].MaxAttendees,
                },                
                new()
                {
                    StartTime = DateTime.Now.AddDays(4),
                    EndTime = DateTime.Now.AddDays(4).AddHours(1),
                    Category = categories[6],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[6].MaxAttendees,
                },
                // Gyümi
                new()
                {
                    StartTime = DateTime.ParseExact("2021.05.15 19:00", "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture),
                    EndTime = DateTime.ParseExact("2021.05.15 23:00", "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture),
                    Category = categories[7],
                    Attendees = new List<User>
                    {
                        users[4], users[5], users[8], users[9], users[11],
                        users[12], users[13], users[14], users[15], users[2],
                    },
                    MaxAttendees = categories[7].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[8],
                    Attendees = new List<User>{},
                    MaxAttendees = categories[8].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[8],
                    Attendees = new List<User>{},
                    MaxAttendees = categories[8].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(2),
                    EndTime = DateTime.Now.AddHours(3),
                    Category = categories[8],
                    Attendees = new List<User>{},
                    MaxAttendees = categories[8].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(3),
                    EndTime = DateTime.Now.AddHours(4),
                    Category = categories[8],
                    Attendees = new List<User>{},
                    MaxAttendees = categories[8].MaxAttendees,
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(4),
                    EndTime = DateTime.Now.AddHours(5),
                    Category = categories[8],
                    Attendees = new List<User>{},
                    MaxAttendees = categories[8].MaxAttendees,
                },
            };
    }
}