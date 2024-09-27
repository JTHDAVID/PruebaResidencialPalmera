using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PruebaResidencialPalmera.Codigo
{
    /// <summary>
    /// Descripción breve de AutocompleteHandler
    /// </summary>
    public class AutocompleteHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hola a todos");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}