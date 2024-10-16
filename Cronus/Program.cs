
using Serilog;
using Cronus.Source.Database;
using Cronus.Source.Database.Entities;
using Cronus.Source.Repositories;
using Cronus.Source.Utilities;

namespace Cronus
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string connectionString = "Host=localhost;Port=5432;Database=dbtest;Username=postgres;Password=a";

                using var dbContext = new DatabaseContext(connectionString);
                var userRepository = new Repository<User>(connectionString);

                var newUser = new User
                {
                    Username = "hi",
                    Email = "hi@hi.com",
                    Password = "hihihi"
                };

                await userRepository.SaveAsync(newUser);
                 

                Logger.Information($"User {newUser.Username} created with ID: {newUser.Id}");

                var user = await userRepository.FindByIdAsync(newUser.Id);
                Log.Information($"Found User: {newUser.Username}, Email: {newUser.Email}");

                var users = await userRepository.GetAllAsync();
                Log.Information("All Users:");
                foreach (var u in users)
                {
                    Log.Information("Username: {Username}, Email: {Email}", u.Username, u.Email);
                }

                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured: {ex.Message}");
            } 

        }
    }
}