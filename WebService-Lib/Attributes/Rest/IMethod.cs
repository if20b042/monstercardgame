using System;

namespace WebService_Lib.Attributes.Rest
{
    public interface IMethod
    {
        // Make field Path mandatory
        // See: https://stackoverflow.com/a/2115156/12347616
        public string Path { get; }
    }
}