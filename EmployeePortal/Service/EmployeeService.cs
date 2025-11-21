using EmployeePortal.Data;
using EmployeePortal.Helper;
using EmployeePortal.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Service
{
    public class EmployeeService : IEmployeeService
    {

        private readonly AppDBContext _dbContext;
        public EmployeeService(AppDBContext dBContext) 
        {
            _dbContext = dBContext;
        }

        public async Task<APIResponse> CreateEmployeeAsync(Employee employee)
        {
            APIResponse response=new APIResponse();
            bool isValid=true;
            int userid = 0;
            try
            {
                //duplicate employee 
                var employeeData=await _dbContext.Employees.Where(x=>x.Email==employee.Email).ToListAsync();
                if (employeeData.Count > 0)
                {
                    isValid = false;
                    response.Result = "Fail";
                    response.Message = "Duplicate Email or EmpId";

                }
                if(employee != null)
                {
                    var data= await  _dbContext.Employees.AddAsync(employee);
                    await _dbContext.SaveChangesAsync();
                    userid = employee.Id;
                    response.Result = "Pass";
                    response.Message = "New Employee Created Successfully-"+userid;
                    
                }

            }
            catch (Exception ex)
            {

                response.Result = ex.Message;
            }
            return response;
            
        }

        public async Task<APIResponse> DeleteEmployeeAsync(int id)
        {
            APIResponse response = new APIResponse();
            if (id > 0)
            {
                
                var data=_dbContext.Employees.Where(x=>x.Id==id).FirstOrDefault();
                if (data != null)
                {
                   _dbContext.Remove(data);
                    _dbContext.SaveChanges();
                    response.Result = "Pass";
                    response.Message = " Employee Remove Successfully-" +id;


                }
            }
            return response;
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _dbContext.Departments.AsNoTracking().ToListAsync();
        }

        public async Task<List<Designation>> GetDesignationsByDepartmentAsync(int departmentId)
        {
            APIResponse response =new APIResponse();
            var data =new List<Designation>();
            if(departmentId > 0)
            {
                data= await _dbContext.Designations
                .Where(d => d.DepartmentId == departmentId)
                .AsNoTracking()
                .ToListAsync();
                return data;
            }

            return data;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            
            if(id > 0)
            {
                var data=await _dbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .Include(e => e.EmployeeType)
            .FirstOrDefaultAsync(e => e.Id == id);
                if (data != null)
                {
                    return data;
                    
                }
            }
            return null;

        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetEmployees(string? searchTerm, int? departmentId, int? employeeTypeId, int pageNumber, int pageSize)
        {
            var query = _dbContext.Employees
               .Include(e => e.Department)
               .Include(e => e.Designation)
               .Include(e => e.EmployeeType)
               .AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e => e.FullName.Contains(searchTerm));
            }
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }
            if (employeeTypeId.HasValue && employeeTypeId.Value > 0)
            {
                query = query.Where(e => e.EmployeeTypeId == employeeTypeId.Value);
            }

            var totalCount = await query.CountAsync();

            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (employees, totalCount);


        }

        public async Task<List<EmployeeType>> GetEmployeeTypesAsync()
        {
            return await _dbContext.EmployeeTypes.AsNoTracking().ToListAsync();
        }

        public async Task<Employee?> UpdateEmployeeAsync(Employee employee)
        {
            var existing = await _dbContext.Employees.FindAsync(employee.Id);
            if (existing == null) return null;

             _dbContext.Entry(existing).CurrentValues.SetValues(employee);
            await _dbContext.SaveChangesAsync();

            if (existing != null) 
            {
                var department=_dbContext.Departments.Where(x=>x.Id == existing.DepartmentId).FirstOrDefault();
                var designNation=_dbContext.Designations.Where(x=>x.Id==existing.DesignationId).FirstOrDefault();
                var employeeType=_dbContext.EmployeeTypes.Where(x=>x.Id!=existing.EmployeeTypeId).FirstOrDefault();

                if (department != null && designNation != null && employeeType != null) 
                {
                    existing.Department= department;
                    existing.Designation= designNation;
                    existing.EmployeeType= employeeType;
                }

            }

            return existing;
        }
    }
}
