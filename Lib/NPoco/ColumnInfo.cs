﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NPoco
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string ColumnAlias { get; set; }
        public bool ResultColumn { get; set; }
        public bool ComputedColumn { get; set; }
        public ComputedColumnType ComputedColumnType { get; set; }
        public bool IgnoreColumn { get; set; }
        public bool VersionColumn { get; set; }
        public VersionColumnType VersionColumnType { get; set; }
        public bool ForceToUtc { get; set; } = true;
        public Type ColumnType { get; set; }
        public bool ComplexMapping { get; set; }
        public bool ValueObjectColumn { get; set; }
        public string ComplexPrefix { get; set; }
        public bool SerializedColumn { get; set; }
        public ReferenceType ReferenceType { get; set; }
        public string ReferenceMemberName { get; set; }
        public MemberInfo MemberInfo { get; internal set; }
        public bool ExactColumnNameMatch { get; set; }

        public static ColumnInfo FromMemberInfo(MemberInfo mi)
        {
            var ci = new ColumnInfo{MemberInfo = mi};
            var attrs = ReflectionUtils.GetCustomAttributes(mi).ToArray();
            var colAttrs = attrs.OfType<ColumnAttribute>().ToArray();
            var columnTypeAttrs = attrs.OfType<ColumnTypeAttribute>().ToArray();
            var ignoreAttrs = attrs.OfType<IgnoreAttribute>().ToArray();
            var complexMapping = attrs.OfType<ComplexMappingAttribute>().ToArray();
            var serializedColumnAttributes = attrs.OfType<SerializedColumnAttribute>().ToArray();
            var reference = attrs.OfType<ReferenceAttribute>().ToArray();
            var aliasColumn = attrs.OfType<AliasAttribute>().FirstOrDefault();

            // Check if declaring poco has [ExplicitColumns] attribute
            var explicitColumns = mi.DeclaringType.GetTypeInfo().GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Any();

            //check if able to write to property;
            var isReadOnly = false;
            if (mi is PropertyInfo pi)
            {
                if (!pi.CanWrite) isReadOnly = true;
                if (pi.PropertyType!=typeof(string) && !pi.PropertyType.IsValueType && !pi.PropertyType.IsArray) {
                    isReadOnly = true;
                }
            }

            // Ignore column if declarying poco has [ExplicitColumns] attribute
            // and property doesn't have an explicit [Column] attribute,
            // or property has an [Ignore] attribute
            if ((explicitColumns && !colAttrs.Any() && !reference.Any() && !complexMapping.Any()) || ignoreAttrs.Any() || isReadOnly)
            {
                ci.IgnoreColumn = true;
            }

            var complexMappingAttribute = mi.GetMemberInfoType().GetCustomAttribute<ComplexMappingAttribute>();

            if (complexMapping.Any())
            {
                ci.ComplexMapping = complexMapping.First().ComplexMapping;
                ci.ComplexPrefix = complexMapping.First().CustomPrefix;
            }
            else if (complexMappingAttribute != null)
            {
                ci.ComplexMapping = complexMappingAttribute.ComplexMapping;
                ci.ComplexPrefix = complexMappingAttribute.CustomPrefix;
            }
            else if (mi.GetMemberInfoType().GetInterfaces().Any(x => x == typeof(IValueObject)))
            {
                ci.ValueObjectColumn = true;
            }
            else if (serializedColumnAttributes.Any())
            {
                ci.SerializedColumn = true;
            }
            else if (reference.Any())
            {
                ci.ReferenceType = reference.First().ReferenceType;
                ci.ReferenceMemberName = reference.First().ReferenceMemberName ?? "Id";
                ci.ColumnName = reference.First().ColumnName ?? mi.Name + "Id";
                return ci;
            }
            else if (PocoDataBuilder.IsList(mi))
            {
                ci.ReferenceType = ReferenceType.Many;
                return ci;
            }
            else if (mi.GetMemberInfoType().IsAClass() && !colAttrs.Any())
            {
                ci.ComplexMapping = true;
            }

            // Read attribute
            if (colAttrs.Any())
            {
                ci.ColumnName = colAttrs.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name))?.Name ?? mi.Name;
                ci.ForceToUtc = colAttrs.All(x => x.ForceToUtc);
                ci.ExactColumnNameMatch = colAttrs.All(x => x.ExactNameMatch);

                var resultAttr = colAttrs.OfType<ResultColumnAttribute>().FirstOrDefault();
                ci.ResultColumn = resultAttr != null;

                if (!ci.ResultColumn)
                {
                    var versionAttr = colAttrs.OfType<VersionColumnAttribute>().FirstOrDefault();
                    ci.VersionColumn = versionAttr != null;
                    ci.VersionColumnType = versionAttr?.VersionColumnType ?? ci.VersionColumnType;
                }

                if (!ci.VersionColumn && !ci.ResultColumn)
                {
                    var computedAttr = colAttrs.OfType<ComputedColumnAttribute>().FirstOrDefault();
                    ci.ComputedColumn = computedAttr != null;
                    ci.ComputedColumnType = computedAttr?.ComputedColumnType ?? ComputedColumnType.Always;
                }
            }
            else
            {
                ci.ColumnName = mi.Name;
            }

            ci.ColumnAlias = aliasColumn?.Alias;

            if (columnTypeAttrs.Any())
            {
                ci.ColumnType = columnTypeAttrs.First().Type;
            }

            return ci;
        }
    }
}
