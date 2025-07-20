# CodeLeap Backend

## Setup Instructions

### Prerequisites
- **Development Environment**: Visual Studio 2022 or later, .NET 9 SDK.
- **Database**: SQL Server 2016 
- **Tools**: Git, Docker (optional).

### Installation Steps
1. **Clone the Repository**:  
   - Run: `git clone <repository-url>`

2. **Restore Dependencies**:  
   - Run: `dotnet restore`

3. **Configure `appsettings.json`**:  
   - Update with:
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
     
4. **Build the Project**:
- Run: dotnet build
5. **Database Migration**:
- Run the following commands to apply database migrations:
  
  ```  
  dotnet ef migrations add InitialData --project CodeLeap.Infrastructure --startup-project CodeLeap.Backend  
  dotnet ef database update --project CodeLeap.Infrastructure --startup-project CodeLeap.Backend
  ```
  
6. **Run the Application**:
```  
Run: dotnet run from the the root directory
```

7. **Run Tests**:   
```  
Run: cd ../CodeLeap.Backend.Tests && dotnet test
```  
8. **Access Swagger UI**:
```  
Open / or /swagger in a browser   
```     

### Design Decisions
- Clean Architecture: Organizes the application into distinct layers—presentation (controllers), business logic (services), and data access (repositories)—to ensure separation of concerns and facilitate easier maintenance.
- JWT Authentication: Used for stateless security with [Authorize] and [ClaimRequirement] for access control.
- Serilog Logging: Implemented with file rotation and error-level logging for debugging.
- Swagger Documentation: Adopted for auto-generated API docs using Swashbuckle.

 
