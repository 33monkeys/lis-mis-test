using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lis.Test.Integration.Common;
using NUnit.Framework;
using RestSharp;

namespace Lis.Test.Terminology
{
    [TestFixture]
    class TerminologyTest
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TerminologyHelper.DeleteMkb10();
            TerminologyHelper.CreateMkb10ValueSet();
            TerminologyHelper.CreateMkb10();
            TerminologyHelper.LoadMkb10();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void Test_Create_Delete_Dictionary()
        {
            
        }
    }
}
