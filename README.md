# StockQuoteAlert

Console simples para monitorar preço de ativo via `brapi.dev` e mandar alerta por email quando bater nos gatilhos de compra ou venda.

## Estrutura

`Program.cs`
  `serv/`
    `CotacaoSrv.cs`
    `EmailSrv.cs`
  `model`
    `AppConfig.cs`
  `config.json`

## Configuração (config.json)

Edite `config.json` na raiz do projeto:

```json
{
  "emailDestino": "seuemail@gmail.com",
  "smtp": {
    "host": "smtp.gmail.com",
    "porta": 587,
    "usuario": "seuemail@gmail.com",
    "senha": "SUA_SENHA_APP"
  },
  "intervaloSegundos": 30
}
```

OBS!: no Gmail, use **senha de app** (não a senha normal).

## Como executar:

Exemplo:

```bash
dotnet run -- PETR4 22.67 22.59
```

Parâmetros:

- `ATIVO`: ex.: `PETR4`
- `PRECO_VENDA`: preço para disparar alerta de venda
- `PRECO_COMPRA`: preço para disparar alerta de compra

## API de cotação

Endpoint usado:

- `https://brapi.dev/api/quote/PETR4`
