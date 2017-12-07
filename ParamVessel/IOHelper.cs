using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowsBetterParamEditor
{
    public static class IOHelper
    {
        public static string RemoveExtension(string fileName, string extension)
        {
            var extIndex = fileName.ToUpper().LastIndexOf(extension.ToUpper());
            if (extIndex >= 0)
                return fileName.Substring(0, extIndex);
            else
                return fileName;
        }

        public static string AddExtension(string fileName, string extension, bool onlyAddIfNotAlreadyThere = true)
        {
            if (onlyAddIfNotAlreadyThere && fileName.ToUpper().EndsWith(extension.ToUpper()))
                return fileName;
            else
                return fileName + extension;
        }

        public static string Frankenpath(params string[] pathParts)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < pathParts.Length; i++)
            {
                sb.Append(pathParts[i].Trim('\\'));
                if (i < pathParts.Length - 1)
                    sb.Append('\\');
            }

            return sb.ToString();
        }
    }
}
