//-----------------------------------------------------------------------
// <copyright file="FlickrFactory.cs" company="Sondre Bjellås">
// This software is licensed as Microsoft Public License (Ms-PL).
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net;
using FlickrNet;

namespace FlickrDownloader
{
    public sealed class FlickrFactory
    {
        // These are the keys for Flickr Downloadr application
        private const string key = "761deaf71208e06c7ed5b9e0e0a4e169";
        private const string secret = "93e5bc508d2137b9";

        private FlickrFactory()
        {
        }

        public static Flickr GetInstance()
        {
            if (!string.IsNullOrEmpty(Settings.Default.AuthToxen))
                return GetInstance(Settings.Default.AuthToxen);
            else
                return GetInstance(null);
        }

        public static Flickr GetInstance(string token)
        {
            Flickr.CacheTimeout = new TimeSpan(1, 0, 0, 0, 0);
            Flickr f = new Flickr(key, secret, token);
            f.Proxy = GetProxy();
            return f;
        }

        public static WebProxy GetProxy()
        {
            //bool useProxy = Settings.Default.UseProxy;

            //if (!useProxy)
            //{
            return WebProxy.GetDefaultProxy();
            //}

            //WebProxy proxy = new WebProxy();
            //proxy.Address = new Uri("http://" + Settings.Get("ProxyIPAddress") + ":" + Settings.Get("ProxyPort"));
            //if (Settings.Contains("ProxyUsername") && Settings.Get("ProxyUsername").Length > 0)
            //{
            //    NetworkCredential creds = new NetworkCredential();
            //    creds.UserName = Settings.Get("ProxyUsername");
            //    creds.Password = Settings.Get("ProxyPassword");
            //    creds.Domain = Settings.Get("ProxyDomain");
            //    proxy.Credentials = creds;
            //}

            //return proxy;
        }
    }
}