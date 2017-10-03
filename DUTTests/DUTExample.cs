using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesGenerationTests
{
    class StatisticProxy
    {
        public List<ChangeKey> Changes;

        public string da;
        public string db;
        public string dc;

        public StatisticProxy(List<ChangeKey> keys, string da, string db, string dc)
        {
            this.Changes = keys;
            this.da = da;
            this.db = db;
            this.dc = dc;
        }

        public StatisticProxy()
        {
            Changes = new List<ChangeKey>();
            db = "n";
            da = "n";
            dc = "n";
        }

        public bool StatisticProxyValidator(StatisticProxy entity)
        {
            return true;
        }
    }

    public class ChangeKey
    {
        public string a;
        public string b;
        public int? d;

        public ChangeKey()
        {
            this.a = "n";
            this.b = "n";
            this.d = 0;
        }

        public ChangeKey(string a, string b, int? d)
        {
            this.a = a;
            this.b = b;
            this.d = d;
        }
    }

    public class ChangeSubKey
    {
        public double? a;
        public string b;
        public int? c;

        public ChangeSubKey()
        {
            this.a = 0;
            this.b = "n";
            this.c = 0;
        }

        public ChangeSubKey(double? a, string b, int? c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }
}
