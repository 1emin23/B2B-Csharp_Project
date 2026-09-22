namespace C1Soft.DataAccess.Security;

/// <summary>
/// Güvenli şifre hashleme ve doğrulama sözleşmesi.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Düz metin şifreden kriptografik tuz (Salt) ve hash üretir.
    /// </summary>
    void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt);

    /// <summary>
    /// Girilen düz metin şifrenin, veritabanında saklanan tuz ve hash ile eşleşip eşleşmediğini doğrular.
    /// </summary>
    bool VerifyPasswordHash(string password, string storedHash, string storedSalt);
}
