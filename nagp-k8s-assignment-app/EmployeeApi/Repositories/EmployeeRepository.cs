using Dapper;
using EmployeeApi.Models;
using Npgsql;
using System.Data;

namespace EmployeeApi.Repositories;

public class EmployeeRepository
{
    private readonly IConfiguration _configuration;

    public EmployeeRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetConnectionString()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        // Use env vars if available, otherwise fallback to appsettings (local dev)
        if (!string.IsNullOrEmpty(host))
        {
            return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
        }

        return _configuration.GetConnectionString("Default")!;
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        
        string sql = @"
            SELECT *
            FROM employees
            ORDER BY employee_id";

        return await connection.QueryAsync<Employee>(sql);
    }

    public async Task<Employee?> GetEmployee(int id)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());

        string sql = @"
            SELECT *
            FROM employees
            WHERE employee_id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
    }

    public async Task<int> CreateEmployee(Employee employee)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());

        string sql = @"
        INSERT INTO employees (employee_code, first_name, last_name, email, phone_number, 
                               department, designation, manager_name, location, 
                               joining_date, employment_type, salary, is_active)
        VALUES (@Employee_Code, @First_Name, @Last_Name, @Email, @Phone_Number,
                @Department, @Designation, @Manager_Name, @Location,
                @Joining_Date, @Employment_Type, @Salary, @Is_Active)
        RETURNING employee_id;";

        return await connection.ExecuteScalarAsync<int>(sql, employee);
    }
}

public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value)
    {
        return value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType()} to DateOnly")
        };
    }
}

