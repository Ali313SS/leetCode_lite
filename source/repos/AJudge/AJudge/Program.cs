
using AJudge.Infrastructure.Data;
using AJudge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AJudge.Application.services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AJudge
{
    public class Program
    {
       
        public static void Main(string[] args)
        {
            // Corrected the SMTP server address and fixed the typo in "smtp.gmail.com"
            
            //ISender x = new Sender("smtp.gmail.com", 587, "ara010250@gmail.com", "A.R.A313");
            
    
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<PasswordHasher>();
        
            // Ensure the Microsoft.EntityFrameworkCore.SqlServer package is installed
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnstring"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
    }
             
}