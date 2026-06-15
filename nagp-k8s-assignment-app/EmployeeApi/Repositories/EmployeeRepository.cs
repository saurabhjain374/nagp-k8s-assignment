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

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        var connectionString =
            _configuration.GetConnectionString("Default");

        using var connection =
            new NpgsqlConnection(connectionString);

        string sql = @"
            SELECT *
            FROM employees
            ORDER BY employee_id";

            return await connection.QueryAsync<Employee>(sql);
    }

    public async Task<Employee?> GetEmployee(int id)
    {
        var connectionString =
            _configuration.GetConnectionString("Default");

        using var connection =
            new NpgsqlConnection(connectionString);

        string sql = @"
            SELECT *
            FROM employees
            WHERE employee_id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Employee>(
            sql,
            new { Id = id });
    }

    public async Task<int> CreateEmployee(Employee employee)
    {
        var connectionString =
            _configuration.GetConnectionString("Default");

        using var connection =
            new NpgsqlConnection(connectionString);

        string sql = @"
        INSERT INTO employees
        (
            employee_code,
            first_name,
            last_name,
            email,
            phone_number,
            department,
            designation,
            manager_name,
            location,
            joining_date,
            employment_type,
            salary,
            is_active
        )
        VALUES
        (
            @employee_code,
            @first_name,
            @last_name,
            @email,
            @phone_number,
            @department,
            @designation,
            @manager_name,
            @location,
            @joining_date,
            @employment_type,
            @salary,
            @is_active
        )
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

