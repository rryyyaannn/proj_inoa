using System.Globalization;
using System.Text.Json;
using StockQuoteAlert.model;
using StockQuoteAlert.serv;

if (args.Length != 3)
{
    Console.WriteLine("Uso: stock-quote-alert.exe ATIVO PRECO_VENDA PRECO_COMPRA");
    return;
}

var ativo = args[0];
var precoVenda = decimal.Parse(args[1], CultureInfo.InvariantCulture);
var precoCompra = decimal.Parse(args[2], CultureInfo.InvariantCulture);

Console.WriteLine($"Monitorando {ativo}...");

// carrega config
var cfgTxt = await File.ReadAllTextAsync("config.json");
var cfg = JsonSerializer.Deserialize<AppConfig>(cfgTxt, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
})!;

var cotSrv = new CotacaoSrv();
var emailSrv = new EmailSrv(cfg);

bool avisouVenda = false;
bool avisouCompra = false;

while (true)
{
    var precoAtual = await cotSrv.ObterPrecoAsync(ativo);

    if (precoAtual == null)
    {
        Console.WriteLine("Falha ao obter cotação.");
        await Task.Delay(5000);
        continue;
    }

    Console.WriteLine($"{DateTime.Now:HH:mm:ss} -> {precoAtual}");

    // venda
    if (precoAtual >= precoVenda && !avisouVenda)
    {
        await emailSrv.EnviarAsync(
            $"Alerta de VENDA - {ativo}",
            $"Preço atual: {precoAtual}");

        avisouVenda = true;
        avisouCompra = false;
    }

    // compra
    if (precoAtual <= precoCompra && !avisouCompra)
    {
        await emailSrv.EnviarAsync(
            $"Alerta de COMPRA - {ativo}",
            $"Preço atual: {precoAtual}");

        avisouCompra = true;
        avisouVenda = false;
    }

    await Task.Delay(cfg.IntervaloSegundos * 1000);
}
