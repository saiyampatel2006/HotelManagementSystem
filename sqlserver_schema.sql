CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "FullName" TEXT NOT NULL,
    "Address" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "Facilities" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Facilities" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "IconClass" TEXT NOT NULL,
    "Description" TEXT NULL
);

CREATE TABLE "RoomTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RoomTypes" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "BasePrice" decimal(18,2) NOT NULL,
    "DefaultCapacity" INTEGER NOT NULL,
    "ImageUrl" TEXT NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Rooms" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Rooms" PRIMARY KEY AUTOINCREMENT,
    "RoomNumber" TEXT NOT NULL,
    "RoomTypeId" INTEGER NOT NULL,
    "PricePerNight" decimal(18,2) NOT NULL,
    "Capacity" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "ImageUrl" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "FloorNumber" INTEGER NOT NULL,
    "IsFeatured" INTEGER NOT NULL,
    CONSTRAINT "FK_Rooms_RoomTypes_RoomTypeId" FOREIGN KEY ("RoomTypeId") REFERENCES "RoomTypes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Bookings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Bookings" PRIMARY KEY AUTOINCREMENT,
    "BookingReference" TEXT NOT NULL,
    "UserId" TEXT NOT NULL,
    "RoomId" INTEGER NOT NULL,
    "CheckInDate" TEXT NOT NULL,
    "CheckOutDate" TEXT NOT NULL,
    "TotalNights" INTEGER NOT NULL,
    "SubTotal" decimal(18,2) NOT NULL,
    "TaxAmount" decimal(18,2) NOT NULL,
    "TotalPrice" decimal(18,2) NOT NULL,
    "Adults" INTEGER NOT NULL,
    "Children" INTEGER NOT NULL,
    "SpecialRequests" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "ActualCheckInTime" TEXT NULL,
    "ActualCheckOutTime" TEXT NULL,
    "CancellationReason" TEXT NULL,
    "CancelledAt" TEXT NULL,
    CONSTRAINT "FK_Bookings_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Bookings_Rooms_RoomId" FOREIGN KEY ("RoomId") REFERENCES "Rooms" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Reviews" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reviews" PRIMARY KEY AUTOINCREMENT,
    "RoomId" INTEGER NOT NULL,
    "UserId" TEXT NOT NULL,
    "Rating" INTEGER NOT NULL,
    "Comment" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Reviews_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Reviews_Rooms_RoomId" FOREIGN KEY ("RoomId") REFERENCES "Rooms" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RoomFacilities" (
    "RoomId" INTEGER NOT NULL,
    "FacilityId" INTEGER NOT NULL,
    CONSTRAINT "PK_RoomFacilities" PRIMARY KEY ("RoomId", "FacilityId"),
    CONSTRAINT "FK_RoomFacilities_Facilities_FacilityId" FOREIGN KEY ("FacilityId") REFERENCES "Facilities" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RoomFacilities_Rooms_RoomId" FOREIGN KEY ("RoomId") REFERENCES "Rooms" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Payments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Payments" PRIMARY KEY AUTOINCREMENT,
    "BookingId" INTEGER NOT NULL,
    "Amount" decimal(18,2) NOT NULL,
    "PaymentMethod" TEXT NOT NULL,
    "TransactionId" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "PaymentDate" TEXT NOT NULL,
    "Notes" TEXT NULL,
    CONSTRAINT "FK_Payments_Bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES "Bookings" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE UNIQUE INDEX "IX_Bookings_BookingReference" ON "Bookings" ("BookingReference");

CREATE INDEX "IX_Bookings_RoomId" ON "Bookings" ("RoomId");

CREATE INDEX "IX_Bookings_UserId" ON "Bookings" ("UserId");

CREATE INDEX "IX_Payments_BookingId" ON "Payments" ("BookingId");

CREATE INDEX "IX_Reviews_RoomId" ON "Reviews" ("RoomId");

CREATE INDEX "IX_Reviews_UserId" ON "Reviews" ("UserId");

CREATE INDEX "IX_RoomFacilities_FacilityId" ON "RoomFacilities" ("FacilityId");

CREATE UNIQUE INDEX "IX_Rooms_RoomNumber" ON "Rooms" ("RoomNumber");

CREATE INDEX "IX_Rooms_RoomTypeId" ON "Rooms" ("RoomTypeId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261029135729_InitialCreate', '8.0.12');

COMMIT;

