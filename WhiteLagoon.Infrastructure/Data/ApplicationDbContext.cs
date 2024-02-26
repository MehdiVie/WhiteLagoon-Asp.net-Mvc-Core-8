using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaNumber> VillaNumbers { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name= "Royal Villa",
                    Description= "The word \"villa\" is frequently used when describing home styles and design trends – so frequently, in fact, that it can be difficult to know exactly what it means. Our FAQ will go over what a villa residence is, where the term comes from, and how you can make plans for villa style homes. Anyone interested in building a home or making important renovations on a beloved property should take a look!",
                    Occupancy=4,
                    ImageUrl= "~/Static_Files/Images/Villa-Images/Royal-Villa-1.jpg",
                    Price=200,
                    Sqft=500
                },
                new Villa
                {
                    Id = 2,
                    Name = "Premium Pool Villa",
                    Description = "A villa style house is traditionally a more secluded house, often single-level, designed to be a home for a single family, usually on spacious property that puts it at a distance from other houses – or at least come with a private courtyard or other areas that solely belongs to the property owners. These homes often have lengthy driveways that help set them back from the road a bit for more privacy.",
                    Occupancy = 5,
                    ImageUrl = "~/Static_Files/Images/Villa-Images/Premium-Pool-Villa-1.jpg",
                    Price = 300,
                    Sqft = 500
                },
                new Villa
                {
                    Id = 3,
                    Name = "Luxury Pool Villa",
                    Description = "The name villa comes from ancient Rome, where it described a residence owned by wealthy citizens that served as a retreat from the populace, or a vacation estate on rich land. They were specifically designed to be as self-sufficient as possible and were frequently the site of vineyards or other important crops. Even today, the word villa is often associated with resorts, luxury, and getting away from everyday life.",
                    Occupancy = 6,
                    ImageUrl = "~/Static_Files/Images/Villa-Images/Luxury-Pool-Villa-1.jpg",
                    Price = 400,
                    Sqft = 700
                }
            );


            modelBuilder.Entity<VillaNumber>().HasData(

                new VillaNumber
                {
                    Villa_Number = 101,
                    VillaId = 1
                },
                new VillaNumber
                {
                    Villa_Number = 102,
                    VillaId = 1
                },
                new VillaNumber
                {
                    Villa_Number = 103,
                    VillaId = 1
                },
                new VillaNumber
                {
                    Villa_Number = 201,
                    VillaId = 2
                },
                new VillaNumber
                {
                    Villa_Number = 202,
                    VillaId = 2
                },
                new VillaNumber
                {
                    Villa_Number = 203,
                    VillaId = 2
                },
                new VillaNumber
                {
                    Villa_Number = 301,
                    VillaId = 3
                },
                new VillaNumber
                {
                    Villa_Number = 302,
                    VillaId = 3
                },
                new VillaNumber
                {
                    Villa_Number = 401,
                    VillaId = 4
                }
            );

            modelBuilder.Entity<Amenity>().HasData(
                new Amenity
                {
                    Id = 1,
                    VillaId=1,
                    Name="Private Pool"
                },
                new Amenity
                {
                    Id = 2,
                    VillaId = 1,
                    Name = "Microwave"
                },
                new Amenity
                {
                    Id = 3,
                    VillaId = 1,
                    Name = "Private Balcony"
                },
                new Amenity
                {
                    Id = 4,
                    VillaId = 2,
                    Name = "1 King Bed and Sofa Bed"
                },
                new Amenity
                {
                    Id = 5,
                    VillaId = 2,
                    Name = "Private Plunge Pool"
                },
                new Amenity
                {
                    Id = 6,
                    VillaId = 2,
                    Name = "Microwave and Mini Refrigerator"
                },
                new Amenity
                {
                    Id = 7,
                    VillaId = 3,
                    Name = "Private Balcony"
                },
                new Amenity
                {
                    Id = 8,
                    VillaId = 3,
                    Name = "Private Pool"
                },
                new Amenity
                {
                    Id = 9,
                    VillaId = 4,
                    Name = "Jacuzzi"
                },
                new Amenity
                {
                    Id = 10,
                    VillaId = 4,
                    Name = "Private Balcony"
                },
                new Amenity
                {
                    Id = 11,
                    VillaId = 11,
                    Name = "Private Plunge Pool"
                }
            );

        }
    }
}
