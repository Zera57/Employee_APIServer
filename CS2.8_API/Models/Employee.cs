using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace API_Employee.Models
{
	public class DataBD
	{
		private SqlConnection sqlConnection;

		public DataBD()
		{
			string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={AppContext.BaseDirectory}App_Data\Database.mdf;Integrated Security=True";

			sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
		}

		public List<Employee> GetEmployees(int DepartmentId)
		{
			List<Employee> list = new List<Employee>();

			using (SqlCommand com = new SqlCommand($@"SELECT * FROM Employee WHERE DepartmentId = {DepartmentId}", sqlConnection))
			{
				using (SqlDataReader reader = com.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(
							new Employee()
							{
								Id = (int)reader["Id"],
								DepartmentId = (int)reader["DepartmentId"],
								Position = reader["Position"].ToString().Trim(),
								FirstName = reader["FirstName"].ToString().Trim(),
								SecondName = reader["SecondName"].ToString().Trim(),
								Age = (int)reader["Age"],
								BaseSalary = (int)reader["BaseSalary"],
								Hours = (int)reader["Hours"]
							});
					}
				}
			}

			return list;
		}

		public Employee GetEmployeeById(int Id)
		{
			Employee Employee = null;
			using (SqlCommand com = new SqlCommand($@"SELECT * FROM Employee WHERE Id={Id}", sqlConnection))
			{
				using (SqlDataReader reader = com.ExecuteReader())
				{
					while (reader.Read())
					{
						Employee =
							new Employee()
							{
								Id = (int)reader["Id"],
								DepartmentId = (int)reader["DepartmentId"],
								Position = reader["Position"].ToString().Trim(),
								FirstName = reader["FirstName"].ToString().Trim(),
								SecondName = reader["SecondName"].ToString().Trim(),
								Age = (int)reader["Age"],
								BaseSalary = (int)reader["BaseSalary"],
								Hours = (int)reader["Hours"]
							};
					}
				}
			}
			return Employee;
		}

		public bool InsertEmployee(Employee Employee)
		{
			try
			{
				string sqlAdd = $@"INSERT INTO Employee(DepartmentId, Position,	FirstName,
														SecondName, Age, BaseSalary, Hours)
												VALUES({Employee.DepartmentId},
														N'{Employee.Position}',
														N'{Employee.FirstName}',
														N'{Employee.SecondName}',
														{Employee.Age},
														{Employee.BaseSalary},
														{Employee.Hours});
														";
				using (var com = new SqlCommand(sqlAdd, sqlConnection))
				{
					com.ExecuteNonQuery();
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool UpdateEmployee(Employee Employee)
		{
			try
			{
				string sqlUpdate = $@"UPDATE Employee SET FirstName = N'{Employee.FirstName}', SecondName = N'{Employee.SecondName}', Position = N'{Employee.Position}', Age = {Employee.Age}, Hours = {Employee.Hours}, BaseSalary = {Employee.BaseSalary}, DepartmentId = {Employee.DepartmentId} WHERE ID = {Employee.Id}";
				using (var com = new SqlCommand(sqlUpdate, sqlConnection))
				{
					com.ExecuteNonQuery();
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool DeleteEmployee(Employee Employee)
		{
			try
			{
				string sqlDelete = $@"DELETE FROM Employee WHERE ID = {Employee.Id}";

				using (var com = new SqlCommand(sqlDelete, sqlConnection))
				{
					com.ExecuteNonQuery();
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public List<Department> GetDepartments()
		{
			List<Department> list = new List<Department>();

			using (SqlCommand com = new SqlCommand(@"SELECT * FROM Departments", sqlConnection))
			{
				using (SqlDataReader reader = com.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(
							new Department()
							{
								Id = (int)reader["Id"],
								Name = reader["Name"].ToString().Trim()
							});
					}
				}
			}

			return list;
		}

		public bool InsertDepartment(Department department)
		{
			department.Id = GetDepartments().Count + 1;
			try
			{
				string sqlAdd = $@"INSERT INTO Departments(ID, Name) VALUES({department.Id}, N'{department.Name}');";
				using (var com = new SqlCommand(sqlAdd, sqlConnection))
				{
					com.ExecuteNonQuery();
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool UpdateDepartment(Department department)
		{
			try
			{
				string sqlUpdate = $@"UPDATE Departments SET Name = N'{department.Name}' WHERE ID = {department.Id}";
				using (var com = new SqlCommand(sqlUpdate, sqlConnection))
				{
					com.ExecuteNonQuery();
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool DeleteDepartment(int Id)
		{
			try
			{
				string sqlDelete = $@"DELETE FROM Departments WHERE ID = {Id}";

				using (var com = new SqlCommand(sqlDelete, sqlConnection))
				{
					com.ExecuteNonQuery();
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

	}

	public class Department
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

	public class GavnoEmployee
	{
		public string FirstName { get; set; }
		public string SecondName { get; set; }

		public string Position { get; set; }

		public int Age { get; set; }
		public int Hours { get; set; }
		public int BaseSalary { get; set; }
		public int Id { get; set; }
		public int DepartmentId { get; set; }
	}
		public class Employee
	{
		public string FirstName { get; set; }
		public string SecondName { get; set; }

		public string Position { get; set; }

		public int Age { get; set; }
		public int Hours { get; set; }
		public int BaseSalary { get; set; }
		public int Salary => BaseSalary * Hours;
		public int Id { get; set; }
		public int DepartmentId { get; set; }

		public void AddHours()
		{
			Hours += 8;
		}

		public override string ToString()
		{
			return Id.ToString() + " " + FirstName + " " + SecondName;
		}

		public DataRow UpdateRow(DataRow EmployeeRow)
		{
			EmployeeRow["Id"] = this.Id;
			EmployeeRow["DepartmentId"] = this.DepartmentId;
			EmployeeRow["FirstName"] = this.FirstName;
			EmployeeRow["SecondName"] = this.SecondName;
			EmployeeRow["Position"] = this.Position;
			EmployeeRow["Age"] = this.Age;
			EmployeeRow["BaseSalary"] = this.BaseSalary;
			EmployeeRow["Hours"] = this.Hours;

			return EmployeeRow;
		}
	}
}