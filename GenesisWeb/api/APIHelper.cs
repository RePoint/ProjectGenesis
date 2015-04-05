using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisWeb.api
{
    class APIHelper
    {
        public static string WriteErrorInfo(Exception ex)
        {
            var sb = new StringBuilder();

            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);

            while (ex.InnerException != null)
            {
                sb.AppendLine(ex.InnerException.Message);
                sb.AppendLine(ex.InnerException.StackTrace);

                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }


}
