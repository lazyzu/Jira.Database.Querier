namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class mailserver
{
    public decimal ID { get; set; }

    public string NAME { get; set; }

    public string DESCRIPTION { get; set; }

    public string mailfrom { get; set; }

    public string PREFIX { get; set; }

    public string smtp_port { get; set; }

    public string protocol { get; set; }

    public string server_type { get; set; }

    public string SERVERNAME { get; set; }

    public string JNDILOCATION { get; set; }

    public string mailusername { get; set; }

    public string mailpassword { get; set; }

    public string ISTLSREQUIRED { get; set; }

    public decimal? TIMEOUT { get; set; }

    public string socks_port { get; set; }

    public string socks_host { get; set; }

    public string cipher_type { get; set; }

    public string auth_conf { get; set; }
}
