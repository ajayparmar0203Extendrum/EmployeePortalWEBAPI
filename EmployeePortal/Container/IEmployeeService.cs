using EmployeePortal.Helper;
using EmployeePortal.Models;

namespace EmployeePortal.Service
{
    public interface IEmployeeService
    {
        Task<(List<Employee> Employees, int TotalCount)> GetEmployees(
           string? searchTerm,
           int? departmentId,
           int? employeeTypeId,
           int pageNumber,
           int pageSize
       );

        Task<Employee?> GetEmployeeByIdAsync(int id);

        Task<APIResponse> CreateEmployeeAsync(Employee employee);

        Task<Employee> UpdateEmployeeAsync(Employee employee);

        Task<APIResponse> DeleteEmployeeAsync(int id);

        // Master tables retrieval methods
        Task<List<Department>> GetDepartmentsAsync();

        Task<List<EmployeeType>> GetEmployeeTypesAsync();

        Task<List<Designation>> GetDesignationsByDepartmentAsync(int departmentId);

        
    }
}
