using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUD_ASP.NET_CORE_WEBAPI.Models;
using CRUD_ASP.NET_CORE_WEBAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CRUD_ASP.NET_CORE_WEBAPI.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cors;

namespace CRUD_ASP.NET_CORE_WEBAPI.Controllers
{
    [EnableCors("CORSAPI")]
    public class AuthenticationController : BaseController
    {
        private readonly AppDbContext _dbcontext;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthenticationController(AppDbContext dbcontext, UserManager<IdentityUser> userManager)
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (model.Email != null)
            {
               var emailExist= _dbcontext.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                if (emailExist != null)
                {
                    return Ok(new Response { Status = Constants.ConstError, Message = Constants.ConstEmailAlreadyExist });
                }
                IdentityUser user = new IdentityUser
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.UserName
                };

                user.PasswordHash= _userManager.PasswordHasher.HashPassword(user, model.Password);
                var result= await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new ResponseForRegister { Status = Constants.ConstSuccess, Message = Constants.ConstRegistrationSuccess });
                }
                else
                {
                    return Ok(new Response { Status = Constants.ConstError, Message = Constants.ConstRegistrationFailed });
                }
                
            }
            return Ok(new Response { Status = Constants.ConstRegistrationFailed, Message = Constants.ConstInvalidInput });
        }

        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                try
                {
                    var user= _dbcontext.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                    if (user != null)
                    {
                        if(user!=null && await _userManager.CheckPasswordAsync(user, model.Password))
                        {
                            return Ok(new Response { Status = Constants.ConstSuccess, Message = Constants.ConstLoginSuccess });
                        }
                        return Ok(new Response { Status = Constants.ConstError, Message = Constants.ConstPasswordInvalid });
                    }
                    return Ok(new Response { Status = Constants.ConstError, Message = Constants.ConstEmailInvalid });
                }
                catch (Exception ex)
                {

                    return Ok(new Response { Status = Constants.ConstError, Message = ex.ToString() });
                }
            }
            return Ok(new Response { Status = Constants.ConstError, Message = Constants.ConstEmailIdEmpty });
        }
    }
}
