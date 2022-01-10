using System;

namespace WebService_Lib.Attributes.Rest
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Patch : Attribute, IMethod
    {
        public string Path { get; }

        public Patch(string path)
        {
            this.Path = path;
        }
    }
}