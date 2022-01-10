using System;

namespace WebService_Lib.Attributes.Rest
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Delete : Attribute, IMethod
    {
        public string Path { get; }

        public Delete(string path)
        {
            this.Path = path;
        }
    }
}