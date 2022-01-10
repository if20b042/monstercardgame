using System;
using System.Collections.Generic;
using System.Reflection;
using WebService_Lib.Attributes;

namespace WebService_Lib
{
    /// <summary>
    /// Initializes all classes to concrete instance that should be managed
    /// by <c>WebService_Lib</c>. Can also autowire (=set field values) components
    /// with each other.
    /// </summary>
    public class Container : IContainer
    {
        private Dictionary<Type, object> container = new Dictionary<Type, object>();

        public Dictionary<Type, object> GetContainer => container;

        public Container(List<Type> components)
        {
            foreach (var component in components)
            {
                // Instance components
                // See: https://stackoverflow.com/a/755/12347616
                var instance = Activator.CreateInstance(component);
                container.Add(component, instance);
            }

            Autowire(components);
        }

        /// <summary>
        /// Add an object to the container and perform autowiring.
        /// </summary>
        /// <param name="obj">Object to be added</param>
        public void Add(object obj)
        {
            container.Add(obj.GetType(), obj);
            Autowire(new List<Type>(container.Keys));
        }

        /// <summary>
        /// Get object from container that matches given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Found object or null</returns>
        public object? Get(Type type)
        {
            return container[type];
        }

        /// <summary>
        /// Get all objects from container that match list of types.
        /// </summary>
        /// <param name="types"></param>
        /// <returns>List of objects</returns>
        public List<object> GetObjects(List<Type> types)
        {
            var objects = new List<object>();
            foreach (var type in types)
            {
                if (container.ContainsKey(type)) objects.Add(container[type]);
            }

            return objects;
        }

        /// <summary>
        /// Perform autowiring on all components.
        /// Autowiring means that fields with the <c>[Autowired]</c> attribute will
        /// be dynamically set when a matching component is found.
        /// </summary>
        /// <param name="components"></param>
        private void Autowire(List<Type> components)
        {
            foreach (var component in components)
            {
                // In C# there is a distinct difference between fields and props
                // See: https://stackoverflow.com/a/295109/12347616
                // We want to autowire FIELDS, so GetFields needs to be called
                // Also BindingFlags need to be set in order to get private fields
                // See: https://stackoverflow.com/a/1040816/12347616
                var fields = component.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in fields)
                {
                    // Check if Autowiring needs to be performed
                    // See: https://stackoverflow.com/a/2051116/12347616
                    var needsAutowire = Attribute.IsDefined(field, typeof(Autowired));
                    if (needsAutowire)
                    {
                        // Perform it when possible
                        Type fType = field.FieldType;
                        if (container.ContainsKey(fType))
                        {
                            var instance = container[component];
                            var instanceToWire = container[fType];
                            // Set object field using reflection
                            // See: https://stackoverflow.com/a/619788/12347616
                            field.SetValue(instance, instanceToWire);
                        }
                        // Omit error message for auth autowiring
                        // Happens later on
                        else if (fType != typeof(AuthCheck))
                        {
                            Console.Error.WriteLine("Err: Can not find property of type " + fType.FullName +
                                                    " to autowire field " + field.Name + " in Class " + component.FullName);
                            Console.Error.WriteLine("Err: Field will not be initialized");
                        }
                    }
                }
            }
        }
    }
}