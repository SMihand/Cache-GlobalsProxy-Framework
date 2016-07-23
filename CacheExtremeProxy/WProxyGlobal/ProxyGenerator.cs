using CacheEXTREME2.WMetaGlobal;
using InterSystems.Globals;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CacheEXTREME2.WProxyGlobal
{
    /// <summary>
    /// Generate new CodeTypeDeclaration of class declared as GlobalMeta
    /// </summary>
    public class EntityGenerator
    {
        private List<ValueMeta> keysMeta;
        private List<ValueMeta> valuesMeta;
        private string className;
        private EntityMeta entityMeta;
        //
        private CodeCompileUnit targetUnit;
        private CodeTypeDeclaration targetClass;
        private CodeTypeDeclaration targetKeyClass;
        //
        public EntityGenerator(GlobalMeta globalMeta, int classIndex,string _namespace)
        {
            keysMeta = globalMeta.GetKeysMeta().GetRange(0,classIndex);
            className = globalMeta[classIndex].Key+"Proxy";
            valuesMeta = globalMeta[classIndex].Value;
            //
            targetUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(_namespace);
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            //codeNamespace.Imports.Add(new CodeNamespaceImport("CacheEXTREME2"));
            targetClass = new CodeTypeDeclaration(className);
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;//| TypeAttributes.Sealed;
            targetKeyClass = new CodeTypeDeclaration(className + "Key");
            targetKeyClass.IsClass = true;
            targetKeyClass.TypeAttributes = TypeAttributes.Public;//| TypeAttributes.Sealed;
            codeNamespace.Types.Add(targetClass);
            codeNamespace.Types.Add(targetKeyClass);
            targetUnit.Namespaces.Add(codeNamespace);
            main();
        }
        public EntityGenerator(EntityMeta entityMeta, string _namespace)
        {
            this.entityMeta = entityMeta;
            keysMeta = entityMeta.KyesMeta;
            className = entityMeta.EntityName + "Proxy";
            valuesMeta = entityMeta.ValuesMeta;
            //
            targetUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(_namespace);
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            //codeNamespace.Imports.Add(new CodeNamespaceImport("CacheEXTREME2"));
            targetClass = new CodeTypeDeclaration(className);
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;//| TypeAttributes.Sealed;
            targetKeyClass = new CodeTypeDeclaration(className + "Key");
            targetKeyClass.IsClass = true;
            targetKeyClass.TypeAttributes = TypeAttributes.Public;//| TypeAttributes.Sealed;
            codeNamespace.Types.Add(targetClass);
            codeNamespace.Types.Add(targetKeyClass);
            targetUnit.Namespaces.Add(codeNamespace);
            main();
        }
        //
        private void main()
        {
            AddProxyFields();
            AddProxyFullConstructor();
            AddProxyDefaultConstructor();
            //AddFullDefaultConstructor();
            //
            AddProxyKeyFields();
            AddProxyKeyConstructor();
            AddProxyKeyDefaultConstructor();
            AddValidationMethod();
        }
        //
        public void AddProxyFields()
        {
            for (int i = 0; i < keysMeta.Count; i++)
            {
                ValueMeta fieldMeta = keysMeta[i];
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Public;
                field.Name = fieldMeta.SemanticName;
                field.Comments.Add(new CodeCommentStatement(fieldMeta.ToString()));
                //
                string typeName = fieldMeta.GetCSharpTypeName();
                switch (fieldMeta.ExtremeType){
                    case ExtremeTypes.EXTREME_STRING:
                    {
                        field.Type = new CodeTypeReference(typeName);
                        break;
                    }
                    case ExtremeTypes.EXTREME_STRUCT:
                    {
                        field.Type = new CodeTypeReference(typeName);
                        break;
                    }
                    default:
                    {
                        field.Type = new CodeTypeReference(typeName + "?");
                        break;
                    }
                }
                targetClass.Members.Add(field);
            }
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                ValueMeta fieldMeta = valuesMeta[i];
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Public;
                field.Name = fieldMeta.SemanticName;
                string typeName = fieldMeta.GetCSharpTypeName();
                field.Type = new CodeTypeReference(typeName);
                field.Comments.Add(new CodeCommentStatement(fieldMeta.ToString()));
                targetClass.Members.Add(field);
            }
        }
        public void AddProxyKeyFields()
        {
            for (int i = 0; i < keysMeta.Count; i++)
            {
                ValueMeta fieldMeta = keysMeta[i];
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Public;
                field.Name = fieldMeta.SemanticName;
                string typeName = fieldMeta.GetCSharpTypeName();
                field.Type = new CodeTypeReference(typeName);
                field.Comments.Add(new CodeCommentStatement(fieldMeta.ToString()));
                //
                switch (fieldMeta.ExtremeType){
                    case ExtremeTypes.EXTREME_STRING:
                    {
                        field.Type = new CodeTypeReference(typeName);
                        break;
                    }
                    case ExtremeTypes.EXTREME_STRUCT:
                    {
                        field.Type = new CodeTypeReference(typeName);
                        break;
                    }
                    default:
                    {
                        field.Type = new CodeTypeReference(typeName + "?");
                        break;
                    }
                }
                targetKeyClass.Members.Add(field);
            }
        }
        //
        public void AddProxyFullConstructor()
        {
            // Declare the constructor
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            // Add parameters.
            for (int i = 0; i < keysMeta.Count; i++)
            {
                ValueMeta valMeta = keysMeta[i];
                string keyTypeName = valMeta.GetCSharpTypeName();
                //Parameter declaration
                CodeParameterDeclarationExpression paramDeclaration;
                switch (valMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRING:
                    {
                        paramDeclaration = new CodeParameterDeclarationExpression(keyTypeName, valMeta.SemanticName);
                        break;
                    }
                    case ExtremeTypes.EXTREME_STRUCT:
                    {
                        paramDeclaration = new CodeParameterDeclarationExpression(keyTypeName, valMeta.SemanticName);
                        break;
                    }
                    default:
                    {
                        paramDeclaration = new CodeParameterDeclarationExpression(keyTypeName+"?", valMeta.SemanticName);
                        break;
                    }
                }
                // Add field initialization logic
                // -> this.field
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), valMeta.SemanticName);
                // -> valueRef(this.field)=valMeta.Name
                CodeAssignStatement argToField = new CodeAssignStatement(valueRef
                    , new CodeArgumentReferenceExpression(valMeta.SemanticName));
                //
                constructor.Parameters.Add(paramDeclaration);
                constructor.Statements.Add(argToField);
            }
            //
            // Add parameters.
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                ValueMeta valMeta = valuesMeta[i];
                string typeName = valMeta.GetCSharpTypeName();
                constructor.Parameters.Add(new CodeParameterDeclarationExpression(
                    typeName, valMeta.SemanticName)
                );
                /*constructor.Parameters.Add(new CodeParameterDeclarationExpression(
                   getValueMetaType(valMeta), valMeta.Name));*/

                // Add field initialization logic
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), valMeta.SemanticName);
                constructor.Statements.Add(
                    new CodeAssignStatement(
                    valueRef, new CodeArgumentReferenceExpression(valMeta.SemanticName)));
            }
            targetClass.Members.Add(constructor);
        }
        public void AddProxyDefaultConstructor()
        {
            // Declare the constructor
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            // Add statements.
            for (int i = 0; i < keysMeta.Count; i++)
            {
                ValueMeta keyMeta = keysMeta[i];
                string valTypeName = keyMeta.GetCSharpTypeName();
                // Add field initialization logic
                // -> this.field
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), keyMeta.SemanticName);
                string keyTypeName = keyMeta.GetCSharpTypeName();
                if (keyMeta.ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                {
                    constructor.Statements.Add(new CodeAssignStatement(
                        valueRef
                        , new CodeObjectCreateExpression(keyTypeName))
                    );
                    continue;
                }
                // -> valueRef(this.field)=valMeta.GetDefaultValue()
                CodeAssignStatement argToField = new CodeAssignStatement(valueRef
                    , new CodePrimitiveExpression(keyMeta.GetDefaultValue()));
                //
                constructor.Statements.Add(argToField);
            }
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                ValueMeta valMeta = valuesMeta[i];
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), valMeta.SemanticName);
                string valTypeName = valMeta.GetCSharpTypeName();
                if (valMeta.ExtremeType == ExtremeTypes.EXTREME_LIST
                    || valMeta.ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                {
                    constructor.Statements.Add(new CodeAssignStatement(
                        valueRef
                        , new CodeObjectCreateExpression(valTypeName))
                    );
                    continue;
                }
                constructor.Statements.Add(
                        new CodeAssignStatement(
                            valueRef
                            , new CodePrimitiveExpression(valMeta.GetDefaultValue())
                        )
                    );
            }
            targetClass.Members.Add(constructor);
        }
        public void AddProxyKeyConstructor()
        {
            // Declare the constructor
            CodeConstructor keyConstructor = new CodeConstructor();
            keyConstructor.Attributes = MemberAttributes.Public;
            // Add parameters.
            for (int i = 0; i < keysMeta.Count; i++)
            {
                ValueMeta valMeta = keysMeta[i];
                string keyTypeName = valMeta.GetCSharpTypeName();
                //Parameter declaration
                CodeParameterDeclarationExpression paramDeclaration;
                switch (valMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRING:
                    {
                        paramDeclaration = new CodeParameterDeclarationExpression(keyTypeName, valMeta.SemanticName);
                        break;
                    }
                    case ExtremeTypes.EXTREME_STRUCT:
                    {
                        paramDeclaration = new CodeParameterDeclarationExpression(keyTypeName, valMeta.SemanticName);
                        break;
                    }
                    default:
                    {
                        paramDeclaration = new CodeParameterDeclarationExpression(keyTypeName + "?", valMeta.SemanticName);
                        break;
                    }
                }
                // Add field initialization logic
                // -> this.field
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), valMeta.SemanticName);
                // -> valueRef(this.field)=valMeta.Name
                CodeAssignStatement argToField = new CodeAssignStatement(valueRef
                    , new CodeArgumentReferenceExpression(valMeta.SemanticName));
                //
                keyConstructor.Parameters.Add(paramDeclaration);
                keyConstructor.Statements.Add(argToField);
            }
            targetKeyClass.Members.Add(keyConstructor);
        }
        public void AddProxyKeyDefaultConstructor()
        {
            // Declare the constructor
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            // Add statements.
            for (int i = 0; i < keysMeta.Count; i++)
            {
                ValueMeta keyMeta = keysMeta[i];
                string valTypeName = keyMeta.GetCSharpTypeName();
                // Add field initialization logic
                // -> this.field
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), keyMeta.SemanticName);
                string keyTypeName = keyMeta.GetCSharpTypeName();
                if (keyMeta.ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                {
                    constructor.Statements.Add(new CodeAssignStatement(
                        valueRef
                        , new CodeObjectCreateExpression(keyTypeName))
                    );
                    continue;
                }
                // -> valueRef(this.field)=valMeta.GetDefaultValue()
                CodeAssignStatement argToField = new CodeAssignStatement(valueRef
                    , new CodePrimitiveExpression(keyMeta.GetDefaultValue()));
                //
                constructor.Statements.Add(argToField);
            }
            this.targetKeyClass.Members.Add(constructor);
        }
        //
        public void AddValidationMethod()
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = this.entityMeta.EntityName + "Validator";
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(true)));
            method.Parameters.Add(new CodeParameterDeclarationExpression(className, "entity"));
            targetClass.Members.Add(method);
        }
        //
        public CodeTypeDeclaration GetDeclaration()
        {
            return targetClass;
        }
        public CodeTypeDeclaration GetKeyDeclaration()
        {
            return targetKeyClass;
        }
        public void GenerateCSharpCode(string fileName)
        {
            GetDeclaration();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sourceWriter, "    ");

                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);

                tw.Close();
            }
        }
    }


    public class StructGenerator
    {
        private StructValMeta structMeta;
        //
        private CodeTypeDeclaration structDeclaration;
        //
        public StructGenerator()
        {
        }
        //
        public CodeTypeDeclaration GetDeclaration(StructValMeta structMeta)
        {
            this.structMeta = structMeta;
            //
            structDeclaration = new CodeTypeDeclaration();
            structDeclaration.IsClass = true;
            structDeclaration.Attributes = MemberAttributes.Public;
            structDeclaration.Name = structMeta.StructTypeName;
            //
            main();
            return structDeclaration;
        }
        //
        private void main()
        {
            addFields();
            AddDefaultConstructor();
            addFullConstructor();
        }
        //
        private void addFields()
        {
            for (int i = 0; i < structMeta.elementsMeta.Count; i++)
            {
                ValueMeta fieldMeta = structMeta.elementsMeta[i];
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Public;
                field.Name = fieldMeta.SemanticName;
                string typeName = fieldMeta.GetCSharpTypeName();
                field.Type = new CodeTypeReference(typeName);
                field.Comments.Add(new CodeCommentStatement(fieldMeta.ToString()));
                switch (fieldMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRING:
                        {
                            field.Type = new CodeTypeReference(typeName);
                            break;
                        }
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            field.Type = new CodeTypeReference(typeName);
                            break;
                        }
                    case ExtremeTypes.EXTREME_LIST:
                        {
                            field.Type = new CodeTypeReference(typeName);
                            break;
                        }
                    default:
                        {
                            field.Type = new CodeTypeReference(typeName + "?");
                            break;
                        }
                }
                structDeclaration.Members.Add(field);
            }
        }
        private void addFullConstructor()
        {
             // Declare the constructor
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            
            for (int i = 0; i < structMeta.elementsMeta.Count; i++)
            {
                ValueMeta valMeta = structMeta.elementsMeta[i];
                string typeName = valMeta.GetCSharpTypeName();
                //Parameter declaration
                CodeParameterDeclarationExpression paramDeclaration;
                switch (valMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRING:
                        {
                            paramDeclaration = new CodeParameterDeclarationExpression(typeName, valMeta.SemanticName);
                            break;
                        }
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            paramDeclaration = new CodeParameterDeclarationExpression(typeName, valMeta.SemanticName);
                            break;
                        }
                    case ExtremeTypes.EXTREME_LIST:
                        {
                            paramDeclaration = new CodeParameterDeclarationExpression(typeName, valMeta.SemanticName);
                            break;
                        }
                    default:
                        {
                            paramDeclaration = new CodeParameterDeclarationExpression(typeName + "?", valMeta.SemanticName);
                            break;
                        }
                }
                // Add field initialization logic
                // -> this.field
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), valMeta.SemanticName);
                // -> valueRef(this.field)=valMeta.Name
                CodeAssignStatement argToField = new CodeAssignStatement(valueRef
                    , new CodeArgumentReferenceExpression(valMeta.SemanticName));
                //
                constructor.Parameters.Add(paramDeclaration);
                constructor.Statements.Add(argToField);
            }
            structDeclaration.Members.Add(constructor);
        }
        private void AddDefaultConstructor()
        {
            // Declare the constructor
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            // Add statements.
            for (int i = 0; i < structMeta.elementsMeta.Count; i++)
            {
                ValueMeta valMeta = structMeta.elementsMeta[i];
                CodeFieldReferenceExpression valueRef = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), valMeta.SemanticName);
                string valTypeName = valMeta.GetCSharpTypeName();
                if (valMeta.ExtremeType == ExtremeTypes.EXTREME_LIST 
                    || valMeta.ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                {
                    constructor.Statements.Add(new CodeAssignStatement(
                        valueRef
                        , new CodeObjectCreateExpression(valTypeName))
                    );
                    continue;
                }
                constructor.Statements.Add(
                        new CodeAssignStatement(
                            valueRef
                            , new CodePrimitiveExpression(valMeta.GetDefaultValue())
                        )
                    );
            }
            structDeclaration.Members.Add(constructor);
        }
    }


    public class EntitiesGenerator
    {
        private CodeCompileUnit targetUnit;
        //
        CodeNamespace codeNamespace;
        //
        private string fileName;

        private GlobalMeta globalMeta;

        private List<CodeTypeDeclaration> targetEntities;
        //
        public EntitiesGenerator(GlobalMeta globalMeta, string namespaceName, string fileName)
        {
            this.globalMeta = globalMeta;
            this.fileName = fileName;
            targetUnit = new CodeCompileUnit();
            codeNamespace = new CodeNamespace(namespaceName);
        }
        public EntitiesGenerator(GlobalMeta globalMeta)
            : this(globalMeta, "", "")
        {
            this.codeNamespace.Name = globalMeta.GlobalName;
            this.fileName = globalMeta.GlobalName;
        }

        void main()
        {
            setUsings();
            this.targetEntities = getEntitiesDeclaration();
            codeNamespace.Types.AddRange(targetEntities.ToArray());
            targetUnit.Namespaces.Add(codeNamespace);
        }
        //
        private void setUsings(){
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
        }
        public List<CodeTypeDeclaration> getEntitiesDeclaration()
        {
            List<CodeTypeDeclaration> entitiesDeclaration = new List<CodeTypeDeclaration>();
            for (int i = 1; i <= globalMeta.KeysCount; i++)
            {
                if (globalMeta[i].Value.Count >= 1)
                {
                    EntityGenerator gen = new EntityGenerator(globalMeta.GetEntityMeta(i), "");
                    //EntityGenerator gen = new EntityGenerator(globalMeta, i, "");
                    entitiesDeclaration.Add(gen.GetDeclaration());
                    entitiesDeclaration.Add(gen.GetKeyDeclaration());
                }
            }
            StructGenerator structGen = new StructGenerator();
            foreach (StructValMeta structMeta in globalMeta.GetLocalStructs())
            {
                entitiesDeclaration.Add(structGen.GetDeclaration(structMeta));
            }
            return entitiesDeclaration;
        }
        public void GenerateCSharpCode()
        {
            main();
            //
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sourceWriter, "    ");

                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);

                tw.Close();
            }
        } 
    }


    public class ContextGenerator
    {
        private CodeCompileUnit targetUnit;
        //
        CodeNamespace codeNamespace;
        //
        private CodeTypeDeclaration targetClass;
        //
        private string fileName;
        public EntitiesGenerator enGen;

        private GlobalMeta globalMeta;
        private string dataGlobalName;

        private List<string> entities;

        private List<CodeTypeDeclaration> targetEntities;
        //
        public ContextGenerator(GlobalMeta globalMeta, string dataGlobalName, string namespaceName, string fileName)
        {
            this.globalMeta = globalMeta;
            this.dataGlobalName = dataGlobalName;
            this.fileName = fileName;
            targetUnit = new CodeCompileUnit();
            codeNamespace = new CodeNamespace(namespaceName);
            setUsings();
            //
            string firstLetter = dataGlobalName[0].ToString().ToUpper();
            string className = dataGlobalName.Substring(1, dataGlobalName.Length - 1) + "Context";
            className = firstLetter + className;
            targetClass = new CodeTypeDeclaration(className);
            targetClass.IsClass = true;
            targetClass.BaseTypes.Add(typeof(CacheEXTREMEcontext));
            //
            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);
        }
    
        private void setUsings()
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport("CacheEXTREME2.WProxyGlobal"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("CacheEXTREME2.WMetaGlobal"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("InterSystems.Globals"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
        }
        //
        public void AddContextFields()
        {
            entities = new List<string>();
            for (int i = 1; i <= globalMeta.KeysCount; i++)
            {
                if (globalMeta[i].Value.Count >= 1)
                {
                    entities.Add(globalMeta[i].Key);
                    string ProxyTypeName = globalMeta[i].Key + "Manager";
                    CodeMemberField field = new CodeMemberField();
                    field.Attributes = MemberAttributes.Public;
                    field.Name = ProxyTypeName;
                    /*field.Type = new CodeTypeReference(
                        "ProxyManager<" + globalMeta[i].Key + "Entity>"
                        );*/
                    field.Type = new CodeTypeReference("ProxyManager", new[] { new CodeTypeReference(globalMeta[i].Key + "Proxy"), new CodeTypeReference(globalMeta[i].Key + "ProxyKey") }); 
                    //
                    targetClass.Members.Add(field);
                }
            }
        }
        public void AddContextConstructor()
        {
            // Declare the constructor
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            //
            // Add Connection parameter.
            constructor.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(Connection), "conn "));
            // Add globalName parameter.
            constructor.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(string), "global = " + '"'+dataGlobalName +'"'));
            constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("conn"));
            constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("global"));
            constructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(globalMeta.GlobalMetaName));
            //
            CodeBaseReferenceExpression baseref = new CodeBaseReferenceExpression();
            //
            //Adding structManagers Initialization
            List<StructValMeta> structs = globalMeta.GetLocalStructs();
            for (int i = 0; i < structs.Count; i++)
            {
                CodeObjectCreateExpression structObjCreateCode
                    = new CodeObjectCreateExpression(
                        new CodeTypeReference("StructManager<" + structs[i].StructTypeName + ">")
                        , new CodeFieldReferenceExpression(baseref, "globalMeta.GetLocalStructs()[" + i + "]")
                        , new CodeFieldReferenceExpression(baseref, "structsManagers"));
                        //, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(baseref, "globalRef"),"Conn"));
                CodeMethodInvokeExpression codeInvokeExpression = new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(baseref, "structsManagers")
                        , "Add"
                        , structObjCreateCode);
                constructor.Statements.Add(codeInvokeExpression);
            }
            //
            //Init proxyManagers initialization
            foreach(string entity in entities){

                //= new ProxyManager<ProxyT>(entitiesMeta[typeof(ProxyT).Name], globalRef);
                CodeObjectCreateExpression newObjCode
                    = new CodeObjectCreateExpression(
                        new CodeTypeReference("ProxyManager", new[] { new CodeTypeReference(entity + "Proxy"), new CodeTypeReference(entity + "ProxyKey")})
                        , new CodeFieldReferenceExpression(baseref, "entitiesMeta[typeof(" + entity + "Proxy).Name]")
                        , new CodeFieldReferenceExpression(baseref, "globalRef")
                        , new CodeFieldReferenceExpression(baseref, "structsManagers")
                );

                //this.<Entity>Manager
                CodeFieldReferenceExpression managerRef 
                    = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), entity+"Manager");

                constructor.Statements.Add(new CodeAssignStatement(managerRef, newObjCode));
            }
            //
            targetClass.Members.Add(constructor);
        }
        //
        private void main()
        {
            AddContextFields();
            AddContextConstructor();
            enGen = new EntitiesGenerator(globalMeta);
            this.targetEntities = enGen.getEntitiesDeclaration();
            codeNamespace.Types.AddRange(new CodeTypeDeclarationCollection(this.targetEntities.ToArray()));
        }
        public void GenerateCSharpCode()
        {
            main();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sourceWriter, "    ");

                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);

                tw.Close();
            }
        }
    }
}
