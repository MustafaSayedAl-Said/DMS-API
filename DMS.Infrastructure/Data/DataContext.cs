using DMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Document> Documents { get; set; }

        public virtual DbSet<MyDirectory> Directories { get; set; }

        public virtual DbSet<Workspace> Workspaces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .HasOne(u => u.Workspace)
            .WithOne(w => w.User)
            .HasForeignKey<Workspace>(w => w.UserId);

            modelBuilder.Entity<MyDirectory>()
            .HasOne(d => d.Workspace)
            .WithMany(w => w.Directories)
            .HasForeignKey(d => d.WorkspaceId);

            modelBuilder.Entity<Document>()
            .HasOne(d => d.MyDirectory)
            .WithMany(dir => dir.Documents)
            .HasForeignKey(d => d.DirectoryId);

            // Seed data
            modelBuilder.Entity<Workspace>().HasData(
                new Workspace
                {
                    Id = 1,
                    Name = "Default Workspace",
                    UserId = 1
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    email = "admin@example.com",
                    password = "AdminPassword123",
                    NID = "1234567890",
                    Role = UserRole.Admin
                },
                new User
                {
                    Id = 2,
                    email = "user@example.com",
                    password = "UserPassword123",
                    NID = "0987654321",
                    Role = UserRole.User
                }
            );

            modelBuilder.Entity<MyDirectory>().HasData(
                new MyDirectory
                {
                    Id = 1,
                    Name = "Root Directory",
                    WorkspaceId = 1
                }
            );

            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    Id = 1,
                    Name = "Sample Document",
                    DirectoryId = 1
                }
            );
        }
    }
}
