namespace Larry.Source.Interfaces
{
    public interface ITokenPayload
    {
        string App { get; set; }
        string Sub { get; set; }
        int Dvid { get; set; }
        bool Mver { get; set; }
        string Clid { get; set; }
        string Dn { get; set; }
        string Am { get; set; }
        string P { get; set; }
        string Iai { get; set; }
        int Sec { get; set; }
        string Clsvc { get; set; }
        string T { get; set; }
        bool Ic { get; set; }
        string Jti { get; set; }
        string CreationDate { get; set; }
        int ExpiresIn { get; set; }
    }
}
