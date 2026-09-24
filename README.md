# CarsSpec

### AI-Powered Car Search & Information Platform

CarsSpec is a full-stack automobile platform designed to help users discover, search and explore cars available in India.

The application combines a structured vehicle database with REST APIs, a modern Angular frontend and an AI-powered assistant capable of retrieving application data through controlled backend tools.

The project was independently designed and developed to demonstrate practical experience in **.NET, Angular, SQL Server, cloud deployment, AI integration and modern software engineering practices**.

---

## 🚗 What is CarsSpec?

CarsSpec provides a centralized platform for exploring automobile information such as:

- Car manufacturers and models
- Variants and pricing
- Vehicle specifications
- Features and equipment
- Powertrains
- Fuel efficiency
- Dimensions
- Seating capacity
- Drivetrains
- On-road pricing
- Comparisons
- EMI and affordability calculations

The platform is designed around structured vehicle data rather than relying solely on static or AI-generated information.



## 🤖 AI-Powered Assistant

CarsSpec includes an AI-powered conversational assistant that allows users to interact with the vehicle database using natural language.

Instead of allowing the AI model to directly access the database, the assistant uses controlled backend tools.

### Example

A user can ask:

> "Show me automatic SUVs under ₹15 lakh."

The AI can determine the required operation and request the appropriate backend tool.

The backend then:

1. Validates the request
2. Executes the required business logic
3. Retrieves data from SQL Server
4. Returns structured results to the AI
5. Generates the final response for the user

This architecture keeps **business logic and database access inside the application backend**, while using the AI primarily for reasoning and conversational interaction.

---

## 🧠 AI & Automation

The project also includes an AI-based vehicle data processing pipeline.

Vehicle brochures can be processed to extract structured information such as:

- Models
- Variants
- Specifications
- Features
- Powertrain information
- Performance data
- Dimensions
- Fuel efficiency
- Pricing

AI-generated data is validated using application-level rules and can go through an administrative review workflow before being added to the primary vehicle database.

The system uses structured AI outputs and validation to reduce inconsistent or unusable extracted data.

---

## 🏗️ Application Architecture

CarsSpec follows a layered full-stack architecture:

**Angular Frontend**  
↓  
**ASP.NET Core Web API**  
↓  
**Business Services / AI Services / Tool Calling / Data Access**  
↓  
**SQL Server**

The main application flow is:

**User → Angular → ASP.NET Core API → Business Services / AI Tools → SQL Server**

This architecture keeps business logic, database access and deterministic calculations inside the backend while allowing AI models to interact with application data through controlled tools.

---

## 🛠️ Technology Stack

### Backend

- **C#**
- **ASP.NET Core**
- **.NET Web API**
- **Entity Framework Core**
- **LINQ**
- **REST APIs**
- **Dependency Injection**
- **Asynchronous Programming**

### Frontend

- **Angular**
- **TypeScript**
- **Tailwind CSS**
- **Responsive UI**
- **REST API Integration**

### Database

- **Microsoft SQL Server**
- **Relational Database Design**
- **Entity Relationships**
- **Query Optimization**
- **Structured Vehicle Data**

### AI

- **OpenAI APIs**
- **AI Tool Calling**
- **Structured Outputs**
- **Prompt Engineering**
- **AI-powered Data Extraction**
- **AI Validation & Guardrails**
- **MCP-based Application Tools**

### Cloud & DevOps

- **Microsoft Azure**
- **Azure Container Apps**
- **Azure Container Registry**
- **Azure CLI**
- **Docker**
- **GitHub**
- **GitHub Actions**
- **CI/CD**

---

## ⚙️ Key Engineering Areas

CarsSpec demonstrates hands-on implementation across several areas of modern software development:

- RESTful API development
- Relational database design
- Backend business logic
- AI integration
- Backend tool calling
- Structured AI outputs
- Data validation
- Authentication and authorization
- Administrative workflows
- Document processing
- Production-style logging and error handling
- Docker containerization
- Cloud deployment
- CI/CD automation
- Performance optimization
- AI token and cost optimization

---

## 📊 Vehicle Data Platform

CarsSpec uses a structured relational data model to represent automobile information and its relationships.

The platform maintains data across:

- **Manufacturers**
- **Models**
- **Parent Variants**
- **Variants**
- **Powertrains**
- **Transmissions**
- **Drivetrains**
- **Specifications**
- **Features**
- **Pricing**
- **Fuel Efficiency**
- **Vehicle Dimensions**

### Vehicle Data Hierarchy

**Manufacturer → Model → Parent Variant → Variant**

A variant can then be associated with:

- Powertrain
- Transmission
- Drivetrain
- Specifications
- Features
- Pricing
- Fuel efficiency
- Dimensions

This structured approach allows CarsSpec to perform reliable searches, filtering and calculations using application data rather than treating vehicle information as unstructured text.

---

## 💰 Pricing & Calculations

CarsSpec includes backend services for automobile-related calculations, including:

- On-road price
- Registration costs
- Insurance
- Tax components
- TCS
- Fastag and HSRP charges
- EMI
- Affordability
- Fuel cost calculations

These calculations are implemented as backend application logic rather than delegated to the AI model, ensuring deterministic and consistent results.

---

## 🔐 AI Safety & Data Control

A key design principle of CarsSpec is keeping AI capabilities controlled by the application.

The AI model does **not** directly access SQL Server.

Instead, application data is accessed through controlled backend tools:

**User Request**  
↓  
**AI Model**  
↓  
**Tool Selection**  
↓  
**Backend Validation**  
↓  
**Business Service**  
↓  
**SQL Server**  
↓  
**Structured Tool Result**  
↓  
**AI Response**

This approach provides better control over:

- Data access
- User input validation
- Business rules
- Tool permissions
- Deterministic calculations
- AI-generated responses
- Reduction of AI hallucinations

---

## 📱 Application Areas

The platform includes functionality for:

- Car discovery
- Vehicle search
- Model and variant exploration
- Vehicle specifications
- Vehicle pricing
- AI-assisted car queries
- Brochure processing
- AI-generated data review
- Administrative data management
- Batch processing
- Vehicle data validation

---

## 🎯 Project Objective

CarsSpec was built as an independent full-stack project to explore how traditional web application architecture can be combined with modern AI capabilities.

The project focuses on using AI where it provides value while keeping **data, business rules and deterministic operations under application control**.

---

## 👨‍💻 About the Project

CarsSpec is an independently designed and developed project demonstrating hands-on experience with:

**.NET + Angular + SQL Server + Azure + Docker + AI**

The project covers practical implementation across:

- Backend development
- Frontend development
- Database engineering
- REST API development
- AI application development
- Cloud deployment
- Containerization
- CI/CD automation
- Application architecture
- Data processing and validation
