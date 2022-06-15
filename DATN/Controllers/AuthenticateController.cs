using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DATN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;


        public AuthenticateController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            
        }

        [HttpGet]
        [Route("getAccount")]

        public async Task<IActionResult> GetAllAccount()
        {
           
            var result=(from a in _context.AppUsers
                        select new
                        {
                            Id=a.Id,
                            Avatar=a.Avatar,
                            Username=a.UserName,
                            Email=a.Email,
                            Phone=a.PhoneNumber,
                            Address=a.ShippingAddress,
                            AccoutType=a.AccoutType,
                            IsLocked=a.IsLocked,
                        }).ToList();
            return Ok(result);
        }

        [HttpGet]
        [Route("getAccountById")]

        public async Task<IActionResult> GetAccount(string id)
        {

            var result = (from a in _context.AppUsers
                          where a.Id== id
                          select new
                          {
                              Id = a.Id,
                              FullName=a.FullName,
                              Avatar = a.Avatar,
                              Username = a.UserName,
                              Email = a.Email,
                              Phone = a.PhoneNumber,
                              Address = a.ShippingAddress,
                              AccoutType = a.AccoutType,
                              IsLocked = a.IsLocked,
                              Password=a.PasswordHash
                          }).FirstOrDefault();
            return Ok(result);
        }

        [HttpPost]
        [Route("lockAccount")]

        public async Task<IActionResult> LockAccount(string id)
        {

            var acc=await _context.AppUsers.FindAsync(id);

            if(acc!=null)
            {
                if(acc.IsLocked==true)
                {
                    acc.IsLocked = false;
                    _context.AppUsers.Update(acc);
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        status = 200,
                        msg = "Đã mở khoá tài khoản"
                    });
                }
                acc.IsLocked=true;
                _context.AppUsers.Update(acc);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status=200,
                    msg="Đã khoá tài khoản"
                });
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("editAcount")]

        public async Task<IActionResult> EditAccount(EditAccountModel model)
        {
            var user= await  userManager.FindByIdAsync(model.Id);
            if(user!=null)
            {
                var result= await userManager.ChangePasswordAsync(user, user.PasswordHash,model.Password);
                user.Email=model.Email;
                user.PhoneNumber = model.Phone;
                user.ShippingAddress = model.Address;
                _context.Update(user);
                _context.SaveChanges();
                if(result.Succeeded)
                {
                    return Ok(new
                    {
                        status = 200,
                        msg = "Đã cập nhật thông tin tài khoản"
                    }) ;
                }
            
            }
            return Ok(new
            {
                status = 500,
                msg = "Cập nhật tài khoản thất bại"
            });


        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
          
            var user = await userManager.FindByNameAsync(model.Username);
          
            
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password)&&user.IsLocked==false)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             
                };

                if (userRoles.Count >0)
                {
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
               
                }else
                {
                 
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    id=user.Id,
                    address=user.ShippingAddress,
                    phone=user.PhoneNumber,
                    role=user.AccoutType,
                    username=user.UserName,




                });
            }
            return Unauthorized(new
            {
                status=400
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            AppUser user = new AppUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                PhoneNumber=model.Phone,
                Avatar=model.Avatar,
                AccoutType="User",
                FullName=model.FullName,
                ShippingAddress=model.ShippingAddress,
                IsLocked=false
               
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            AppUser user = new AppUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                AccoutType ="Admin",
                FullName = model.FullName,
                ShippingAddress = model.ShippingAddress,
                IsLocked =false
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
    }
}
