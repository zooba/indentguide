using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IndentGuide;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests {
    [TestClass]
    public class VersionTests {
        [TestMethod]
        public void VersionsMatch() {
            var package = typeof(IndentGuidePackage);
            var assem = package.Assembly;
            
            var packageVersion = package.GetCustomAttributes(false).OfType<InstalledProductRegistrationAttribute>().Single();
            var fileVersion = assem.GetCustomAttributes(false).OfType<AssemblyFileVersionAttribute>().Single();

            var ver = Version.Parse(fileVersion.Version);
            string verStr;
            if (ver.Minor < 9) {
                verStr = ver.Major.ToString();
            } else {
                verStr = string.Format("{0} (Beta {1})", ver.Major + 1, ver.Build + 1);
            }
            Assert.AreEqual(verStr, packageVersion.ProductId, "Version in package registration is incorrect");

            var di = new DirectoryInfo(Path.GetDirectoryName(assem.Location));
            var files = di.GetFiles("*.vsixmanifest");
            while (files.Length == 0 && di.Parent != di.Root) {
                di = di.Parent;
                files = di.GetFiles("*.vsixmanifest");
            }

            if (files.Length == 0) {
                Assert.Fail("Cannot find VSIX Manifest file from {0}", assem.Location);
            }

            var doc = XDocument.Load(files[0].FullName);
            var xmlns = "http://schemas.microsoft.com/developer/vsx-schema/2010";
            var xmlVerStr = doc
                .Element(XName.Get("Vsix", xmlns))
                .Element(XName.Get("Identifier", xmlns))
                .Element(XName.Get("Version", xmlns))
                .Value;

            var xmlVer = Version.Parse(xmlVerStr);
            
            Assert.AreEqual(ver, xmlVer, "Version in VSIX Manifest is incorrect");
        }
    }
}
