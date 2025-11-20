using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotless.Infrastructure.Migrations.Application
{
    [DbContext(typeof(Spotless.Infrastructure.Context.ApplicationDbContext))]
    [Migration("20251120024730_AddCartAndServiceMaxWeight")]
    partial class AddCartAndServiceMaxWeight
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("Spotless.Domain.Entities.Cart", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
                b.Property<Guid>("CustomerId").HasColumnType("uniqueidentifier");
                b.Property<DateTime>("CreatedDate").HasColumnType("datetime2");
                b.Property<DateTime>("LastModifiedDate").HasColumnType("datetime2");
                b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
                b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
                b.HasKey("Id");
                b.ToTable("Carts");
            });

            modelBuilder.Entity("Spotless.Domain.Entities.CartItem", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
                b.Property<Guid>("CartId").HasColumnType("uniqueidentifier");
                b.Property<Guid>("ServiceId").HasColumnType("uniqueidentifier");
                b.Property<int>("Quantity").HasColumnType("int");
                b.Property<DateTime>("AddedDate").HasColumnType("datetime2");
                b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
                b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
                b.HasKey("Id");
                b.HasIndex("CartId");
                b.HasIndex("ServiceId");
                b.ToTable("CartItems");
            });

            modelBuilder.Entity("Spotless.Domain.Entities.Service", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
                b.Property<string>("Name").IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
                b.Property<decimal>("MaxWeightKg").HasColumnType("decimal(18,2)");
                b.HasKey("Id");
                b.ToTable("Services");
            });
        }
    }
}
