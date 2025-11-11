public interface IVeiculoService
{
    List<Veiculo> GetVeiculos(int? pag = 1, string? nome = null, string? marca = null);
    Veiculo VeiculoById(int id);
    void Add(Veiculo veiculo);
    void Update (Veiculo veiculo);
    void Delete(Veiculo veiculo);
}