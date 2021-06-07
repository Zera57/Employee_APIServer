using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2._5
{
	public class Model
	{
		public List<Department> CurrentDepartments;

		SqlDataAdapter adapterDepartment;
		SqlDataAdapter adapterEmployee;

		DataTable dtDepartment;
		DataTable dtEmployee;


		public Model(SqlConnection connection)
		{
			CurrentDepartments = new List<Department>();

			#region Department

			adapterDepartment = new SqlDataAdapter();

			SqlCommand command = new SqlCommand("SELECT ID, Name FROM Departments", connection);

			adapterDepartment.SelectCommand = command;

			dtDepartment = new DataTable();
			adapterDepartment.Fill(dtDepartment);

			#region InsertDepartment
			command = new SqlCommand(@"INSERT INTO Departments (Id, Name) VALUES (@Id, @Name);", connection);
			command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
			command.Parameters.Add("@Name", SqlDbType.NChar, 20, "Name");

			SqlParameter param;// = command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
			//param.Direction = ParameterDirection.Output;
			adapterDepartment.InsertCommand = command;
			#endregion
			#endregion

			#region Employee
			adapterEmployee = new SqlDataAdapter();

			command = new SqlCommand("SELECT * FROM Employee", connection);
			adapterEmployee.SelectCommand = command;

			dtEmployee = new DataTable();
			adapterEmployee.Fill(dtEmployee);



			//Заносим департаменты
			foreach (DataRow row in dtDepartment.Rows)
			{
				CurrentDepartments.Add(new Department(row["Name"].ToString()));

				var Employees =	from DataRow n       
								in dtEmployee.Rows
								where (int)n["DepartmentId"] == CurrentDepartments.Count
								select n;
				//Заносим рабочих
				foreach  (var RowEmployee in Employees)
				{
					CurrentDepartments.Last().AddEmployee(new Employee(RowEmployee));
				}
			}
			//Добавляем кнопку Departments Add
			CurrentDepartments.Add(new Department("Add new Department +"));

			#region InsertEploee
			command = new SqlCommand(@"INSERT INTO Employee (FirstName, SecondName, Position, Age, Hours, BaseSalary, DepartmentId)
													VALUES (@FirstName, @SecondName, @Position, @Age, @Hours, @BaseSalary, @DepartmentId);
													SET @ID = @@IDENTITY;", connection);

			command.Parameters.Add("@FirstName", SqlDbType.NChar, 30, "FirstName");
			command.Parameters.Add("@SecondName", SqlDbType.NChar, 30, "SecondName");
			command.Parameters.Add("@Position", SqlDbType.NChar, 30, "Position");
			command.Parameters.Add("@Age", SqlDbType.Int, 0, "Age");
			command.Parameters.Add("@Hours", SqlDbType.Int, 0, "Hours");
			command.Parameters.Add("@BaseSalary", SqlDbType.Int, 01, "BaseSalary");
			command.Parameters.Add("@DepartmentId", SqlDbType.Int, 0, "DepartmentId");

			param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
			param.Direction = ParameterDirection.Output;
			adapterEmployee.InsertCommand = command;
			#endregion

			#region UpdateEploee
			command = new SqlCommand(@"UPDATE Employee SET FirstName = @FirstName, SecondName = @SecondName, Position = @Position, Age = @Age, Hours = @Hours,
															BaseSalary = @BaseSalary, DepartmentId = @DepartmentId WHERE ID = @ID", connection);

			command.Parameters.Add("@FirstName", SqlDbType.NChar, 30, "FirstName");
			command.Parameters.Add("@SecondName", SqlDbType.NChar, 30, "SecondName");
			command.Parameters.Add("@Position", SqlDbType.NChar, 30, "Position");
			command.Parameters.Add("@Age", SqlDbType.Int, 0, "Age");
			command.Parameters.Add("@Hours", SqlDbType.Int, 0, "Hours");
			command.Parameters.Add("@BaseSalary", SqlDbType.Int, 01, "BaseSalary");
			command.Parameters.Add("@DepartmentId", SqlDbType.Int, 0, "DepartmentId");

			param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
			param.SourceVersion = DataRowVersion.Original;
			adapterEmployee.UpdateCommand = command;
			#endregion

			#region DeleteEmployee
			command = new SqlCommand("DELETE FROM Employee WHERE ID = @ID", connection);
			param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
			param.SourceVersion = DataRowVersion.Original;
			adapterEmployee.DeleteCommand = command;
			#endregion
			#endregion
		}

		public void InsertDepartment(Department department)
		{
			DataRow row = dtDepartment.NewRow();
			row["Id"] = CurrentDepartments.Count;
			row["Name"] = department.name;
			dtDepartment.Rows.Add(row);
			adapterDepartment.Update(dtDepartment);
			CurrentDepartments.Insert(CurrentDepartments.Count - 1, department);
		}

		public void InsertEmployee(Employee Employee)
		{
			DataRow row = dtEmployee.NewRow();
			Employee.UpdateRow(row);
			dtEmployee.Rows.Add(row);
			adapterEmployee.Update(dtEmployee);
			adapterEmployee.Fill(dtEmployee);
			Employee.Id = (int)dtEmployee.Rows[dtEmployee.Rows.Count - 1]["Id"];
		}

		public void UpdateEmployee(Employee Employee)
		{
			foreach (DataRow row in dtEmployee.Rows)
			{
				if(row["Id"].ToString() == Employee.Id.ToString())
				{
					Employee.UpdateRow(row);
				}
			}
			adapterEmployee.Update(dtEmployee);
		}

		public void DeleteEmployee(Employee Employee)
		{
			foreach (DataRow row in dtEmployee.Rows)
			{
				if (row["Id"].ToString() == Employee.Id.ToString())
					row.Delete();
			}
			adapterEmployee.Update(dtEmployee);
		}
	}

	public class Department
	{
		public string name { get; }
		public ObservableCollection<Employee> Employees { get; private set; }

		public Department(string _name)
		{
			name = _name;
			Employees = new ObservableCollection<Employee>();
		}

		public Department(string _name, IEnumerable<Employee> collection)
		{
			name = _name;
			Employees = new ObservableCollection<Employee>(collection);
		}


		public void AddEmployee(Employee Employee)
		{
			Employees.Add(Employee);
		}

		public void RemoveEmployee(Employee Employee)
		{
			Employees.Remove(Employee);
		}

		public override string ToString()
		{
			return name;
		}
	}


	public class Employee
	{
		string			FirstName { get; }
		string			SecondName { get; }

		public string	Position { get; }

		public int		Age	{ get; }
		public int		Hours { get; private set; }
		public int		BaseSalary { get; }
		public int		Salary => BaseSalary * Hours;
		public int		Id { get; set; }
		public int		DepartmentId { get; }

		public Employee(int _Id, int _DepartmentId, string _Position, string _FirstName, string _SecondName, int _Age, int _BaseSalary, int _Hours)
		{
			Id = _Id;
			DepartmentId = _DepartmentId;
			Position = _Position;
			FirstName = _FirstName;
			SecondName = _SecondName;
			Age = _Age;
			BaseSalary = _BaseSalary;
			Hours = _Hours;
		}

		public Employee(int _DepartmentId, string _Position, string _FirstName, string _SecondName, int _Age, int _BaseSalary, int _Hours)
		{
			DepartmentId = _DepartmentId;
			Position = _Position;
			FirstName = _FirstName;
			SecondName = _SecondName;
			Age = _Age;
			BaseSalary = _BaseSalary;
			Hours = _Hours;
		}

		public Employee(DataRow row)
		{
			Id = (int)row["Id"];
			DepartmentId = (int)row["DepartmentId"];
			Position = row["Position"].ToString().Trim();
			FirstName = row["FirstName"].ToString().Trim();
			SecondName = row["SecondName"].ToString().Trim();
			Age = (int)row["Age"];
			BaseSalary = (int)row["BaseSalary"];
			Hours = (int)row["Hours"];
		}

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
