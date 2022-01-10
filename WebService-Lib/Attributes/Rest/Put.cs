using System;

namespace WebService_Lib.Attributes.Rest
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Put : Attribute, IMethod
    {
        public string Path { get; }

        public Put(string path)
        {
            this.Path = path;
        }
    }
}