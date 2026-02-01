# CartonCaps

Referral and invitation management system with rate limiting and validations.

## 📁 Project Structure

### CartonCaps.Api
Backend API built with .NET 8 using layered architecture:
- **Controllers**: REST API endpoints (ReferralController)
- **BL (Business Logic)**: Business logic layer (ReferralService)
- **Domain**: Domain entities, interfaces, and configurations
- **Infrastructure**: Repository implementations and database context
- **Validators**: Validations using FluentValidation
- **BackgroundServices**: Background services for periodic tasks (referral expiration)

### CartonCaps.Web
Frontend application built with Angular 21:
- User interface for the referral system
- Integration with Angular Material for UI components
- Communication with the API through HTTP services

### CartonCaps.Tests
Unit and integration testing project:
- Framework: xUnit
- Mocking: Moq
- Assertions: FluentAssertions
- Code coverage with Coverlet

## 🚀 Technologies Used

### Backend
- **.NET 8**: Main framework
- **Entity Framework Core 8**: ORM with InMemory database
- **FluentValidation**: Model validation
- **AspNetCoreRateLimit**: Rate limiting to prevent endpoint abuse
- **Swagger**: Interactive API documentation

### Frontend
- **Angular 21**: Frontend framework
- **Angular Material 21**: UI component library
- **RxJS**: Reactive programming
- **TypeScript 5.9**: Primary language
- **Vitest**: Testing framework

### Testing
- **xUnit**: Testing framework
- **Moq**: Mocking library
- **FluentAssertions**: Fluent assertions
- **Coverlet**: Code coverage analysis

## ⚙️ How to Run the Project

### Prerequisites
- .NET 8 SDK
- Node.js (v18 or higher)
- npm 10.9.2 or higher

### Backend (API)

1. Navigate to the API project folder:
```bash
cd CartonCaps.Api
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

### Frontend (Web)

1. Navigate to the web project folder:
```bash
cd CartonCaps.Web
```

2. Install dependencies:
```bash
npm install
```

3. Run the application:
```bash
npm start
```

The application will be available at: `http://localhost:4200`

### Tests

1. Navigate to the tests folder:
```bash
cd CartonCaps.Tests
```

2. Run the tests:
```bash
dotnet test
```

## 🔄 How It Works

1. **Invitation Generation**:
   - User requests an invitation link through the frontend
   - Frontend sends a POST request to `/api/referral/invitation-link`
   - API validates the request and generates a unique referral code
   - Rate limiting is applied (max. 10 requests per hour)

2. **Referral Registration**:
   - A new user accesses through the invitation link
   - Frontend sends the referral code to the `/api/referral/referral-registration` endpoint
   - API validates that the code exists, is not expired, and is active
   - New user is created and associated with the referral
   - Rate limiting is applied (max. 5 requests per hour)

3. **Automatic Expiration**:
   - A background service (BackgroundService) runs periodically
   - Marks referrals as expired when they exceed their validity period
   - Expired referrals cannot be used for new registrations

4. **Rate Limiting**:
   - Protection against endpoint abuse through IP-based request limiting
   - Custom configuration per endpoint
   - HTTP 429 response when limits are exceeded

5. **Validations**:
   - FluentValidation validates all input models
   - Business validations in the BL layer
   - Standardized error responses

## 🗃️ Database

The project uses Entity Framework Core with an **InMemory** database for development and testing. Data is initialized through a DataSeeder when the application starts.

## 🔒 Security

- **CORS** configured to allow requests from `http://localhost:4200`
- **Rate Limiting** to prevent abuse of critical endpoints
- **Comprehensive validation** of inputs with FluentValidation
- **HTTPS** redirection enabled