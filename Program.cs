using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalDio.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MINIMAL_TESTE")));

builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<IVeiculoService, VeiculosService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorService admService) =>
{
    if (admService.Login(loginDTO) != null)
        return Results.Ok("Login com Sucesso");
    else
        return Results.Unauthorized();
}).WithTags("Administrador");

#region Ve�culos

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
    };

    veiculoService.Add(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}",veiculo);
}).WithTags("Ve�culos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.GetVeiculos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Ve�culos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.VeiculoById(id);

    return Results.Ok(veiculo);
}).WithTags("Ve�culos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.VeiculoById(id);
    if (veiculo == null) return Results.NotFound();

    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoService.Update(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Ve�culos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.VeiculoById(id);
    if (veiculo == null) return Results.NotFound();

    veiculoService.Delete(veiculo);

    return Results.NoContent();
}).WithTags("Ve�culos");
#endregion


app.Run();