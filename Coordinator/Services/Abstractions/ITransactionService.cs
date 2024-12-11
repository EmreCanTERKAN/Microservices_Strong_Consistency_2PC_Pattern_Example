namespace Coordinator.Services.Abstractions
{
    public interface ITransactionService
    {
        //Hazır olduğunda yada olmadığında geriye dönen değerle istediğimiz fonsiyonu gerçekleştiriyoruz.
        Task<bool> CheckReadyServicesAsync(Guid transactionId);
        //Transactionları kontrol etmemiz gerekmektedir. Yani servisler başarılı bir şekilde tamamladı mı tamamlamadı mı ?
        Task<bool> CheckTransactionStateServicesAsync(Guid transactionId);
        // İkinci talimat olan kordinatörün ikinci talimatı vermesi gerekmektedir.
        Task CommitAsync(Guid transactionId);
        //Transaction oluşturmamız içindir.
        Task<Guid> CreateTransactionAsync();
        // kullanıcıdan gelen talep doğrultusunda servislerin hazır olup olmadığını bu metotda kontrol edeceğiz..
        Task PrepareServicesAsync(Guid transactionId);
        // Transactionlar başarısız olduğu zaman geriye Rollback döndürmemiz gerekecektir.
        Task RollbackAsync(Guid transactionId);

    }
}
