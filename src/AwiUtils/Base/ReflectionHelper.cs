using System;
using System.Linq;
using System.Reflection;

namespace AwiUtils
{
    public static class ReflectionHelper
    {
        /// <summary> Setzt den Wert value in die Property mit Namen name des Objektes o.</summary>
        /// <param name="value">Wird in den passenden Typ konvertiert, falls möglich. Achtung wg. Dezimalkomma oder -punkt. </param>
        /// <param name="shallIgnoreCase">Falls T : Ignoriere Case beim Namen der Property. </param>
        /// <returns>T, falls es geklappt hat. </returns>
        public static bool SetProperty(Object o, string name, string value, bool shallIgnoreCase)
        {
            PropertyInfo pi = GetPropertyInfo(o, name, shallIgnoreCase);
            if (pi != null)
                pi.SetValue(o, Convert.ChangeType(value, pi.PropertyType), null);
            else
                ExLogger.Instance.LogWarning("Could not find property " + Ext.ToDebug(name) + " on type " + o.GetType().Name);
            return pi != null;
        }

        public static PropertyInfo GetPropertyInfo(Object o, string name, bool shallIgnoreCase)
        {
            Type t = o.GetType();
            PropertyInfo[] pis = t.GetProperties();
            PropertyInfo pi = pis.FirstOrDefault(p => p.Name == name);
            if (pi == null && shallIgnoreCase)
                pi = pis.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return pi;
        }
    }
}
