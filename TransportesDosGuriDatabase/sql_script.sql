--TRANSPORTES DOS GURI DATABASE CREATION--

IF NOT EXISTS(
    SELECT 1
    FROM sys.databases
    WHERE name = 'TransportesDosGuriDB'
)

BEGIN
    CREATE DATABASE TransportesDosGuriDB;
END

GO

USE TransportesDosGuriDB;

GO

CREATE TABLE [dbo].[AspNetUsers] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [UserName] NVARCHAR(256) NULL,
    [NormalizedUserName] NVARCHAR(256) NULL,
    [Email] NVARCHAR(256) NULL,
    [NormalizedEmail] NVARCHAR(256) NULL,
    [EmailConfirmed] BIT NOT NULL DEFAULT 0,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [SecurityStamp] NVARCHAR(MAX) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
    [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
    [LockoutEnd] DATETIMEOFFSET NULL,
    [LockoutEnabled] BIT NOT NULL DEFAULT 0,
    [AccessFailedCount] INT NOT NULL DEFAULT 0,

    
    [Name] NVARCHAR(100) NULL,
    [LastName] NVARCHAR(100) NULL,
    [IdentityNumber] NVARCHAR(50) NULL,
    [ZipCode] NVARCHAR(20) NULL,
    [Address] NVARCHAR(256) NULL,
    [AddressNumber] INT NOT NULL DEFAULT 0,
    [District] NVARCHAR(100) NULL,
    [City] NVARCHAR(100) NULL,
    [State] NVARCHAR(50) NULL,
    [CustomerAsaasId] NVARCHAR(100) NULL,
    [RefreshToken] NVARCHAR(MAX) NULL,
    [RefreshTokenExpirationDateTime] DATETIME2 NULL,

    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
ON [dbo].[AspNetUsers] ([NormalizedUserName] ASC)
WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE NONCLUSTERED INDEX [EmailIndex]
ON [dbo].[AspNetUsers] ([NormalizedEmail] ASC);
GO

CREATE TABLE [dbo].[AspNetRoles] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(256) NULL,
    [NormalizedName] NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
ON [dbo].[AspNetRoles] ([NormalizedName] ASC)
WHERE [NormalizedName] IS NOT NULL;
GO

CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] BIGINT NOT NULL,
    [RoleId] BIGINT NOT NULL,

    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] BIGINT NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId]
ON [dbo].[AspNetUserClaims] ([UserId]);
GO

CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [RoleId] BIGINT NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId]
ON [dbo].[AspNetRoleClaims] ([RoleId]);
GO

CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [ProviderKey] NVARCHAR(128) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId] BIGINT NOT NULL,

    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId]
ON [dbo].[AspNetUserLogins] ([UserId]);
GO

CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId] BIGINT NOT NULL,
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(128) NOT NULL,
    [Value] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [dbo].[Aircraft] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [Type] INT NOT NULL, 
    [Model] NVARCHAR(100) NULL,

    CONSTRAINT [PK_Aircraft] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[Airport] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [City] NVARCHAR(100) NULL,
    [State] NVARCHAR(100) NULL,
    [Country] NVARCHAR(100) NULL,

    CONSTRAINT [PK_Airport] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[AsaasIntegration] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [Environment] NVARCHAR(50) NULL,
    [ApiKey] NVARCHAR(255) NULL,
    [BaseUrl] NVARCHAR(255) NULL,
    [IsActive] BIT NOT NULL,

    CONSTRAINT [PK_AsaasIntegration] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[Trip] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [TripName] NVARCHAR(150) NULL,
    [OriginAirportId] BIGINT NOT NULL,
    [DestinyAirportId] BIGINT NOT NULL,
    [DepartureTime] DATETIME2 NOT NULL,
    [ArrivalTime] DATETIME2 NOT NULL,
    [TotalPrice] DECIMAL(18, 2) NOT NULL,

    CONSTRAINT [PK_Trip] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Trip_OriginAirport] FOREIGN KEY ([OriginAirportId]) REFERENCES [dbo].[Airport] ([Id]),
    CONSTRAINT [FK_Trip_DestinyAirport] FOREIGN KEY ([DestinyAirportId]) REFERENCES [dbo].[Airport] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Trip_OriginAirportId] ON [dbo].[Trip] ([OriginAirportId] ASC);
CREATE NONCLUSTERED INDEX [IX_Trip_DestinyAirportId] ON [dbo].[Trip] ([DestinyAirportId] ASC);
GO

CREATE TABLE [dbo].[Flight] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [AircraftId] BIGINT NOT NULL,
    [OriginAirportId] BIGINT NOT NULL,
    [DestinyAirportId] BIGINT NOT NULL,
    [DepartureTime] DATETIME2 NOT NULL,
    [ArrivalTime] DATETIME2 NOT NULL,
    [BasePrice] DECIMAL(18, 2) NOT NULL,
    [TripId] BIGINT NOT NULL,

    CONSTRAINT [PK_Flight] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Flight_Aircraft] FOREIGN KEY ([AircraftId]) REFERENCES [dbo].[Aircraft] ([Id]),
    CONSTRAINT [FK_Flight_OriginAirport] FOREIGN KEY ([OriginAirportId]) REFERENCES [dbo].[Airport] ([Id]),
    CONSTRAINT [FK_Flight_DestinyAirport] FOREIGN KEY ([DestinyAirportId]) REFERENCES [dbo].[Airport] ([Id]),
    CONSTRAINT [FK_Flight_Trip] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trip] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Flight_AircraftId] ON [dbo].[Flight] ([AircraftId] ASC);
