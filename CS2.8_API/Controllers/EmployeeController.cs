using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using API_Employee.Models;
using Newtonsoft.Json;

namespace API_Employee._8_API.Controllers
{
	public class EmployeeController : ApiController
	{

		DataBD data = new DataBD();

		[Route("getDepartments")]
		public IEnumerable<Department> GetDepartments() => data.GetDepartments();

		[Route("addDepartment")]
		public HttpResponseMessage InsertDepartment([FromBody]Department department)
		{
			return data.InsertDepartment(department)
				? Request.CreateResponse(HttpStatusCode.Created)
				: Request.CreateResponse(HttpStatusCode.BadRequest);
		}

		[Route("updateDepartment")]
		public HttpResponseMessage UpdateDepartment([FromBody]Department department)
		{
			return data.UpdateDepartment(department)
				? Request.CreateResponse(HttpStatusCode.Created)
				: Request.CreateResponse(HttpStatusCode.BadRequest);

		}

		[HttpPost]
		[Route("deleteDepartment")]
		public HttpResponseMessage DeleteDepartment([FromBody]Department department)
		{
			return data.DeleteDepartment(department.Id)
				? Request.CreateResponse(HttpStatusCode.OK)
				: Request.CreateResponse(HttpStatusCode.BadRequest);
		}

		[Route("getEmployee/list/{DepartmentId}")]
		public IEnumerable<Employee> GetAllEmployees(int DepartmentId) => data.GetEmployees(DepartmentId);

		[Route("getEmployee/{id}")]
		public Employee GetEmployee(int id) => data.GetEmployeeById(id);

		[Route("addEmployee")]
		public HttpResponseMessage InsertEmployee([FromBody]Employee Employee)
		{
			return data.InsertEmployee(Employee)
				? Request.CreateResponse(HttpStatusCode.Created)
				: Request.CreateResponse(HttpStatusCode.BadRequest);
		}

		[Route("updateEmployee")]
		public HttpResponseMessage UpdateEmployee([FromBody]Employee Employee)
		{
			return data.UpdateEmployee(Employee)
				? Request.CreateResponse(HttpStatusCode.Created)
				: Request.CreateResponse(HttpStatusCode.BadRequest);

		}

		[HttpPost]
		[Route("deleteEmployee")]
		public HttpResponseMessage DeleteEmployee([FromBody]Employee Employee)
		{
			return data.DeleteEmployee(Employee)
				? Request.CreateResponse(HttpStatusCode.Created)
				: Request.CreateResponse(HttpStatusCode.BadRequest);
		}
	}
}
