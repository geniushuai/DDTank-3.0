using System.Web;
using System;

namespace Road.Request
{
    public class LinqHttpModule : IHttpModule
    {
        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(EndRequest);
        }

        public void EndRequest(Object sender, EventArgs e)
        {
            DbCenter.ClearDataContext();
        }
    }
}