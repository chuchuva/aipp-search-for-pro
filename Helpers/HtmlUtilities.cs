using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SearchForPro.Helpers
{
   public class HtmlUtilities
   {
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
   }
}