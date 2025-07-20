# CodeLeap Backend

## Setup Instructions

### Prerequisites
- **Development Environment**: Visual Studio 2022 or later, .NET 8 SDK.
- **Dependencies**: Ensure the following NuGet packages are installed:
  - `Microsoft.AspNetCore.Mvc`
  - `Serilog.AspNetCore`
  - `Serilog.Sinks.File`
  - `Swashbuckle.AspNetCore`
  - `NUnit`, `NUnit3TestAdapter`, `Moq`, `Microsoft.NET.Test.Sdk` (for tests)
- **Database**: A compatible database (e.g., SQL Server) configured for service implementations.
- **Tools**: Git, Docker (optional).

### Installation Steps
1. Clone the repository: `git clone <repository-url>`
2. Restore dependencies: `dotnet restore`
3. Configure `appsettings.json`:
   ```json
   {
     "Jwt": {
       "SecretKey": "your-secret-key-should-be-at-least-16-characters-long",
       "Issuer": "your-issuer",
       "Audience": "your-audience"
     },
     "ConnectionStrings": {
       "DefaultConnection": "your-connection-string"
     }
   }
   
