# Spotless

Graduation project for DEPI: a cleaning services platform.

<p align="center">
  <img src="docs/spotless_logo.png" alt="Project Logo" width="200"/>
</p>

![License](https://img.shields.io/github/license/m-atef1999/Spotless)
![Contributors](https://img.shields.io/github/contributors/m-atef1999/Spotless)
![Last Commit](https://img.shields.io/github/last-commit/m-atef1999/Spotless)
![Made with .NET](https://img.shields.io/badge/Made%20with-.NET-blue)

---

## 📚 Table of Contents
- [📌 Project Overview](#-project-overview)
- [🌐 Live Demo](#-live-demo)
- [🛠 Tech Stack](#-tech-stack)
- [📂 Project Structure](#-project-structure)
- [⚡ Getting Started](#-getting-started)
- [👥 Contributors](#-team-members)
- [📄 License](#-license)

---

## 📌 Project Overview
- 🔧 Built with ASP.NET Core + SQL Server
- 🎯 Purpose: Cleaning Services Platform
- 👥 Team: Cleaning Services Platform

---

## 👥 Team Members

| <a href="https://github.com/m-atef1999"><img src="https://github.com/m-atef1999.png?size=100" width="100"/><br /><span style="font-size:14px;"><b>Mahmoud Atef</b></span></a> | <a href="https://github.com/simonnoshy"><img src="https://github.com/simonnoshy.png?size=100" width="100"/><br /><span style="font-size:14px;"><b>Simon Noshy</b></span></a> | <a href="https://github.com/amiraamin279-collab"><img src="https://github.com/amiraamin279-collab.png?size=100" width="100"/><br /><span style="font-size:14px;"><b>Amira Amin</b></span></a> | <a href="https://github.com/RodainaMahmoud"><img src="https://github.com/RodainaMahmoud.png?size=100" width="100"/><br /><span style="font-size:14px;"><b>Rodaina Mahmoud</b></span></a> | <a href="https://github.com/Shosha101"><img src="https://github.com/Shosha101.png?size=100" width="100"/><br /><span style="font-size:14px;"><b>Shosha</b></span></a> |
|---|---|---|---|---|


---

## 🌐 Live Demo
👉 [Check out the website](https://preview--quickclean-wash-wave.lovable.app/)

---

## 🛠 Tech Stack
- ASP.NET Core, C# (.NET 8)
- EF Core + SQL Server
- Blazor/MVC
- HTML, CSS, (Angular, React, Bootstrap)
- Docker
- Jira
- Figma
- GitHub Actions

---

## 📂 Project Structure

```bash
📦 Spotless/
│
├── src/
│   ├── Spotless.API/              → Presentation Layer  
│   ├── Spotless.Application/      → Application Layer (Use Cases + CQRS)
│   ├── Spotless.Domain/           → Domain Layer (Core Business Rules)
│   ├── Spotless.Infrastructure/   → Infrastructure Layer (EF Core, Repos, External Services)
│   │
│   └── Spotless.sln               → Solution file
│
├── db/                            → SQL Scripts / Data Exports
│
└── docs/                          → Documentation & Architecture Notes
```

---

### 🧠 Layer Responsibilities

| Layer | Folder | Description |
|-------|---------|-------------|
| **Presentation (API)** | `Spotless.API` | Hosts the ASP.NET Core Web API. Handles endpoints, middleware, authentication, Swagger, and application startup. |
| **Application** | `Spotless.Application` | Implements use cases using CQRS + MediatR. Contains DTOs, validators, interfaces, and mapping profiles—no EF or domain rules. |
| **Business Logic (Domain)** | `Spotless.Domain` | Core business model: entities, value objects, domain events, enums, and business rules. Completely independent and framework-free. |
| **Data Access (Infrastructure)** | `Spotless.Infrastructure` | Handles persistence and integrations: EF Core DbContext, repositories, migrations, external services, and configuration. Implements Application layer contracts. |


---

## ⚡ Getting Started

```bash
# Clone the repository
git clone https://github.com/m-atef1999/Spotless.git

# Navigate into the API project
cd Spotless/src/Spotless.API

# Restore dependencies
dotnet restore

# Run the API
dotnet run
```
---
## 📄 License

- This project is licensed under the MIT License.
