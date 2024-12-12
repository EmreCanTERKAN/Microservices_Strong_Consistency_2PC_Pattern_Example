using Coordinator.Models.Contexts;
using Coordinator.Services;
using Coordinator.Services.Abstractions;
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

builder.Services.AddTransient<ITransactionService, TransactionService>();   

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Bir endpoint tan�mlar�z
app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1 - Prepare
    var transactionID = await transactionService.CreateTransactionAsync();
    // T�m servislerde gerekli kontrolleri ger�ekle�tiriyoruz.
     await transactionService.PrepareServicesAsync(transactionID);
    // �kinci a�amaya ge�meden transaction�n genel bir state'ine bakmam�z gerekecektir.
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionID);

    if (transactionState)
    {
        //Phase 2 - Commit
        await transactionService.CommitAsync(transactionID);
        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionID);
    }

    // Bu a�amalar neticesinde transactionState 
    if (!transactionState)
        await transactionService.RollbackAsync(transactionID);
});


app.Run();
