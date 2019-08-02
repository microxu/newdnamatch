using Microsoft.VisualStudio.TestTools.UnitTesting;
using newdnagraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newdnagraph.Tests
{
    [TestClass()]
    public class ProcessFragmentsTests
    {
        [TestMethod()]
        public void ReconstructFragmentSentencesTest()
        {
            var fragments = "m quaerat voluptatem.;pora incidunt ut labore et d;, consectetur, adipisci velit;olore magnam aliqua;idunt ut labore et dolore magn;uptatem.;i dolorem ipsum qu;iquam quaerat vol;psum quia dolor sit amet, consectetur, a;ia dolor sit amet, conse;squam est, qui do;Neque porro quisquam est, qu;aerat voluptatem.;m eius modi tem;Neque porro qui;, sed quia non numquam ei;lorem ipsum quia dolor sit amet;ctetur, adipisci velit, sed quia non numq;unt ut labore et dolore magnam aliquam qu;dipisci velit, sed quia non numqua;us modi tempora incid;Neque porro quisquam est, qui dolorem i;uam eius modi tem;pora inc;am al"
                .Split(';')
                .ToList();

            ProcessFragments p = new ProcessFragments();
            string actual = p.ReconstructFragmentSentences(fragments);

            Assert.AreEqual("Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem.", actual);
        }
        [TestMethod()]
        public void ReconstructFragmentSentencesTest2()
        {
            var fragments = " ; ; ; ; ; "
                .Split(';')
                .ToList();

            ProcessFragments p = new ProcessFragments();
            string actual = p.ReconstructFragmentSentences(fragments);

            Assert.AreEqual("", actual);
        }
    }
}