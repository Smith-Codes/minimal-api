using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalDio.Context;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration["Jwt:Key"];

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token : {token aqui}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddDbContext<ApiDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MINIMAL_TESTE")));

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<IVeiculoService, VeiculosService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

# region ADM

string GerarTokenJwt(Administrador adm)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", adm.Email)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials : credentials
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}



app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorService admService) =>
{
    var adm = admService.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(token);
    }
    else
        return Results.Unauthorized();
}).WithTags("Administrador");

app.MapGet("/administradores", (IAdministradorService admService) =>
{
    var adms = admService.GetAdmins();

    return Results.Ok(adms);
}).RequireAuthorization().WithTags("Administrador");


app.MapPost("/administradores", ([FromBody] AdministradorDTO dto, IAdministradorService admService) =>
{
    var adm = new Administrador
    {
        Email = dto.Email,
        Senha = dto.Senha,
    };

    admService.Add(adm);

    return Results.Created($"/veiculo/{adm.Id}", adm);
}).RequireAuthorization().WithTags("Administrador");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorService admService) =>
{
    var adm = admService.AdministradorById(id);

    return Results.Ok(adm);
}).RequireAuthorization().WithTags("Administrador");
# endregion

#region Veículos

ValidationErrors ValidaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ValidationErrors
    {
        Mensages = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensages.Add("O nome não pode ser vazio");
    if (veiculoDTO.Ano < 1950)
        validacao.Mensages.Add("O ano não pode ser´inferior a 1950");
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensages.Add("A marca não pode ser vazia");

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var validacao = ValidaDTO(veiculoDTO);

    if(validacao.Mensages.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
    };

    veiculoService.Add(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}",veiculo);
}).RequireAuthorization().WithTags("Veículos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.GetVeiculos(pagina);

    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veículos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.VeiculoById(id);

    return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veículos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.VeiculoById(id);
    if (veiculo == null) return Results.NotFound();

    var validacao = ValidaDTO(veiculoDTO);

    if (validacao.Mensages.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoService.Update(veiculo);

    return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veículos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.VeiculoById(id);
    if (veiculo == null) return Results.NotFound();

    veiculoService.Delete(veiculo);

    return Results.NoContent();
}).RequireAuthorization().WithTags("Veículos");
#endregion


app.Run();