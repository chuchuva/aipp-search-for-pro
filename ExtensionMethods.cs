using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchForPro
{
   public static class ExtensionMethods
   {
      /// <summary>
      /// Answers true if this String is either null or empty.
      /// </summary>
      /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
      public static bool IsNullOrEmpty(this string s)
      {
         return string.IsNullOrEmpty(s);
      }

      /// <summary>
      /// Answers true if this String is neither null or empty.
      /// </summary>
      /// <remarks>I'm also tired of typing !String.IsNullOrEmpty(s)</remarks>
      public static bool HasValue(this string s)
      {
         return !string.IsNullOrEmpty(s);
      }
   }
}