using System;

namespace WebService_Lib.Attributes.Rest
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Get : Attribute, IMethod
    {
        public string Path { get; }

        public Get(string path)
        {
            this.Path = path;
        }
    }
}