public class RegistroPonto
{
    public int Id { get; set; }

    public int MatriculaFuncionario { get; set; }

    public Funcionario Funcionario { get; set; }

    public DateTime DataRegistro { get; set; }
    public TimeSpan HoraBatida { get; set; }
    public string TipoBatida { get; set; }
}