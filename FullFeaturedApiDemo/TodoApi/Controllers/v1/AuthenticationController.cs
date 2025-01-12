﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApi.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TodoApi.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0", Deprecated = true)]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtOptions _jwtOptions;

    public record AuthenticationData(string? UserName, string? Password, string? EmailAddress);

    public AuthenticationController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticationData auth)
    {
        var user = await ValidateCredentials(auth);

        if (user is null)
        {
            return Unauthorized();
        }

        var token = GenerateToken(user);
        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthenticationData auth)
    {
        var user = new IdentityUser
        {
            UserName = auth.UserName,
            Email = auth.EmailAddress,
        };

        var result = await _userManager.CreateAsync(user, auth.Password);
        if (result.Succeeded)
        {
            return NoContent();
        }
        else
        {
            var errorOut = string.Empty;
            foreach (var error in result.Errors)
            {
                errorOut += " " + error.Description;
            }
            return BadRequest(errorOut);
        }
    }

    private string GenerateToken(IdentityUser user)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
                _jwtOptions.SecretKey));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private async Task<IdentityUser?> ValidateCredentials(AuthenticationData auth)
    {
        IdentityUser user = await _userManager.FindByNameAsync(auth.UserName);
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, auth.Password, true);
        if (result.Succeeded)
        {
            return user;
        }

        return null;
    }
}
