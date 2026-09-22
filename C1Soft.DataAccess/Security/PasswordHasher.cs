using System.Security.Cryptography;
using System.Text;

namespace C1Soft.DataAccess.Security;

/// <summary>
/// OWASP standartlarına uygun PBKDF2 (HMAC-SHA256) tabanlı tuzlanmış (salted) şifreleme motoru.
/// Şifreler kesinlikle düz metin (plain text) saklanmaz; her kullanıcı için rastgele 32 byte tuz (salt) üretilir.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 32; // 256 bit
    private const int HashSize = 32; // 256 bit
    private const int Iterations = 100_000; // Brute-force saldırılarına karşı yüksek iterasyon
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Şifre boş olamaz.", nameof(password));

        // 1. Rastgele kriptografik tuz (Salt) üret
        byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);

        // 2. PBKDF2 ile şifreyi tuzlayarak hash'le
        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            saltBytes,
            Iterations,
            Algorithm,
            HashSize);

        // 3. Veritabanında saklanmak üzere Base64 string'e dönüştür
        passwordSalt = Convert.ToBase64String(saltBytes);
        passwordHash = Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(storedSalt))
            return false;

        try
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] expectedHashBytes = Convert.FromBase64String(storedHash);

            // Veritabanındaki tuz ile girilen şifreyi aynı algoritmayla tekrar hash'le
            byte[] actualHashBytes = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                Iterations,
                Algorithm,
                HashSize);

            // Zamanlama saldırılarını (Timing Attacks) önlemek için CryptographicOperations.FixedTimeEquals kullan
            return CryptographicOperations.FixedTimeEquals(actualHashBytes, expectedHashBytes);
        }
        catch
        {
            return false;
        }
    }
}
