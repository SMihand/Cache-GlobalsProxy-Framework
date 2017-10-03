using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CacheEXTREME2.WMetaGlobal;
using CacheEXTREME2.WProxyGlobal;
using CacheEXTREME2.WDirectGlobal;
using InterSystems.Globals;
using System.Collections;
using System.Reflection;
using EntitiesGenerationTests;

namespace DUTTests
{
    public partial class DUTExampleForm : Form
    {
        private static string _namespace = "USER";
        private static string _user = "mihand";
        private static string _password = "19735";
        private static Connection conn;
        private static DUTManager<ChangeKey, StatisticProxy> statistic;

        public DUTExampleForm()
        {
            InitializeComponent();
        }

        private void DUTExampleForm_Load(object sender, EventArgs e)
        {
            try
            {
                conn = ConnectionContext.GetConnection();
                conn.Connect(_namespace, _user, _password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private void initializeManager()
        {
            /*StructValMeta subKeyStructMeta = new StructValMeta("ChangeSubKey","c", new List<ValueMeta>(){
                                new DoubleValMeta(typeof(ChangeSubKey).GetFields()[0].Name)
                                ,new StringValMeta(typeof(ChangeSubKey).GetFields()[1].Name)
                                ,new IntValMeta(typeof(ChangeSubKey).GetFields()[2].Name)
                            });
            subKeyStructMeta.StructId = 1;*/
            StructValMeta keyStructMeta = new StructValMeta("Changes", "ChangeKey", new List<ValueMeta>(){
                                new StringValMeta(typeof(ChangeKey).GetFields()[0].Name)
                                ,new StringValMeta(typeof(ChangeKey).GetFields()[1].Name)
                                //,new StructValMeta(typeof(ChangeKey).GetFields()[2].Name, subKeyStructMeta)
                                ,new IntValMeta(typeof(ChangeKey).GetFields()[2].Name)
                            });
            keyStructMeta.structDefinition.StructId = 0;
            List<ValueMeta> valuesMeta = new List<ValueMeta>(){
                            new StringValMeta(typeof(StatisticProxy).GetFields()[1].Name)
                            ,new StringValMeta(typeof(StatisticProxy).GetFields()[2].Name)
                            ,new StringValMeta(typeof(StatisticProxy).GetFields()[3].Name)
                        };
            //
            List<IStructManager> structsManagers = new List<IStructManager>();
            structsManagers.Add(new StructManager<ChangeKey>(keyStructMeta.structDefinition, structsManagers));
            //structsManagers.Add(new StructManager<ChangeSubKey>(subKeyStructMeta, structsManagers));
            //
            //
            statistic = new DUTManager<ChangeKey, StatisticProxy>(
                   keyStructMeta
                   , valuesMeta
                   , new TrueNodeReference(conn, "wows")
                   , structsManagers);
        }

        delegate void onFullSequenceDelegate(List<ChangeKey> keys);
        private void onFullSequence(List<ChangeKey> keys)
        {
            List<ChangeKey> keySum = new List<ChangeKey>(keys.Count);
            foreach(ChangeKey key in keys)
            {
                keySum.Add(key);
                StatisticProxy curStat = statistic.Get(keySum);
                if (curStat != null)
                {
                    curStat.da = (int.Parse(curStat.da) + 1).ToString();
                    statistic.Save(curStat);
                    statistic.Kill(keySum);
                    continue;
                }
                StatisticProxy stat = new StatisticProxy(keySum, key.a.ToString(), "n", "n");
                statistic.Save(stat);
                statistic.Kill(keySum);
            }
        }

        private void StatManagerTest()
        {
            onFullSequenceDelegate onFull = new onFullSequenceDelegate(onFullSequence);
            DUTStatManager<ChangeKey, StatisticProxy> statManager = new DUTStatManager<ChangeKey, StatisticProxy>(4, onFull);
            //
            for (int i = 1; i <= 16; i++)
            {
                ChangeKey chk = new ChangeKey(i.ToString(), i.ToString(),  i);
                statManager.AddStat(chk);
            }
        }

        private void DUTManagerExample(object sender, EventArgs e)
        {
            try
            {
                initializeManager();
                StatManagerTest();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
