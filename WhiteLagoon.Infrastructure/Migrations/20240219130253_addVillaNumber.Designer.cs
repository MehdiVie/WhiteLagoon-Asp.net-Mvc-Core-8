﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WhiteLagoon.Infrastructure.Data;

#nullable disable

namespace WhiteLagoon.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240219130253_addVillaNumber")]
    partial class addVillaNumber
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.1.24081.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.Villa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("Create_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Occupancy")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Sqft")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Update_Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Villas");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "The word \"villa\" is frequently used when describing home styles and design trends – so frequently, in fact, that it can be difficult to know exactly what it means. Our FAQ will go over what a villa residence is, where the term comes from, and how you can make plans for villa style homes. Anyone interested in building a home or making important renovations on a beloved property should take a look!",
                            ImageUrl = "~/Static_Files/Images/Villa-Images/Royal-Villa-1.jpg",
                            Name = "Royal Villa",
                            Occupancy = 4,
                            Price = 200.0,
                            Sqft = 500
                        },
                        new
                        {
                            Id = 2,
                            Description = "A villa style house is traditionally a more secluded house, often single-level, designed to be a home for a single family, usually on spacious property that puts it at a distance from other houses – or at least come with a private courtyard or other areas that solely belongs to the property owners. These homes often have lengthy driveways that help set them back from the road a bit for more privacy.",
                            ImageUrl = "~/Static_Files/Images/Villa-Images/Premium-Pool-Villa-1.jpg",
                            Name = "Premium Pool Villa",
                            Occupancy = 5,
                            Price = 300.0,
                            Sqft = 500
                        },
                        new
                        {
                            Id = 3,
                            Description = "The name villa comes from ancient Rome, where it described a residence owned by wealthy citizens that served as a retreat from the populace, or a vacation estate on rich land. They were specifically designed to be as self-sufficient as possible and were frequently the site of vineyards or other important crops. Even today, the word villa is often associated with resorts, luxury, and getting away from everyday life.",
                            ImageUrl = "~/Static_Files/Images/Villa-Images/Luxury-Pool-Villa-1.jpg",
                            Name = "Luxury Pool Villa",
                            Occupancy = 6,
                            Price = 400.0,
                            Sqft = 700
                        });
                });

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.VillaNumber", b =>
                {
                    b.Property<int>("Villa_Number")
                        .HasColumnType("int");

                    b.Property<string>("SpecialDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VillaId")
                        .HasColumnType("int");

                    b.HasKey("Villa_Number");

                    b.HasIndex("VillaId");

                    b.ToTable("VillaNumbers");

                    b.HasData(
                        new
                        {
                            Villa_Number = 101,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 102,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 103,
                            VillaId = 1
                        },
                        new
                        {
                            Villa_Number = 201,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 202,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 203,
                            VillaId = 2
                        },
                        new
                        {
                            Villa_Number = 301,
                            VillaId = 3
                        },
                        new
                        {
                            Villa_Number = 302,
                            VillaId = 3
                        },
                        new
                        {
                            Villa_Number = 401,
                            VillaId = 4
                        });
                });

            modelBuilder.Entity("WhiteLagoon.Domain.Entities.VillaNumber", b =>
                {
                    b.HasOne("WhiteLagoon.Domain.Entities.Villa", "Villa")
                        .WithMany()
                        .HasForeignKey("VillaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Villa");
                });
#pragma warning restore 612, 618
        }
    }
}
