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

Console.WriteLine($"monitorando {ativo}...");

// preco de compra deve ser menor que preco de venda
if (precoCompra >= precoVenda)
{
    Console.WriteLine("aviso: preço de compra >= preço de venda");
    var tmp = precoVenda;
    precoVenda = precoCompra;
    precoCompra = tmp;
    Console.WriteLine($"novo preço venda: {precoVenda} | novo preço compra: {precoCompra}");
}


if (!File.Exists("config.json"))
{
    Console.WriteLine("arquivo config.json nao encontrado.");
    return; 
}

var cfgTxt = await File.ReadAllTextAsync("config.json");
var cfg = JsonSerializer.Deserialize<AppConfig>(cfgTxt, new JsonSerializerOptions 
{
    PropertyNameCaseInsensitive = true
}); 

if (cfg == null)
{
    Console.WriteLine("falha ao carregar config.json"); 
    return;
}
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
    else if (precoAtual <= precoCompra && !avisouCompra)
    {
        await emailSrv.EnviarAsync(
            $"Alerta de COMPRA - {ativo}",
            $"Preço atual: {precoAtual}");

        avisouCompra = true;
        avisouVenda = false;
    }
    
    Console.WriteLine($"status -> venda:{avisouVenda} compra:{avisouCompra}");

    var delayMs = Math.Max(1, cfg.IntervaloSegundos) * 1000;
    await Task.Delay(delayMs);
}
