using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// Activate Nullable attributes
// See: https://github.com/dotnet/roslyn/issues/36986#issuecomment-508842786
namespace System.Diagnostics.CodeAnalysis
{
    class MaybeNullAttribute : System.Attribute { }
    class AllowNullAttribute : System.Attribute {}
}

namespace WebService_Lib.Server
{
    /// <summary>
    /// Used to extract values from URI Path like:
    /// <c>/foos/{id}</c>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PathVariable<T> where T : IComparable
    {
        // Class should only allow string or numbers
        // See: https://stackoverflow.com/a/3329610/12347616
        
        // Allow null value without specifying class or struct
        // See: https://stackoverflow.com/a/57796924/12347616
        [MaybeNull, AllowNull]
        public T Value { get; }
        public bool Ok { get; }
        public PathVariable(string? value)
        {
            if (value == null)
            {
                Ok = false;
                Value = default;
            }
            else
            {
                // Change type dynamically
                // See: https://stackoverflow.com/a/4010198/12347616
                try
                {
                    var cast = Convert.ChangeType(value, typeof(T));
                    if (cast != null)
                    {
                        Ok = true;
                        Value = (T) cast;
                    }
                    else
                    {
                        Ok = false;
                        Value = default;
                    }
                }
                catch (Exception ex)
                {
                    Ok = false;
                    Value = default;
                }
            }
        }
    }

    /// <summary>
    /// Used to extract values from query string like:
    /// <c>/foos/?id=abc</c>
    /// </summary>
    public class RequestParam
    {
        public Dictionary<string, string> Value { get; }

        public bool Empty => Value.Count == 0;

        public RequestParam(string? value)
        {
            Value = new Dictionary<string, string>();
            if (value != null)
            {
                var entries = value.Split('&');
                foreach (var entry in entries)
                {
                    var tmp = entry.Split('=');
                    if (tmp.Length == 2)
                    {
                        var key = tmp[0];
                        var val = tmp[1];
                        Value.Add(key, val);
                    }
                }
            }
            
        }
    }
}