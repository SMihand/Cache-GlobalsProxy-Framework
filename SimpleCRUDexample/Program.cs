using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WMetaGlobal;
using CacheEXTREME2.WProxyGlobal;
using InterSystems.Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NaviesNamespace;

namespace SimpleCRUDexample
{
    static class Program
    {
        public static Connection conn;
        private static string _namespace = "USER";
        private static string _user = "_user";
        private static string _password = "_password";
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            try
            {
                conn = ConnectionContext.GetConnection();
                conn.Connect(_namespace, _user, _password);
                createWowsGlobalMeta();
                fillWowsGlobal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Application.Run(new ShipsEditor(conn));
        }
        static void createWowsGlobalMeta()
        {
            GlobalMeta gm = new GlobalMeta("WowsCrud", "WowsCrudMeta");
            gm.GlobalSemantic = "Navies";
            //
            //Step 1: 
            //  Structs meta declarations
            //
            StructDefinition contactInfoStruct = new StructDefinition();
            contactInfoStruct.StructTypeName = "Contact";
            contactInfoStruct.elementsMeta = new List<ValueMeta>()
                {
                    new StringValMeta(new ArrayList(){"Name","String",0,255,"Name"})
                    ,new IntValMeta(new ArrayList(){"Phone","Integer",1111111,9999999,1111111})
                };

            StructDefinition classificationStruct = new StructDefinition();
            classificationStruct.StructTypeName = "Classification";
            classificationStruct.elementsMeta = new List<ValueMeta>()
                {
                    new StringValMeta(new ArrayList(){"ClassType","String",0,255,"ClassType"})
                    ,new IntValMeta(new ArrayList(){"Rank","Integer",0,10,0})
                };
            //
            //Step 2:
            //  Adding structs definitions (type = StructDefinitions) to global meta
            //
            gm.AddStruct(contactInfoStruct);
            gm.AddStruct(classificationStruct);
            //
            //Step 3:
            //  Indexes meta declarations
            //  (ArrayList fills according to metaglobal specification)
            gm.AddKeyMeta(new StringValMeta(new ArrayList() { "Country", "String", 0, 255, "Country" }), "Manufacturer");
            gm.AddKeyMeta(new StructValMeta( "Classification", classificationStruct), "ShipCounter");
            gm.AddKeyMeta(new StringValMeta(new ArrayList() { "Name", "String", 0, 255, "ShipName" }), "ShipInfo");       
            //
            //Step 4:
            //  Values meta declarations
            //      first index Manufacturer values
            gm.AddValuesMeta(1, new List<ValueMeta>(){
                            new StructValMeta("Charge", contactInfoStruct)
                            ,new ListValMeta(new ArrayList {"Ports","list", 0,1000}, new StructValMeta("PortsListElem", contactInfoStruct))
                            ,new ListValMeta(new ArrayList {"RecursiveList","list", 0,1000}, 
                                new ListValMeta(
                                    new ListValMeta(
                                        new ListValMeta(
                                            new ListValMeta(
                                                //new ListValMeta(
                                                    new ListValMeta(new ArrayList {"LastLevel","list", 0,1000}, new StructValMeta("PortsListElem", contactInfoStruct))
                                                //)
                                            )
                                        )
                                    )
                                )
                            )
                        });
            //      second index ClassInfo values
            gm.AddValuesMeta(2, new List<ValueMeta>() { 
                            new IntValMeta(new ArrayList(){"Count","Integer", 0, 100500, 0})
                        });
            //      third index ShipInfo values
            gm.AddValuesMeta(3, new List<ValueMeta>(){
                            new StructValMeta("Captain", contactInfoStruct)
                            ,new IntValMeta(new ArrayList(){"StuffCount","Integer",1,5000,1})
                            ,new DoubleValMeta(new ArrayList(){"Efficienty","Double",0,1,0}) 
                        });
            //
            //
            TrueNodeReference t = new TrueNodeReference(conn, gm.GlobalMetaName);
            t.Kill();
            //
            //Saving meta
            MetaReaderWriter w = new MetaReaderWriter(conn);
            w.SaveMeta(gm);
            GlobalMeta tryRead = w.GetMeta(gm.GlobalMetaName);
            tryRead.GetLocalStructs()[1].elementsMeta[0] 
                = new StringValMeta(new ArrayList() { "OLOLO", "String", 0, 255, "FUCKYEAH!" });
            //
            //Generating context file
            string appPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\") + 1);
            ContextGenerator gen = new ContextGenerator(gm, gm.GlobalName, gm.GlobalName + "Namespace", appPath + gm.GlobalName + "ContextBackup.cs");
            gen.GenerateCSharpCode();
        }

