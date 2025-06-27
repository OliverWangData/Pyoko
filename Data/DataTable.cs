using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using SQLib.Extensions;
using System.Diagnostics;
using System.Text;
using Godot;
using System.Text.RegularExpressions;

namespace SQGame.Data
{
    /// <summary>
    /// Stores CSV data in a dictionary.
    /// Requires an 'Id' column, and a struct that has fields for all the columns.
    /// </summary>
    /// <typeparam name="TKey">     Type of the 'Id' field.</typeparam>
    /// <typeparam name="TEntry">   Struct type with fields for each column.</typeparam>
    [Serializable]
    public class DataTable<TKey, TEntry> where TEntry : struct
    {
        // [Fields]
        // ****************************************************************************************************
        private Dictionary<TKey, TEntry> _data = new Dictionary<TKey, TEntry>();

        // Caches the order of the TKeys in the csv
        // Could also be done by turning _data into an OrderedDictionary. But that requires an extra dependency (System.Collections.Specialized)
        private List<TKey> _order = new List<TKey>();
        private string[] _columns;

        private string _csvPath { get; init; }
        private bool _isDirty;

        private const string REGEX_SPLIT_COMMA_OUTSIDE_QUOTES = "[,]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";

        // [Initialization]
        // ****************************************************************************************************

        public DataTable(string csvPath)
        {
            _csvPath = csvPath;
            InitializeFromCsv();
        }

        /// <remarks>
        /// Fills the data dictionary from the csv text by doing the following:
        ///     1. Read the csv line by line into an array
        ///     2. Create a TEntry struct for each line by the following steps:
        ///         a. Caches a value (string type) for each field of the TEntry struct
        ///         b. Caches a reference to each field/property of the TEntry struct
        ///         c. Generically assigns the string value into the field (Preserves type generically)
        ///     3. Adds all the TEntry structs into data dictionary
        /// </remarks>
        private void InitializeFromCsv()
        {
            _data.Clear();

            // Reads the .csv line by line into a string array
            string[] lines = ReadCsv()
                .Split('\n').Select(text => text.Trim())
                .Where(text => text != "").ToArray();

            string[] columns = Regex.Split(lines[0], REGEX_SPLIT_COMMA_OUTSIDE_QUOTES);
            _columns = columns;

            // Caches the values of every row in the csv
            Dictionary<TKey, string[]> entries = new(lines.Length - 1);

            for (int i = 1; i < lines.Length; i++)  // First line is the column name
            {
                string[] values = Regex.Split(lines[i], REGEX_SPLIT_COMMA_OUTSIDE_QUOTES);

                // Skip empty rows
                if (values[0] == "")
                {
                    continue;
                }

                TKey id = values[0].ToType<TKey>();

                Debug.Assert(values.Length == columns.Length, $"Number of values do not match number of columns for {typeof(TEntry)}. Ensure no commas exist in any values.");
                Debug.Assert(!entries.ContainsKey(id), $"An element with the key '{id}' already exists in the dictionary for {typeof(TEntry)}.");
                Debug.Assert(values
                    .Select(value => value.Count() - value.Trim().Count())  // Difference in length between the value string and trimmed value string
                    .Select(diff => diff != 0)
                    .Any(),
                    $"A value for the key '{id}' has length mismatch when trimmed. Check that there are no leading or trailing white-space characters in any values."
                    );

                entries[id] = values;
                _order.Add(id);
            }

            // Caches FieldInfo / PropertyInfo for the TEntry struct
            List<FieldInfo> fields = new();
            List<PropertyInfo> properties = new();
            bool[] isField = new bool[columns.Length];  // Whether the column in the ordered columns array is a field or property
            int[] index = new int[columns.Length];      // Index of the field/property in the field or property list

            for (int i = 0; i < columns.Length; i++)
            {
                FieldInfo field = typeof(TEntry).GetField(Manipulations.Capitalize(columns[i]), BindingFlags.Public | BindingFlags.Instance);

                // Match field
                if (field is not null)
                {
                    isField[i] = true;
                    fields.Add(field);
                    index[i] = fields.Count - 1;
                }
                // Match property
                else
                {
                    PropertyInfo property = typeof(TEntry).GetProperty(Manipulations.Capitalize(columns[i]), BindingFlags.Public | BindingFlags.Instance);
                    Debug.Assert(property is not null, $"Could not find a public field or property '{Manipulations.Capitalize(columns[i])}' in struct '{typeof(TEntry)}'.");

                    isField[i] = false;
                    properties.Add(property);
                    index[i] = properties.Count - 1;
                }
            }

            // Creates tentry struct for each row
            foreach ((TKey key, string[] values) in entries)
            {
                // Boxed TEntry struct for the FieldInfo.SetValue method.
                object entry = (TEntry)Activator.CreateInstance(typeof(TEntry));

                // Sets the field of the TEntry struct using a string.
                for (int i = 0; i < columns.Length; i++)
                {
                    // Empty entry. Should warn instead of error out for testing purposes.
                    if (values[i] == "")
                    {
                        GD.PushWarning($"Missing entry value for {columns[i]} for id '{key}' in csv {_csvPath}. Setting as default.");
                    }


                    // Converts the string into the field or property type generically using reflections, and then set the field/property value using that converted type
                    object[] parameters = new object[] { values[i], true };

                    if (isField[i])
                    {
                        fields[index[i]].SetValue(entry, typeof(Conversions).InvokeGeneric("ToType", fields[index[i]].FieldType, parameters));
                    }
                    else
                    {
                        properties[index[i]].SetValue(entry, typeof(Conversions).InvokeGeneric("ToType", properties[index[i]].PropertyType, parameters));
                    }
                }

                _data[key] = (TEntry)entry;
            }
        }


