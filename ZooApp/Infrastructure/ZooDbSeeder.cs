using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using ZooApp.Models;
using BCrypt.Net;

namespace ZooApp.Data
{
    public static class ZooDbSeeder
    {
        public static void Seed(MongoDbContext db)
        {
            
            db.Animals.DeleteMany(Builders<Animal>.Filter.Empty);
            db.Cages.DeleteMany(Builders<Cage>.Filter.Empty);
            db.Employees.DeleteMany(Builders<Employee>.Filter.Empty);
            db.Suppliers.DeleteMany(Builders<Supplier>.Filter.Empty);
            db.Feeds.DeleteMany(Builders<Feed>.Filter.Empty);
            db.FeedingSchedules.DeleteMany(Builders<FeedingSchedule>.Filter.Empty);
            db.ExchangeRecords.DeleteMany(Builders<ExchangeRecord>.Filter.Empty);
            db.MedicalRecords.DeleteMany(Builders<MedicalRecord>.Filter.Empty);
            
           
            db.KeyUsers.DeleteMany(Builders<KeyUser>.Filter.Empty);
            var keyUsers = GetKeyUsers();
            db.KeyUsers.InsertMany(keyUsers);

            
            var animals = GetAnimals();
            db.Animals.InsertMany(animals); 

            var cages = GetCages();
            db.Cages.InsertMany(cages);

            var employees = GetEmployees();
            db.Employees.InsertMany(employees);

           
            var suppliers = GetSuppliers();
            db.Suppliers.InsertMany(suppliers);

            var feeds = GetFeeds(suppliers);
            db.Feeds.InsertMany(feeds);

            
            var feeding = GetFeeding(animals);
            db.FeedingSchedules.InsertMany(feeding);

            var medicals = GetMedicalRecords(animals);
            db.MedicalRecords.InsertMany(medicals);

            var exchanges = GetExchanges(animals);
            db.ExchangeRecords.InsertMany(exchanges);
            
            
            AssignAnimalsToCages(db);
            AssignEmployeesToAnimals(db);
        }
        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }
        
