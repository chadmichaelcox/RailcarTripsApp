# Railcar Trips Processing Application

## Test Guide

Write a Blazor Web Assembly application with the following requirements:

- Create a 'Railcar Trips' page.
- The page has a way to process railcar/equipment events into trips by uploading a file such as the attached: equipment_events.csv. Note: event times are local to the time zone of the corresponding city and are not ordered in the attached file.
- Events should be processed into trips per railcar/equipment and stored in a database where Trip is a parent.
- The logic for processing trips is as follows:
  - Event code W (Released) starts a new trip.
  - Event code Z (Placed) ends a trip.
- Trips should contain fields:  equipment id, origin city id, destination city id, start utc, end utc, and total trip hours (all stored in the database).
- The railcar trips page should show the processed trips in a grid (equipment id, origin, destination, start date/time, end date/time, total trip hours).
- There should be a way to select a particular trip and see its events in order (bonus / nice to have).
- Entity Framework should be used for database access.
- Note: the other 2 CSV files can be scripted into the database (don't need a UI to upload or configure).

Please use your discretion for how much time to spend on this. We’re looking to understand how you would organize an appropriately sized solution for this task and considerations you’ve made, rather than a complete fully flushed out working solution. TODO comments are appropriate to show considerations you’ve made that you don’t have time to fully implement. Please also list questions you have and the assumptions you’ve made to answer them.

Code can be submitted as a zip file attachment to this email or a repository such as GitHub.

## Overview

The Railcar Trips Processing Application is a Blazor WebAssembly application designed to process railcar/equipment events from CSV files, organize them into trips, and store the data in a SQL Server database. The application provides a user-friendly interface to upload event files, view processed trips, and analyze trip details, including events associated with each trip.

This project follows a clean architecture approach with three separate projects:

- The Blazor WebAssembly frontend.
- The ASP.NET Core Web API backend.
- Shared models between the client and server.

---

## Features

- **File Upload**: Users can upload a CSV file containing railcar/equipment events.
- **Data Processing**: The application converts event records into trips.
- **Database Storage**: Processed trips and event details are stored in SQL Server.
- **Trip Listing**: Users can view all processed trips in a table format.
- **Event History**: Clicking on a trip allows users to view detailed event records.
- **Modern UI**: The interface is built with Blazor WebAssembly, CSS Grid, and toastr.js for notifications.
- **Entity Framework Core**: Used for database access.
- **Cross-Origin Support**: Configured CORS settings allow frontend-backend communication.

---

## Technologies Used

| Technology                 | Purpose                      |
| -------------------------- | ---------------------------- |
| **Blazor WebAssembly**     | Frontend UI framework        |
| **ASP.NET Core Web API**   | Backend logic & API handling |
| **Entity Framework Core**  | Database ORM for SQL Server  |
| **SQL Server**             | Data storage                 |
| **CsvHelper**              | CSV parsing & processing     |
| **JavaScript (toastr.js)** | UI notifications             |
| **CSS Grid & Flexbox**     | Responsive layout            |

---

## Installation & Setup

### **1. Prerequisites**

Ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (or **VS Code** with C# extension)
- [Postman](https://www.postman.com/) (Optional for API testing)

### **2. Clone the Repository**

```sh
git clone https://github.com/chadmichaelcox/RailcarTripsApp.git
cd RailcarTripsApp
```

### **3. Database Configuration**

#### **A. Update Connection String**

Modify `appsettings.json` in ``:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=RailcarTrips;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Replace `YOUR_SERVER` with your **SQL Server instance name**.

#### **B. Apply Migrations & Create Database**

```sh
cd RailcarTripsApp.Server

# Ensure EF Core CLI is installed
dotnet tool install --global dotnet-ef

# Apply migrations
dotnet ef migrations add InitialCreate

dotnet ef database update
```

---

### **4. Run the Application**

#### **Start the Backend API**

```sh
cd RailcarTripsApp.Server

# Run the backend
dotnet run
```

#### **Start the Frontend**

```sh
cd RailcarTripsApp.Client

# Run Blazor WebAssembly Client
dotnet run
```

The application will be available at:

- **Frontend:** `http://localhost:5204`
- **Backend API:** `http://localhost:5000`

---

## Using Postman to Seed Initial Data
To prepopulate the **Cities** and **EventCodes** tables, use the provided **Postman collection** (`RailcarTripsAppAPI.postman_collection.json`).

### **Steps to Import and Use the Postman Collection**
1. **Open Postman** and navigate to **File > Import**.
2. Select and import `RailcarTripsAppAPI.postman_collection.json`.
3. Locate the `Inject Cities` and `Inject Event Codes` API requests.
4. Click **Send** to execute each request and populate the database.

These requests upload `canadian_cities.csv` and `event_code_definitions.csv` to the appropriate API endpoints:
- **Seed Cities:** `POST http://localhost:5000/api/data-seed/seed-cities`
- **Seed Event Codes:** `POST http://localhost:5000/api/data-seed/seed-event-codes`

---

## Usage Guide

### **1. Upload a CSV File**

- Click **"Choose File"** and select `equipment_events.csv`.
- Click **"Upload & Process"**.
- The system will parse events and create trips.

### **2. View Processed Trips**

- The **Trips Table** displays processed trips with columns:
  - **Equipment ID**
  - **Origin City**
  - **Destination City**
  - **Start & End Time (UTC)**
  - **Total Hours**
  - **Actions (View Events)**

### **3. View Trip Events**

- Click **"View Events"** next to a trip.
- The **Trip Events Table** will show:
  - **Event Code**
  - **City**
  - **Local & UTC Time**
 
---

## Assumptions and Justifications

| Thought                                                                  | Answer                                                                                                                                                                     |
| ------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Would a user prefer seeing the city name instead of a city ID?           | Users would rather view city names over IDs in the GUI (IDs are used in the database per the requirements).                                                                |
| How should trips be ordered?                                             | Trips are ordered by Equipment ID and further ordered chronologically by Start UTC.                                                                                        |
| Should comments be used in the code?                                     | Some companies/people have strong opinions on comments. Instead, descriptive naming was used as per standard practice.                                                     |
| How is the time difference handled?                                      | The local-to-UTC conversion is done in the CSV file API call.                                                                                                              |
| How should front-end components be structured?                           | Standard Blazor practices were used to create reusable components for the file upload and tables.                                                                          |
| How is styling applied?                                                  | A global `site.css` file is used for styling.                                                                                                                              |
| How should users be notified about file uploads?                         | Toastr notifications inform the user of the success or failure of the CSV file upload.                                                                                     |
| What is assumed about CSV file formats?                                  | The CSV file format (header names and column numbers) is assumed to always remain the same.                                                                                |
| Would users need to modify or delete trips after processing?             | No modification or deletion of processed trips is currently allowed, assuming that uploaded data is correct and immutable. However, this could be reconsidered.            |
| Should time zones be handled differently?                                | The system assumes cities always have correct time zone mappings and converts local event times to UTC. Handling daylight savings changes might require extra validation. |
| Do we expect huge data loads?                                            | The app assumes a moderate number of trips and events. For larger datasets, batch processing and database indexing may be required.                                        |
| Should uploaded CSV files be retained?                                   | Currently, the application processes CSV files but does not store them for future reference. Storing them in Azure Blob Storage could be an option.                        |
| Should API endpoints be exposed publicly?                                | The API is currently open but should be secured using authentication methods like JWT or OAuth before real-world deployment.                                               |
| How should access be managed?                                            | There is no user authentication or role-based access control implemented yet. Using Azure Active Directory (AAD) or Identity Server could restrict access.                 |

---

## Further Implementations

- Add unit and integration tests using xUnit and Moq for automated testing of controllers and services.
- Refactor services and business logic to leverage dependency injection more effectively, promoting modularity and easier unit testing.
- Do something custom instead of the standard Blazor WASM load GIF on app initialization.
- Implement logic in the TripsController to skip or overwrite duplicate entries when uploading a new file.
- Improve CSS styling for better UI aesthetics.
- Add responsive design to adjust automatically to different screen sizes (currently optimized for 1920x1080).
- Implement search and filtering in tables for a better user experience.
- Implement user authentication and authorization to control access to the application.
- Add loading indicators for API calls that take time to process.
- Implement a reset button to clear existing data before uploading a new file.
- Use DTOs (Data Transfer Objects) for structured CRUD operations and dynamic table rendering.
- Once the above point is complete, replace the two table components with one that can be reused to generate various tables dependent on the parameters passed to it.
- Secure access to API endpoints using JWT-based authentication with bearer tokens to ensure only authorized users can interact with the backend.
- Improve API error handling and logging to provide better debugging and production support.
- Implement bulk trip deletion and modification so that users can delete or modify incorrectly processed trips.
- Support additional file formats by extending file processing to support Excel (.xlsx) and JSON for broader compatibility.
- Enable user roles and permissions to restrict functionalities based on user roles such as Admin, Viewer, and Data Uploader.
- Add notifications for long-running tasks to provide real-time updates on file processing progress.
- Optimize database queries to improve EF Core query performance by using stored procedures or batch processing.
- Implement audit logging for data changes to keep a history of changes to trips and events for auditing and debugging purposes.
- Add a dark mode toggle to improve UI/UX by allowing users to switch between light and dark mode.
- Introduce CSV processing validation to implement detailed validation checks to handle missing or malformed data before inserting into the database.
- Implement pagination for tables to avoid large dataset loading issues by utilizing server-side pagination.
- Display user-friendly error messages instead of raw exceptions to provide more informative and structured error handling.
- Support multi-language UI by implementing localization for different regions to enhance accessibility.
- Enable data export functionality to allow users to export trips and events in CSV, JSON, or PDF formats.


### **Real-World Implementation in Azure**

- Deploy the frontend as an Azure Static Web App with Azure Blob Storage for storing uploaded CSV files.
- Deploy the backend API to Azure App Services with SQL Server Azure Database.
- Use Azure Functions to process the uploaded CSV asynchronously.
- Implement Azure Active Directory (AAD) authentication for secure access.
- Utilize Application Insights for logging and monitoring API performance.
- Containerize the application with Docker to deploy the API using Azure Kubernetes Service (AKS) for enhanced scalability.
- Use Azure Key Vault to secure database connection strings, API keys, and JWT secrets by storing them securely.
- Improve performance using Azure Redis Cache by caching frequent queries such as trip lists to enhance application response time.
- Set up CI/CD with GitHub Actions and Azure DevOps to automate deployments to Azure App Services using structured pipelines.
- Enable API rate limiting and throttling to protect API endpoints from abuse and overuse by implementing rate limiting policies.
- Deploy the app to multiple Azure regions to ensure high availability and disaster recovery in case of failures.

---

## License

This project is licensed under the **MIT License**. See `LICENSE` for details.

Thank you for using the Railcar Trips Processing Application.
