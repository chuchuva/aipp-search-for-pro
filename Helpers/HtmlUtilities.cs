using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SearchForPro.Helpers
{
   public class HtmlUtilities
   {
      private static readonly Regex _newLine =
          new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);
      private static readonly Regex _markdownMiniBold =
          new Regex(@"(?<=^|[\s,(])(?:\*\*|__)(?=\S)(.+?)(?<=\S)(?:\*\*|__)(?=[\s,?!.)]|$)", RegexOptions.Compiled);

      private static readonly Regex _markdownMiniItalic =
          new Regex(@"(?<=^|[\s,(])(?:\*|_)(?=\S)(.+?)(?<=\S)(?:\*|_)(?=[\s,?!.)]|$)", RegexOptions.Compiled);
      private static readonly Regex _markdownMiniLink = new Regex(
          @"(?<=\s|^)
\[
  (?<name>[^\]]+)
\]
\(
  (?<url>(https?|ftp)://[^)\s]+?)
  (
      \s(""|&quot;)
      (?<title>[^""]+)
      (""|&quot;)
  )?
\)",
          RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

      private static readonly Regex _quoteSinglePair = new Regex(@"(?<![A-Za-z])'(.*?)'(?![A-Za-z])",
                                                                 RegexOptions.Compiled);

      private static readonly Regex _quoteSingle = new Regex(@"(?<=[A-Za-z0-9])'([A-Za-z]+)", RegexOptions.Compiled);
      private static readonly Regex _quoteDoublePair = new Regex(@"""(.*?)""", RegexOptions.Compiled);
      private static Regex _nofollow = new Regex(@"(<a\s+href=""([^""]+)"")([^>]*>)",
                                                 RegexOptions.IgnoreCase | RegexOptions.Compiled);

      private static readonly Regex _sanitizeUrl = new Regex(@"[^-a-z0-9+&@#/%?=~_|!:,.;\(\)]",
                                                             RegexOptions.IgnoreCase | RegexOptions.Compiled);


      /// <summary>
      /// Produces optional, URL-friendly version of a title, "like-this-one". 
      /// hand-tuned for speed, reflects performance refactoring contributed by John Gietzen (user otac0n) 
      /// </summary>
      public static string URLFriendly(string title)
      {
         if (title == null) return "";

         const int maxlen = 80;
         int len = title.Length;
         bool prevdash = false;
         var sb = new StringBuilder(len);
         string s;
         char c;

         for (int i = 0; i < len; i++)
         {
            c = title[i];
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
               sb.Append(c);
               prevdash = false;
            }
            else if (c >= 'A' && c <= 'Z')
            {
               // tricky way to convert to lowercase
               sb.Append((char)(c | 32));
               prevdash = false;
            }
            else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_')
            {
               if (!prevdash && sb.Length > 0)
               {
                  sb.Append('-');
                  prevdash = true;
               }
            }
            else if (c >= 128)
            {
               s = c.ToString().ToLowerInvariant();
               if ("àåáâäãåą".Contains(s))
               {
                  sb.Append("a");
               }
               else if ("èéêëę".Contains(s))
               {
                  sb.Append("e");
               }
               else if ("ìíîïı".Contains(s))
               {
                  sb.Append("i");
               }
               else if ("òóôõöø".Contains(s))
               {
                  sb.Append("o");
               }
               else if ("ùúûü".Contains(s))
               {
                  sb.Append("u");
               }
               else if ("çćč".Contains(s))
               {
                  sb.Append("c");
               }
               else if ("żźž".Contains(s))
               {
                  sb.Append("z");
               }
               else if ("śşš".Contains(s))
               {
                  sb.Append("s");
               }
               else if ("ñń".Contains(s))
               {
                  sb.Append("n");
               }
               else if ("ýŸ".Contains(s))
               {
                  sb.Append("y");
               }
               else if (c == 'ł')
               {
                  sb.Append("l");
               }
               else if (c == 'đ')
               {
                  sb.Append("d");
               }
               else if (c == 'ß')
               {
                  sb.Append("ss");
               }
               else if (c == 'ğ')
               {
                  sb.Append("g");
               }
               prevdash = false;
            }
            if (i == maxlen) break;
         }

         if (prevdash)
            return sb.ToString().Substring(0, sb.Length - 1);
         else
            return sb.ToString();
      }

      /// <summary>
      /// returns Html Encoded string
      /// </summary>
      public static string Encode(string s)
      {
         return HttpUtility.HtmlEncode(s);
      }

      /// <summary>
      /// returns Url Encoded string
      /// </summary>
      public static string UrlEncode(string html)
      {
         return HttpUtility.UrlEncode(html);
      }

      /// <summary>
      /// tiny subset of Markdown: *italic* and __bold__ and [link](http://example.com "title") only  
      /// </summary>
      public static string MarkdownMini(string text)
      {
         if (text.IsNullOrEmpty())
            return "";

         text = HttpUtility.HtmlEncode(text);

         if (_newLine.IsMatch(text))
         {
            text = "<p>" + _newLine.Replace(text, "</p><p>") + "</p>";
         }


         bool hasEligibleChars = false;

         // for speed, quickly screen out strings that don't contain anything we can possibly work on
         char pc = ' ';
         foreach (char c in text)
         {
            if (c == '*' || c == '_' || (pc == ']' && c == '('))
            {
               hasEligibleChars = true;
               break;
            }
            pc = c;
         }

         if (!hasEligibleChars)
            return text;

         // replace any escaped characters, first, so we don't do anything with them
         text = text.Replace(@"\*", "&#42;");
         text = text.Replace(@"\_", "&#95;");
         text = text.Replace(@"\[", "&#91;");
         text = text.Replace(@"\]", "&#93;");
         text = text.Replace(@"\(", "&#40;");
         text = text.Replace(@"\)", "&#41;");

         // must handle bold first (it's longer), then italic
         text = _markdownMiniBold.Replace(text, "<b>$1</b>");
         text = _markdownMiniItalic.Replace(text, "<i>$1</i>");

         text = _markdownMiniLink.Replace(text, new MatchEvaluator(MarkdownMiniLinkEvaluator));

         return text;
      }

      private static string MarkdownMiniLinkEvaluator(Match match)
      {
         string url = match.Groups["url"].Value;
         string name = match.Groups["name"].Value;
         string title = match.Groups["title"].Value;
         string link;

         url = SanitizeUrl(url);
         // we don't need to sanitize name here, as we encoded in the parent function

         if (title.HasValue())
         {
            title = title.Replace("\"", "");
            link = String.Format(@"<a href=""{0}"" title=""{2}"" rel=""nofollow"">{1}</a>", url, name, title);
         }
         else
         {
            link = String.Format(@"<a href=""{0}"" rel=""nofollow"">{1}</a>", url, name);
         }

         // if this is a link to a site in our network, it's whitelisted and safe to follow
         if (true /*SiteExtensions.IsInNetwork(url)*/)
            link = link.Replace(@" rel=""nofollow""", "");

         return link;
      }

      /// <summary>
      /// returns "safe" URL, stripping anything outside normal charsets for URL
      /// </summary>
      public static string SanitizeUrl(string url)
      {
         if (url.IsNullOrEmpty()) return url;
         return _sanitizeUrl.Replace(url, "");
      }
      
      /// <summary>
      /// converts to fancy typographical HTML entity versions of "" and '' and -- and ...
      /// loosely based on rules at http://daringfireball.net/projects/smartypants/
      /// assumes NO HTML MARKUP TAGS inside the text!
      /// </summary>
      private static string SmartyPantsMini(string s)
      {
         bool hasEligibleChars = false;
         char p = ' ';

         // quickly screen out strings that don't contain anything we can possibly work on
         // the VAST majority of actual titles have none of these chars
         foreach (char c in s)
         {
            if ((p == '-' && c == '-') || c == '\'' || c == '"' || (p == '.' && c == '.') || c == '&')
            {
               hasEligibleChars = true;
               break;
            }
            p = c;
         }

         if (!hasEligibleChars) return s;

         // convert encoded quotes back to regular quotes for simplicity
         s = s.Replace("&quot;", @"""");

         // ... (or more) becomes &hellip;
         if (s.Contains("..."))
            s = Regex.Replace(s, @"\.{3,}", @"&hellip;");

         // --- or -- becomes &mdash;
         if (s.Contains("--"))
            s = Regex.Replace(s, @"---?(\s)", @"&mdash;$1");

         // "foo" becomes &ldquo;foo&rdquo;
         if (s.Contains("\""))
            s = _quoteDoublePair.Replace(s, "&ldquo;$1&rdquo;");

         // 'foo' becomes &lsquo;foo&rsquo;
         // A's and O'Malley becomes &rsquo;s
         if (s.Contains("'"))
         {
            s = _quoteSinglePair.Replace(s, "&lsquo;$1&rsquo;");
            s = _quoteSingle.Replace(s, "&rsquo;$1");
         }

         return s;
      }


      /// <summary>
      /// encodes any HTML, also adds any fancy typographical entities versions of "" and '' and -- and ...
      /// </summary>
      public static string EncodeFancy(string s)
      {
         if (s.IsNullOrEmpty())
            return s;
         string html = SmartyPantsMini(Encode(s));

         return html;
      }
   }
}