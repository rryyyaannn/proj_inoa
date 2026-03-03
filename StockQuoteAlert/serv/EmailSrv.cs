using System.Net;
using System.Net.Mail;
using StockQuoteAlert.model;

namespace StockQuoteAlert.serv;

public class EmailSrv
{
    private readonly AppConfig _cfg;

    public EmailSrv(AppConfig cfg)
    {
        _cfg = cfg;
    }

    public async Task EnviarAsync(string assunto, string corpo)
    {
        using var smtp = new SmtpClient(_cfg.Smtp.Host, _cfg.Smtp.Porta)
        {
            Credentials = new NetworkCredential(
                _cfg.Smtp.Usuario,
                _cfg.Smtp.Senha),
            EnableSsl = true
        };

        var mail = new MailMessage(
            _cfg.Smtp.Usuario,
            _cfg.EmailDestino,
            assunto,
            corpo);

        await smtp.SendMailAsync(mail);
    }
}
