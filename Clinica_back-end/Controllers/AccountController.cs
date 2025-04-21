using Clinica_back_end.DTO.Auth;
using Clinica_back_end.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Clinica_back_end.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("/api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
            this._signInManager = signInManager;
            this._context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginAccountCredentialsDTO loginAccountCredentialsDTO)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(loginAccountCredentialsDTO.Email, loginAccountCredentialsDTO.Password, isPersistent: false, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginAccountCredentialsDTO.Email);
                return await BuildJwt(user!);
            }
            else
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "Login incorrecto"
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] CreateAccountCredentialsDTO accountCredentialsDTO)
        {
            var user = new ApplicationUser
            {
                UserName = accountCredentialsDTO.Email,
                Email = accountCredentialsDTO.Email,
                PhoneNumber = accountCredentialsDTO.PhoneNumber,
                FullName = accountCredentialsDTO.FullName,
            };

            var userCreated = await _userManager.CreateAsync(user, accountCredentialsDTO.Password);
            if (userCreated.Succeeded)
            {
                var userIndentity = await _userManager.FindByNameAsync(accountCredentialsDTO.Email);
                await AddDefaultRole(user, "User");

                var patient = new Paciente
                {
                    ApplicationUserId = user.Id
                };

                _context.Pacientes.Add(patient);
                await _context.SaveChangesAsync();

                return await BuildJwt(userIndentity!);
            }
            else
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No se pudo crear el usuario"
                });
            }
        }

        private async Task AddDefaultRole(ApplicationUser user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task<List<Claim>> AddValidClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
               new(ClaimTypes.Email, user.Email!),
               new ("fullname", user.FullName!),
               new ("phone", user.PhoneNumber!),
            };

            // get user claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // get the user roles, and added to the claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;

        }

        private async Task<AuthResponseDTO> BuildJwt(ApplicationUser user)
        {
            var claims = await AddValidClaims(user);

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSECRET"]!));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var expiresIn = DateTime.UtcNow.AddDays(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiresIn, signingCredentials: creds);

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiresIn,
            };
        }


    }
}
