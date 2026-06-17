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
        INSERT INTO employees (...)
        VALUES (...)
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

