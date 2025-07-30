// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;
using Wisp.Framework.Configuration;

using NCSResponse = NetCoreServer.HttpResponse;

namespace Wisp.Framework.Http.Impl.NetCoreServer;

/// <summary>
/// This is the HTTP backend implementation for NetCoreServer
/// </summary>
/// <param name="config"></param>
/// <param name="router"></param>
/// <param name="log"></param>
/// <param name="middlewares"></param>
public class NetCoreServerAdapter(IOptions<WispConfiguration> config, Router router, ILogger<NetCoreServerAdapter> log, IEnumerable<IHttpMiddleware> middlewares)
    : HttpServer(IPAddress.Parse(config.Value.Host), config.Value.Port), IHttpServer
{
    protected override TcpSession CreateSession()
    {
        return new AdapterSession(this, router, log, middlewares);
    }

    public Task StartAsync()
    {
        Start();
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        Stop();
        return Task.CompletedTask;
    }

    private class AdapterSession : HttpSession
    {
        private readonly Router _router;

        private readonly ILogger<NetCoreServerAdapter> _log;

        private readonly List<IHttpMiddleware> _middlewares;

        public AdapterSession(NetCoreServerAdapter server, Router router, ILogger<NetCoreServerAdapter> log, IEnumerable<IHttpMiddleware> middlewares) : base(server)
        {
            _router = router;
            _log = log;
            _middlewares = middlewares.ToList();
        }
        
        protected override async void OnReceivedRequest(HttpRequest request)
        {
            var context = new AdapterContext(request, this);

            foreach (var m in _middlewares)
            {
                await m.OnRequestReceived(context);
            }

            if (context.IsHandled)
            {
                var r = await MakeResponse(context.Response);
                try
                {
                    SendResponse(r);
                }
                catch (ObjectDisposedException ex)
                {
                    _log.LogError(ex, "could not handle static file");
                    return;
                }
                
                return;
            }

            var response = await _router.Dispatch(context);

            response.Body.Position = 0;
            var bodyText = await new StreamReader(response.Body).ReadToEndAsync();

            var res = new NCSResponse(response.StatusCode, "HTTP/1.1");

            foreach (var (k, v) in response.Headers)
            {
                res.SetHeader(k, v);
            }

            foreach (var (k, v) in response.Cookies)
            {
                res.SetCookie(k, v);
            }

            res.SetHeader("Content-Type", response.ContentType);
            res.SetBody(bodyText);

            SendResponse(res);
        }

        private async Task<NCSResponse> MakeResponse(IHttpResponse res)
        {
            var response = new NCSResponse(res.StatusCode, "HTTP/1.1");

            foreach (var (k, v) in res.Headers)
            {
                response.SetHeader(k, v);
            }

            foreach (var (k, v) in res.Cookies)
            {
                response.SetCookie(k, v);
            }

            var ms = new MemoryStream();
            await res.Body.CopyToAsync(ms);
            var body = ms.ToArray();

            response.SetHeader("Content-Type", res.ContentType);
            response.SetBody(body);

            return response;
        }

        protected override void OnError(SocketError error)
        {
            _log.LogError("socket error: {Err}", error);
        }
    }
}