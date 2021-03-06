﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SSDTDevPack.Merge.MergeDescriptor;

namespace SSDTDevPack.Common.Dac
{
    public static class ObjectIdentifierExtensions
    {
        public static string GetName(this ObjectIdentifier name)
        {
            return name.Parts.LastOrDefault();
        }

        public static string GetSchema(this ObjectIdentifier name)
        {
            if (name.Parts.Count == 3)
            {
                return name.Parts[1];
            }

            if (name.Parts.Count == 2)
            {
                return name.Parts[0];
            }

            return null;
        }

        public static string GetSchemaObjectName(this ObjectIdentifier name)
        {
            return string.Format("{0}.{1}", name.GetSchema(), name.GetName());
        }

        public static bool EqualsName(this ObjectIdentifier source, SchemaObjectName target)
        {
            if (target.SchemaIdentifier == null)
                return Quote.Name(source.GetName()) == Quote.Name(target.BaseIdentifier.Value);

            return Quote.Name(source.GetSchema()) == Quote.Name(target.SchemaIdentifier.Value) && Quote.Name(source.GetName()) == Quote.Name(target.BaseIdentifier.Value);
        }

        public static Identifier ToIdentifier(this ObjectIdentifier source)
        {
            var name = source.GetName();

            return new Identifier()
            {
                Value = name.Quote() 
            };
        }

        public static SchemaObjectName ToSchemaObjectName(this ObjectIdentifier source)
        {
            var target = new SchemaObjectName();
            target.Identifiers.Add(source.GetSchema().ToScriptDomIdentifier().Quote());
            target.Identifiers.Add(source.GetName().ToScriptDomIdentifier().Quote());
            
            return target;
        }

    }

    public static class StringExtensions
    {
        public static Identifier ToScriptDomIdentifier(this string source)
        {
            return new Identifier()
            {
                Value = source
            };
        }

        public static string Quote(this string source)
        {
            if (!source.StartsWith("["))
                source = "[" + source;

            if (!source.EndsWith("]"))
                source = source + "]";

            return source;

        }
    }

    public static class IdentifierExtensions
    {
        public static Identifier Quote(this Identifier src)
        {
            src.Value = src.Value.Quote();
            return src;
        }
    }

    public static class SchemaObjectNameExtensions
    {
        public static ObjectIdentifier ToObjectIdentifier(this SchemaObjectName source)
        {
            return new ObjectIdentifier(source.SchemaIdentifier.Value, source.BaseIdentifier.Value);
        }

        public static Identifier ToIdentifier(this SchemaObjectName source)
        {
            return source.BaseIdentifier;
        }
    }
}
