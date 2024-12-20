using Microsoft.EntityFrameworkCore;
using studentsAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Allows requests from any origin
              .AllowAnyMethod() // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
              .AllowAnyHeader(); // Allows any HTTP headers
    });
});

// Add services to the container.
builder.Services.AddDbContext<StudentContext>(optoins => optoins.UseSqlServer(builder.Configuration.GetConnectionString("defcon")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
