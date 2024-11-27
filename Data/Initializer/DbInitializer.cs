using System;
using System.Linq;
using TransportCompanyWithAuthorize.Models;
using TransportCompanyWithAuthorize.Data;

namespace TransportCompanyWithAuthorize.Data.Initializer
{
    public static class DbInitializer
    {
        public static void Initialize(HairdressingContext db)
        {
            db.Database.EnsureCreated();

            if (db.Clients.Any())
            {
                return; // Если есть данные, инициализация не требуется
            }

            Random randObj = new(1);

            // Заполнение типов услуг
            string[] serviceTypeNames = { "Стрижка", "Укладка", "Окрашивание", "Спа-уход", "Массаж головы" };
            foreach (var serviceTypeName in serviceTypeNames)
            {
                db.ServiceTypes.Add(new ServiceType
                {
                    Name = serviceTypeName,
                    Description = "Описание типа услуги " + serviceTypeName
                });
            }
            db.SaveChanges();

            // Заполнение услуг
            var serviceTypes = db.ServiceTypes.ToList();
            for (int i = 0; i < 20; i++)
            {
                var serviceType = serviceTypes[randObj.Next(serviceTypes.Count)];
                db.Services.Add(new Services
                {
                    Name = "Услуга_" + i,
                    ServiceTypeId = serviceType.Id,
                    Description = "Описание услуги " + i,
                    Innovations = "Инновации для услуги " + i,
                    Price = randObj.Next(500, 5000)
                });
            }
            db.SaveChanges();

            // Заполнение клиентов
            string[] clientNames = { "Иван Иванов", "Петр Петров", "Сергей Сергеев", "Дмитрий Дмитриев", "Алексей Алексеев" };
            for (int i = 0; i < 15; i++)
            {
                db.Clients.Add(new Client
                {
                    FullName = clientNames[i % clientNames.Length] + "_" + i,
                    Address = "Адрес_" + i,
                    Phone = "+7 (900) 123-45-" + i.ToString("00"),
                    Discount = randObj.Next(0, 21),
                    TotalServicesCost = 0
                });
            }
            db.SaveChanges();

            // Заполнение сотрудников
            string[] employeeNames = { "Анна Антонова", "Мария Маркова", "Ольга Ольгина", "Екатерина Екатерина", "Татьяна Татьянова" };
            string[] positions = { "Мастер", "Ассистент" };
            for (int i = 0; i < 10; i++)
            {
                db.Employees.Add(new Employee
                {
                    FullName = employeeNames[i % employeeNames.Length] + "_" + i,
                    Position = positions[i % positions.Length],
                    Phone = "+7 (800) 555-35-" + i.ToString("00")
                });
            }
            db.SaveChanges();

            // Заполнение расписания сотрудников
            var employees = db.Employees.ToList();
            for (int i = 0; i < employees.Count; i++)
            {
                for (int day = 0; day < 30; day++)
                {
                    db.EmployeeSchedules.Add(new EmployeeSchedule
                    {
                        EmployeeId = employees[i].Id,
                        WorkDate = DateOnly.FromDateTime(DateTime.Now.AddDays(day)),
                        IsWorking = day % 2 == 0
                    });
                }
            }
            db.SaveChanges();

            // Заполнение выполненных услуг
            var clients = db.Clients.ToList();
            var services = db.Services.ToList();
            for (int i = 0; i < 50; i++)
            {
                var client = clients[randObj.Next(clients.Count)];
                var service = services[randObj.Next(services.Count)];
                var employee = employees[randObj.Next(employees.Count)];
                var serviceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(1, 30)));

                db.PerformedServices.Add(new PerformedService
                {
                    ClientId = client.Id,
                    ServiceId = service.Id,
                    EmployeeId = employee.Id,
                    ServiceDate = serviceDate,
                    Cost = service.Price
                });

                client.TotalServicesCost += service.Price;
            }
            db.SaveChanges();

            // Заполнение отзывов
            for (int i = 0; i < 30; i++)
            {
                var client = clients[randObj.Next(clients.Count)];
                var service = services[randObj.Next(services.Count)];

                db.Reviews.Add(new Review
                {
                    ClientId = client.Id,
                    ServiceId = service.Id,
                    ReviewText = "Отзыв клиента о " + service.Name,
                    ReviewDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(1, 15)))
                });
            }
            db.SaveChanges();
        }
    }
}
