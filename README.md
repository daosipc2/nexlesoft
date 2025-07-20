# CodeLeap Backend

## Setup Instructions

### Prerequisites
- **Development Environment**: Visual Studio 2022 or later, .NET 8 SDK.
- **Database**: SQL server 2016 
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
4. Build the Project:
- Run: dotnet build
5. Run the Application:
Run: dotnet run from the the root directory
6. Run Tests:
Run: cd ../CodeLeap.Backend.Tests && dotnet test
7. Access Swagger UI:
Open / or /swagger in a browser   
   
