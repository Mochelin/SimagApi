using System.Security.Cryptography;
using System.Text;

namespace LandingData.Utilidad
{


    public static class PasswordHelper
    {
        // Método para generar un hash de contraseña
        public static byte[] CrearPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "La contraseña no puede estar vacía.");

            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        // Método para validar una contraseña contra un hash
        public static bool ValidarPassword(string passwordIngresada, byte[] hashAlmacenado)
        {
            if (hashAlmacenado == null || hashAlmacenado.Length == 0)
                throw new ArgumentException("El hash almacenado no puede estar vacío.", nameof(hashAlmacenado));

            using var sha256 = SHA256.Create();
            var hashIngresado = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordIngresada));
            return hashIngresado.SequenceEqual(hashAlmacenado);
        }
    }
}
