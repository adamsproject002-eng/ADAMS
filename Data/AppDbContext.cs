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
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Models.TimeZone> TimeZone { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<Pond> Pond { get; set; }
        public DbSet<FishVariety> FishVariety { get; set; }
        public DbSet<Fry> Fry { get; set; }
        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Tenant ↔ AccountGroup
            b.Entity<AccountGroup>()
                .HasOne(g => g.Tenant)
                .WithMany(t => t.AccountGroups)
                .HasForeignKey(g => g.TenantSN)
                .OnDelete(DeleteBehavior.Restrict);

            //一個 Tenant 有多個 Account
            b.Entity<Account>()
                .HasOne(a => a.Tenant)
                .WithMany(t => t.Accounts)
                .HasForeignKey(a => a.TenantSN)
                .OnDelete(DeleteBehavior.Restrict);

            // Account ↔ AccountGroup
            b.Entity<Account>()
                .HasOne(a => a.AccGroup)
                .WithMany(g => g.Accounts)
                .HasForeignKey(a => a.AccGroupSN)
                .OnDelete(DeleteBehavior.Restrict);

            // AccountGroup ↔ Authorization
            b.Entity<Authorization>()
                .HasOne(a => a.AccountGroup)
                .WithMany(g => g.Authorizations)
                .HasForeignKey(a => a.AccGroupSN)
                .OnDelete(DeleteBehavior.Restrict);

            // Function ↔ Authorization
            b.Entity<Authorization>()
                .HasOne(a => a.Function)
                .WithMany(f => f.Authorizations)
                .HasForeignKey(a => a.FunctionSN)
                .OnDelete(DeleteBehavior.Restrict);

            //一個 Tenant 有多個 Area
            b.Entity<Area>()
                .HasOne(a => a.Tenant)
                .WithMany(t => t.Areas)
                .HasForeignKey(a => a.TenantSN)
                .OnDelete(DeleteBehavior.Restrict);

            //一個 Tenant 有多個 Pond
            b.Entity<Pond>()
                .HasOne(p => p.Tenant)
                .WithMany(t => t.Ponds)
                .HasForeignKey(p => p.TenantSN)
                .OnDelete(DeleteBehavior.Restrict);

            //一個 Area 有多個 Pond
            b.Entity<Pond>()
                .HasOne(p => p.Area)
                .WithMany(a => a.Ponds)
                .HasForeignKey(p => p.AreaSN)
                .OnDelete(DeleteBehavior.Restrict);

            // 一個Tenant 有多個 Fry
            b.Entity<Fry>()
                .HasOne(f => f.Tenant)
                .WithMany(t => t.Fries)
                .HasForeignKey(f => f.TenantSN)
                .OnDelete(DeleteBehavior.Restrict);

            // 一個Supplier 有多個 Fry
            b.Entity<Fry>()
                .HasOne(f => f.Supplier)
                .WithMany()
                .HasForeignKey(f => f.SupplierSN)
                .OnDelete(DeleteBehavior.Restrict);

            //一個 FishVariety 有多個 Fry
            b.Entity<Fry>()
                .HasOne(f => f.FishVariety)
                .WithMany(v => v.Fries)
                .HasForeignKey(f => f.FVSN)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
