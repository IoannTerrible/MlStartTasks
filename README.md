<br/>
<p align="center">
  <h3 align="center">CoolREADME file</h3>

  <p align="center">
    
  </p>
</p>

![Stargazers](https://img.shields.io/github/stars/ShaanCoding/ReadME-Generator?style=social) ![License](https://img.shields.io/github/license/ShaanCoding/ReadME-Generator) 

# MlStartTasks
StartDate:14.01.2024  
## Technologies Used

- 🛢️ **DBMS**: Microsoft SQL Server
- 🖥️ **Framework**: WPF app targeting .NET 8.0, including net8.0-windows, and utilizing WPF components.
- 🚀 **Project Entry Points**:
  - For the client: `SocketClient\App.Xaml.cs.Main`
  - For the Host Server: `ServerHost\Programm.cs`
- 📜 **Logs Directory**: `(SocketClient\ServerHost)\bin\Debug\logs\`
- ⚙️ **Configuration File**: `SocketClient\bin\Debug\config.xml` 

## Database Structure
DataBase name: MLstartDataBase

### Table `Userss`
```sql
CREATE TABLE IF NOT EXISTS Userss (
    Personid INT PRIMARY KEY IDENTITY,
    Login VARCHAR(255) NOT NULL,
    PassWord VARCHAR(255) NOT NULL
);
