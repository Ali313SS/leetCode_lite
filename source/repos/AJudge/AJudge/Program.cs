
using AJudge.Infrastructure.Data;
using AJudge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AJudge.Application.services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Repositories;
using System.Runtime.Serialization;
using System.Reflection;

namespace AJudge
{
    public class Program
    {
       
        public static void Main(string[] args)
        {
            // Corrected the SMTP server address and fixed the typo in "smtp.gmail.com"

            //ISender x = new Sender("smtp.gmail.com", 587, "ara010250@gmail.com", "******");


            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<PasswordHasher>();

            builder.Services.AddScoped<IGroupServices, GroupServices>();
            builder.Services.AddScoped<ISubmissionService, SubmissionService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IContestServices, ContestServices>();

            builder.Services.AddTransient<IProblemService, ProblemService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Ensure the Microsoft.EntityFrameworkCore.SqlServer package is installed
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnstring"));
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"] ??
                                throw new InvalidOperationException("JWT SigningKey is not configured")))
                    };
                });
           

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AJudge API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer {token}"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            new string[] {}
        }
    });


                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
            }
            
            );
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            

            
            

            var app = builder.Build();
            app.UseCors("AllowAll");
            // Configure the HTTP request pipeline.
            //   if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
                
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
             
}