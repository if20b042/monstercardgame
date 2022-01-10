using System;

namespace WebService_Lib.Attributes.Rest
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Post : Attribute, IMethod
    {
        public string Path { get; }

        public Post(string path)
        {
            this.Path = path;
        }
    }
}