CREATE NONCLUSTERED INDEX [IX_Flight_OriginAirportId] ON [dbo].[Flight] ([OriginAirportId] ASC);
CREATE NONCLUSTERED INDEX [IX_Flight_DestinyAirportId] ON [dbo].[Flight] ([DestinyAirportId] ASC);
CREATE NONCLUSTERED INDEX [IX_Flight_TripId] ON [dbo].[Flight] ([TripId] ASC);
CREATE NONCLUSTERED INDEX [IX_Flight_DepartureTime] ON [dbo].[Flight] ([DepartureTime] ASC);
GO

CREATE TABLE [dbo].[FlightSeat] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [AircraftId] BIGINT NOT NULL,
    [SeatNumber] NVARCHAR(10) NULL,
    [Class] INT NOT NULL,    
    [Location] INT NOT NULL, 
    [Side] INT NOT NULL,     

    CONSTRAINT [PK_FlightSeat] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FlightSeat_Aircraft] FOREIGN KEY ([AircraftId]) REFERENCES [dbo].[Aircraft] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_FlightSeat_AircraftId] ON [dbo].[FlightSeat] ([AircraftId] ASC);
GO

CREATE TABLE [dbo].[Seat] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [AircraftId] BIGINT NOT NULL,
    [SeatNumber] NVARCHAR(10) NULL,
    [Class] INT NOT NULL,    
    [Location] INT NOT NULL, 
    [Side] INT NOT NULL,    

    CONSTRAINT [PK_Seat] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Seat_Aircraft] FOREIGN KEY ([AircraftId]) REFERENCES [dbo].[Aircraft] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Seat_AircraftId] ON [dbo].[Seat] ([AircraftId] ASC);
GO

CREATE TABLE [dbo].[Schedule] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [FlightId] BIGINT NOT NULL,
    [AirportId] BIGINT NOT NULL,
    [DepartureTime] DATETIME2 NOT NULL,
    [ArrivalTime] DATETIME2 NOT NULL,

    CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Schedule_Flight] FOREIGN KEY ([FlightId]) REFERENCES [dbo].[Flight] ([Id]),
    CONSTRAINT [FK_Schedule_Airport] FOREIGN KEY ([AirportId]) REFERENCES [dbo].[Airport] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Schedule_FlightId] ON [dbo].[Schedule] ([FlightId] ASC);
CREATE NONCLUSTERED INDEX [IX_Schedule_AirportId] ON [dbo].[Schedule] ([AirportId] ASC);
GO

CREATE TABLE [dbo].[Purchase] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [ApplicationUserId] BIGINT NOT NULL,
    [PurchaseDate] DATETIME2 NOT NULL,
    [PurchasePrice] DECIMAL(18, 2) NOT NULL,
    [Status] INT NOT NULL, 

    CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Purchase_AspNetUsers] FOREIGN KEY ([ApplicationUserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Purchase_ApplicationUserId] ON [dbo].[Purchase] ([ApplicationUserId] ASC);
GO

CREATE TABLE [dbo].[Reservation] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [ApplicationUserId] BIGINT NOT NULL,
    [FlightSeatId] BIGINT NOT NULL,
    [PurchaseId] BIGINT NOT NULL,
    [ReservationDate] DATETIME2 NOT NULL,
    [Status] INT NOT NULL, 
    [Price] DECIMAL(18, 2) NOT NULL,

    CONSTRAINT [PK_Reservation] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Reservation_AspNetUsers] FOREIGN KEY ([ApplicationUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Reservation_FlightSeat] FOREIGN KEY ([FlightSeatId]) REFERENCES [dbo].[FlightSeat] ([Id]),
    CONSTRAINT [FK_Reservation_Purchase] FOREIGN KEY ([PurchaseId]) REFERENCES [dbo].[Purchase] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Reservation_ApplicationUserId] ON [dbo].[Reservation] ([ApplicationUserId] ASC);
CREATE NONCLUSTERED INDEX [IX_Reservation_FlightSeatId] ON [dbo].[Reservation] ([FlightSeatId] ASC);
CREATE NONCLUSTERED INDEX [IX_Reservation_PurchaseId] ON [dbo].[Reservation] ([PurchaseId] ASC);
GO

CREATE TABLE [dbo].[UserRequest] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [DueDate] DATE NOT NULL,
    [ApplicationUserId] BIGINT NOT NULL,
    [AsaasSubscriptionId] NVARCHAR(100) NULL,

    CONSTRAINT [PK_UserRequest] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRequest_AspNetUsers] FOREIGN KEY ([ApplicationUserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_UserRequest_ApplicationUserId] ON [dbo].[UserRequest] ([ApplicationUserId] ASC);
GO