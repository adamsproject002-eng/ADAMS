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

        public DbSet<Area> Area { get; set; }
        public DbSet<Pond> Pond { get; set; }
        public DbSet<FryRecord> FryRecord { get; set; }

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

            b.Entity<Pond>()
                .HasOne(p => p.Area)
                .WithMany(a => a.Ponds)
                .HasForeignKey(p => p.AreaSN)
                .OnDelete(DeleteBehavior.Restrict); // 刪除 Area 不會 cascade 刪除 Pond

            b.Entity<FryRecord>()
                .HasOne(f => f.Pond)
                .WithMany(p => p.FryRecords)
                .HasForeignKey(f => f.PondSN)
                .OnDelete(DeleteBehavior.Restrict); // 刪除 Pond 不會 cascade 刪除 FryRecord

        }
    }
}
