# HomeHero

Graduation project for DEPI: a home maintenance service platform.

## Tech Stack
- ASP.NET Core, C# (.NET 8)
- EF Core + SQL Server
- Optional: Blazor/MVC, Docker, GitHub Actions

## Getting Started (Local)
1. Install .NET 8 SDK and Docker.
2. `cp .env.example .env` then fill values.
3. `docker compose -f db/docker-compose.yml up -d` (starts SQL Server)
4. `dotnet restore`
5. `dotnet ef database update -s src/HomeHero.Api -p src/HomeHero.Infrastructure`
6. `dotnet run --project src/HomeHero.Api`

## Project Structure
See repository tree above.

## Contributing
- Create a branch: `feature/<desc>`
- Commit convention: `type(scope): message` (feat, fix, chore, docs, test)
- Open PR → 1 review required → squash merge.

## License
MIT
