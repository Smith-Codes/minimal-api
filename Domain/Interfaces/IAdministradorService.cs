public interface IAdministradorService
{
    Administrador? Login(LoginDTO login);
    public List<Administrador> GetAdmins();
    Administrador AdministradorById(int id);
    public void Add(Administrador adm);
}