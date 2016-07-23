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
using System.Data;
using System.Reflection;
using WowsCrudNamespace;

namespace SimpleCRUDexample
{
    public partial class ShipsEditor : Form
    {
        public Connection conn;
        WowsCrudContext wowsContext;

        public ShipsEditor(Connection conn)
        {
            this.conn = conn;
            InitializeComponent();
        }

        private void CRUDExample_Load(object sender, EventArgs e)
        {
            try
            {
                wowsContext = new WowsCrudContext(conn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Counting class ships count of manufacturers
            //  reseting counters
            List<ShipCounterProxy> shipsCounters = wowsContext.ShipCounterManager.GetAll();
            for (int i = 0; i < shipsCounters.Count; i++)
            {
                shipsCounters[i].Count = 0;
                wowsContext.ShipCounterManager.Save(shipsCounters[i]);
            }
            //  counting
            ArrayList manufacturers = wowsContext.TrueGetKeys(new ArrayList(){""});
            foreach (ArrayList manufacturer in manufacturers)
            {
                List<ShipInfoProxy> manufacturerShips = wowsContext.ShipInfoManager.GetByKeyMask(
                    new ShipInfoProxyKey(
                        (string)manufacturer[0]
                        , new Classification("", null)
                        , "")
                );
                //
                foreach(ShipInfoProxy ship in manufacturerShips){
                    ShipCounterProxy shipsCounter = wowsContext.ShipCounterManager.Get(
                        new ShipCounterProxyKey(
                            ship.Country
                            , ship.Classification
                        )
                    );
                    if (shipsCounter != null)
                    {
                        shipsCounter.Count++;
                        wowsContext.ShipCounterManager.Save(shipsCounter);
                        continue;
                    }
                    if (shipsCounter == null) 
                    {
                        wowsContext.ShipCounterManager.Save(new ShipCounterProxy(ship.Country, ship.Classification, 1));
                    }
                }
            }
        }
        //
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ShipInfoProxy ship = new ShipInfoProxy();
                ship.Country = txtCountry.Text;
                ship.Classification.ClassType = txtClass.Text;
                ship.Classification.Rank = (int)numRank.Value;
                ship.Name = txtShipName.Text;
                ship.Captain.Name = txtCaptainName.Text;
                ship.StuffCount = (int)numStuffCount.Value;
                ship.Efficienty = (double)numEff.Value;
                if (wowsContext.ShipInfoManager.Get(ship) == null)
                {
                    ShipCounterProxy shipsCounter = wowsContext.ShipCounterManager.Get(new ShipCounterProxyKey(ship.Country, ship.Classification));
                    if (shipsCounter == null)
                    {
                        wowsContext.ShipCounterManager.Save(new ShipCounterProxy(ship.Country, ship.Classification, 1));
                    }
                    else
                    {
                        shipsCounter.Count++;
                        wowsContext.ShipCounterManager.Save(shipsCounter);
                    }
                }
                wowsContext.ShipInfoManager.Save(ship);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                string continent = txtCountry.Text;
                string _class = txtClass.Text;
                int? rank = null;
                if(((int)numRank.Value)!=0){
                    rank = (int)numRank.Value;
                }
                double? eff = null;
                if(numEff.Value!=0){
                    eff = (double)numEff.Value;
                }
                ShipInfoProxyKey key = new ShipInfoProxyKey(continent, new Classification(_class, rank), txtShipName.Text);
                List<ShipInfoProxy> ships = wowsContext.ShipInfoManager.GetByKeyMask(key);

                fillShipsGrid(ships);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnGetAll_Click(object sender, EventArgs e)
        {
            List<ShipInfoProxy> ships = wowsContext.ShipInfoManager.GetAll();
            fillShipsGrid(ships);
        }
        private void btnKill_Click(object sender, EventArgs e)
        {
            try
            {
                string country = txtCountry.Text;
                string _class = txtClass.Text;
                int rank = (int)numRank.Value;
                double eff = (double)numEff.Value;
                wowsContext.ShipInfoManager.Delete(new ShipInfoProxyKey(country, new Classification(_class, rank), txtShipName.Text));
                //
                ShipCounterProxy shipsCounter = wowsContext.ShipCounterManager.Get(new ArrayList { country, _class, rank });
                shipsCounter.Count--;
                if (shipsCounter.Count == 0)
                {
                    wowsContext.ShipCounterManager.Delete(shipsCounter);
                    return;
                }
                wowsContext.ShipCounterManager.Save(shipsCounter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //
        private void fillShipsGrid(List<ShipInfoProxy> list)
        {
            grdEntities.Rows.Clear();
            int i = 0;
            foreach (ShipInfoProxy ship in list)
	        {
                grdEntities.Rows.Add();
                grdEntities["colNum", i].Value = i+1;
                grdEntities["colCountry", i].Value = ship.Country;
                grdEntities["colClass", i].Value = ship.Classification.ClassType;
                grdEntities["colRank", i].Value = ship.Classification.Rank;
                grdEntities["colName", i].Value = ship.Name;
                grdEntities["colCaptain", i].Value = ship.Captain.Name;
                grdEntities["colCapacity", i].Value = ship.StuffCount;
                grdEntities["colEff", i].Value = ship.Efficienty;
                i++;
            }
        }
        //
        private void grdEntities_SelectionChanged(object sender, EventArgs e)
        {
            if (grdEntities.Rows.Count > 1 && grdEntities.SelectedRows.Count == 1)
            {
                int i = grdEntities.SelectedRows[0].Index;
                txtCountry.Text = grdEntities["colCountry", i].Value.ToString();
                txtClass.Text = grdEntities["colClass", i].Value.ToString();
                numRank.Value = (int)grdEntities["colRank", i].Value;
                double d;
                Double.TryParse(grdEntities["colEff", i].Value.ToString(), out d);
                numEff.Value = (decimal)d;
                txtShipName.Text = grdEntities["colName", i].Value.ToString();
                txtCaptainName.Text = grdEntities["colCaptain", i].Value.ToString();
                numStuffCount.Value = (int)grdEntities["colCapacity", i].Value;
            }
        }
        //
        private void btnClearKey_Click(object sender, EventArgs e)
        {
            txtClass.Text = "";
            txtCountry.Text = "";
            txtShipName.Text = "";
            numRank.Value = 0;
        }
    }
}
