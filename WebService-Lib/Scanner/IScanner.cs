using System;
using System.Collections.Generic;

namespace WebService_Lib
{
    /// <summary>
    /// Interface used to define a "Scanner" class that scans through
    /// an assembly for concrete attributes.
    /// </summary>
    public interface IScanner
    {
        /// <summary>
        /// Scan the assembly for <c>Component</c>, <c>Controller</c> and <c>Security</c> attributes.
        /// </summary>
        /// <returns>
        /// Tuple consisting of
        ///     1. All matching types
        ///     2. All matching types that are controllers
        ///     3. Last matching security type 
        /// </returns>
        (List<Type>, List<Type>, Type?) ScanAssembly();
    }
}