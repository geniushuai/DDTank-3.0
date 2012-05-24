using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MultiThreadTest.RService;

namespace MultiThreadTest
{
    class Program
    {
        static RService.PlayerServiceClient client = new MultiThreadTest.RService.PlayerServiceClient();
        static List<Thread> thread = new List<Thread>();
        static bool running = true;

        [MTAThread]
        static void Main(string[] args)
        {

            for (int i = 0; i < 10; i++)
            {
                Thread th = new Thread(new ThreadStart(LoginAsync));
                th.Start();
                thread.Add(th);
            }

            Console.ReadLine();

            running = false;
        }

        static void ThreadMethod()
        {
            while (running)
            {
                long tick = DateTime.Now.Millisecond;
                PlayerInfo info = client.Login("tionase@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E");

                Console.WriteLine(string.Format("name:{0}   password:{0}", info.UserName, info.PassWord));

                tick = DateTime.Now.Millisecond - tick;
                if(tick < 0)
                {
                    tick += 1000;
                }
                Console.WriteLine(string.Format("{0} Threads:  {1} ms! ", Thread.CurrentThread.ManagedThreadId, tick));

                Thread.Sleep(5);
            }
        }

        static void LoginAsync()
        {
            long tick = DateTime.Now.Millisecond;

            //client.BeginLogin("tionase@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E", new AsyncCallback(LoginCallBack), Thread.CurrentContext);
            client.BeginGetPlayerFriendsOwerId(12, 12, new AsyncCallback(LoginCallBack), null);

            tick = DateTime.Now.Millisecond - tick;
            if(tick < 0)
            {
                tick += 1000;
            }
            Console.WriteLine(string.Format("{0} Threads:  {1} ms! ", Thread.CurrentThread.ManagedThreadId, tick));

            Thread.Sleep(5);
        }

        static void LoginCallBack(IAsyncResult result)
        {
            //PlayerInfo info = client.EndLogin(result);
            //Console.WriteLine(string.Format("name:{0}   password:{0}", info.UserName, info.PassWord));
            client.EndGetPlayerFriendsOwerId(result);
        }
    }
}
