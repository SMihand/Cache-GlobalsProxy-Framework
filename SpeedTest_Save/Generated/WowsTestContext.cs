//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:2.0.50727.8669
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Generated
{
    using CacheEXTREME2.WProxyGlobal;
    using CacheEXTREME2.WMetaGlobal;
    using InterSystems.Globals;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    
    public class WowsTestContext : CacheEXTREME2.WProxyGlobal.CacheEXTREMEcontext
    {
        
        public ProxyManager<classinfoProxy, classinfoProxyKey> classinfoManager;
        
        public ProxyManager<stuffProxy, stuffProxyKey> stuffManager;
        
        public ProxyManager<ShipInfoProxy, ShipInfoProxyKey> ShipInfoManager;
        
        public ProxyManager<TestEntityProxy, TestEntityProxyKey> TestEntityManager;
        
        public WowsTestContext(InterSystems.Globals.Connection conn , string global = "wowsTest") : 
                base(conn, global, "wowsTestMeta")
        {
            base.structsManagers.Add(new StructManager<Classification>(base.globalMeta.GetStructDefinition("Classification"), base.structsManagers, base.globalRef.Conn));
            base.structsManagers.Add(new StructManager<Contact>(base.globalMeta.GetStructDefinition("Contact"), base.structsManagers, base.globalRef.Conn));
            this.classinfoManager = new ProxyManager<classinfoProxy, classinfoProxyKey>(base.entitiesMeta[typeof(classinfoProxy).Name], base.globalRef, base.structsManagers);
            this.stuffManager = new ProxyManager<stuffProxy, stuffProxyKey>(base.entitiesMeta[typeof(stuffProxy).Name], base.globalRef, base.structsManagers);
            this.ShipInfoManager = new ProxyManager<ShipInfoProxy, ShipInfoProxyKey>(base.entitiesMeta[typeof(ShipInfoProxy).Name], base.globalRef, base.structsManagers);
            this.TestEntityManager = new ProxyManager<TestEntityProxy, TestEntityProxyKey>(base.entitiesMeta[typeof(TestEntityProxy).Name], base.globalRef, base.structsManagers);
        }
    }
    
    public class classinfoProxy
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        // name: shipsCount(Int32), min: 0, max: 1000, def: 0
        public Int32 shipsCount;
        
        public classinfoProxy(String shipclass, Int32 shipsCount)
        {
            this.shipclass = shipclass;
            this.shipsCount = shipsCount;
        }
        
        public classinfoProxy()
        {
            this.shipclass = "";
            this.shipsCount = 0;
        }
        
        public virtual bool classinfoValidator(classinfoProxy entity)
        {
            return true;
        }
    }
    
    public class classinfoProxyKey
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        public classinfoProxyKey(String shipclass)
        {
            this.shipclass = shipclass;
        }
        
        public classinfoProxyKey()
        {
            this.shipclass = "";
        }
    }
    
    public class stuffProxy
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        // name: shiprank(Int32), min: 0, max: 1005000, def: 5
        public Int32? shiprank;
        
        // name: workers(List<name: (String), min: 0, max: 255, def: 0>), min: 0, max: 100
        public List<String> workers;
        
        // name: officers(List<name: (String), min: 0, max: 255, def: 0>), min: 0, max: 100
        public List<String> officers;
        
        // name: workerSalary(Int32), min: 1, max: 2000, def: 0
        public Int32 workerSalary;
        
        // name: photo(byte[]), max: 200000
        public Byte[] photo;
        
        public stuffProxy(String shipclass, Int32? shiprank, List<String> workers, List<String> officers, Int32 workerSalary, Byte[] photo)
        {
            this.shipclass = shipclass;
            this.shiprank = shiprank;
            this.workers = workers;
            this.officers = officers;
            this.workerSalary = workerSalary;
            this.photo = photo;
        }
        
        public stuffProxy()
        {
            this.shipclass = "";
            this.shiprank = 5;
            this.workers = new List<String>();
            this.officers = new List<String>();
            this.workerSalary = 0;
            this.photo = null;
        }
        
        public virtual bool stuffValidator(stuffProxy entity)
        {
            return true;
        }
    }
    
    public class stuffProxyKey
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        // name: shiprank(Int32), min: 0, max: 1005000, def: 5
        public Int32? shiprank;
        
        public stuffProxyKey(String shipclass, Int32? shiprank)
        {
            this.shipclass = shipclass;
            this.shiprank = shiprank;
        }
        
        public stuffProxyKey()
        {
            this.shipclass = "";
            this.shiprank = 5;
        }
    }
    
    public class ShipInfoProxy
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        // name: shiprank(Int32), min: 0, max: 1005000, def: 5
        public Int32? shiprank;
        
        // name: Classification(Classification)
        public Classification Classification;
        
        // name: Name(String), min: 0, max: 255, def: ShipName
        public String Name;
        
        // name: captains(List<name: captainContact(Contact)>), min: 0, max: 100
        public List<Contact> captains;
        
        // name: workers(List<name: (String), min: 0, max: 255, def: 0>), min: 0, max: 100
        public List<String> workers;
        
        // name: officers(List<name: (String), min: 0, max: 255, def: 0>), min: 0, max: 100
        public List<String> officers;
        
        // name: workerSalary(Int32), min: 1, max: 2000, def: 0
        public Int32 workerSalary;
        
        // name: photo(byte[]), max: 200000
        public Byte[] photo;
        
        public ShipInfoProxy(String shipclass, Int32? shiprank, Classification Classification, String Name, List<Contact> captains, List<String> workers, List<String> officers, Int32 workerSalary, Byte[] photo)
        {
            this.shipclass = shipclass;
            this.shiprank = shiprank;
            this.Classification = Classification;
            this.Name = Name;
            this.captains = captains;
            this.workers = workers;
            this.officers = officers;
            this.workerSalary = workerSalary;
            this.photo = photo;
        }
        
        public ShipInfoProxy()
        {
            this.shipclass = "";
            this.shiprank = 5;
            this.Classification = new Classification();
            this.Name = "ShipName";
            this.captains = new List<Contact>();
            this.workers = new List<String>();
            this.officers = new List<String>();
            this.workerSalary = 0;
            this.photo = null;
        }
        
        public virtual bool ShipInfoValidator(ShipInfoProxy entity)
        {
            return true;
        }
    }
    
    public class ShipInfoProxyKey
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        // name: shiprank(Int32), min: 0, max: 1005000, def: 5
        public Int32? shiprank;
        
        // name: Classification(Classification)
        public Classification Classification;
        
        // name: Name(String), min: 0, max: 255, def: ShipName
        public String Name;
        
        public ShipInfoProxyKey(String shipclass, Int32? shiprank, Classification Classification, String Name)
        {
            this.shipclass = shipclass;
            this.shiprank = shiprank;
            this.Classification = Classification;
            this.Name = Name;
        }
        
        public ShipInfoProxyKey()
        {
            this.shipclass = "";
            this.shiprank = 5;
            this.Classification = new Classification();
            this.Name = "ShipName";
        }
    }
    
    public class TestEntityProxy
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        // name: shiprank(Int32), min: 0, max: 1005000, def: 5
        public Int32? shiprank;
        
        // name: Classification(Classification)
        public Classification Classification;
        
        // name: Name(String), min: 0, max: 255, def: ShipName
        public String Name;
        
        // name: SubClassification(Classification)
        public Classification SubClassification;
        
        // name: TestIntVal(Int32), min: 0, max: 1005000, def: 5
        public Int32 TestIntVal;
        
        public TestEntityProxy(String shipclass, Int32? shiprank, Classification Classification, String Name, Classification SubClassification, Int32 TestIntVal)
        {
            this.shipclass = shipclass;
            this.shiprank = shiprank;
            this.Classification = Classification;
            this.Name = Name;
            this.SubClassification = SubClassification;
            this.TestIntVal = TestIntVal;
        }
        
        public TestEntityProxy()
        {
            this.shipclass = "";
            this.shiprank = 5;
            this.Classification = new Classification();
            this.Name = "ShipName";
            this.SubClassification = new Classification();
            this.TestIntVal = 5;
        }
        
        public virtual bool TestEntityValidator(TestEntityProxy entity)
        {
            return true;
        }
    }
    
    public class TestEntityProxyKey
    {
        
        // name: shipclass(String), min: 0, max: 255, def: 
        public String shipclass;
        
        // name: shiprank(Int32), min: 0, max: 1005000, def: 5
        public Int32? shiprank;
        
        // name: Classification(Classification)
        public Classification Classification;
        
        // name: Name(String), min: 0, max: 255, def: ShipName
        public String Name;
        
        // name: SubClassification(Classification)
        public Classification SubClassification;
        
        public TestEntityProxyKey(String shipclass, Int32? shiprank, Classification Classification, String Name, Classification SubClassification)
        {
            this.shipclass = shipclass;
            this.shiprank = shiprank;
            this.Classification = Classification;
            this.Name = Name;
            this.SubClassification = SubClassification;
        }
        
        public TestEntityProxyKey()
        {
            this.shipclass = "";
            this.shiprank = 5;
            this.Classification = new Classification();
            this.Name = "ShipName";
            this.SubClassification = new Classification();
        }
    }
    
    public class Classification
    {
        
        // name: ClassType(String), min: 0, max: 255, def: ClassType
        public String ClassType;
        
        // name: Rank(Int32), min: 0, max: 1005000, def: 0
        public Int32? Rank;
        
        public Classification()
        {
            this.ClassType = "ClassType";
            this.Rank = 0;
        }
        
        public Classification(String ClassType, Int32? Rank)
        {
            this.ClassType = ClassType;
            this.Rank = Rank;
        }
    }
    
    public class Contact
    {
        
        // name: Name(String), min: 0, max: 255, def: Name
        public String Name;
        
        // name: Phone(Int32), min: 111111, max: 99999999, def: 111111
        public Int32? Phone;
        
        public Contact()
        {
            this.Name = "Name";
            this.Phone = 111111;
        }
        
        public Contact(String Name, Int32? Phone)
        {
            this.Name = Name;
            this.Phone = Phone;
        }
    }
}
