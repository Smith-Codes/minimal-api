
using MinimalDio.Context;

public class VeiculosService : IVeiculoService
{
    private readonly ApiDbContext _context;

    public VeiculosService(ApiDbContext context)
    {
        _context = context;
    }

    public void Add(Veiculo veiculo)
    {
       _context.Veiculos.Add(veiculo);
       _context.SaveChanges();
    }

    public void Delete(Veiculo veiculo)
    {
        _context.Veiculos.Remove(veiculo);
        _context.SaveChanges();
    }

    public List<Veiculo> GetVeiculos(int? pag = 1, string? nome = null, string? marca = null)
    {
        var query = _context.Veiculos.AsQueryable();

        if(!string.IsNullOrEmpty(nome))
        {
            query = query.Where(x=> x.Nome == nome);
        }
        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(x => x.Marca == marca);
        }

        int itensPerPage = 10;

        if(pag != null)
        {
            query = query.Skip((int)((pag - 1) * itensPerPage)).Take(itensPerPage);
        }
        

        return query.ToList();

    }

    public void Update(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo);
        _context.SaveChanges();

    }

    public Veiculo VeiculoById(int id)
    {
        var veiculo = _context.Veiculos.Where(x => x.Id == id).FirstOrDefault();
        return veiculo;
    }
}