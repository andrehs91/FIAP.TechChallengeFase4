namespace TechChallenge.Dominio.Entities;

public class ItemOrdenacao
{
    public Usuario Solucionador { get; set; } = null!;
    public int PrazoEstimadoTotal { get; set; }
    public int QuantidadeDeDemandas { get; set; }
}
