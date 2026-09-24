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

Angular Frontend
       │
       ▼
ASP.NET Core Web API
       │
       ├── Business Services
       │
       ├── AI Services / Tool Calling
       │
       └── Data Access
               │
               ▼
          SQL Server
