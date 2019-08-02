using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace newdnagraph
{
    public class Fragments : IComparable
    {
        public Fragments(string source, string target)
        {
            Source = source; Target = target;
        }
        public Fragments(string source, string target, string overlap, int overlength)
        {
            Source = source; Target = target;
            OverLap = overlap; OverLength = overlength;
        }
        public Fragments(Fragments f)
        {
            Source = f.Source; Target = f.Target;
            OverLap = f.OverLap; OverLength = f.OverLength;
        }
        public string Source { get; set; }
        public string Target { get; set; }
        public string OverLap { get; set; }
        public int OverLength { get; set; }
        public int CompareTo(object obj)  
        {
            if (obj is Fragments)
            {
                Fragments p = obj as Fragments;
                return p.OverLength.CompareTo(this.OverLength);
            }
            else
            {
                throw new ArgumentException("Object to compare to is not a Fragments object");
            }
        }
        private string GetOneMaxOverlapString(string str1, string str2)
        {
            string maxStr = "";
            int l = str1.Length > str2.Length ? str2.Length : str1.Length;
            for (int i = l; i >= 1; i--)
            {
                string temp = str1.Substring(0, i);
                if (str2.Substring(str2.Length - i, i) == temp)
                {
                    maxStr = temp;
                    break;
                }
            }
            return maxStr;
        }

        public void GetOverlapAndLength()
        {
            string FrontSubStr = this.GetOneMaxOverlapString(this.Source, this.Target);
            string BackSubStr = this.GetOneMaxOverlapString(this.Target, this.Source);
            if (FrontSubStr.Length > BackSubStr.Length)
            {
                this.OverLength = FrontSubStr.Length;
                this.OverLap = this.Target+  (FrontSubStr.Length == 0 ? this.Source : this.Source.Substring(FrontSubStr.Length, this.Source.Length - FrontSubStr.Length));
            }
            else
            {
                this.OverLength = BackSubStr.Length;
                this.OverLap = this.Source + (BackSubStr.Length == 0 ? this.Target : this.Target.Substring(BackSubStr.Length, this.Target.Length - BackSubStr.Length));
            }

        }
    }
   
    public class ProcessFragments
    {
        private List<string> _lResult = new List<string>() { "", "0" };
        private List<Fragments> _fList = new List<Fragments>();
        private Fragments FindFragments(List<Fragments> f, string source ,string target)
        {
            Fragments sF = null;
            if (f.Count > 0)
            {
                for (int m = 0; m < f.Count; m++)
                {
                    if (f[m].Source == source && f[m].Target == target)
                    {
                        sF = new Fragments(f[m]);
                        break;
                    }
                }
            }
            return sF;
        }
        private void InsertFragmentsList(List<Fragments> f, string source, string target)
        {
            Fragments ff = new Fragments(source, target);
            ff.GetOverlapAndLength();
            f.Add(ff);
        }
        private Fragments CopyFragments(Fragments f)
        {
            return new Fragments(f.Source, f.Target, f.OverLap, f.OverLength);
        }
        /// <summary>
        /// initial a list<Fragment> and delete including fragments
        /// </summary>
        /// <param name="l">original list</param>
        /// <returns>list of Fragments</returns>
        private List<Fragments> GetFragmentListFromList(List<string> l)
        {
            List<Fragments> fR = new List<Fragments>();
            Fragments fTemp=null;
            HashSet<string> sRedun = new HashSet<string>();
            string strTemp = "";
            for (int i = 0; i < l.Count - 1; i++)
                for (int j = i + 1; j < l.Count; j++)
                {
                    strTemp = InsideMatch(l[i],l[j]);
                    if (strTemp != "")
                    {
                        sRedun.Add(strTemp);
                    }
                    else
                    {
                        fTemp = this.FindFragments(_fList, l[i], l[j]);
                        if (fTemp == null)
                        {
                            this.InsertFragmentsList(_fList, l[i], l[j]);
                            this.InsertFragmentsList(fR, l[i], l[j]);
                        }
                        else
                        {
                            fR.Add(fTemp);
                        }
                    }
                }
            foreach (string sItem in sRedun)
            {
                l.Remove(sItem);
            }
            fR.Sort();
            return fR;
        }
        private List<Fragments> GetMaxLenthFragmentsList(List<Fragments> f, List<string> ltempResult)
        {
            //FragmentsList fR = (FragmentsList)_fList.Clone();
            List<Fragments> fR = new List<Fragments>();
            int iMaxLength=0;
            if (f.Count > 0)
            {
                iMaxLength = f[0].OverLength;
                ltempResult[1] = (int.Parse(ltempResult[1]) + iMaxLength).ToString();

                for (int i = 0; i < f.Count; i++)
                {
                    if (f[i].OverLength == iMaxLength)
                    {
                        fR.Add(this.CopyFragments(f[i]));
                    }
                }
            }
            return fR;

        }
        private List<Fragments> GetAllMaxOverlapString(List<string> OriginalList, List<string> ltempResult)
        {

            List<Fragments> fR =this.GetFragmentListFromList(OriginalList);
            return (this.GetMaxLenthFragmentsList(fR,ltempResult));
        }
        private List<string> CopyOList(List<string> OringalList)
        {
            List<string> newList = new List<string>();
            foreach (var j in OringalList)
            {
                newList.Add(j);
            }
            return newList;
        }
        private int[] getNext(string target)
        {
            int[] next = new int[target.Length];
            next[0] = -1;
            int k = -1;

            for (int i = 1; i < target.Length; i++)
            {
                while (k > -1 && target[k + 1] != target[i])
                {
                    k = next[k];
                }
                if (target[k + 1] == target[i])
                {
                    k = k + 1;
                }
                next[i] = k;
            }
            return next;
        }
        private int KMPSearch(string target, string source)
        {
            int[] next = this.getNext(target);
            int k = next[0];
            for (int i = 0; i < source.Length; i++)
            {
                while (k > -1 && target[k + 1] != source[i])
                {
                    k = next[k];
                }
                if (target[k + 1] == source[i])
                {
                    k = k + 1;
                }
                if (k == target.Length - 1)
                {
                    return i - target.Length + 1;
                }
            }
            return -1;
        }
        private void DfsReconstructFragment(List<string> OriginalList, List<Fragments> lMax, List<string> ltempResult)
        {
            if (OriginalList.Count == 1)
            {
                ltempResult[0] = OriginalList[0];
                if (int.Parse(ltempResult[1]) > int.Parse(_lResult[1]))
                {
                    _lResult[0] = ltempResult[0];
                    _lResult[1] = ltempResult[1];
                }
                return;
            }
            lMax = this.GetAllMaxOverlapString(OriginalList, ltempResult);
            OriginalList = this.CopyOList(OriginalList);
            ltempResult = this.CopyOList(ltempResult);
            for (int i = 0; i < lMax.Count; i++)
            {
                OriginalList.Remove(lMax[i].Source);
                OriginalList.Remove(lMax[i].Target);
                OriginalList.Add(lMax[i].OverLap);
                DfsReconstructFragment(OriginalList, lMax, ltempResult);

                OriginalList.Add(lMax[i].Source);
                OriginalList.Add(lMax[i].Target);
                OriginalList.Remove(lMax[i].OverLap);

            }

        }
        private string InsideMatch(string source,string target)
        {
            string strTemp = "";
            string maxStr = source.Length > target.Length ? source : target;
            string minStr = maxStr == source ? target : source;
            if (this.KMPSearch(minStr, maxStr) > -1)
            {
                strTemp = minStr;
            }
            return strTemp;

        }
        public string ReconstructFragmentSentences(List<string> OringialList)
        {

            List<Fragments> lMax = new List<Fragments>();

            List<string> ltempResult = new List<string>() { "", "0" };
            List<string> l = OringialList;

            this.DfsReconstructFragment(l, lMax, ltempResult);
            return _lResult[0];
        }
        static void Main(string[] args)
        {

            List<string> l = new List<string>();
            l = "m quaerat voluptatem.;pora incidunt ut labore et d;, consectetur, adipisci velit;olore magnam aliqua;idunt ut labore et dolore magn;uptatem.;i dolorem ipsum qu;iquam quaerat vol;psum quia dolor sit amet, consectetur, a;ia dolor sit amet, conse;squam est, qui do;Neque porro quisquam est, qu;aerat voluptatem.;m eius modi tem;Neque porro qui;, sed quia non numquam ei;lorem ipsum quia dolor sit amet;ctetur, adipisci velit, sed quia non numq;unt ut labore et dolore magnam aliquam qu;dipisci velit, sed quia non numqua;us modi tempora incid;Neque porro quisquam est, qui dolorem i;uam eius modi tem;pora inc;am al"

            .Split(';')
            .ToList();
            ProcessFragments p = new ProcessFragments();
            Console.WriteLine(p.ReconstructFragmentSentences(l));
            Console.ReadLine();
        }
    }
}
