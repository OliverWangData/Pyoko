using Godot;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using SQLib.Extensions;

namespace SQGame.Data
{
    /// <summary>
    /// Database that stores a bunch of Table.cs objects, which use a struct blueprint in order to store csv data.
    /// Each table can be accessed using its unique csv struct. 
    /// </summary>
    public class Database
    {
        // [Fields]
        // ****************************************************************************************************
        private Dictionary<Type, object> _cache = new(); // Boxed table cache

        // [Initialization]
        // ****************************************************************************************************
        /// <summary>
        /// Uses a default csv path of "/Data/", and a default csv struct namespace of "Game.Data".
        /// </summary>
        public Database() : this(Path.Combine("Data", "CSVs"), "SQGame.Data") { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="csvPath">          Path to the .csv files. </param>
        /// <param name="structNamespace">  Namespace of the csv structs. </param>
        public Database(string csvPath, string structNamespace)
        {
            // Load csv by existing structs in the StructNamespaces
            Type[] dataStructs = new Type[0];
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                dataStructs = assemblies[i].GetTypes().Where(t => 
                string.Equals(t.Namespace, structNamespace, StringComparison.Ordinal) && t.IsValueType).ToArray();
                if (dataStructs.Length > 0) break;
            }

            if (dataStructs.Length == 0) GD.PushWarning($"Could not find any structs in {structNamespace}.");

            string[] paths = dataStructs.Select(t => Path.Combine("res://", csvPath, t.Name + ".csv")).ToArray();

            for (int i = 0; i < paths.Length; i++)
            {
                Type tentry = dataStructs[i];
                Type tkey = GetIdType(tentry);
                Type ttable = GetTableType(tkey, tentry);
                object table = Activator.CreateInstance(ttable, new object[] { paths[i] });
                _cache[dataStructs[i]] = table;
            }
        }


        // [Methods]
        // ****************************************************************************************************

        /// <summary>
        /// Grabs the TEntry struct with the key "id" from the TEntry's table.
        /// </summary>
        /// <typeparam name="TKey">             Type of the table key. </typeparam>
        /// <typeparam name="TEntry">           Type of the table entry struct. </typeparam>
        /// <param name="id">                   Key for the entry. </param>
        /// <returns>                           Entry with the id key. </returns>
        public TEntry Get<TKey, TEntry>(TKey id) where TEntry : struct => GetTable<TKey, TEntry>().Get(id);

        /// <summary>
        /// Grabs the TEntry's table. Caches result.
        /// </summary>
        /// <typeparam name="TKey">             Type of the table key. </typeparam>
        /// <typeparam name="TEntry">           Type of the table entry struct. </typeparam>
        /// <returns>                           Table with the corresponding key and entry types. </returns>
        public DataTable<TKey, TEntry> GetTable<TKey, TEntry>() where TEntry : struct
        {
            return (DataTable<TKey, TEntry>)_cache[typeof(TEntry)];
        }

        /// <summary>
        /// Grabs the TEntry table's number of entries. Caches the table.
        /// </summary>
        /// <typeparam name="TKey">             Type of the table key. </typeparam>
        /// <typeparam name="TEntry">           Type of the table entry struct. </typeparam>
        /// <returns>                           Number of entries in the table with corresponding key and entry types. </returns>
        public int Count<TKey, TEntry>() where TEntry : struct => GetTable<TKey, TEntry>().Count;

        public TKey[] GetKeys<TKey, TEntry>() where TEntry : struct
        {
            return GetTable<TKey, TEntry>().Keys.ToArray();
        }

        // [Low Performance Methods]
        // ****************************************************************************************************
        /// <summary>
        /// Gets an array of all values in the Table. 
        /// Low performance due to multiple reflection calls and hardcoded string method names.
        /// </summary>
        public TEntry[] GetValues<TEntry>() where TEntry : struct
        {
            Type idType = GetIdType(typeof(TEntry));
            object table = this.InvokeGeneric("GetTable", new Type[] { idType, typeof(TEntry) });
            return (TEntry[])table.GetType().GetMethod("GetValuesArray").Invoke(table, null);
        }

        // [Helpers]
        // ****************************************************************************************************
        private Type GetTableType(Type key, Type entry)
        {
            return typeof(DataTable<,>).MakeGenericType(new Type[] { key, entry });
        }

        public Type GetIdType(Type entry)
        {
            FieldInfo fieldInfo = entry.GetField("Id");
            Debug.Assert(fieldInfo is not null, $"Class '{entry}' for '{entry}.csv' is missing an 'Id' field.");
            return fieldInfo.FieldType;
            
        }
    }
}