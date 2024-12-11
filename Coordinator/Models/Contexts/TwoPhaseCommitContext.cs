using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.Contexts
{
    public class TwoPhaseCommitContext : DbContext
    {
        public TwoPhaseCommitContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeState> NodeStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Diğer servislere dair seeddata oluşturuyoruz. Ve ardindan tekrardan migrate etmemiz gerekir.

            modelBuilder.Entity<Node>().HasData(
                new Node("Order.API") { Id = new Guid("00000000-0000-0000-0000-000000000001") },
                new Node("Payment.API") { Id = new Guid("00000000-0000-0000-0000-000000000002") },
                new Node("Stock.API") { Id = new Guid("00000000-0000-0000-0000-000000000003") }
            );

        }
    }
}
