using System.Text.Json;

namespace StockQuoteAlert.serv;

public class CotacaoSrv
{
    private readonly HttpClient _http = new();

    public async Task<decimal?> ObterPrecoAsync(string ativo)
    {
        try
        {
            var url = $"https://brapi.dev/api/quote/{ativo}";
            var resp = await _http.GetStringAsync(url);

            using var doc = JsonDocument.Parse(resp);
            var preco = doc
                .RootElement
                .GetProperty("results")[0]
                .GetProperty("regularMarketPrice")
                .GetDecimal();

            return preco;
        }
        catch
        {
            return null;
        }
    }
}
