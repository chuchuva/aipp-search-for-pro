using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchForPro.Models;

namespace UnitTests
{
   [TestClass]
   public class TestHonours
   {
      [TestMethod]
      public void TestAipp()
      {
         Assert.AreEqual("M.Photog", Honours.Convert("MP"));
         Assert.AreEqual("M.Photog I", Honours.Convert("MPI"));
         Assert.AreEqual("M.Photog II", Honours.Convert("MPII"));
         Assert.AreEqual("M.Photog III", Honours.Convert("MPIII"));
         Assert.AreEqual("M.Photog IV", Honours.Convert("MPIV"));
         Assert.AreEqual("M.Photog, G.M. Photog", Honours.Convert("MP, GP"));
         Assert.AreEqual("M.Photog, M.Photog I", Honours.Convert("MP,MPI"));
         Assert.AreEqual("M.Photog, AIPP", Honours.Convert("MP, AIPP"));
         Assert.AreEqual("Fellow of the NZIPP", Honours.Convert("FNZIPP"));
         Assert.AreEqual("Fellow, 1111, Honorary Fellow, Fellow of the NZIPP", Honours.Convert("FAIPP, 1111, Hon.FAIPP, FNZIPP"));
      }
   }
}
