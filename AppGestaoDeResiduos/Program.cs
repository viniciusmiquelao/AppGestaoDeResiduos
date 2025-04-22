using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AppGestaoDeResiduos.Data;
using AppGestaoDeResiduos.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conectando o app com o Banco de Dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar TestService
builder.Services.AddScoped<TestService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64; // Ajuste conforme necessário
    });

// Configurar autenticação JWT
var secretKey = "your_very_long_secret_key_32_chars_minimum"; // Deve ter pelo menos 32 caracteres
var issuer = "useradmin";
var audience = "adminaudience";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Adicionar middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Adicionar dados de teste e exibir token antes de iniciar a aplicação
using (var scope = app.Services.CreateScope())
{
    var testService = scope.ServiceProvider.GetRequiredService<TestService>();
    await testService.AddTestDataAsync();

    // Gerar e exibir o token JWT de teste
    var tokenGenerator = new TokenGenerator(secretKey, issuer, audience);
    var testToken = tokenGenerator.GenerateTestToken();
    Console.WriteLine("Test JWT Token: " + testToken);
}

app.Run();

public class TestService
{
    private readonly ApplicationDbContext _context;

    public TestService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddTestDataAsync()
    {
        var testUser = new Usuario
        {
            Nome = "Danilo",
            Email = "danilo@exemplo.com",
            AgendouColeta = false,
            FoiNotificado = false,
            EnderecoId = 1
        };

        _context.Usuarios.Add(testUser);
        await _context.SaveChangesAsync();
    }
}

public class TokenGenerator
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenGenerator(string secretKey, string issuer, string audience)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateTestToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Danilo"),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
