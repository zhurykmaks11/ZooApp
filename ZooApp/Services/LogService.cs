using System;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class LogService
    {
        private readonly IMongoCollection<LogRecord> _logs;

        public LogService(MongoDbContext context)
        {
            _logs = context.Logs;
        }

        public void Write(string user, string action, string details = "")
        {
            var log = new LogRecord
            {
                Timestamp = DateTime.Now,
                User = user,
                Action = action,
                Details = details
            };

            _logs.InsertOne(log);
        }

        public List<LogRecord> GetAll()
        {
            return _logs.Find(_ => true)
                .SortByDescending(l => l.Timestamp)
                .ToList();
        }
    }
}