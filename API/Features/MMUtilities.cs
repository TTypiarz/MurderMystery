using System.Diagnostics;
using System.Reflection;

namespace MurderMystery.API.Features
{
    public static class MMUtilities
    {
        public static string GetCallerString()
        {
            try
            {
                MethodBase method = new StackFrame(2).GetMethod();

                return $"[{method.DeclaringType.Name}::{method.Name}]";
            }
            catch
            {
                return "[null]";
            }
        }
    }
}
