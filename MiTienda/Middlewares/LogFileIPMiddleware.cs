﻿namespace MiTienda.Middlewares
{
    public class LogFileIPMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        // Equipamos al middleware con lo que va a necesitar. Para que un middleware de paso al siguiente, debemos inyectar RequestDelegate y llamar
        // a la propiedad que lo coge como _next (podría tener otro nombre)
        // En este caso necesitamos IWebHostEnvironment para poder acceder a información del sistema de carpetas del servidor
        public LogFileIPMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        // Invoke o InvokeAsync
        // Este método se va a ejecutar automáticamente en cada petición porque en el program hemos registrado el middleware así:
        // app.UseMiddleware<LogFileIPMiddleware>();
        // Directamente httpContext tiene información de la petición que viene
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var IP = httpContext.Connection.RemoteIpAddress.ToString();

            //if (IP == "::1") // Bloquearía las peticiones de una IP
            //{
            //    httpContext.Response.StatusCode = 400;
            //    return;
            //}

            // PARA BLOQUEAR METODOS, EN ESTE CASO BLOQUEAMOS LOS GET
            // GET

            //if (httpContext.Request.Method == "GET")
            //{
            //    httpContext.Response.StatusCode = 400;
            //    return;
            //}



            var ruta = httpContext.Request.Path.ToString();
            var metodo = httpContext.Request.Method;

            var path = $@"{_env.ContentRootPath}\wwwroot\log.txt";


            //if (IP == "::1" && metodo == "POST" || metodo == "DELETE")
            //{
            //    using (StreamWriter writer = new StreamWriter(path, append: true))
            //    {
            //        writer.WriteLine($@"{IP} - {metodo} - {ruta} - {DateTime.Now} - {"Bloqueado"}");
            //    }
            //    httpContext.Response.StatusCode = 400;
            //    httpContext.Response.ContentType = "text/plain";
            //    await httpContext.Response.WriteAsync("Tu IP no tiene permisos para realizar esta petición.");

            //    return;
            //}

            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine($@"{IP} - {DateTime.Now} -  {ruta} - {metodo}");
                writer.WriteLine();

            }

            await _next(httpContext);
        }
    }
}
