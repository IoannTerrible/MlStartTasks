<br/>
<p align="center">
  <h3 align="center">CoolREADME file</h3>

  <p align="center">
    
  </p>
</p>

![Stargazers](https://img.shields.io/github/stars/ShaanCoding/ReadME-Generator?style=social) ![License](https://img.shields.io/github/license/ShaanCoding/ReadME-Generator) 

## Commit Message Template:
[Version Number] Summary of the changes made  
### Example:  
1.009 Merge and small code improvements  

# MlStartTasks  
StartDate:14.01.2024  
## Technologies Used  
- 🛢️ **DBMS**: Microsoft SQL Server
- 🖥️ **Framework**: WPF app targeting .NET 8.0, including net8.0-windows, and utilizing WPF components.
- 🚀 **Project Entry Points**:
  - For the client: `Client\App.Xaml.cs.Main`
- 📜 **Logs Directory**: `Client\bin\Debug\logs\`
- ⚙️ **Configuration File**: `Client\bin\Debug\config.xml` 

## Database Structure
DataBase name: MLstartDataBase

### Table `Userss`  
```sql
CREATE TABLE IF NOT EXISTS Userss (
    Personid INT PRIMARY KEY IDENTITY,
    Login VARCHAR(255) NOT NULL,
    PassWord VARCHAR(255) NOT NULL
);
```
### Table `EventLog`  
```sql
CREATE TABLE IF NOT EXISTS EventLog (
    UserName VARCHAR(255) NULL,
    FileName VARCHAR(255) NULL,
    FramePath NVARCHAR(MAX) NULL,
    MetaData NVARCHAR(MAX) NULL
);
```
