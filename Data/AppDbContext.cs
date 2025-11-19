using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Account { get; set; }
        public DbSet<AccountGroup> AccountGroup { get; set; }
        public DbSet<Authorization> Authorization { get; set; }
        public DbSet<Function> Function { get; set; }
        public DbSet<Tenant> Tenant { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // 一個 Tenant 可有多個 AccountGroup，但刪除時不 cascade
            b.Entity<AccountGroup>()
                .HasOne(g => g.Tenant)
                .WithMany(t => t.AccountGroups)
                .HasForeignKey(g => g.TenantSN)
                .OnDelete(DeleteBehavior.Restrict); 

            // 一個 Account 可對應一個 Tenant（nullable 可允許 null）
            b.Entity<Account>()
                .HasOne(a => a.Tenant)
                .WithMany()
                .HasForeignKey(a => a.TenantSN)
                .OnDelete(DeleteBehavior.Restrict); 

            // 一個 Account 可對應一個 AccountGroup
            b.Entity<Account>()
                .HasOne(a => a.AccGroup)
                .WithMany(g => g.Accounts)
                .HasForeignKey(a => a.AccGroupSN)
                .OnDelete(DeleteBehavior.Restrict); 

            // 一個 AccountGroup 有多個 Authorization
            b.Entity<Authorization>()
                .HasOne(a => a.AccountGroup)
                .WithMany(g => g.Authorizations)
                .HasForeignKey(a => a.AccGroupSN)
                .OnDelete(DeleteBehavior.Restrict);

            // 一個 Function 有多個 Authorization
            b.Entity<Authorization>()
                .HasOne(a => a.Function)
                .WithMany(f => f.Authorizations)
                .HasForeignKey(a => a.FunctionSN)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
