namespace Coordinator.Services.Abstractions
{
    public interface ITransactionService
    {
        //Transaction oluşturmamız içindir.
        Task<Guid> CreateTransaction();
        // kullanıcıdan gelen talep doğrultusunda servislerin hazır olup olmadığını bu metotda kontrol edeceğiz..
        Task PrepareServices(Guid transactionId);
        //Hazır olduğunda yada olmadığında geriye dönen değerle istediğimiz fonsiyonu gerçekleştiriyoruz.
        Task<bool>CheckReadyServices(Guid transactionId);

    }
}
