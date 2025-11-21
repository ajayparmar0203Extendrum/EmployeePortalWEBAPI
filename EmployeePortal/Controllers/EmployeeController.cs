using EmployeePortal.Models;
using EmployeePortal.Service;
using EmployeePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService) 
        {
            _employeeService = employeeService;
        }

        [HttpGet("employeedetails")]
        public async Task<IActionResult> List(
            string? searchTerm,
            int? SelectedDepartmentId,
            int? SelectedEmployeeTypeId,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var (employees, totalCount) = await _employeeService.GetEmployees(searchTerm, SelectedDepartmentId, SelectedEmployeeTypeId, pageNumber, pageSize);


            EmployeeListViewModels viewModel = new EmployeeListViewModels();

            if(employees !=null)
            { 
            viewModel.Employees = employees;
            viewModel.PageNumber = pageNumber;
            viewModel.PageSize = pageSize;
            viewModel.Total = totalCount;
            viewModel.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            viewModel.SearchTerm = searchTerm;
            SelectedDepartmentId = SelectedDepartmentId;
            viewModel.SelectedEmployeeTypeId = SelectedEmployeeTypeId;
            viewModel.Departments = await _employeeService.GetDepartmentsAsync();
            viewModel.EmployeeTypes = await _employeeService.GetEmployeeTypesAsync();
                return Ok(viewModel);
            }
            
             return NotFound();
        }

        [HttpGet("create-data")]
        public async Task<IActionResult> GetCreateData()
        {
            var vm = new EmployeeCreateUpdateViewModel
            {
                Departments = await _employeeService.GetDepartmentsAsync(),
                EmployeeTypes = await _employeeService.GetEmployeeTypesAsync(),
                Designations = new List<Designation>() // initially empty
            };
            return Ok(vm);
        }

        [HttpPost("createemployee")]
        public async Task<IActionResult> Create(EmployeeModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = new Employee
            {
                FullName = vm.FullName,
                Email = vm.Email,
                DepartmentId = vm.DepartmentId,
                DesignationId = vm.DesignationId,
                HireDate = vm.HireDate,
                DateOfBirth = vm.DateOfBirth,
                EmployeeTypeId = vm.EmployeeTypeId,
                Gender = vm.Gender,
                Salary = vm.Salary,
            };

            var created = await _employeeService.CreateEmployeeAsync(employee);
            return Ok(created); // return created employee object (with Id)
        }

        //deleteemployee method
        [HttpDelete("deleteemployee")]
        public async Task<IActionResult> DeleteEmployee(int empId)
        {
            if (empId > 0)
            {
                var data = await _employeeService.DeleteEmployeeAsync(empId);
                return Ok(data);
            }
            return NotFound();
        }


        [HttpGet("getdesignationbyid")]
        public async Task<IActionResult> GetDesignationbyDeptId(int deptId)
        {
            if(deptId > 0)
            {
                var data=await _employeeService.GetDesignationsByDepartmentAsync(deptId);
                return Ok(data);
            }
            return NotFound();
        }


        // GET: api/employees/{id}
        [HttpGet("getemployeebyId")]
        public async Task<IActionResult> GetById(int empId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(empId);
            if (employee == null) return NotFound();

            var vm = new EmployeeCreateUpdateViewModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DesignationId = employee.DesignationId,
                HireDate = employee.HireDate,
                DateOfBirth = employee.DateOfBirth,
                EmployeeTypeId = employee.EmployeeTypeId,
                Gender = employee.Gender,
                Salary = employee.Salary,
                Departments = await _employeeService.GetDepartmentsAsync(),
                EmployeeTypes = await _employeeService.GetEmployeeTypesAsync(),
                Designations = await _employeeService.GetDesignationsByDepartmentAsync(employee.DepartmentId)
            };
            return Ok(vm);
        }

        // PUT: api/employees/{id}
        [HttpPut("updateemployee")]
        public async Task<IActionResult> Update(int id, EmployeeModel vm)
        {
            if (id != vm.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employee = new Employee
            {
                Id = vm.Id!.Value,
                FullName = vm.FullName,
                Email = vm.Email,
                DepartmentId = vm.DepartmentId,
                DesignationId = vm.DesignationId,
                HireDate = vm.HireDate,
                DateOfBirth = vm.DateOfBirth,
                EmployeeTypeId = vm.EmployeeTypeId,
                Gender = vm.Gender,
                Salary = vm.Salary
            };

            var updated = await _employeeService.UpdateEmployeeAsync(employee);
            if (updated == null) return NotFound();

            return Ok(updated);
        }


        //[HttpPost("employeecreate")]
        //public async Task<IActionResult> Create(EmployeeCreateUpdateViewModel vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var employee = new Employee
        //        {
        //            FullName = vm.FullName,
        //            Email = vm.Email,
        //            DepartmentId = vm.DepartmentId,
        //            DesignationId = vm.DesignationId,
        //            HireDate = vm.HireDate,
        //            DateOfBirth = vm.DateOfBirth,
        //            EmployeeTypeId = vm.EmployeeTypeId,
        //            Gender = vm.Gender,
        //            Salary = vm.Salary,
        //        };
        //        var data= await _employeeService.CreateEmployeeAsync(employee);
        //        vm.Departments = await _employeeService.GetDepartmentsAsync();
        //        vm.EmployeeTypes = await _employeeService.GetEmployeeTypesAsync();
        //        vm.Designations = vm.DepartmentId != 0 ? await _employeeService.GetDesignationsByDepartmentAsync(vm.DepartmentId) : new List<Designation>();
        //        return Ok(data);

        //    }
        //    return NotFound();

        //}

        //[HttpGet("getemployeebyid")]
        //public async Task<IActionResult> Update(int empid)
        //{
        //    if (empid != 0)
        //    {
        //        var employee=await _employeeService.GetEmployeeByIdAsync(empid);
        //        if (employee != null)
        //        {
        //            var vm = new EmployeeCreateUpdateViewModel
        //            {
        //                Id = employee.Id,
        //                FullName = employee.FullName,
        //                Email = employee.Email,
        //                DepartmentId = employee.DepartmentId,
        //                DesignationId = employee.DesignationId,
        //                HireDate = employee.HireDate,
        //                DateOfBirth = employee.DateOfBirth,
        //                EmployeeTypeId = employee.EmployeeTypeId,
        //                Gender = employee.Gender,
        //                Salary = employee.Salary,
        //                Departments = await _employeeService.GetDepartmentsAsync(),
        //                EmployeeTypes = await _employeeService.GetEmployeeTypesAsync(),
        //                Designations = await
        //                  _employeeService.GetDesignationsByDepartmentAsync(employee.DepartmentId)
        //            };
        //            //return View(vm);
        //            return Ok(employee);
        //        }
        //    }
        //    return NotFound();





    }
}
