# Library Management System

A full-stack web application that allows users to manage their personal library.  
The system supports book management, user authentication, admin privileges, and an AI-powered query assistant.

---

## Project Overview

**Library Management System** is a web-based application where users can:
- Manage their personal book collections
- Organize books by genre
- Track reading status
- Interact with an AI assistant to query library data

Admins have full control over users and books across the platform.

---

## Architecture

### Backend
- **.NET (ASP.NET Core Web API)**
- **Clean Architecture**
  - Domain
  - Application
  - Infrastructure
  - API

### Frontend
- **React**
- REST API communication with backend

### AI Integration
- **Ollama**
- **Model:** `gemma:3 1b`
- Natural language querying over library data

---

## Authentication & Authorization

- JWT-based authentication
- Role-based authorization:
  - User
  - Admin

---

## Technologies Used

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Clean Architecture
- JWT Authentication
- Ollama (AI integration)

### Frontend
- React
- React Router
- Axios / Fetch API
- Modern component-based architecture
