using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Application.Tools.Other
{
    internal static class Hasher
    {
        #region ToByte Members

        internal static byte[] ToByteMD5(string text)
        {
            if (String.IsNullOrEmpty(text))
                return new byte[0];

            using (var hashingAlgorithm = System.Security.Cryptography.MD5.Create())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                return hashingAlgorithm.ComputeHash(textData);
            }
        }

        internal static byte[] ToByteSha1(string text)
        {
            if (String.IsNullOrEmpty(text))
                return new byte[0];

            using (var hashingAlgorithm = new System.Security.Cryptography.SHA1Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                return hashingAlgorithm.ComputeHash(textData);
            }
        }

        internal static byte[] ToByteSha256(string text)
        {
            if (String.IsNullOrEmpty(text))
                return new byte[0];

            using (var hashingAlgorithm = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                return hashingAlgorithm.ComputeHash(textData);
            }
        }

        internal static byte[] ToByteSha512(string text)
        {
            if (String.IsNullOrEmpty(text))
                return new byte[0];

            using (var hashingAlgorithm = new System.Security.Cryptography.SHA512Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                return hashingAlgorithm.ComputeHash(textData);
            }
        }

        #endregion // ToByte Members

        #region ToString Members

        internal static string ToStringMD5(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var hashingAlgorithm = System.Security.Cryptography.MD5.Create())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = hashingAlgorithm.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        internal static string ToStringSha1(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var hashingAlgorithm = new System.Security.Cryptography.SHA1Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = hashingAlgorithm.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        internal static string ToStringSha256(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var hashingAlgorithm = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = hashingAlgorithm.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        internal static string ToStringSha512(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var hashingAlgorithm = new System.Security.Cryptography.SHA512Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = hashingAlgorithm.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        #endregion // ToString Members
    }
}
