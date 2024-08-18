﻿// <auto-generated />
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DMS.Infrastructure.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240818112354_DocumentContent_prop")]
    partial class DocumentContent_prop
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DMS.Core.Entities.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DirectoryId")
                        .HasColumnType("int");

                    b.Property<string>("DocumentContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DirectoryId");

                    b.ToTable("Documents");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DirectoryId = 1,
                            Name = "Sample Document"
                        });
                });

            modelBuilder.Entity("DMS.Core.Entities.MyDirectory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("Directories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Root Directory",
                            WorkspaceId = 1
                        });
                });

            modelBuilder.Entity("DMS.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("NID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            NID = "1234567890",
                            Role = 0,
                            email = "admin@example.com",
                            password = "AdminPassword123"
                        },
                        new
                        {
                            Id = 2,
                            NID = "0987654321",
                            Role = 1,
                            email = "user@example.com",
                            password = "UserPassword123"
                        });
                });

            modelBuilder.Entity("DMS.Core.Entities.Workspace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Workspaces");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Default Workspace",
                            UserId = 1
                        });
                });

            modelBuilder.Entity("DMS.Core.Entities.Document", b =>
                {
                    b.HasOne("DMS.Core.Entities.MyDirectory", "MyDirectory")
                        .WithMany("Documents")
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MyDirectory");
                });

            modelBuilder.Entity("DMS.Core.Entities.MyDirectory", b =>
                {
                    b.HasOne("DMS.Core.Entities.Workspace", "Workspace")
                        .WithMany("Directories")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("DMS.Core.Entities.Workspace", b =>
                {
                    b.HasOne("DMS.Core.Entities.User", "User")
                        .WithOne("Workspace")
                        .HasForeignKey("DMS.Core.Entities.Workspace", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DMS.Core.Entities.MyDirectory", b =>
                {
                    b.Navigation("Documents");
                });

            modelBuilder.Entity("DMS.Core.Entities.User", b =>
                {
                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("DMS.Core.Entities.Workspace", b =>
                {
                    b.Navigation("Directories");
                });
#pragma warning restore 612, 618
        }
    }
}
