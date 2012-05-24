using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Game.Base
{
    /// <summary>
    /// Helps managing embedded resources
    /// </summary>
    public class ResourceUtil
    {
        /// <summary>
        /// Searches for a specific resource and returns the stream
        /// </summary>
        /// <param name="fileName">the resource name</param>
        /// <returns>the resource stream</returns>
        public static Stream GetResourceStream(string fileName,Assembly assem)
        {
            fileName = fileName.ToLower();
            foreach (string name in assem.GetManifestResourceNames())
            {
                if (name.ToLower().EndsWith(fileName))
                    return assem.GetManifestResourceStream(name);
            }
            return null;
        }

        /// <summary>
        /// Extracts a given resource
        /// </summary>
        /// <param name="fileName">the resource name</param>
        public static void ExtractResource(string fileName,Assembly assembly)
        {
            ExtractResource(fileName, fileName, assembly);
        }

        /// <summary>
        /// Extracts a given resource
        /// </summary>
        /// <param name="resourceName">the resource name</param>
        /// <param name="fileName">the external file name</param>
        public static void ExtractResource(string resourceName, string fileName,Assembly assembly)
        {
            FileInfo finfo = new FileInfo(fileName);
            if (!finfo.Directory.Exists)
                finfo.Directory.Create();

            using (StreamReader reader = new StreamReader(GetResourceStream(resourceName, assembly)))
            {
                using (StreamWriter writer = new StreamWriter(File.Create(fileName)))
                {
                    writer.Write(reader.ReadToEnd());
                }
            }
        }
    }
}
