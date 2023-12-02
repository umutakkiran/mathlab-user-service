using Application.Abstraction.AbsToken;
using Application.ViewModel;
using Domain.Entities.user;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace UserManagement.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenHandler _tokenHandler;
        readonly IConfiguration _configuration;


        public UsersController(UserManager<ApplicationUser> userManager, ITokenHandler tokenHandler, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;


        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = _userManager.Users.ToList();
                var userViewModels = users.Select(user => new GetUsersViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    email = user.Email
                    // Diğer kullanıcı özelliklerini ekleyebilirsiniz
                }).ToList();

                return Ok(userViewModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(CreateUserModel model)
        {
            var user = new ApplicationUser {

                Id = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Email = model.email,
            };


            if (model.password == model.passwordConfirm)
            {
                IdentityResult result = await _userManager.CreateAsync(user, model.password);


                if (result.Succeeded)
                {
                    // Kullanıcıyı "ordinaryUser" role atama
                    await _userManager.AddToRoleAsync(user, "ordinaryUser");
                    return Ok("Başarılı");
                }
                else
                    return Ok(result.Errors.First().Description);

            }
            else
            {
                return BadRequest("Şifreler eşleşmiyor");
            }





        }

        [HttpPost("[action]")]
        public async Task<IActionResult> createAdmin(CreateUserModel model)
        {

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Email = model.email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.password);



            if (result.Succeeded)
            {
                // Kullanıcıyı "admin" role atama
                await _userManager.AddToRoleAsync(user, "Admin");
                return Ok("Başarılı");
            }
            else
                return Ok(result.Errors.First().Description);

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> createRole( CreateRoleModels model)
        {
            IdentityResult result = await _roleManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.name

            }) ;

            if (result.Succeeded)
                return Ok("Başarılı");
            else
                return Ok(result.Errors.First().Description);

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> login(Application.ViewModel.LoginUser model)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(model.userNameorEmail);

            if (user == null)
                user = await _userManager.FindByEmailAsync(model.userNameorEmail);
           
            if(user == null)
                return BadRequest("Kullanıcı Adı veya Email hatalı..");

             bool result = await _userManager.CheckPasswordAsync(user, model.password);

            // burada giriş yapan kullanıcıdan gelen bilgiler ile hangi rolde olduğunu bulup ona göre token üretmem gerek
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var role = roles.FirstOrDefault();



            Application.DTOs.Token token = new();

            if (role == "Admin")
            {
                token = _tokenHandler.createAccessToken(45, _configuration["Token:SecurityKey"], _configuration["Token:Audience"], _configuration["Token:Issuer"], user);
            }
            else
            {
                token = _tokenHandler.createAccessToken(45, _configuration["userToken:SecurityKey"], _configuration["userToken:Audience"], _configuration["userToken:Issuer"], user);
            }

            if (result)
                return Ok(token);
            else
                return BadRequest("Şifre Hatalı");

        }
    }
}
