using System.IO;

namespace ZendeskApi_v2.Test.Util
{
    public static class ResourceUtil
    {
        public static byte[] GetResource(string resourceName)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (var resFilestream = a.GetManifestResourceStream(resourceName))
            {
                if (resFilestream == null)
                {
                    throw new FileNotFoundException(resourceName);
                }

                var ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }
}