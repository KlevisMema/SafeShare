# Security Computer Project

## Overview
This document outlines the requirements, features, and instructions for implementing a secure client/server system for managing group expenses.

## Table of Contents
1. [Introduction](#1-introduction)
2. [System Characteristics](#2-system-characteristics)
3. [Features](#3-features)
4. [Technologies Used](#4-technologies-used)
5. [Requirements](#5-requirements)
6. [Setup Instructions](#6-setup-instructions)

## 1. Introduction
The project aims to implement a secure system for handling common expenses within a group, emphasizing security aspects in data storage and transmission.

### Key Points
- Focus on security protocols and techniques.

## 2. System Characteristics
The system follows a client/server architecture with specific roles for users and the server. Key tasks include user registration, authentication, and various operations for authenticated users.

### Server
- Not a trusted entity.
- Secure transfer and ownership verification mechanisms.

### Clients and Users
- Users can register, log in, and perform tasks securely.
- Generation of authentication material during registration.

### Groups
- Users can create groups for encoding expenses.
- Only the group owner can manage group membership.

## 3. Features
This section outlines the high-level features and protocols required for secure data exchange, storage, and user activities.

### User Registration, Authentication, and Revocation
- Secure credentials generation for user registration.
- Users can log in from different devices.

### Creating Groups
- Any user can create a group for encoding expenses.
- Group ownership management.

### Creating an Expense
- Group members can create expenses.

### Consulting Expenses
- Group members can view all group expenses.

### Editing and Deleting Expenses
- Only the expense creator can edit or delete it.

### Viewing Balances
- Group members can view balances of other members.

## 4. Technologies Used
- **.NET 8 Web API**: Used for building the server-side application.
- **Blazor**: Utilized for building interactive web applications.
- **SQL Server**: Chosen as the relational database management system.
- **SendGrid**: Integrated for handling email functionalities.
- **Serilog and Seq**: Employed for structured logging.
- **Libraries**: Mediator, Automapper, JWT, Entity Framework, HTTP Client, Mud Blazor.

## 5. Requirements
Ensure the following components are installed and configured on your system:

- **.NET 8 SDK**: [Download .NET SDK] https://dotnet.microsoft.com/en-us/download/dotnet/8.0
- **SQL Server**: Installed and running. https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- **SSMS**: Installed and running. https://aka.ms/ssmsfullsetup
- **Seq**: Running on http://localhost:5341/ | Download in https://datalust.co/download/begin?version=2023.4.10219

## 6. Setup Instructions
# Setup Instructions

## Prerequisites:
   - Bash is required to run these scripts.

## For a clean setup of the process, run the scripts in order:

### 1. setup1.sh
   - This script will prompt you for values that will be stored in environment variables.
   - The application reads certain values from the code, so accurate naming and values are crucial.

### 2. setup2.sh
   - This script will build and run the main API project.
   - Running this project will automatically create two tables using the seeder.
   - Ensure you have SQL Server and MSSQL installed on your machine, and create a Windows account.
   - The connection string is straightforward and creates the database in Windows accounts.

### 3. runall.sh
   - Runs the entire application, including the Blazor client, proxy API, and main API.
