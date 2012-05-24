using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Collections;
using System.IO;
using Game.Base;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Game.Base.Config;

namespace Game.Server.Managers
{
    public class ScriptMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<string,Assembly> m_scripts = new Dictionary<string,Assembly>();

        public static Assembly[] Scripts
        {
            get 
            {
                lock (m_scripts)
                {
                    return m_scripts.Values.ToArray();
                }
            }
        }

        public static bool InsertAssembly(Assembly ass)
        {
            lock (m_scripts)
            {
                if (!m_scripts.ContainsKey(ass.FullName))
                {
                    m_scripts.Add(ass.FullName, ass);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool RemoveAssembly(Assembly ass)
        {
            lock (m_scripts)
            {
                return m_scripts.Remove(ass.FullName);
            }
        }

        #region Compile Scripts

        public static bool CompileScripts(bool compileVB, string path, string dllName, string[] asm_names)
        {
            if (!path.EndsWith(@"\") && !path.EndsWith(@"/"))
                path = path + "/";

            ArrayList files = ParseDirectory(new DirectoryInfo(path), compileVB ? "*.vb" : "*.cs", true);
            if (files.Count == 0)
            {
                return true;
            }

            if (File.Exists(dllName))
                File.Delete(dllName);

            CompilerResults res = null;
            try
            {
                CodeDomProvider compiler=null;
                if(compileVB)
                {
                    compiler = new VBCodeProvider(); 
                }
                else
                {
                    compiler = new CSharpCodeProvider();
                }
                CompilerParameters param = new CompilerParameters(asm_names, dllName, true);
                param.GenerateExecutable = false;
                param.GenerateInMemory = false;
                param.WarningLevel = 2;
                param.CompilerOptions = @"/lib:.";

                string[] filepaths = new string[files.Count];
                for (int i = 0; i < files.Count; i++)
                    filepaths[i] = ((FileInfo)files[i]).FullName;
                res = compiler.CompileAssemblyFromFile(param, filepaths);
                GC.Collect();

                if (res.Errors.HasErrors)
                {
                    foreach (CompilerError err in res.Errors)
                    {
                        if (err.IsWarning) continue;

                        StringBuilder builder = new StringBuilder();
                        builder.Append("   ");
                        builder.Append(err.FileName);
                        builder.Append(" Line:");
                        builder.Append(err.Line);
                        builder.Append(" Col:");
                        builder.Append(err.Column);
                        if (log.IsErrorEnabled)
                        {
                            log.Error("Script compilation failed because: ");
                            log.Error(err.ErrorText);
                            log.Error(builder.ToString());
                        }
                    }

                    return false;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CompileScripts", e);
            }

            if (res != null && res.Errors.HasErrors == false)
            {
                InsertAssembly(res.CompiledAssembly);
            }

            return true;
        }

        private static ArrayList ParseDirectory(DirectoryInfo path, string filter, bool deep)
        {
            ArrayList files = new ArrayList();

            if (!path.Exists)
                return files;

            files.AddRange(path.GetFiles(filter));

            if (deep)
            {
                foreach (DirectoryInfo subdir in path.GetDirectories())
                    files.AddRange(ParseDirectory(subdir, filter, deep));
            }

            return files;
        }

        #endregion

        #region Type Utils
        /// <summary>
        /// Search for a type by name; first in GameServer assembly then in scripts assemblies
        /// </summary>
        /// <param name="name">The type name</param>
        /// <returns>Found type or null</returns>
        public static Type GetType(string name)
        {
            foreach (Assembly asm in Scripts)
            {
                Type t = asm.GetType(name);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        public static object CreateInstance(string name)
        {
            foreach (Assembly asm in Scripts)
            {
                Type t = asm.GetType(name);
                if (t != null && t.IsClass)
                {
                    return Activator.CreateInstance(t);
                }
            }
            return null;
        }

        public static object CreateInstance(string name, Type baseType)
        {
            foreach (Assembly asm in Scripts)
            {
                Type t = asm.GetType(name);
                if (t != null && t.IsClass && baseType.IsAssignableFrom(t))
                {
                    return Activator.CreateInstance(t);
                }
            }
            return null;
 
        }

        /// <summary>
        /// Finds all classes that derive from given type.
        /// First check scripts then GameServer assembly.
        /// </summary>
        /// <param name="baseType">The base class type.</param>
        /// <returns>Array of types or empty array</returns>
        public static Type[] GetDerivedClasses(Type baseType)
        {
            if (baseType == null)
                return new Type[0];

            ArrayList types = new ArrayList();
            ArrayList asms = new ArrayList(Scripts);

            foreach (Assembly asm in asms)
                foreach (Type t in asm.GetTypes())
                {
                    if (t.IsClass && baseType.IsAssignableFrom(t))
                        types.Add(t);
                }

            return (Type[])types.ToArray(typeof(Type));
        }

        /// <summary>
        /// Finds all classes the implements the given interface.
        /// </summary>
        /// <param name="baseInterface"></param>
        /// <returns></returns>
        public static Type[] GetImplementedClasses(string baseInterface)
        {
            ArrayList types = new ArrayList();
            ArrayList asms = new ArrayList(Scripts);
            foreach (Assembly asm in asms)
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (t.IsClass && t.GetInterface(baseInterface) != null)
                    {
                        types.Add(t);
                    }
                }
            }
            return (Type[])types.ToArray(typeof(Type));
        }
        #endregion
    }
}
