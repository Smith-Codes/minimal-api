using MinimalDio.Context;

public class AdministradorService : IAdministradorService
{
    private readonly ApiDbContext _context;
    public AdministradorService(ApiDbContext context)
    {
        _context = context;
    }

    public Administrador? Login(LoginDTO login)
    {
        var acesso = _context.Administradores.Where(x => x.Email == login.Email && x.Senha == login.Senha).FirstOrDefault();
        return acesso;
    }
}