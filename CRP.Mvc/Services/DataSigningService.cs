using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace CRP.Mvc.Services
{
    public interface IDataSigningService
    {
        string Sign(IDictionary<string, string> paramsArray);
        bool Check(IDictionary<string, string> paramsArray, string signature);
    }

    public class DataSigningService : IDataSigningService
    {
        private readonly string _secretKey;

        public DataSigningService()
        {
            // yeet. i got you fam. okuuuuur!
            // so now we can do pair programming remotely
            // i think you have write permission?
            // yup i can write anything
            
            _secretKey = WebConfigurationManager.AppSettings["CyberSource.SecretKey"];
        }

        public string Sign(IDictionary<string, string> paramsArray)
        {
            return SignData(BuildDataToSign(paramsArray), _secretKey);
        }

        public bool Check(IDictionary<string, string> paramsArray, string signature)
        {
            try
            {
                return signature == SignData(BuildDataToSign(paramsArray), _secretKey);
            }
            catch (Exception ex)
            {
                //Log.Error(ex, ex.Message);
                return false;
            }
        }

        private static string SignData(string data, string secretKey)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);

            var hmacsha256 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(data);
            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
        }

        private static string BuildDataToSign(IDictionary<string, string> paramsArray)
        {
            var signedFieldNames = paramsArray["signed_field_names"].Split(',');
            var dataToSign = signedFieldNames.Select(signedFieldName => signedFieldName + "=" + paramsArray[signedFieldName]).ToList();

            return CommaSeparate(dataToSign);
        }

        private static string CommaSeparate(IEnumerable<string> dataToSign)
        {
            return string.Join(",", dataToSign);
        }
    }
}