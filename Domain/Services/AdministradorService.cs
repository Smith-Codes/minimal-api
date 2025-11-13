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

    public List<Administrador> GetAdmins()
    {
        var query = _context.Administradores.AsQueryable();
        return query.ToList();

    }

    public void Add(Administrador adm)
    {
        _context.Administradores.Add(adm);
        _context.SaveChanges();
    }
    public Administrador AdministradorById(int id)
    {
        var adm = _context.Administradores.Where(x => x.Id == id).FirstOrDefault();
        return adm;
    }
}