using Coordinator.Enums;

namespace Coordinator.Models
{
    public record NodeState (Guid TransactionId)
    {
        // Kullanıcıdan gelen talep doğrultusunda node'ların state yani durumlarını veritabanında tutmuş olacağız.
        public Guid Id { get; set; }

        /// <summary>
        /// 1. aşamanın durumunu ifade ediyor
        /// </summary>
        public ReadyType IsReady { get; set; }
        /// <summary>
        /// 2.aşamanın neticesini ifade eder.
        /// </summary>
        public TransactionState TransactionState { get; set; }
        public Node Node { get; set; }
    }
}
