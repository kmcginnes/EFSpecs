﻿IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Test')
DROP DATABASE [Test]
GO

CREATE DATABASE [Test]
GO

USE [Test]

CREATE TABLE Addresses
(
	AddressId INT IDENTITY(1,1) PRIMARY KEY,
	Street NVARCHAR(MAX) NULL,
	City NVARCHAR(MAX) NULL,
	ZipCode NVARCHAR(MAX) NULL,
)
GO

CREATE TABLE Users
(
	UserId INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(MAX) NULL,
	Age INT NOT NULL,
	BillingAddressId INT NULL,
    CONSTRAINT [Addresses_BillingAddressId] FOREIGN KEY ([BillingAddressId]) REFERENCES [dbo].[Addresses] ([AddressId])
)
GO