        private static List<KeyUser> GetKeyUsers()
        {
            return new List<KeyUser>
            {
                new KeyUser
                {
                    Login = "admin",
                    Password = HashPassword("1234"), 
                    Role = "admin"
                },
                new KeyUser
                {
                    Login = "operator",
                    Password = HashPassword("1234"), 
                    Role = "operator"
                },
                new KeyUser
                {
                    Login = "authorized", 
                    Password = HashPassword("1234"), 
                    Role = "authorized"
                },
                new KeyUser
                {
                    Login = "guest",
                    Password = HashPassword("1234"), 
                    Role = "guest"
                }
            };
        }

        
        private static List<Animal> GetAnimals()
        {
            var rnd = new Random();
            string[] predators = { "Wolf", "Tiger", "Lion", "Fox" };
            string[] herbivores = { "Deer", "Sheep", "Giraffe", "Zebra", "Monkey" };

            var animals = new List<Animal>();

            
            for (int i = 0; i < 12; i++)
            {
                animals.Add(new Animal
                {
                    Name = predators[rnd.Next(predators.Length)],
                    Species = "Spec" + rnd.Next(1, 50),
                    Gender = rnd.Next(2) == 0 ? "male" : "female",
                    BirthDate = DateTime.Now.AddYears(-rnd.Next(1, 8)),
                    Weight = rnd.Next(30, 150),
                    Height = rnd.Next(50, 140),
                    Type = "predator",
                    ClimateZone = rnd.Next(2) == 0 ? "north" : "temperate",
                    NeedsWarmShelter = rnd.Next(2) == 0,
                    IsIsolated = false,
                    IsolationReason = null
                });
            }
            
            for (int i = 0; i < 15; i++)
            {
                animals.Add(new Animal
                {
                    Name = herbivores[rnd.Next(herbivores.Length)],
                    Species = "Spec" + rnd.Next(1, 100),
                    Gender = rnd.Next(2) == 0 ? "male" : "female",
                    BirthDate = DateTime.Now.AddYears(-rnd.Next(1, 13)),
                    Weight = rnd.Next(50, 300),
                    Height = rnd.Next(70, 230),
                    Type = "herbivore",
                    ClimateZone = "tropical",
                    NeedsWarmShelter = true,
                    IsIsolated = false,
                    IsolationReason = null
                });
            }

            return animals;
        }

        
        private static List<Cage> GetCages() => new()
        {
            new Cage
            {
                Id = ObjectId.GenerateNewId(),
                Number = 1,
                Location = "North zone A1",
                Capacity = 3,
                Heated = false,
                AllowedTypes = new List<string>{ "predator" },
                CompatibleSpecies = new List<string>(),
                NeighborCageIds = new List<string>(),
                Animals = new List<ObjectId>()
            },
            new Cage
            {
                Id = ObjectId.GenerateNewId(),
                Number = 2,
                Location = "South zone B1",
                Capacity = 6,
                Heated = true,
                AllowedTypes = new List<string>{ "herbivore" },
                CompatibleSpecies = new List<string>(),
                NeighborCageIds = new List<string>(),
                Animals = new List<ObjectId>()
            },
            new Cage
            {
                Id = ObjectId.GenerateNewId(),
                Number = 3,
                Location = "Central C3",
                Capacity = 10,
                Heated = true,
                AllowedTypes = new List<string>{ "herbivore" },
                CompatibleSpecies = new List<string>(),
                NeighborCageIds = new List<string>(),
                Animals = new List<ObjectId>()
            },
            new Cage
            {
                Id = ObjectId.GenerateNewId(),
                Number = 4,
                Location = "Predator zone D2",
                Capacity = 4,
                Heated = false,
                AllowedTypes = new List<string>{ "predator" },
                CompatibleSpecies = new List<string>(),
                NeighborCageIds = new List<string>(),
                Animals = new List<ObjectId>()
            }
        };

        
        private static List<Employee> GetEmployees() => new()
        {
            new Employee
            {
                FullName="Ivan Vetrov",
                Category="vet",
                Gender="male",
                BirthDate=DateTime.Now.AddYears(-40),
                WorkStartDate=DateTime.Now.AddYears(-10),
                Salary=24000,
                AnimalsUnderCare = new List<string>()
            },
            new Employee
            {
                FullName="Anna Vet",
                Category="vet",
                Gender="female",
                BirthDate=DateTime.Now.AddYears(-35),
                WorkStartDate=DateTime.Now.AddYears(-7),
                Salary=23000,
                AnimalsUnderCare = new List<string>()
            },
            new Employee
            {
                FullName="Oleh Clean",
                Category="cleaner",
                Gender="male",
                BirthDate=DateTime.Now.AddYears(-30),
                WorkStartDate=DateTime.Now.AddYears(-5),
                Salary=15000,
                AnimalsUnderCare = new List<string>()
            },
            new Employee
            {
                FullName="Masha Clean",
                Category="cleaner",
                Gender="female",
                BirthDate=DateTime.Now.AddYears(-28),
                WorkStartDate=DateTime.Now.AddYears(-3),
                Salary=15500,
                AnimalsUnderCare = new List<string>()
            },
            new Employee
            {
                FullName="Andy Trainer",
                Category="trainer",
                Gender="male",
                BirthDate=DateTime.Now.AddYears(-33),
                WorkStartDate=DateTime.Now.AddYears(-8),
                Salary=26000,
                AnimalsUnderCare = new List<string>()
            }
        };

        
        private static List<Supplier> GetSuppliers() => new()
        {
            new Supplier
            {
                Name="EcoFeeds",
                Address="Kyiv",
                Phone="+380930000001",
                FeedTypes=new List<string>{"meat","fruit"},
                Contracts = new List<string>()
            },
            new Supplier
            {
                Name="GreenFarm",
                Address="Lviv",
                Phone="+380670000003",
                FeedTypes=new List<string>{"plant"},
                Contracts = new List<string>()
            }
        };

       
        private static List<Feed> GetFeeds(List<Supplier> suppliers)
        {
            var list = new List<Feed>();

            var supIds = suppliers
                .Select(s =>
                {
                    if (ObjectId.TryParse(s.Id, out var oid))
                        return oid;
                    return ObjectId.Empty;
                })
                .Where(x => x != ObjectId.Empty)
                .ToList();

            list.Add(new Feed
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Meat",
                Type = "meat",
                ProducedByZoo = false,
                Suppliers = new List<ObjectId>(supIds)
            });

