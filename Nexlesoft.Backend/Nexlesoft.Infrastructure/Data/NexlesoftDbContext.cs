using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nexlesoft.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexlesoft.Infrastructure.Data
{
    //public class NexlesoftDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    public class NexlesoftDbContext : DbContext
    {
        public NexlesoftDbContext(DbContextOptions<NexlesoftDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).HasMaxLength(32);
                entity.Property(e => e.LastName).HasMaxLength(32);
                entity.Property(e => e.Email).HasMaxLength(64);
                entity.Property(e => e.Hash).HasMaxLength(255);
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.UpdatedAt) ;
            });
            
            modelBuilder.Entity<Token>(entity =>
            {
                entity.ToTable("token");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId);
                entity.Property(e => e.RefreshToken);
                entity.Property(e => e.ExpiresIn).HasMaxLength(64);
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.UpdatedAt);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Tokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