        static void fillWowsGlobal()
        {
            WowsCrudContext wows = new WowsCrudContext(conn);
            //
            ShipInfoProxy ship = new ShipInfoProxy();
            ship.Country = "German";
            ship.Classification.ClassType = "Kruiser";
            ship.Classification.Rank = 4;
            ship.Name = "Karlsruhe";
            ship.Captain.Name = "SorryCap";
            ship.StuffCount = 450;
            ship.Efficienty = 0.3;
            wows.ShipInfoManager.Save(ship);
            //
            ship.Country = "America";
            ship.Classification.ClassType = "Carryer";
            ship.Classification.Rank = 4;
            ship.Name = "Bogue";
            ship.Captain.Name = "Captain Carry";
            ship.StuffCount = 1104;
            ship.Efficienty = 0.66;
            wows.ShipInfoManager.Save(ship);
            //
            ship.Country = "America";
            ship.Classification.ClassType = "Destroyer";
            ship.Classification.Rank = 6;
            ship.Name = "Faragut";
            ship.Captain.Name = "Captain Japan";
            ship.StuffCount = 324;
            ship.Efficienty = 0.7;
            wows.ShipInfoManager.Save(ship);
            //
            ship.Country = "Japan";
            ship.Classification.ClassType = "Battleship";
            ship.Classification.Rank = 10;
            ship.Name = "Yamato";
            ship.Captain.Name = "YamatoCaptain";
            ship.StuffCount = 1250;
            ship.Efficienty = 0.55;
            wows.ShipInfoManager.Save(ship);
            //
            ship.Country = "Japan";
            ship.Classification.ClassType = "Destroyer";
            ship.Classification.Rank = 5;
            ship.Name = "Minekaze";
            ship.Captain.Name = "Captain America";
            ship.StuffCount = 250;
            ship.Efficienty = 0.73;
            wows.ShipInfoManager.Save(ship);
            //
            ship.Country = "Россия";
            ship.Classification.ClassType = "Destroyer";
            ship.Classification.Rank = 5;
            ship.Name = "Гремящий";
            ship.Captain.Name = "Андрей";
            ship.StuffCount = 224;
            ship.Efficienty = 0.8;
            wows.ShipInfoManager.Save(ship);
            //
            ship.Country = "Россия";
            ship.Classification.ClassType = "Destroyer";
            ship.Classification.Rank = 5;
            ship.Name = "Гневный";
            ship.Captain.Name = "Гневный";
            ship.StuffCount = 185;
            ship.Efficienty = 0.67;
            wows.ShipInfoManager.Save(ship);
            //
            ManufacturerProxy manufacter = new ManufacturerProxy();
            manufacter.Country = "Great Britan";
            manufacter.Charge = new Contact(Name: "D. Cameron", Phone: 5);
            manufacter.Ports = new List<Contact> {
                new Contact("Portsmun", 6)
                , new Contact("Dartford",7) 
            };
            try
            {
                wows.ManufacturerManager.Save(manufacter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //
            manufacter.Country = "Great Britan";
            manufacter.Charge = new Contact(Name: "D. Cameron", Phone: 1112233);
            manufacter.Ports = new List<Contact> {
                new Contact("Portsmun", 2223344)
                , new Contact("Dartford",3334455) 
            };
            try
            {
                wows.ManufacturerManager.Validate = false;

                manufacter.RecursiveList = new List<List<List<List<List<List<Contact>>>>>>();
                manufacter.RecursiveList.Add(new List<List<List<List<List<Contact>>>>>());
                manufacter.RecursiveList[0].Add(new List<List<List<List<Contact>>>>());
                manufacter.RecursiveList[0][0].Add(new List<List<List<Contact>>>());
                manufacter.RecursiveList[0][0][0].Add(new List<List<Contact>>());
                manufacter.RecursiveList[0][0][0].Add(new List<List<Contact>>());
                manufacter.RecursiveList[0][0][0][0].Add(new List<Contact>());
                manufacter.RecursiveList[0][0][0][0][0].Add(new Contact("fork1", 1111111));
                manufacter.RecursiveList[0][0][0][1].Add(new List<Contact>());
                manufacter.RecursiveList[0][0][0][1].Add(new List<Contact>());
                manufacter.RecursiveList[0][0][0][1][0].Add(new Contact("fork2_1", 2222111));
                manufacter.RecursiveList[0][0][0][1][0].Add(new Contact("fork2_2", 2222222));
                manufacter.RecursiveList[0][0][0][1].Add(new List<Contact>());
                manufacter.RecursiveList[0][0][0][1][1].Add(new Contact("inreclist",6666666));
                wows.ManufacturerManager.Save(manufacter);

                wows.ManufacturerManager.Validate = true;
                ManufacturerProxy readed = wows.ManufacturerManager.Get(manufacter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}