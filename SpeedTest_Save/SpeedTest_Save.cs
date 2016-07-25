using InterSystems.Globals;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections;

using CacheEXTREME2.WMetaGlobal;
using CacheEXTREME2.WDirectGlobal;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using CacheEXTREME2.WProxyGlobal;
using EXTREMEAccessTESTS.Generated;
using System.Text;

namespace EXTREMEAccessTESTS
{
    public partial class EXTREMEcomparator : Form
    {
        private string _namespace = "USER";
        private string _user = "mihand";
        private string _password = "_password";
        public Connection conn;
        WowsTestContext trueWowsTestContext;
        //
        public EXTREMEcomparator()
        {
            InitializeComponent();
        }
        //
        private void EXTREMEcomparator_Load(object sender, EventArgs e)
        {
            try
            {
                if (conn == null)
                {
                    conn = ConnectionContext.GetConnection();
                    conn.Connect(_namespace, _user, _password);
                }
                initWowsTestMetaGlobal();
                trueWowsTestContext = new WowsTestContext(conn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void initWowsTestMetaGlobal()
        {
            GlobalMeta gm = new GlobalMeta("wowsTest", "wowsTestMeta");
            //structs zone
            //  empty
            //
            //
            //subscripts zone
            gm.AddKeyMeta(new StringValMeta(new ArrayList { "shipclass","String",0,255,""}), "classinfo");
            gm.AddKeyMeta(new IntValMeta(new ArrayList { "shiprank", "Integer", 0, 1000000, 5 }), "stuff");
            gm.AddKeyMeta(new StringValMeta(new ArrayList { "shipname", "string", 0, 50, 5 }), "shipinfo");
            //
            //
            //values zone
            gm.SetValuesMeta(1, new List<ValueMeta>{
                new IntValMeta(new ArrayList{"shipsCount","integer",0,1000,0})
            });
            gm.SetValuesMeta(2, new List<ValueMeta>{
                new ListValMeta(new ArrayList{"workers","List",0,100}, new StringValMeta(new ArrayList{"","String",0,255,0}))
                ,new ListValMeta(new ArrayList{"officers","List",0,100} ,new StringValMeta(new ArrayList{"","String",0,255,0}))
                ,new IntValMeta(new ArrayList{"workerSalary","integer",1,2000,0})
                ,new BytesValMeta(new ArrayList{"photo","bytes",200000})
            });
            gm.SetValuesMeta(3, new List<ValueMeta>{
                new ListValMeta(new ArrayList{"captains","list",0,100},new StringValMeta(new ArrayList{"","String",0,255,0}))
            });
            //
            //saving meta
            MetaReaderWriter mr = new MetaReaderWriter(conn);
            NodeReference nr = conn.CreateNodeReference(gm.GlobalMetaName);
            nr.Kill();
            //
            mr.SaveMeta(gm);
        }
        //
        private void compareSaveSpeed(int count)
        {
            native_VS_true_VS_object(count);
            GC.Collect();
            //testObjectSave(count);
        }
        private void native_VS_true_VS_object(int count,bool invoke = true)
        {
            txtInfo.Text = ""; txtInfo.Refresh();
            dataGridView.Enabled = false;
            progressNative.Value = 0;
            progressTrue.Value = 0;
            progressObject.Value = 0;
            killOld(new KillParam(count));
            progressClear.Value = 0;

            //object
            List<TimeSpan> objSpans = testObjectSave(count);
            long Osumm = objSpans.Sum(e => e.Ticks);
            double Oavrg = objSpans.Average(e => e.TotalMilliseconds);
            TimeSpan oTotal = new TimeSpan(Osumm);
            txtInfo.Text += " _ Object: " + oTotal.ToString() + "sec (" + Oavrg + "msec) _"; txtInfo.Refresh();
            Thread.Sleep(100);
            killOld(new KillParam(count));

            //native
            List<TimeSpan> nativeSpans = testNativeSave(count);
            long Nsumm = nativeSpans.Sum(e => e.Ticks);
            double Navrg = nativeSpans.Average(e => e.TotalMilliseconds);
            TimeSpan nTotal = new TimeSpan(Nsumm);
            txtInfo.Text += " _ Native: " + nTotal.ToString() + "sec (" + Navrg + "msec) _ "; txtInfo.Refresh();
            Thread.Sleep(100);
            killOld(new KillParam(count));
            Thread.Sleep(100);

            //true
            /*List<TimeSpan> trueSpans = testTrueSave(count);
            long Tsumm = trueSpans.Sum(e => e.Ticks);
            double Tmax = trueSpans.Max(e => e.TotalMilliseconds);
            TimeSpan tTotal = new TimeSpan(Tsumm);
            txtInfo.Text += " _ True: " + tTotal.ToString() + "(" + Tmax + ") _ "; txtInfo.Refresh();
            Thread.Sleep(100);
            killOld(new KillParam(count));
            Thread.Sleep(100);*/

            //
            dataGridView.Rows.Clear();
            int div = 100;
            if (count >= 10000) { div = 100; }
            if (count >= 50000) { div = 50; }
            if (count >= 100000) { div = 100; }
            if (count >= 200000) {div = 200;}
            int step = count / div;

            /*for (int i = 0, j = 0; i < count; i += step, j++)
            {
                dataGridView.Rows.Add();
                dataGridView["probeCol", j].Value = i;
                dataGridView["nativeCol", j].Value = nativeSpans[0].TotalMilliseconds;
                //dataGridView["trueCol", j].Value = trueSpans[0].TotalMilliseconds;
                dataGridView["objectCol", j].Value = objSpans[0].TotalMilliseconds;
                objSpans.RemoveAt(0);
                //trueSpans.RemoveAt(0);
                nativeSpans.RemoveAt(0);
            }*/
            txtInfo.Text += " _ obj/nat= " + ((double)Osumm / (double)Nsumm).ToString(); txtInfo.Refresh();
            dataGridView.Enabled = true;
        }

        private List<TimeSpan> testNativeSave(int count = 100)
        {
            List<TimeSpan> spans = new List<TimeSpan>();
            NodeReference wtr = conn.CreateNodeReference("wowsTest");
            Stopwatch nWatch = new Stopwatch();
            progressNative.Value = 0;
            //
            //Dataholders
            string shipClass = "kruis";
            object[] keys = new object[] { shipClass, 0 };
            object[] oficersNames = new object[] { "", "", "" };
            object[] workersNames = new object[] { "", "", "" };
            StringBuilder name1 = new StringBuilder(20);
            StringBuilder name2 = new StringBuilder(20);
            StringBuilder name3 = new StringBuilder(20);
            int s = 1000;
            byte[] byteValue = new byte[] { 6, 6, 6 };
            //
            for (int i = 0; i < count; i++)
            {
                name1.Remove(0, name1.Length);
                name2.Remove(0, name2.Length);
                name3.Remove(0, name3.Length);
                name1.Insert(0, i + 1);
                name2.Insert(0, i + 2);
                name3.Insert(0, i + 3);

                //отсюда
                keys[0] = shipClass;
                keys[1] = i;
                oficersNames[0] = name1.ToString();
                oficersNames[1] = name2.ToString();
                oficersNames[2] = name3.ToString();
                workersNames[0] = name3.ToString();
                workersNames[1] = name2.ToString();
                workersNames[2] = name1.ToString();
                s = s;
                byteValue = byteValue;
                //досюда
                {
                    nWatch.Reset();
                    nWatch.Start();

                    ValueList oficersVL = conn.CreateList();
                    oficersVL.Append(oficersNames);
                    ValueList workersVL = conn.CreateList();
                    workersVL.Append(workersNames);
                    ValueList vlist = conn.CreateList();
                    vlist.Append(oficersVL);
                    vlist.Append(workersVL);
                    vlist.Append(s);
                    vlist.Append(byteValue);
                    wtr.Set(vlist, keys);

                    nWatch.Stop();
                }

                TimeSpan nTS = nWatch.Elapsed;
                spans.Add(nTS);
                progressNative.Value = i * 100 / count;
            }
            progressNative.Value = 100;
            return spans;
        }
        private List<TimeSpan> testTrueSave(int count = 100)
        {
            List<TimeSpan> spans = new List<TimeSpan>();
            TrueNodeReference WTr = new TrueNodeReference(conn, "wowsTest");
            Stopwatch tWatch = new Stopwatch();
            progressTrue.Value = 0;
            for (int i = 0; i < count; i++)
            {
                string i1 = (i + 1).ToString(), i2 = (i + 2).ToString(), i3 = (i + 3).ToString();
                int s = 1000;
                //ОТСЮДА
                ArrayList akeys = new ArrayList() { "kruis", i };
                ArrayList asublist1 = new ArrayList() { i1, i2, i3 };
                ArrayList asublist2 = new ArrayList() { i3, i2, i1 };
                byte[] bytes = new byte[] { 6, 6, 6 };
                ArrayList alist = new ArrayList() { asublist1, asublist2, s, bytes };
                ArrayList values = new ArrayList() { 
                    new ArrayList() { i1, i2, i3 }
                    , new ArrayList() { i3, i2, i1 }
                    , 1000
                    , new byte[] { 6, 6, 6 } };
                //Досюда
                {
                    tWatch.Reset();
                    tWatch.Start();
                    
                    WTr.SetValues(akeys, alist);
                    
                    tWatch.Stop();
                }
                TimeSpan tTS = tWatch.Elapsed;
                spans.Add(tTS);
                progressTrue.Value = i * 100 / count;
            }
            progressTrue.Value = 100;
            return spans;
        }
        private List<TimeSpan> testObjectSave(int count = 100)
        {
            List<TimeSpan> spans = new List<TimeSpan>();
            TrueNodeReference WTr = new TrueNodeReference(conn, "wowsTest");
            Stopwatch oWatch = new Stopwatch();
            progressObject.Value = 0;
            //
            trueWowsTestContext.stuffManager.Validate = false;
            Random rand = new Random();
            //
            //Data holders
            string shipClass = "kruis";
            int s = 1000;
            List<string> oficersNames = new List<string> { "", "", "" };
            List<string> workersNames = new List<string> { "", "", "" };
            StringBuilder name1 = new StringBuilder(20);
            StringBuilder name2 = new StringBuilder(20);
            StringBuilder name3 = new StringBuilder(20);
            byte[] byteValue = new byte[] { 6, 6, 6 };
            stuffProxy entity = new stuffProxy();
            //
            for (int i = 0; i < count; i++)
            {
                name1.Remove(0, name1.Length);
                name2.Remove(0, name2.Length);
                name3.Remove(0, name3.Length);
                name1.Insert(0, i + 1);
                name2.Insert(0, i + 2);
                name3.Insert(0, i + 3);
                oficersNames[0] = name1.ToString();
                oficersNames[1] = name2.ToString();
                oficersNames[2] = name3.ToString();
                workersNames[0] = name3.ToString();
                workersNames[1] = name2.ToString();
                workersNames[2] = name1.ToString();
                //ОТСЮДА
                entity.shipclass = shipClass;
                entity.shiprank = i;
                entity.officers = oficersNames;
                entity.workers = workersNames;
                entity.workerSalary = s;
                entity.photo = byteValue;
                //Досюда
                {
                    oWatch.Reset();
                    oWatch.Start();

                    trueWowsTestContext.stuffManager.Save(entity);

                    oWatch.Stop();
                }
                TimeSpan tTS = oWatch.Elapsed;
                spans.Add(tTS);
                progressObject.Value = i * 100 / count;
            }
            progressObject.Value = 100;
            return spans;
        }
        //
        class KillParam
        {
            public int param;
            public KillParam(int param) { this.param = param; }
        }
        void killOld(object param)
        {
            int count = ((KillParam)param).param;
            progressClear.Value = 0;

            NodeReference wtr = conn.CreateNodeReference("wowsTest");
            object[] akeys = new object[2]{"kruis",0};
            for (int i = 0; i < count; i++)
            {
                akeys[1] = i;
                wtr.Kill(akeys);
                progressClear.Value = i * 100 / count;
            }
            progressClear.Value = 100;
        }
        //
        private void btnTest_Click(object sender, EventArgs e)
        {
            int count = (int)numericUpDown1.Value;
            if (MessageBox.Show("At the moment this test may force a lot of RAM, it may temporarily slow your system. For example, saving 300000 units takes approximately 2Gb of RAM.\n\n Do you want to continue?", "WARNING!!!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                compareSaveSpeed(count);
            }
            /*Thread tr = new Thread(new ParameterizedThreadStart(killOld));
            tr.Start(new KillParam(count));*/
        }
    }
}
