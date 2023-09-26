using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JWTSwagger.Authentication;
using Swashbuckle.AspNetCore.Annotations;

namespace JWTExample.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class RolesController : Controller
  {
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
      _roleManager = roleManager;
    }

    [HttpGet]
    [SwaggerOperation(summary: "Return all roles", null)]
    [SwaggerResponse(200, "Success", typeof(RoleDTO))]
    public IActionResult Get() => Ok(_roleManager.Roles.OrderBy(r => r.Name).Select(r => new RoleDTO { Id = r.Id, Name = r.Name }));
    [HttpPost]
    [Route("create")]
    [SwaggerOperation(summary: "Create role", null)]
    [SwaggerResponse(204, "Role created", null)]
    public async Task<IActionResult> Create([FromBody] CreateRole model)
    {
      // Check if role already exists
      var role = await _roleManager.FindByNameAsync(model.RoleName);
      if (role != null)
        return Conflict(new { Message = $"{model.RoleName} is a duplicate Role" });

      var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to create role" });
      return NoContent();
    }
    [HttpDelete("{rolename}")]
    [SwaggerOperation(summary: "Delete role", null)]
    [SwaggerResponse(204, "Role deleted", null)]
    public async Task<IActionResult> Delete([FromRoute] string rolename)
    {
      // check for existence of role
      var role = await _roleManager.FindByNameAsync(rolename);
      if (role == null)
        return NotFound(new { Message = "Role Not Found"});

      var result = await _roleManager.DeleteAsync(role);
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to delete role" });

      return NoContent();
    }
  }
}
