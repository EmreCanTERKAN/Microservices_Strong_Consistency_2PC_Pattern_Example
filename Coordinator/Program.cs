using Coordinator.Models.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SQLServer");

builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
{
   options.UseSqlServer(connectionString);
});

builder.Services.AddHttpClient("Order.API", client => client.BaseAddress = new("https://localhost:7132"));
builder.Services.AddHttpClient("Payment.API", client => client.BaseAddress = new("https://localhost:7067"));
builder.Services.AddHttpClient("Stock.API", client => client.BaseAddress = new("https://localhost:7215"));

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
