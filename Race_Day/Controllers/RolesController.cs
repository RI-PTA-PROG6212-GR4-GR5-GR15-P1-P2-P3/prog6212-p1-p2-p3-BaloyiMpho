using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Race_Day.Data;
using Race_Day.Models;
using Race_Day.Models.DTOs;

namespace Race_Day.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        public readonly ApplicationDbcontext dbcontext;
        public RolesController(ApplicationDbcontext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = dbcontext.roles.ToList();

            return Ok(roles);
        }
        [HttpGet("{id}")]
        public IActionResult GetRoleById(Guid id)
        {
            var role = dbcontext.roles.FirstOrDefault(r => r.RoleId == id);
            if (role == null)
            {
                return NotFound(new { message = "Role not found." });
            }
            return Ok(role);
        }
        [HttpGet]
        [Route("GetRoleByName/{roleName}")]
        public IActionResult GetRoleByName(string roleName)
        {
            var role = dbcontext.roles.FirstOrDefault(r => r.RoleName == roleName);
            if (role == null)
            {
                return NotFound(new { message = "Role not found." });
            }
            return Ok(role);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewRole(NewRoleDTO roleDto) 
        {
            if (dbcontext.roles.Any(r => r.RoleName == roleDto.RoleName))
                return Conflict(new { message = "Role already exists." });
            var newRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = roleDto.RoleName,
                Description = roleDto.Description,
                CreatedAt = DateTime.UtcNow
            };
            
            dbcontext.roles.Add(newRole);
            dbcontext.SaveChanges();
            return Ok(new { message = "Role added successfully." });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteRole(Guid id)
        {
            var role = dbcontext.roles.Find(id);
            if (role == null)
            {
                return NotFound(new { message = "Role not found." });
            }
            dbcontext.roles.Remove(role);
            dbcontext.SaveChanges();
            return Ok(new { message = "Role deleted successfully." });
        }


    }
}
