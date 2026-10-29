using HotelManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created (or migrate if migrations are applied)
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Roles
            string[] roles = ["Admin", "Customer"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Admin User
            var adminEmail = "admin@grandaurelia.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Director of Hospitality",
                    EmailConfirmed = true,
                    PhoneNumber = "+1 (800) 555-0199",
                    Address = "700 Luxury Boulevard, Suite A, Grand Aurelia",
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Seed Demo Customer User
            var customerEmail = "customer@gmail.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);
            if (customerUser == null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FullName = "Alexander Wright",
                    EmailConfirmed = true,
                    PhoneNumber = "+1 (555) 432-8765",
                    Address = "14 Kensington Crest, New York, NY",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                };
                var result = await userManager.CreateAsync(customerUser, "Customer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, "Customer");
                }
            }

            // 4. Seed Facilities
            if (!await context.Facilities.AnyAsync())
            {
                var facilities = new List<Facility>
                {
                    new() { Name = "Ultra High-Speed Wi-Fi 6", IconClass = "bi-wifi", Description = "Complimentary gigabit mesh Wi-Fi covering all rooms and terraces" },
                    new() { Name = "65\" OLED Smart Cinema TV", IconClass = "bi-tv", Description = "Equipped with Netflix, Apple TV, Prime & international channels" },
                    new() { Name = "Private Heated Jacuzzi", IconClass = "bi-water", Description = "Hydro-massage tub with therapeutic chromotherapy lights" },
                    new() { Name = "Skyline Panoramic Balcony", IconClass = "bi-building-up", Description = "Private terrace offering unhindered sunset and city views" },
                    new() { Name = "Nespresso & Artisan Tea Bar", IconClass = "bi-cup-hot", Description = "Premium roast capsules, organic teas, and crystal glassware" },
                    new() { Name = "Italian Calacatta Marble Bath", IconClass = "bi-droplet-half", Description = "Walk-in monsoon rain shower and designer toiletries" },
                    new() { Name = "24/7 Dedicated Butler Service", IconClass = "bi-person-badge", Description = "On-demand personal concierge and unpacking service" },
                    new() { Name = "Dual-Zone Climate Control", IconClass = "bi-snow", Description = "Whisper-quiet intelligent micro-climate regulation" },
                    new() { Name = "Aurelia Signature Cloud Bed", IconClass = "bi-moon-stars", Description = "Custom memory mattress with 800-thread Egyptian cotton linen" },
                    new() { Name = "In-Room Biometric Safe", IconClass = "bi-shield-lock", Description = "Fits 16-inch laptops with internal charging port" }
                };

                await context.Facilities.AddRangeAsync(facilities);
                await context.SaveChangesAsync();
            }

            // 5. Seed Room Types
            if (!await context.RoomTypes.AnyAsync())
            {
                var roomTypes = new List<RoomType>
                {
                    new()
                    {
                        Name = "Deluxe Heritage King Room",
                        Description = "Sophisticated sanctuary featuring hand-carved wood accents, plush king bedding, bespoke reading nook, and expansive floor-to-ceiling windows overlooking manicured gardens.",
                        BasePrice = 280.00m,
                        DefaultCapacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=1200&q=80"
                    },
                    new()
                    {
                        Name = "Executive Skyline Suite",
                        Description = "Corner residence elevated above the city with separate living salon, marble dining bar, executive workstation, and an opulent Italian marble bathroom with deep soaking tub.",
                        BasePrice = 450.00m,
                        DefaultCapacity = 3,
                        ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=1200&q=80"
                    },
                    new()
                    {
                        Name = "Royal Aurelia Penthouse",
                        Description = "The pinnacle of grand living. Double-height ceilings, private panoramic terrace with heated spa tub, custom bar lounge, dining salon for six, and private butler pantry.",
                        BasePrice = 850.00m,
                        DefaultCapacity = 4,
                        ImageUrl = "https://images.unsplash.com/photo-1591088398332-8a7791972843?auto=format&fit=crop&w=1200&q=80"
                    },
                    new()
                    {
                        Name = "Grand Garden Pool Villa",
                        Description = "Secluded private sanctuary enveloped in lush subtropical flora. Private plunge pool, sun loungers, outdoor al fresco rain shower, and seamless indoor-outdoor living pavilion.",
                        BasePrice = 1100.00m,
                        DefaultCapacity = 5,
                        ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1200&q=80"
                    }
                };

                await context.RoomTypes.AddRangeAsync(roomTypes);
                await context.SaveChangesAsync();
            }

            // 6. Seed Rooms
            if (!await context.Rooms.AnyAsync())
            {
                var deluxeType = await context.RoomTypes.FirstAsync(t => t.Name.Contains("Deluxe"));
                var execType = await context.RoomTypes.FirstAsync(t => t.Name.Contains("Executive"));
                var royalType = await context.RoomTypes.FirstAsync(t => t.Name.Contains("Royal"));
                var villaType = await context.RoomTypes.FirstAsync(t => t.Name.Contains("Villa"));

                var allFacilities = await context.Facilities.ToListAsync();

                var rooms = new List<Room>
                {
                    new()
                    {
                        RoomNumber = "101",
                        RoomTypeId = deluxeType.Id,
                        PricePerNight = 280.00m,
                        Capacity = 2,
                        FloorNumber = 1,
                        Status = RoomStatus.Available,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1000&q=80",
                        Description = "Charming garden-facing deluxe room bathed in morning sunlight, with king size cloud bed and designer velvet armchairs."
                    },
                    new()
                    {
                        RoomNumber = "102",
                        RoomTypeId = deluxeType.Id,
                        PricePerNight = 295.00m,
                        Capacity = 2,
                        FloorNumber = 1,
                        Status = RoomStatus.Available,
                        IsFeatured = false,
                        ImageUrl = "https://images.unsplash.com/photo-1595526114035-0d45ed16cfbf?auto=format&fit=crop&w=1000&q=80",
                        Description = "Tranquil courtyard room featuring artisan oak millwork, walk-in rain shower and French press coffee station."
                    },
                    new()
                    {
                        RoomNumber = "201",
                        RoomTypeId = execType.Id,
                        PricePerNight = 450.00m,
                        Capacity = 3,
                        FloorNumber = 2,
                        Status = RoomStatus.Available,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?auto=format&fit=crop&w=1000&q=80",
                        Description = "Corner suite offering 180-degree skyline vistas, curated contemporary art pieces, and an ergonomic marble executive desk."
                    },
                    new()
                    {
                        RoomNumber = "202",
                        RoomTypeId = execType.Id,
                        PricePerNight = 475.00m,
                        Capacity = 3,
                        FloorNumber = 2,
                        Status = RoomStatus.Booked,
                        IsFeatured = false,
                        ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=1000&q=80",
                        Description = "Sophisticated suite with private sunset viewing balcony, bespoke cocktail cabinet, and soaking freestanding tub."
                    },
                    new()
                    {
                        RoomNumber = "301",
                        RoomTypeId = royalType.Id,
                        PricePerNight = 850.00m,
                        Capacity = 4,
                        FloorNumber = 3,
                        Status = RoomStatus.Available,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1591088398332-8a7791972843?auto=format&fit=crop&w=1000&q=80",
                        Description = "Magnificent Presidential sanctuary boasting a private terrace jacuzzi, crystal chandeliers, grand piano, and butler quarters."
                    },
                    new()
                    {
                        RoomNumber = "302",
                        RoomTypeId = royalType.Id,
                        PricePerNight = 890.00m,
                        Capacity = 4,
                        FloorNumber = 3,
                        Status = RoomStatus.Available,
                        IsFeatured = false,
                        ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=1000&q=80",
                        Description = "Opulent penthouse suite with marble fireplace, walk-in dressing room, sauna shower, and private elevator landing."
                    },
                    new()
                    {
                        RoomNumber = "V-01",
                        RoomTypeId = villaType.Id,
                        PricePerNight = 1100.00m,
                        Capacity = 5,
                        FloorNumber = 1,
                        Status = RoomStatus.Available,
                        IsFeatured = true,
                        ImageUrl = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?auto=format&fit=crop&w=1000&q=80",
                        Description = "Exclusive standalone estate villa with private infinity plunge pool, outdoor dining cabana, private chef grill, and zen reflection pond."
                    },
                    new()
                    {
                        RoomNumber = "V-02",
                        RoomTypeId = villaType.Id,
                        PricePerNight = 1250.00m,
                        Capacity = 6,
                        FloorNumber = 1,
                        Status = RoomStatus.Maintenance,
                        IsFeatured = false,
                        ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?auto=format&fit=crop&w=1000&q=80",
                        Description = "Two-bedroom grand residence villa featuring private heated pool, tropical daybeds, private butler pantry, and manicured grounds."
                    }
                };

                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();

                // Attach facilities to rooms
                var wifi = allFacilities.FirstOrDefault(f => f.IconClass == "bi-wifi");
                var tv = allFacilities.FirstOrDefault(f => f.IconClass == "bi-tv");
                var jacuzzi = allFacilities.FirstOrDefault(f => f.IconClass == "bi-water");
                var balcony = allFacilities.FirstOrDefault(f => f.IconClass == "bi-building-up");
                var coffee = allFacilities.FirstOrDefault(f => f.IconClass == "bi-cup-hot");
                var marbleBath = allFacilities.FirstOrDefault(f => f.IconClass == "bi-droplet-half");
                var butler = allFacilities.FirstOrDefault(f => f.IconClass == "bi-person-badge");
                var ac = allFacilities.FirstOrDefault(f => f.IconClass == "bi-snow");
                var cloudBed = allFacilities.FirstOrDefault(f => f.IconClass == "bi-moon-stars");
                var safe = allFacilities.FirstOrDefault(f => f.IconClass == "bi-shield-lock");

                var roomFacilities = new List<RoomFacility>();

                foreach (var room in rooms)
                {
                    // Every room gets wifi, ac, cloud bed, safe
                    if (wifi != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = wifi.Id });
                    if (ac != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = ac.Id });
                    if (cloudBed != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = cloudBed.Id });
                    if (safe != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = safe.Id });
                    if (tv != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = tv.Id });
                    if (coffee != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = coffee.Id });

                    // Suites and Villas get jacuzzi, balcony, marble bath, butler
                    if (room.PricePerNight >= 400.00m)
                    {
                        if (balcony != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = balcony.Id });
                        if (marbleBath != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = marbleBath.Id });
                    }

                    if (room.PricePerNight >= 800.00m)
                    {
                        if (jacuzzi != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = jacuzzi.Id });
                        if (butler != null) roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = butler.Id });
                    }
                }

                await context.RoomFacilities.AddRangeAsync(roomFacilities);
                await context.SaveChangesAsync();
            }

            // 7. Seed Sample Bookings & Payments for Dashboard Analytics
            if (!await context.Bookings.AnyAsync() && customerUser != null)
            {
                var room202 = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == "202");
                var room101 = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == "101");
                var room301 = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == "301");

                if (room202 != null)
                {
                    var booking1 = new Booking
                    {
                        BookingReference = "AUR-928174",
                        UserId = customerUser.Id,
                        RoomId = room202.Id,
                        CheckInDate = DateTime.Today.AddDays(-1),
                        CheckOutDate = DateTime.Today.AddDays(2),
                        TotalNights = 3,
                        SubTotal = room202.PricePerNight * 3,
                        TaxAmount = (room202.PricePerNight * 3) * 0.12m,
                        TotalPrice = (room202.PricePerNight * 3) * 1.12m,
                        Adults = 2,
                        Children = 0,
                        SpecialRequests = "Late arrival around 8:00 PM. High floor preferred.",
                        Status = BookingStatus.CheckedIn,
                        ActualCheckInTime = DateTime.UtcNow.AddHours(-18),
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    };

                    booking1.Payments.Add(new Payment
                    {
                        Amount = booking1.TotalPrice,
                        PaymentMethod = "Credit Card",
                        TransactionId = "TXN-CC-84729104",
                        Status = PaymentStatus.Completed,
                        PaymentDate = DateTime.UtcNow.AddDays(-10),
                        Notes = "Online prepayment via Visa Signature card ending in 4102."
                    });

                    await context.Bookings.AddAsync(booking1);
                }

                if (room101 != null)
                {
                    var booking2 = new Booking
                    {
                        BookingReference = "AUR-615243",
                        UserId = customerUser.Id,
                        RoomId = room101.Id,
                        CheckInDate = DateTime.Today.AddDays(5),
                        CheckOutDate = DateTime.Today.AddDays(8),
                        TotalNights = 3,
                        SubTotal = room101.PricePerNight * 3,
                        TaxAmount = (room101.PricePerNight * 3) * 0.12m,
                        TotalPrice = (room101.PricePerNight * 3) * 1.12m,
                        Adults = 2,
                        Children = 1,
                        SpecialRequests = "Please arrange an extra baby crib.",
                        Status = BookingStatus.Confirmed,
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    };

                    booking2.Payments.Add(new Payment
                    {
                        Amount = booking2.TotalPrice,
                        PaymentMethod = "UPI / Net Banking",
                        TransactionId = "TXN-UPI-99201844",
                        Status = PaymentStatus.Completed,
                        PaymentDate = DateTime.UtcNow.AddDays(-2),
                        Notes = "Verified instant instant payment reference UPI-0912."
                    });

                    await context.Bookings.AddAsync(booking2);
                }

                if (room301 != null)
                {
                    var booking3 = new Booking
                    {
                        BookingReference = "AUR-384920",
                        UserId = customerUser.Id,
                        RoomId = room301.Id,
                        CheckInDate = DateTime.Today.AddDays(-14),
                        CheckOutDate = DateTime.Today.AddDays(-11),
                        TotalNights = 3,
                        SubTotal = room301.PricePerNight * 3,
                        TaxAmount = (room301.PricePerNight * 3) * 0.12m,
                        TotalPrice = (room301.PricePerNight * 3) * 1.12m,
                        Adults = 2,
                        Children = 0,
                        SpecialRequests = "Anniversary celebration bottle of champagne on ice.",
                        Status = BookingStatus.CheckedOut,
                        ActualCheckInTime = DateTime.UtcNow.AddDays(-14),
                        ActualCheckOutTime = DateTime.UtcNow.AddDays(-11),
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    };

                    booking3.Payments.Add(new Payment
                    {
                        Amount = booking3.TotalPrice,
                        PaymentMethod = "Credit Card",
                        TransactionId = "TXN-AMEX-1029384",
                        Status = PaymentStatus.Completed,
                        PaymentDate = DateTime.UtcNow.AddDays(-25),
                        Notes = "Settled via Amex Platinum Card."
                    });

                    await context.Bookings.AddAsync(booking3);

                    // Add review
                    context.Reviews.Add(new Review
                    {
                        RoomId = room301.Id,
                        UserId = customerUser.Id,
                        Rating = 5,
                        Comment = "An unforgettable stay in the Royal Aurelia Penthouse! The private terrace jacuzzi overlooking the skyline at twilight was unmatched. Exemplary butler service.",
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}

