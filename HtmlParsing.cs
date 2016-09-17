using HtmlAgilityPack;
using System;
using System.Net;
using System.Text;

namespace MustafaUğuz.Utility.System.Internet
{
    public class HtmlParsing
    {
        public static string DownloadHtml(string url)
        {
            Uri uri = new Uri(url);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return WebUtility.HtmlDecode(client.DownloadString(uri));
        }

        public static string DownloadHtml(string url, Encoding encoding)
        {
            Uri uri = new Uri(url);
            WebClient client = new WebClient();
            client.Encoding = encoding;
            return WebUtility.HtmlDecode(client.DownloadString(uri));
        }

        public static string DownloadHtml(Uri uri)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return WebUtility.HtmlDecode(client.DownloadString(uri));
        }

        public static string DownloadHtml(Uri uri, Encoding encoding)
        {
            WebClient client = new WebClient();
            client.Encoding = encoding;
            return WebUtility.HtmlDecode(client.DownloadString(uri));
        }

        public static string DownloadHtml(string url, WebClient client)
        {
            Uri uri = new Uri(url);
            return WebUtility.HtmlDecode(client.DownloadString(uri));
        }
        
        public static string DownloadHtml(Uri uri, WebClient client)
        {
            return WebUtility.HtmlDecode(client.DownloadString(uri));
        }

        public static HtmlDocument GetHtmlDocument(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        public static HtmlNode GetHtmlNode(HtmlDocument document, string xpath)
        {
            return document.DocumentNode.SelectSingleNode(xpath);
        }

        public static HtmlNode GetHtmlNode(string html, string xpath)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document.DocumentNode.SelectSingleNode(xpath);
        }

        public static HtmlNodeCollection GetHtmlNodes(HtmlDocument document, string xpath)
        {
            return document.DocumentNode.SelectNodes(xpath);
        }

        public static HtmlNodeCollection GetHtmlNodes(string html, string xpath)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document.DocumentNode.SelectNodes(xpath);
        }
    }
}
