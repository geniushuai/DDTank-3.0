using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;

namespace Road.Test
{
    struct Student
    {
        
        public int Id;
        
        public string Name;
        
    }

    /// <summary>
    /// Summary description for TestMarshal
    /// </summary>
    [TestClass]
    public class TestMarshal
    {

        [TestMethod]
        public void TestMarshWrite()
        {
            Student st = new Student();
            st.Id = 16;
            st.Name = "How are you ?";

            int size = Marshal.SizeOf(st);

            Console.WriteLine(size);

            IntPtr pt = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(st, pt, true);
            byte[] temp = new byte[size];
            Marshal.Copy(pt, temp, 0, temp.Length);

            Console.Write(Game.Base.Marshal.ToHexDump("Temp:", temp));

        }

        [TestMethod]
        public void TestMarshRead()
        {
            byte[] temp = Encoding.UTF8.GetBytes("How are you?");
            byte[] num = new byte[] { 0, 1, 1, 0 };

            byte[] all = new byte[temp.Length+4];
            Array.Copy(temp,all,temp.Length);
            Array.Copy(num,0,all,temp.Length,num.Length);

            IntPtr pt = Marshal.AllocHGlobal(all.Length);
            Marshal.Copy(all, 0, pt, all.Length);

            Student st =  (Student)Marshal.PtrToStructure(pt, typeof(Student));

            Console.Write(string.Format("Name:{0},Id:{1}", st.Name, st.Id));

        }
    }
}
