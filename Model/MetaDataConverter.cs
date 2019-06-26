using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public static class MetaDataConverter
    {
        public static string Convert(string MetaData)
        {
            int k = MetaData.Length;
            string result = string.Empty; 
            string a = "projections";
            int n = MetaData.IndexOf(a);
            result = MetaData.Remove(0, n);
            a = ",\"pages\"";
            n = result.IndexOf(a);
            result = result.Remove(n);

            for (int i =0; i < result.Length; i++)
            {
                if(result[i] == '{')
                {
                    n = result.IndexOf(result[i]);
                    result = result.Replace("{", "{\n");
                }
            }

            return result;
        }
    }
}
