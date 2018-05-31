using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PvPNetServer
{
    public class Encryption
    {
        public byte[] EncryptSHA(string data, string saltSeed)
        {
            //
            // Create salt using byte length of provided salt seed + 32
            //
            byte[] salt = new byte[Encoding.Unicode.GetBytes(saltSeed).Length + 32];
            RandomNumberGenerator.Create().GetBytes(salt);
            
            // Convert the plain string pwd into bytes
            byte[] plainTextBytes = Encoding.Unicode.GetBytes(data);
            
            // Append salt to pwd before hashing
            byte[] combinedBytes = new byte[plainTextBytes.Length + salt.Length];
            System.Buffer.BlockCopy(plainTextBytes, 0, combinedBytes, 0, plainTextBytes.Length);
            System.Buffer.BlockCopy(salt, 0, combinedBytes, plainTextBytes.Length, salt.Length);
            
            // Create hash for the pwd+salt
            SHA256Managed hashAlgo = new SHA256Managed();
            byte[] hash = hashAlgo.ComputeHash(combinedBytes);
            
            // Append the salt to the hash
            byte[] hashPlusSalt = new byte[hash.Length + salt.Length];
            System.Buffer.BlockCopy(hash, 0, hashPlusSalt, 0, hash.Length);
            System.Buffer.BlockCopy(salt, 0, hashPlusSalt, hash.Length, salt.Length);

            return hashPlusSalt;
        }

        public bool CheckData(string dataToCheck, byte[] hashResult, string saltSeed)
        {
            //
            // Get salt length.
            //
            int saltLength = Encoding.Unicode.GetBytes(saltSeed).Length + 32;
            
            byte[] hashSalt = hashResult.ToList().GetRange(hashResult.Length - saltLength, saltLength).ToArray();
            
            // Convert the plain string pwd into bytes
            byte[] plainTextBytes = Encoding.Unicode.GetBytes(dataToCheck);
            
            // Append salt to pwd before hashing
            byte[] combinedBytes = new byte[plainTextBytes.Length + hashSalt.Length];
            System.Buffer.BlockCopy(plainTextBytes, 0, combinedBytes, 0, plainTextBytes.Length);
            System.Buffer.BlockCopy(hashSalt, 0, combinedBytes, plainTextBytes.Length, hashSalt.Length);
            
            // Create hash for the pwd+salt
            SHA256Managed hashAlgo = new SHA256Managed();
            byte[] hash = hashAlgo.ComputeHash(combinedBytes);
            
            // Append the salt to the hash
            byte[] hashPlusSalt = new byte[hash.Length + hashSalt.Length];
            System.Buffer.BlockCopy(hash, 0, hashPlusSalt, 0, hash.Length);
            System.Buffer.BlockCopy(hashSalt, 0, hashPlusSalt, hash.Length, hashSalt.Length);
            
            var result = StructuralComparisons.StructuralEqualityComparer.Equals(hashPlusSalt, hashResult);

            return result;
        }
    }
}