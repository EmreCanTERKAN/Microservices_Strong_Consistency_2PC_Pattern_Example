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
// Bir endpoint tanýmlarýz
app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1 - Prepare
    var transactionID = await transactionService.CreateTransactionAsync();
    // Tüm servislerde gerekli kontrolleri gerçekleþtiriyoruz.
     await transactionService.PrepareServicesAsync(transactionID);
    // Ýkinci aþamaya geçmeden transactionýn genel bir state'ine bakmamýz gerekecektir.
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionID);

    if (transactionState)
    {
        //Phase 2 - Commit
        await transactionService.CommitAsync(transactionID);
        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionID);
    }

    // Bu aþamalar neticesinde transactionState 
    if (!transactionState)
        await transactionService.RollbackAsync(transactionID);
});


app.Run();
