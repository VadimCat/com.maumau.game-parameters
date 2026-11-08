using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MauMau.GameParameters
{
    /// <summary>Reusable native asset table. Games own the window, tabs and domain-specific adapters.</summary>
    public static class SerializedParameterTable
    {
        public static void Draw(UnityEngine.Object target,string search="",float labelWidth=280)
        {
            var serialized=new SerializedObject(target);serialized.Update();var property=serialized.GetIterator();
            var enter=true;
            while(property.NextVisible(enter))
            {
                enter=false;if(property.name=="m_Script")continue;
                if(!string.IsNullOrEmpty(search)&&property.displayName.IndexOf(search,StringComparison.OrdinalIgnoreCase)<0)continue;
                var field=target.GetType().GetField(property.name,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
                var header=field?.GetCustomAttribute<HeaderAttribute>();
                if(header!=null){GUILayout.Space(12);EditorGUILayout.LabelField(header.header,EditorStyles.boldLabel);}
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent(property.displayName,property.tooltip),GUILayout.Width(labelWidth));
                DrawValue(property,field);
                GUILayout.FlexibleSpace();EditorGUILayout.LabelField(property.type,EditorStyles.miniLabel,GUILayout.Width(110));
                EditorGUILayout.EndHorizontal();
            }
            serialized.ApplyModifiedProperties();
        }
        private static void DrawValue(SerializedProperty property,FieldInfo field)
        {
            // Numeric controls avoid drawing Unity's Header decorator twice in a table row.
            var width=new[]{GUILayout.MinWidth(240),GUILayout.MaxWidth(460)};
            var range=field?.GetCustomAttribute<RangeAttribute>();var min=field?.GetCustomAttribute<MinAttribute>();
            if(property.propertyType==SerializedPropertyType.Float && property.type!="double")
            {
                var value=range!=null ? EditorGUILayout.Slider(property.floatValue,range.min,range.max,width) : EditorGUILayout.FloatField(property.floatValue,width);
                property.floatValue=min!=null ? Mathf.Max(min.min,value):value;
            }
            else if(property.propertyType==SerializedPropertyType.Integer && property.type!="long")
            {
                var value=range!=null ? EditorGUILayout.IntSlider(property.intValue,Mathf.RoundToInt(range.min),Mathf.RoundToInt(range.max),width) : EditorGUILayout.IntField(property.intValue,width);
                property.intValue=min!=null ? Mathf.Max(Mathf.CeilToInt(min.min),value):value;
            }
            else EditorGUILayout.PropertyField(property,GUIContent.none,true,width);
        }
        /// <summary>Export every public or explicitly serialized field; arrays and vectors retain invariant values.</summary>
        public static void ExportCsv(UnityEngine.Object target,string path)
        {
            var rows=new List<string>{"parameter,type,value,scope"};
            foreach(var field in target.GetType().GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
            {
                if(field.IsNotSerialized||(!field.IsPublic&&field.GetCustomAttribute<SerializeField>()==null))continue;
                rows.Add($"{Quote(field.Name)},{Quote(field.FieldType.Name)},{Quote(Value(field.GetValue(target)))},{Quote(target.GetType().Name)}");
            }
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));File.WriteAllLines(path,rows);
        }
        private static string Value(object value)
        {
            if(value is Array array){var entries=new List<string>();foreach(var item in array)entries.Add(Value(item));return string.Join(" | ",entries);}
            if(value is Vector3 v)return string.Format(CultureInfo.InvariantCulture,"({0}, {1}, {2})",v.x,v.y,v.z);
            if(value is Vector2 p)return string.Format(CultureInfo.InvariantCulture,"({0}, {1})",p.x,p.y);
            if(value is Color c)return string.Format(CultureInfo.InvariantCulture,"RGBA({0}, {1}, {2}, {3})",c.r,c.g,c.b,c.a);
            return Convert.ToString(value,CultureInfo.InvariantCulture);
        }
        private static string Quote(string text)=>"\""+(text??"").Replace("\"","\"\"")+"\"";
    }
}