        // [Properties]
        // ****************************************************************************************************
        public int Count
        {
            get { return _data.Count; }
        }

        public Dictionary<TKey, TEntry>.KeyCollection Keys
        {
            get { return _data.Keys; }
        }

        public Dictionary<TKey, TEntry>.ValueCollection Values
        {
            get { return _data.Values; }
        }

        // [Indexers]
        // ****************************************************************************************************
        public TEntry this[int index]
        {
            get => _data[_order[index]];
        }

        // [Methods]
        // ****************************************************************************************************
        public TEntry Get(TKey id)
        {
            if (_data.ContainsKey(id))
            {
                return _data[id];
            }
            else
            {
                Godot.GD.PushWarning($"Id {id} does not exist in the table for {typeof(TEntry).Name}.");
                return default;
            }
        }

        public TEntry[] GetValuesArray()
        {
            return Values.ToArray();
        }

        public void Set(TKey id, TEntry value)
        {
            if (!_data.ContainsKey(id))
            {
                _order.Add(id);
            }

            _data[id] = value;
            _isDirty = true;
        }

        public bool Contains(TKey id) => _data.ContainsKey(id);

        public void Save(bool forceWrite = false)
        {

            if (_isDirty || forceWrite)
            {
                WriteCsv(GetCsvText());
            }
        }

        private string ReadCsv()
        {
            Debug.Assert(FileAccess.FileExists(_csvPath), $"A file at {_csvPath} does not exist.");

            string text;

            using (FileAccess file = FileAccess.Open(_csvPath, FileAccess.ModeFlags.Read))
            {
                text = file.GetAsText();
            }

            return text;
        }

        private void WriteCsv(string text)
        {
            using (FileAccess file = FileAccess.Open(_csvPath, FileAccess.ModeFlags.Write))
            {
                file.StoreString(text);
            }
        }

        private string GetCsvText()
        {
            StringBuilder sb = new();

            for (int i = 0; i < _order.Count; i++)
            {
                TKey key = _order[i];
                string values = string.Join(',', _columns
                        .Select(col => typeof(TEntry).GetProperty(col).GetValue(key).ToString())
                        .ToArray());
                sb.Append(values);
                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}
