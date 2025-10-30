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

<pre>
📦 Spotless
│
├── src/
│   ├── Spotless.API/            → Presentation layer
│   │                             (Controllers, endpoints, dependency injection, Swagger setup)
│   │
│   ├── Spotless.Domain/         → Business logic layer
│   │                             (Entities, DTOs, service interfaces, validation, domain rules)
│   │
│   ├── Spotless.Data/           → Data access layer
│   │                             (EF Core DbContext, repositories, migrations, data seeding)
│   │
│   └── Spotless.sln             → Visual Studio solution file
│
├── db/                          → SQL scripts or manual database exports (optional)
│
└── docs/                        → Documentation, diagrams, and API usage notes
</pre>


---

### 🧠 Layer Responsibilities

| Layer | Folder | Description |
|-------|---------|-------------|
| **Presentation (API)** | `Spotless.API` | Exposes HTTP endpoints, handles requests/responses, and configures dependency injection. |
| **Business Logic (Domain)** | `Spotless.Domain` | Contains entities, DTOs, business rules, and service interfaces. |
| **Data Access** | `Spotless.Data` | Manages persistence with Entity Framework Core (DbContext, repositories, migrations, seeding). |


---

## ⚡ Getting Started

```bash
git clone https://github.com/m-atef1999/Spotless.git
cd Spotless
dotnet run
```
---
## 📄 License

- This project is licensed under the MIT License.
