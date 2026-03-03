namespace StockQuoteAlert.model;

public class AppConfig
{
    public string EmailDestino { get; set; } = "";
    public SmtpCfg Smtp { get; set; } = new();
    public int IntervaloSegundos { get; set; } = 30;
}

public class SmtpCfg
{
    public string Host { get; set; } = "";
    public int Porta { get; set; }
    public string Usuario { get; set; } = "";
    public string Senha { get; set; } = "";
}
