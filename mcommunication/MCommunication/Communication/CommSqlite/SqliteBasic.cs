using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using System.Diagnostics;

namespace Sqlite.Basic
{
    public class SqliteBasic
    {
        public static Semaphore ConnectionSemaphore = new Semaphore(1, 1);

        private static string ConnectionString = @"Data Source=|DataDirectory|\DB\Example.db";
        private string TableName;

        private List<OnePrimaryKey> PrimaryKeys = new List<OnePrimaryKey>();
        private List<OneColumn> Columns = new List<OneColumn>();
        private List<OneColumn> ExcludeColumns = new List<OneColumn>();
        private List<OneCondition> Conditions = new List<OneCondition>();

        public static void Initialize(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Dictionary<string, object> GetFieldNameValuePairs(object instance)
        {
            FieldInfo[] fields = instance.GetType().GetFields();
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();

            foreach (var field in fields)
            {
                object value = field.GetValue(instance);
                fieldValues[field.Name] = value;
            }

            return fieldValues;
        }

        public List<FieldInfo> GetFieldInfos(object instance)
        {
            return instance.GetType().GetFields().ToList();
        }

        public void CreateTable<T>() where T : new()
        {
            TableName = typeof(T).Name;

            Dictionary<string, object> fields = GetFieldNameValuePairs(new T());
            foreach (var field in fields)
            {
                if (field.Value.GetType() == typeof(string) || field.Value.GetType() == typeof(Enum))
                {
                    AddColumn(field.Key, "STRING" + AddOption(field.Key));
                }
                else if (field.Value.GetType() == typeof(bool) || field.Value.GetType() == typeof(int) || field.Value.GetType() == typeof(uint) || field.Value.GetType() == typeof(short) || field.Value.GetType() == typeof(ushort) || field.Value.GetType() == typeof(sbyte) || field.Value.GetType() == typeof(byte) || field.Value.GetType() == typeof(long) || field.Value.GetType() == typeof(ulong))
                {
                    AddColumn(field.Key, "INTEGER" + AddOption(field.Key));
                }
                else if (field.Value.GetType() == typeof(float) || field.Value.GetType() == typeof(double))
                {
                    AddColumn(field.Key, "REAL" + AddOption(field.Key));
                }
                else
                {
                    AddColumn(field.Key, "STRING" + AddOption(field.Key));
                }
            }
            ExecuteNonQuery_Create();
        }

        private string AddOption(string columnName)
        {
            string optionString = string.Empty;

            // Primary Key Option 추가
            int index = PrimaryKeys.FindIndex(x => x.Name == columnName);
            if (index >= 0)
            {
                optionString = " PRIMARY KEY";

                if (PrimaryKeys[index].IsAutoIncrement)
                {
                    optionString += " AUTOINCREMENT";
                }
            }

            return optionString;
        }

        public bool UpdateData<T>()
        {
            TableName = typeof(T).Name;

            return ExecuteNonQuery_Update();
        }

        public bool InsertData<T>(object instance)
        {
            TableName = typeof(T).Name;

            Dictionary<string, object> fields = GetFieldNameValuePairs((T)instance);
            foreach (var field in fields)
            {
                AddColumn(field.Key, field.Value);
            }
            return ExecuteNonQuery_Insert();
        }

        public bool DeleteData<T>()
        {
            TableName = typeof(T).Name;

            return ExecuteNonQuery_Delete();
        }

        public List<T> GetData<T>() where T : new()
        {
            TableName = typeof(T).Name;

            return ExecuteReader<T>();
        }

        private bool ExecuteNonQuery_Insert()
        {
            var excludeColumnNames = ExcludeColumns.Select(c => c.Name).ToList();

            // 컬럼 리스트와 파라미터 리스트 준비
            var columnNames = Columns.Select(c => c.Name);
            var paramNames = Columns.Select(c => "@" + c.Name);

            string columnList = string.Join(", ", columnNames);
            string valueList = string.Join(", ", paramNames);

            string query = $"INSERT INTO {TableName} ({columnList}) VALUES ({valueList});";

            try
            {
                ConnectionSemaphore.WaitOne();

                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        foreach (var column in Columns)
                        {
                            if (excludeColumnNames.Contains(column.Name))
                            {
                                continue;
                            }

                            command.Parameters.AddWithValue($"@{column.Name}", column.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                ConnectionSemaphore.Release();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SQLite 에러\n" + $"Table: {TableName}\nOperation: Insert\n{ex.Message}");

                ConnectionSemaphore.Release();

                return false;
            }
        }

        private bool ExecuteNonQuery_Create()
        {
            string columnList = string.Empty;
            for (int i = 0; i < Columns.Count; i++)
            {
                columnList += $"'{Columns[i].Name}' {Columns[i].Value}";
                if (i != Columns.Count - 1)
                {
                    columnList += ", ";
                }
            }

            string query = $"CREATE TABLE IF NOT EXISTS {TableName}({columnList})";

            try
            {
                ConnectionSemaphore.WaitOne();

                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                ConnectionSemaphore.Release();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Table: {TableName}\nOperation: Create\n{ex.Message}");

                ConnectionSemaphore.Release();

                return false;
            }
        }

        private bool ExecuteNonQuery_Delete()
        {
            string conditionList = string.Empty;
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (i == 0)
                {
                    conditionList += " WHERE ";
                }
                conditionList += $"{Conditions[i].Name} = @{Conditions[i].Name}";
                if (i != Conditions.Count - 1)
                {
                    conditionList += " AND ";
                }
            }

            string query = $"DELETE FROM {TableName}{conditionList};";

            try
            {
                ConnectionSemaphore.WaitOne();

                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        foreach (var condition in Conditions)
                        {
                            command.Parameters.AddWithValue($"@{condition.Name}", condition.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                ConnectionSemaphore.Release();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SQLite 에러\n" + $"Table: {TableName}\nOperation: Delete\n{ex.Message}");

                ConnectionSemaphore.Release();

                return false;
            }
        }

        private bool ExecuteNonQuery_Update()
        {
            if (Columns.Count == 0)
            {
                Debug.WriteLine("SQLite 파라미터\n" +
                                $"Table: {TableName}\nOperation: Update\nSET 절에 항목이 없습니다.");
                return false;
            }

            // 1) SET 절 생성: "col1 = @col1, col2 = @col2, ..."
            string setClause = string.Join(
                ", ",
                Columns.Select(c => $"{c.Name} = @{c.Name}")
            );

            // 2) WHERE 절 생성: "colA = @w_colA AND colB = @w_colB ..."
            //    파라미터 이름 중복 방지를 위해 접두어 w_를 붙입니다.
            string whereClause = string.Join(
                " AND ",
                Conditions.Select(c => $"{c.Name} = @w_{c.Name}")
            );

            string query;
            if (Conditions.Count >= 1)
            {
                query = $"UPDATE {TableName} SET {setClause} WHERE {whereClause};";
            }
            else
            {
                query = $"UPDATE {TableName} SET {setClause};";
            }

            try
            {
                ConnectionSemaphore.WaitOne();

                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        // SET 절 파라미터 등록
                        foreach (var col in Columns)
                        {
                            command.Parameters.AddWithValue($"@{col.Name}", col.Value ?? DBNull.Value);
                        }

                        // WHERE 절 파라미터 등록
                        foreach (var cond in Conditions)
                        {
                            command.Parameters.AddWithValue($"@w_{cond.Name}", cond.Value ?? DBNull.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                ConnectionSemaphore.Release();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SQLite 에러\n" +
                                $"Table: {TableName}\nOperation: Update\n{ex.Message}");

                ConnectionSemaphore.Release();

                return false;
            }
        }

        private List<T> ExecuteReader<T>() where T : new()
        {
            string conditionList = string.Empty;
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (i == 0)
                {
                    conditionList += " WHERE ";
                }
                conditionList += $"{Conditions[i].Name} = @{Conditions[i].Name}";
                if (i != Conditions.Count - 1)
                {
                    conditionList += " AND ";
                }
            }

            string query = $"SELECT * FROM {TableName}{conditionList};";

            try
            {
                ConnectionSemaphore.WaitOne();

                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        foreach (var condition in Conditions)
                        {
                            command.Parameters.AddWithValue($"@{condition.Name}", condition.Value);
                        }

                        SQLiteDataReader reader = command.ExecuteReader();

                        var fields = GetFieldInfos(new T());

                        List<T> dataList = new List<T>();

                        while (reader.Read())
                        {
                            T data = new T();

                            foreach (var field in fields)
                            {
                                if (field.FieldType.IsEnum)
                                {
                                    int intVal = Convert.ToInt32(reader[field.Name]);
                                    field.SetValue((T)data, Enum.ToObject(field.FieldType, intVal));
                                }
                                else if (field.FieldType == typeof(string))
                                {
                                    field.SetValue((T)data, reader[field.Name].ToString());
                                }
                                else if (field.FieldType == typeof(bool))
                                {
                                    field.SetValue((T)data, Convert.ToBoolean(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(byte))
                                {
                                    field.SetValue((T)data, Convert.ToByte(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(sbyte))
                                {
                                    field.SetValue((T)data, Convert.ToSByte(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(ushort))
                                {
                                    field.SetValue((T)data, Convert.ToUInt16(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(short))
                                {
                                    field.SetValue((T)data, Convert.ToInt16(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(uint))
                                {
                                    field.SetValue((T)data, Convert.ToUInt32(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(int))
                                {
                                    field.SetValue((T)data, Convert.ToInt32(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(ulong))
                                {
                                    field.SetValue((T)data, Convert.ToUInt64(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(long))
                                {
                                    field.SetValue((T)data, Convert.ToInt64(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(float))
                                {
                                    field.SetValue((T)data, Convert.ToSingle(reader[field.Name]));
                                }
                                else if (field.FieldType == typeof(double))
                                {
                                    field.SetValue((T)data, Convert.ToDouble(reader[field.Name]));
                                }
                                else
                                {
                                    field.SetValue((T)data, reader[field.Name].ToString());
                                }
                            }

                            dataList.Add(data);
                        }

                        ConnectionSemaphore.Release();

                        return dataList;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SQLite 에러\n" + $"Table: {TableName}\nOperation: Select\n{ex.Message}");

                ConnectionSemaphore.Release();

                return new List<T>();
            }
        }

        public SqliteBasic AddPrimaryKey(string columnName, bool isAutoIncrement)
        {
            var primaryKey = new OnePrimaryKey
            {
                Name = columnName,
                IsAutoIncrement = isAutoIncrement,
            };
            PrimaryKeys.Add(primaryKey);

            return this;
        }

        public SqliteBasic AddColumn(string columnName, object attribute)
        {
            var column = new OneColumn
            {
                Name = columnName,
                Value = attribute
            };
            Columns.Add(column);

            return this;
        }

        public SqliteBasic ExcludeColumn(string columnName)
        {
            var column = new OneColumn
            {
                Name = columnName,
                Value = string.Empty
            };
            ExcludeColumns.Add(column);

            return this;
        }

        public SqliteBasic AddCondition(string columnName, object value)
        {
            var column = new OneCondition
            {
                Name = columnName,
                Value = value
            };
            Conditions.Add(column);

            return this;
        }

        private class OnePrimaryKey
        {
            public string Name;
            public bool IsAutoIncrement;
        }

        private class OneColumn
        {
            public string Name;
            public object Value;
        }

        private class OneCondition
        {
            public string Name;
            public object Value;
        }
    }
}
