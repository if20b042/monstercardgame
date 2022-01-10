using System;
using System.Collections.Generic;

namespace WebService_Lib
{
    /// <summary>
    /// Interface that defines methods for managing an object container.
    /// </summary>
    public interface IContainer
    {
       Dictionary<Type, object> GetContainer { get; }

       /// <summary>
       /// Add an object to the container and perform autowiring.
       /// </summary>
       /// <param name="obj">Object to be added</param>
       void Add(object obj);

       /// <summary>
       /// Get object from container that matches given type.
       /// </summary>
       /// <param name="type"></param>
       /// <returns>Found object or null</returns>
       object? Get(Type type);

       /// <summary>
       /// Get all objects from container that match list of types.
       /// </summary>
       /// <param name="types"></param>
       /// <returns>List of objects</returns>
       List<object> GetObjects(List<Type> types);
    }
}