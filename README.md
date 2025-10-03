# 🚀 API Gateway

A comprehensive **API Gateway** built with **ASP.NET Core 9** that provides a secure, scalable interface for product management with downstream API integration, JWT authentication, and comprehensive documentation.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Installation](#installation)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Usage Examples](#usage-examples)
- [Development](#development)
- [Deployment](#deployment)
- [Contributing](#contributing)

## 🎯 Overview

This API Gateway serves as a secure entry point for product management operations, implementing industry-standard patterns for:

- **Secure Authentication**: JWT-based authentication with role-based authorization
- **API Gateway Pattern**: Routes requests to downstream services
- **Comprehensive Logging**: Structured logging for observability
- **Error Handling**: Global exception handling with consistent responses
- **Input Validation**: FluentValidation for robust data validation
- **Interactive Documentation**: Swagger/OpenAPI integration

## 🏗️ Architecture

### **Design Patterns**
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Client/App    │───▶│   API Gateway    │───▶│ Downstream API  │
│                 │    │                  │    │                 │
│ - React/Angular │    │ - Authentication │    │ - JSON API      │
│ - Mobile Apps   │    │ - Authorization  │    │ - External      │
│ - API Clients   │    │ - Rate Limiting  │    │   Services      │
└─────────────────┘    │ - Request Routing│    └─────────────────┘
                       │ - Error Handling │
                       │ - Logging        │
                       └──────────────────┘
```

### **Project Structure**
```
GatewayApi/
├── Common/
│   ├── Results/           # Result pattern for responses
│   └── Exceptions/        # Global exception handling
├── Features/
│   └── Products/
│       ├── Models/        # Domain models and DTOs
│       ├── Services/      # Business logic and external API calls
│       ├── Validators/    # Input validation rules
│       └── Endpoints/     # REST API controllers
├── Program.cs             # Application configuration
├── appsettings.json       # Configuration settings
└── GatewayApi.csproj      # Project dependencies
```

## ✨ Features

### **🔐 Security & Authentication**
- **JWT Authentication**: Bearer token validation
- **Role-Based Authorization**: User/Admin role policies
- **Secure Configuration**: Environment-based secrets management

### **🌐 API Gateway Capabilities**
- **Downstream Integration**: Routes to external APIs (jsonplaceholder.typicode.com)
- **HTTP Client Management**: Configurable HTTP clients with error handling
- **Response Mapping**: Consistent response format across all endpoints

### **📊 Monitoring & Observability**
- **Structured Logging**: Correlation IDs and contextual information
- **Health Checks**: `/health` endpoint for monitoring
- **Error Tracking**: Comprehensive error logging with stack traces

### **🔧 Developer Experience**
- **Swagger Documentation**: Interactive API documentation at root URL
- **Hot Reload**: Development-time code changes
- **CORS Support**: Configured for frontend development

## 🛠️ Tech Stack

| Component | Technology | Version |
|-----------|------------|---------|
| **Framework** | ASP.NET Core | 9.0 |
| **Language** | C# | 12.0 |
| **Authentication** | JWT Bearer | 9.0.0 |
| **Validation** | FluentValidation | 11.9.2 |
| **Documentation** | Swagger/OpenAPI | 6.8.1 |
| **HTTP Client** | HttpClient | Built-in |
| **JSON** | System.Text.Json | Built-in |

## 🚀 Installation

### **Prerequisites**
- **.NET 9 SDK** or later
- **Git** for version control
- **IDE** (Visual Studio, VS Code, or Rider)

### **Setup Steps**

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd gateway-api
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Project**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Verify Installation**
   ```bash
   curl http://localhost:5000/health
   # Expected: {"Status":"Healthy","Timestamp":"...","Version":"1.0.0"}
   ```

## ⚙️ Configuration

### **Application Settings** (`appsettings.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "GatewayApi": "Information"
    }
  },
  "Jwt": {
    "Issuer": "GatewayApi",
    "Audience": "GatewayApiUsers",
    "SecretKey": "YourSuperSecretKeyHere12345678901234567890"
  },
  "DownstreamApi": {
    "BaseUrl": "https://jsonplaceholder.typicode.com"
  },
  "AllowedHosts": "*"
}
```

### **Environment Variables**

| Variable | Description | Example |
|----------|-------------|---------|
| `JWT__ISSUER` | JWT token issuer | `MyApiGateway` |
| `JWT__AUDIENCE` | JWT token audience | `MyApiUsers` |
| `JWT__SECRETKEY` | JWT signing secret | `MySuperSecretKey123` |
| `DOWNSTREAMAPI__BASEURL` | Downstream API base URL | `https://api.example.com` |

## 📚 API Documentation

### **Interactive Documentation**
- **Swagger UI**: `http://localhost:5000`
- **OpenAPI Spec**: `http://localhost:5000/swagger/v1/swagger.json`

### **Authentication**
All endpoints (except `/health`) require JWT authentication:

```bash
# Include Authorization header
curl -H "Authorization: Bearer <your-jwt-token>" \
     http://localhost:5000/api/products
```

### **API Endpoints**

#### **Health Check**
```http
GET /health
```
**Response**: Health status and version information

#### **Products Management**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| **GET** | `/api/products` | Get all products | ✅ JWT + User/Admin |
| **GET** | `/api/products/{id}` | Get product by ID | ✅ JWT + User/Admin |
| **POST** | `/api/products` | Create new product | ✅ JWT + Admin only |

### **Request/Response Examples**

#### **Get All Products**
```bash
curl -H "Authorization: Bearer <token>" \
     http://localhost:5000/api/products
```

#### **Create Product** (Admin Only)
```bash
curl -X POST \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Premium Widget",
    "description": "High-quality product",
    "price": 29.99,
    "stock": 100
  }' \
  http://localhost:5000/api/products
```

## 💡 Usage Examples

### **Frontend Integration (React/Vue/Angular)**

```javascript
// API Client Configuration
const API_BASE = 'http://localhost:5000';
const token = 'your-jwt-token';

// Get all products
const getProducts = async () => {
  const response = await fetch(`${API_BASE}/api/products`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  return response.json();
};

// Create product (Admin only)
const createProduct = async (productData) => {
  const response = await fetch(`${API_BASE}/api/products`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(productData)
  });
  return response.json();
};
```

### **Testing with Postman/curl**

```bash
# 1. Generate JWT token (using jwt.io or your auth service)
# 2. Use token in requests

# Get products
curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
     http://localhost:5000/api/products

# Create product
curl -X POST \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","price":10.99,"stock":50}' \
  http://localhost:5000/api/products
```

## 🔧 Development

### **Development Workflow**

1. **Start Development Server**
   ```bash
   dotnet run
   ```

2. **Make Changes**
   - Controllers auto-reload on file changes
   - Check `http://localhost:5000` for API documentation

3. **Run Tests**
   ```bash
   dotnet test
   ```

4. **Debug**
   - Use IDE debugger on `https://localhost:5000`
   - Check console logs for request tracing

### **Code Quality**

- **Validation**: Input validation with FluentValidation
- **Error Handling**: Global exception middleware
- **Logging**: Structured logging with correlation IDs
- **Security**: Authentication and authorization middleware

### **Adding New Features**

1. **Create Models**: Add to `Features/{Feature}/Models/`
2. **Add Validation**: Create validator in `Features/{Feature}/Validators/`
3. **Implement Service**: Add business logic in `Features/{Feature}/Services/`
4. **Create Controller**: Add endpoints in `Features/{Feature}/Endpoints/`
5. **Update Documentation**: Endpoints appear automatically in Swagger

## 🚢 Deployment

### **Production Deployment**

1. **Environment Setup**
   ```bash
   # Set production environment variables
   export ASPNETCORE_ENVIRONMENT=Production
   export JWT__SECRETKEY=<secure-random-key>
   export DOWNSTREAMAPI__BASEURL=<production-api-url>
   ```

2. **Build for Production**
   ```bash
   dotnet publish -c Release -o publish
   ```

3. **Deploy to Server**
   ```bash
   # Copy published files to server
   # Configure reverse proxy (nginx/apache)
   # Set up SSL certificates
   # Configure firewall rules
   ```

### **Docker Deployment**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80 443
ENTRYPOINT ["dotnet", "GatewayApi.dll"]
```

### **Health Monitoring**

- **Health Check URL**: `https://your-domain.com/health`
- **Monitoring**: Check application logs for errors
- **Metrics**: Monitor response times and error rates

## 🤝 Contributing

### **Development Process**

1. **Fork the Repository**
2. **Create Feature Branch**: `git checkout -b feature/amazing-feature`
3. **Make Changes**: Follow existing code patterns
4. **Add Tests**: Ensure comprehensive test coverage
5. **Update Documentation**: Update README for new features
6. **Submit Pull Request**

### **Code Standards**

- **Naming Conventions**: PascalCase for classes, camelCase for methods
- **Documentation**: XML comments for public APIs
- **Error Handling**: Use Result pattern for operation outcomes
- **Validation**: FluentValidation for input validation
- **Logging**: Structured logging with appropriate levels

### **Pull Request Process**

1. **Update README**: Document any new features or changes
2. **Add Tests**: Ensure all new code is tested
3. **Code Review**: Address reviewer feedback
4. **Merge**: Once approved, merge to main branch

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:
- **Documentation**: Check this README and API documentation
- **Issues**: Create GitHub issues for bugs and features
- **Discussions**: Use GitHub Discussions for questions

---

**Built with ❤️ using ASP.NET Core 9**
