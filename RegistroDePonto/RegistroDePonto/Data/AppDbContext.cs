using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<RegistroPonto> RegistrosPonto { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RegistroPonto>()
            .HasOne(r => r.Funcionario)
            .WithMany(f => f.RegistrosPonto)
            .HasForeignKey(r => r.MatriculaFuncionario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
