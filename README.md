# HomeHero

Graduation project for DEPI: a home maintenance service platform.

## Tech Stack
- ASP.NET Core, C# (.NET 8)
- EF Core + SQL Server
- Blazor/MVC, Docker, GitHub Actions

## Project Structure
HomeHero/
├─ src/
│  ├─ HomeHero.Api/                 # ASP.NET Core Web API
│  ├─ HomeHero.Web/                 # MVC/Blazor or frontend
│  ├─ HomeHero.Core/                # Domain + Application (Entities, Services, DTOs)
│  └─ HomeHero.Infrastructure/      # EF Core, Repos, Migrations, DB context
├─ tests/
│  └─ HomeHero.Tests/               # xUnit/NUnit tests
├─ db/
│  ├─ scripts/                      # SQL seed/fix scripts
│  └─ docker-compose.yml            # local SQL Server/Postgres container
├─ docs/                            # diagrams, ADRs, screenshots
├─ .github/
│  └─ workflows/
│     └─ dotnet-ci.yml              # CI pipeline
├─ .editorconfig
├─ .gitattributes
├─ .gitignore
├─ LICENSE
└─ README.md

## License
MIT
