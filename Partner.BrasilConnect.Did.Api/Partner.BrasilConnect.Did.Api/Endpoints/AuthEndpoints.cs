using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Partner.BrasilConnect.Did.Api.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Partner.BrasilConnect.Did.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", (LoginRequestDto loginDto, IConfiguration config) =>
        {
            if (loginDto.Username != "admin" || loginDto.Password != "senha123")
            {
                return Results.Unauthorized();
            }

            var jwtSettings = config.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, loginDto.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("did_scope", "activation_write")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Results.Ok(new LoginResponseDto(tokenHandler.WriteToken(token)));
        })
        .WithName("Login")
        .AllowAnonymous()
        .WithOpenApi(operation =>
        {
            operation.Summary = "Gera um token JWT para autenticação";
            operation.Description = "Use as credenciais padrão: username: 'admin', password: 'senha123'. " +
                                   "Copie o token retornado e use no botão 'Authorize' acima.";

            operation.Responses["200"].Description = "Sucesso. Retorna o token JWT.";
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Credenciais inválidas."
            });
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Requisição malformada."
            });

            return operation;
        });
    }
}