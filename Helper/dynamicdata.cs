using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MOFAAPI.Helpers
{
    public static class Extensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                {
                    if (row[property.Name] == DBNull.Value)
                        property.SetValue(item, null, null);
                    else
                    {
                        if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                        {
                            //nullable
                            object convertedValue = null;
                            try
                            {
                                convertedValue = System.Convert.ChangeType(row[property.Name], Nullable.GetUnderlyingType(property.PropertyType));
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            property.SetValue(item, convertedValue, null);
                        }
                        else
                            property.SetValue(item, row[property.Name], null);
                    }
                }
            }
            return item;
        }
    }

    public class DynamicData : DynamicObject
    {

        public IDictionary<string, object> Dictionary { get; set; }

        public DynamicData()
        {
            this.Dictionary = new Dictionary<string, object>();
        }

        public int Count { get { return this.Dictionary.Keys.Count; } }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (this.Dictionary.ContainsKey(binder.Name))
            {
                result = this.Dictionary[binder.Name];
                return true;
            }
            return base.TryGetMember(binder, out result); //means result = null and return = false
        }

        public void SetMember(string name, object value)
        {
            if (!this.Dictionary.ContainsKey(name))
            {
                this.Dictionary.Add(name, value);
            }
            else
                this.Dictionary[name] = value;
        }

        public object GetMember(string name)
        {
            if (this.Dictionary.ContainsKey(name))
            {
                return this.Dictionary[name];
            }
            else
                return null;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!this.Dictionary.ContainsKey(binder.Name))
            {
                this.Dictionary.Add(binder.Name, value);
            }
            else
                this.Dictionary[binder.Name] = value;

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (this.Dictionary.ContainsKey(binder.Name) && this.Dictionary[binder.Name] is Delegate)
            {
                Delegate? del = this.Dictionary[binder.Name] as Delegate;
                result = del.DynamicInvoke(args);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            if (this.Dictionary.ContainsKey(binder.Name))
            {
                this.Dictionary.Remove(binder.Name);
                return true;
            }

            return base.TryDeleteMember(binder);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (string name in this.Dictionary.Keys)
                yield return name;
        }

        public bool isMemberExists(string name)
        {
            if (this.Dictionary.ContainsKey(name))
            {
                return true;
            }
            else
                return false;
        }
    }
}
