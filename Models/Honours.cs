using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SearchForPro.Models
{
   public class Honours
   {
      private static readonly Dictionary<string, string> LookupTable =
         new Dictionary<string, string>() {
            {"MP", "M.Photog"},
            {"MPI", "M.Photog I"},
            {"MPII", "M.Photog II"},
            {"MPIII", "M.Photog III"},
            {"MPIV", "M.Photog IV"},
            {"MPV", "M.Photog V"},
            {"GP", "G.M. Photog"},
            {"GPI", "G.M. Photog I"},
            {"GPII", "G.M. Photog II"},
            {"GPIII", "G.M. Photog III"},
            {"GPIV", "G.M. Photog IV"},
            {"GPV", "G.M. Photog V"},
            {"FAIPP", "Fellow"},
            {"Hon.FAIPP", "Honorary Fellow"},
            {"Hon. LM", "Honorary Life Member"},
            {"OAM", "Medal of the Order of Australia"},
            {"ANZIPP", "Associate of the NZIPP"},
            {"FNZIPP", "Fellow of the NZIPP"},
            {"Hon. EFIAP", "Honorary Excellence Federation Internationale de l'Art Photographique"},
            {"FRPS", "Fellow of the Royal Photographic Society"},
            {"FMPA", "Fellow of the Masters Photographers Association"},
            {"FBIPP", "Fellow of the British Institute of Photography"},
         };

      public static string Convert(string abbreviatedHonours)
      {
         if (abbreviatedHonours.IsNullOrEmpty())
            return abbreviatedHonours;

         var sb = new StringBuilder();
         var arr = abbreviatedHonours.Split(new string[] {", ", ","},
            StringSplitOptions.RemoveEmptyEntries);
         for (int i = 0; i < arr.Length; i++)
			{
            string replacement;
            if (LookupTable.TryGetValue(arr[i], out replacement))
               arr[i] = replacement;
			}
         return string.Join(", ", arr);
      }
   }
}