            list.Add(new Feed
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Fruits",
                Type = "plant",
                ProducedByZoo = true,
                Suppliers = new List<ObjectId>(supIds)
            });

            list.Add(new Feed
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Live mice",
                Type = "live",
                ProducedByZoo = true,
                Suppliers = new List<ObjectId>(supIds)
            });

            return list;
        }

        
        private static List<FeedingSchedule> GetFeeding(List<Animal> animals)
        {
            var rnd = new Random();
            var result = new List<FeedingSchedule>();

            foreach (var a in animals.Take(10))
            {
                string feedType = a.Type == "predator" ? "meat" : "fruits";
                double qty = a.Type == "predator"
                    ? rnd.NextDouble() * 3 + 1      // 1–4 кг
                    : rnd.NextDouble() * 5 + 1;     // 1–6 кг

                result.Add(new FeedingSchedule
                {
                    AnimalId = a.Id,
                    AnimalName = a.Name,
                    FeedType = feedType,
                    QuantityKg = Math.Round(qty, 1),
                    FeedingTime = "10:00",
                    Season = "winter"
                });
            }

            return result;
        }

        
        private static List<MedicalRecord> GetMedicalRecords(List<Animal> animals)
        {
            var rnd = new Random();
            var result = new List<MedicalRecord>();

            foreach (var a in animals.Take(8))
            {
                var checks = new List<Checkup>
                {
                    new Checkup
                    {
                        Date = DateTime.Now.AddMonths(-6),
                        Weight = a.Weight,
                        Height = a.Height,
                        Vaccinations = new List<string>{ "rabies" },
                        Illnesses = new List<string>(),
                        Treatment = ""
                    },
                    new Checkup
                    {
                        Date = DateTime.Now.AddMonths(-2),
                        Weight = a.Weight + rnd.Next(-5, 6),
                        Height = a.Height,
                        Vaccinations = new List<string>(),
                        Illnesses = new List<string>{ "cold" },
                        Treatment = "antibiotics"
                    }
                };

                result.Add(new MedicalRecord
                {
                    AnimalId = a.Id,
                    Checkups = checks
                });
            }

            return result;
        }

        
        private static List<ExchangeRecord> GetExchanges(List<Animal> animals)
        {
            var list = new List<ExchangeRecord>();

            var a1 = animals.FirstOrDefault();
            if (a1 != null)
            {
                list.Add(new ExchangeRecord
                {
                    AnimalId = a1.Id,
                    AnimalName = $"{a1.Name} ({a1.Species})",
                    ExchangeType = "sent",
                    OtherZoo = "Berlin Zoo",
                    Reason = "Genetic Diversification",
                    ExchangeDate = DateTime.Now.AddYears(-1)
                });
            }

            var a2 = animals.Skip(1).FirstOrDefault();
            if (a2 != null)
            {
                list.Add(new ExchangeRecord
                {
                    AnimalId = a2.Id,
                    AnimalName = $"{a2.Name} ({a2.Species})",
                    ExchangeType = "received",
                    OtherZoo = "Prague Zoo",
                    Reason = "Breeding Program",
                    ExchangeDate = DateTime.Now.AddMonths(-8)
                });
            }

            return list;
        }

       
        private static void AssignAnimalsToCages(MongoDbContext db)
        {
            var animals = db.Animals.Find(_ => true).ToList();
            var cages = db.Cages.Find(_ => true).ToList();

            foreach (var animal in animals)
            {
                var suitable = cages.FirstOrDefault(c =>
                    c.AllowedTypes.Contains(animal.Type) &&
                    c.Animals.Count < c.Capacity &&
                    (!animal.NeedsWarmShelter || c.Heated));

                if (suitable == null)
                    continue;

                if (!string.IsNullOrEmpty(animal.Id) && ObjectId.TryParse(animal.Id, out var animalOid))
                {
                    if (!suitable.Animals.Contains(animalOid))
                        suitable.Animals.Add(animalOid);

                    animal.CageId = suitable.Id.ToString();
                }
            }

           
            foreach (var animal in animals)
            {
                db.Animals.ReplaceOne(a => a.Id == animal.Id, animal);
            }

            foreach (var cage in cages)
            {
                db.Cages.ReplaceOne(c => c.Id == cage.Id, cage);
            }
        }

      
        private static void AssignEmployeesToAnimals(MongoDbContext db)
        {
            var animals = db.Animals.Find(_ => true).ToList();
            var employees = db.Employees.Find(_ => true).ToList();

            var vets = employees.Where(e => e.Category == "vet").ToList();
            var cleaners = employees.Where(e => e.Category == "cleaner").ToList();
            var trainers = employees.Where(e => e.Category == "trainer").ToList();

            
            var employeesById = employees.ToDictionary(e => e.Id, e => e);

            foreach (var animal in animals)
            {
                animal.EmployeesAssigned ??= new List<string>();

                
                foreach (var vet in vets)
                {
                    if (!animal.EmployeesAssigned.Contains(vet.Id))
                        animal.EmployeesAssigned.Add(vet.Id);

                    vet.AnimalsUnderCare ??= new List<string>();
                    if (!vet.AnimalsUnderCare.Contains(animal.Id))
                        vet.AnimalsUnderCare.Add(animal.Id);
                }

                
                if (!string.IsNullOrEmpty(animal.CageId) &&
                    ObjectId.TryParse(animal.CageId, out var cageOid))
                {
                    var cage = db.Cages.Find(c => c.Id == cageOid).FirstOrDefault();
                    if (cage != null && cleaners.Any())
                    {
                        var cleaner = cleaners.First(); // простий варіант
                        if (!animal.EmployeesAssigned.Contains(cleaner.Id))
                            animal.EmployeesAssigned.Add(cleaner.Id);

                        cleaner.AnimalsUnderCare ??= new List<string>();
                        if (!cleaner.AnimalsUnderCare.Contains(animal.Id))
                            cleaner.AnimalsUnderCare.Add(animal.Id);
                    }
                }

               
                if (animal.Type == "predator")
                {
                    foreach (var trainer in trainers)
                    {
                        if (!animal.EmployeesAssigned.Contains(trainer.Id))
                            animal.EmployeesAssigned.Add(trainer.Id);

                        trainer.AnimalsUnderCare ??= new List<string>();
                        if (!trainer.AnimalsUnderCare.Contains(animal.Id))
                            trainer.AnimalsUnderCare.Add(animal.Id);
                    }
                }
            }

           
            foreach (var animal in animals)
            {
                db.Animals.ReplaceOne(a => a.Id == animal.Id, animal);
            }

            foreach (var emp in employees)
            {
                db.Employees.ReplaceOne(e => e.Id == emp.Id, emp);
            }
        }
    }
}