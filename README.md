# Spotless

Graduation project for DEPI: a full cleaning services platform. This repository contains both the **backend (ASP.NET Core .NET 8)** and **frontend (React + Vite + Tailwind)**, following a clean architecture structure.

<p align="center">
  <img src="docs/spotless_logo.png" alt="Project Logo" width="200"/>
</p>

![License](https://img.shields.io/github/license/m-atef1999/Spotless)
![Contributors](https://img.shields.io/github/contributors/m-atef1999/Spotless)
![Last Commit](https://img.shields.io/github/last-commit/m-atef1999/Spotless)
![Made with .NET](https://img.shields.io/badge/Made%20with-.NET%208-blue)

---

## 📚 Table of Contents

* [📌 Project Overview](#-project-overview)
* [🌐 Live Demo](#-live-demo)
* [🛠 Tech Stack](#-tech-stack)
* [📂 Project Structure](#-project-structure)
* [🧠 Layer Responsibilities](#-layer-responsibilities)
* [⚡ Getting Started](#-getting-started)
* [👥 Contributors](#-contributors)
* [📄 License](#-license)

---

## 📌 Project Overview

Spotless is a modern **cleaning service platform** built with a scalable, maintainable clean architecture.

Key features:

* 🔧 ASP.NET Core Web API (.NET 8)
* 🗄 EF Core + SQL Server
* 📱 React + Vite + Tailwind frontend
* 🧱 Clean Architecture (Domain → Application → Infrastructure → Presentation)
* 🗺 Designed for modularity & future expansion

---

## 🌐 Live Demo

* 👉 **Frontend Demo:** [https://spotless-project.vercel.app]
* or
* 👉 [https://spotless-alpha.vercel.app]
---

## 🛠 Tech Stack

### **Backend**

* ASP.NET Core (.NET 8)
* Entity Framework Core
* SQL Server
* MediatR (CQRS)
* FluentValidation
* AutoMapper

### **Frontend**

* React
* Vite
* TailwindCSS

### **DevOps & Tools**

* Docker
* GitHub Actions
* Jira
* Figma

---

## 📂 Project Structure

```
📦 Spotless/
│
├── src/
│   ├── Spotless.API/              → Presentation Layer  (Controllers, Swagger, Middleware)
│   ├── Spotless.Application/      → Application Layer   (CQRS, MediatR, DTOs, Validation)
│   ├── Spotless.Domain/           → Domain Layer        (Entities, Rules, Events)
│   ├── Spotless.Infrastructure/   → Infrastructure      (EF Core, Repositories, Migrations)
│   ├── Frontend/                  → React + Vite + Tailwind Frontend
│   │
│   └── Spotless.sln               → Solution file
│
├── db/                            → SQL scripts & exports
│
└── docs/                          → Docs & architecture notes
```

---

## 🧠 Layer Responsibilities

| Layer                  | Folder                    | Description                                                                                                |
| ---------------------- | ------------------------- | ---------------------------------------------------------------------------------------------------------- |
| **Presentation (API)** | `Spotless.API`            | ASP.NET Core Web API. Handles endpoints, authentication, routing, and application configuration.           |
| **Application**        | `Spotless.Application`    | Contains use cases (CQRS), handlers, DTOs, interfaces, validation, and mapping. No EF or domain logic.     |
| **Domain**             | `Spotless.Domain`         | Pure business logic: entities, enums, value objects, domain rules, domain events. Framework-independent.   |
| **Infrastructure**     | `Spotless.Infrastructure` | EF Core DbContext, repositories, migrations, external services, and persistence implementations.           |
| **Frontend**           | `Frontend`                | React + Vite + Tailwind app structure, handles UI components, pages, routing, and frontend business logic. |

---

## ⚡ Getting Started

### **Clone the repo**

```bash
git clone https://github.com/m-atef1999/Spotless.git
cd Spotless
```

---

# 🔵 Backend Setup (ASP.NET Core API)

### Navigate to the backend

```bash
cd src/Spotless.API
```

### Restore packages

```bash
dotnet restore
```

### Run the API

```bash
dotnet run
```

Backend runs on your configured ports.

---

# 🟣 Frontend Setup (React + Vite + Tailwind)

### Navigate to the frontend

```bash
cd src/Frontend
```

### Install dependencies

```bash
npm install
```

### Run development server

```bash
npm run dev
```

Runs on:

```
http://localhost:5173
```

---

## 🔧 Environment Configuration (Optional)

Create a `.env` file in `src/Frontend`:

```
VITE_API_BASE_URL=https://localhost:5001
```

Use it in your frontend:

```ts
const api = import.meta.env.VITE_API_BASE_URL;
```

---

## 👥 Contributors

| Mahmoud Atef                                                                                                    | Simon Noshy                                                                                                     | Amira Amin                                                                                                                        | Rodaina Mahmoud                                                                                                         | Shosha                                                                                                        |
| --------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------- |
| <a href="https://github.com/m-atef1999"><img src="https://github.com/m-atef1999.png?size=100" width="100"/></a> | <a href="https://github.com/simonnoshy"><img src="https://github.com/simonnoshy.png?size=100" width="100"/></a> | <a href="https://github.com/amiraamin279-collab"><img src="https://github.com/amiraamin279-collab.png?size=100" width="100"/></a> | <a href="https://github.com/RodainaMahmoud"><img src="https://github.com/RodainaMahmoud.png?size=100" width="100"/></a> | <a href="https://github.com/Shosha101"><img src="https://github.com/Shosha101.png?size=100" width="100"/></a> |

---

## 📄 License

This project is licensed under the **MIT License**.
