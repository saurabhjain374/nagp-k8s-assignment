public class Employee
{
    public int Employee_Id { get; set; }
    public string Employee_Code { get; set; } = string.Empty;
    public string First_Name { get; set; } = string.Empty;
    public string Last_Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone_Number { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Manager_Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly Joining_Date { get; set; }
    public string Employment_Type { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public bool Is_Active { get; set; }
}