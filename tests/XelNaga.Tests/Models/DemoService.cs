using Aiursoft.Scanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Models
{
    public class InDbEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Filter { get; set; }
    }
    public class SqlDbContext : DbContext
    {
        public DbSet<InDbEntity> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=test.db");
    }

    public class DemoService : ITransientDependency
    {
        public static bool Done;
        public static bool DoneAsync;
        public static int DoneTimes;
        private static object obj = new();
        private readonly SqlDbContext _dbContext;

        public DemoService(SqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DoSomethingSlow()
        {
            Done = false;
            Thread.Sleep(200);
            var _ = _dbContext.Records.ToList();
            Done = true;
        }

        public async Task DoSomethingSlowAsync()
        {
            Console.WriteLine("\a");
            DoneAsync = false;
            await Task.Delay(200);
            _dbContext.Records.Add(new InDbEntity { Content = "Test", Filter = "Test" });
            await _dbContext.SaveChangesAsync();
            DoneAsync = true;
            lock (obj)
            {
                DoneTimes++;
            }
        }
    }
}
