using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for EncryptDecrypt
/// </summary>
public static class EncryptDecrypt
{
    private static byte[] bytes;

    public static string Encrypt(string source)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] Key = { 12, 13, 14, 15, 16, 17, 18, 19 };
        byte[] IV = { 12, 13, 14, 15, 16, 17, 18, 19 };

        ICryptoTransform encryptor = des.CreateEncryptor(Key, IV);

        try
        {
            byte[] IDToBytes = ASCIIEncoding.ASCII.GetBytes(source);
            byte[] encryptedID = encryptor.TransformFinalBlock(IDToBytes, 0, IDToBytes.Length);
            return Convert.ToBase64String(encryptedID);
        }
        catch (FormatException)
        {
            return null;
        }
        catch (Exception)
        {
            throw;
        }
    }


    //public static string DecryptString(string inputString)
    //{
    //    MemoryStream memStream = null;
    //    try
    //    {
    //        byte[] key = { };
    //        byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
    //        string encryptKey = "aXb2uy4z"; // MUST be 8 characters
    //        key = Encoding.UTF8.GetBytes(encryptKey);
    //        byte[] byteInput = new byte[inputString.Length];
    //        byteInput = Convert.FromBase64String(inputString);
    //        DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
    //        memStream = new MemoryStream();
    //        ICryptoTransform transform = provider.CreateDecryptor(key, IV);
    //        CryptoStream cryptoStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);
    //        cryptoStream.Write(byteInput, 0, byteInput.Length);
    //        cryptoStream.FlushFinalBlock();
    //    }
    //    catch (Exception )
    //    {
    //        throw;
    //    }

    //    Encoding encoding1 = Encoding.UTF8;
    //    return encoding1.GetString(memStream.ToArray());
    //}

    //public static string DecryptTo(string cryptedString)
    //{
    //    if (String.IsNullOrEmpty(cryptedString))
    //    {
    //        throw new ArgumentNullException
    //           ("The string which needs to be decrypted can not be null.");
    //    }
    //    byte[] Key = { 12, 13, 14, 15, 16, 17, 18, 19 };
    //    byte[] IV = { 12, 13, 14, 15, 16, 17, 18, 19 };
    //    DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
    //    MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
    //    CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read);
    //    StreamReader reader = new StreamReader(cryptoStream);
    //    return reader.ReadToEnd();
    //}

    public static string Decrypt(string encrypted)
    {
        byte[] Key = { 12, 13, 14, 15, 16, 17, 18, 19 };
        byte[] IV = { 12, 13, 14, 15, 16, 17, 18, 19 };

        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        ICryptoTransform decryptor = des.CreateDecryptor(Key, IV);

        try
        {
            byte[] encryptedIDToBytes = Convert.FromBase64String(encrypted.Replace(" ", "+"));
            byte[] IDToBytes = decryptor.TransformFinalBlock(encryptedIDToBytes, 0, encryptedIDToBytes.Length);
            return ASCIIEncoding.ASCII.GetString(IDToBytes);
        }
        catch (FormatException)
        {
            return null;
        }
        catch (Exception)
        {
            throw;
        }


        

    }
}