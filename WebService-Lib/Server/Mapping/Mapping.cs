using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebService_Lib.Attributes.Rest;
using WebService_Lib.Server.Exceptions;

namespace WebService_Lib.Server
{
    /// <summary>
    /// Maps the paths of the service endpoint to their corresponding methods.
    /// Is also responsible for their invocation.
    /// </summary>
    public class Mapping : IMapping
    {
        private Dictionary<Method, Dictionary<string, MethodCaller>> mappings;
        public Dictionary<Method, Dictionary<string, MethodCaller>> GetMappings => mappings;

        public Mapping(List<object> controllers)
        {
            mappings = new Dictionary<Method, Dictionary<string, MethodCaller>>
            {
                {Method.Get, new Dictionary<string, MethodCaller>()},
                {Method.Post, new Dictionary<string, MethodCaller>()},
                {Method.Put, new Dictionary<string, MethodCaller>()},
                {Method.Delete, new Dictionary<string, MethodCaller>()},
                {Method.Patch, new Dictionary<string, MethodCaller>()}
            };

            foreach (var controller in controllers)
            {
                var cType = controller.GetType();
                var methods = cType.GetMethods();
                foreach (var method in methods)
                {
                    AddMapping(method, controller);
                }
            }
        }

        /// <summary>
        /// Check if a method contains a service endpoint definition.
        /// If so, create a wrapper for it and save it.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="controller"></param>
        private void AddMapping(MethodInfo method, object controller)
        {
            var restMethods = new List<Type> { typeof(Get), typeof(Post), typeof(Put), typeof(Delete), typeof(Patch) };
            foreach (var restMethod in restMethods)
            {
                // Get custom attribute 
                // See: https://stackoverflow.com/a/6538438/12347616
                var customAttribute = method.GetCustomAttribute(restMethod);
                if (!(customAttribute is IMethod attribute)) continue;
                var parameters = method.GetParameters();
                if (parameters.Length > 4)
                {
                    Console.Error.WriteLine("Err: Wrong endpoint method syntax - more than four parameters");
                    Console.Error.WriteLine("Err: Please correct method " + method.Name + " from Class " +
                                            controller.GetType().FullName + " to restore functionality");
                    break;
                }

                var mappingsParam = new List<MappingParams>();
                var pathVariableType = (Type?)null;
                var error = false;
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType == typeof(AuthDetails))
                    {
                        mappingsParam.Add(MappingParams.Auth);
                    }
                    else if (parameter.ParameterType == typeof(string))
                    {
                        mappingsParam.Add(MappingParams.Text);
                    }
                    else if (parameter.ParameterType == typeof(Dictionary<string, object>))
                    {
                        mappingsParam.Add(MappingParams.Json);
                    }
                    // In order to check generic types you must compare the original generic type,
                    // not the concrete one
                    // See: https://stackoverflow.com/a/457708/12347616
                    else if (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(PathVariable<>))
                    {
                        mappingsParam.Add(MappingParams.PathVariable);
                        pathVariableType = parameter.ParameterType.GenericTypeArguments[0];
                    }
                    else if (parameter.ParameterType == typeof(RequestParam))
                    {
                        mappingsParam.Add(MappingParams.RequestParam);
                    }
                    else
                    {
                        Console.Error.WriteLine("Err: Wrong endpoint method syntax - usage of not supported type " +
                                                mappingsParam.GetType());
                        Console.Error.WriteLine("Err: Please correct method " + method.Name + " from Class " +
                                                controller.GetType().FullName + " to restore functionality");
                        error = true;
                        break;
                    }
                }
                // Check if some parameters are duplicate
                // See: https://stackoverflow.com/a/972323/12347616
                var mappingCheck = Enum.GetValues(typeof(MappingParams)).Cast<MappingParams>();
                foreach (var mapping in mappingCheck)
                {
                    // Count occurence
                    // See: https://stackoverflow.com/a/21579086/12347616
                    var count = mappingsParam.Count(c => c == mapping);
                    if (count <= 1) continue;
                    Console.Error.WriteLine("Err: Duplicate parameter of for action " + mapping + " defined");
                    Console.Error.WriteLine("Err: Please correct method " + method.Name + " from Class " +
                                            controller.GetType().FullName + " to restore functionality");
                    error = true;
                    break;
                }
                if (error) break;

                var path = attribute.Path;
                var methodCaller = new MethodCaller(method, controller, mappingsParam, pathVariableType);
                mappings[MethodUtilities.GetMethod(restMethod)].Add(path, methodCaller);
                break;
            }
        }

        /// <summary>
        /// Checks if an endpoint for the given path exists.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>True if an endpoint is mapped, else false.</returns>
        public bool Contains(Method method, string path)
        {
            return mappings[method].ContainsKey(path);
        }

        /// <summary>
        /// Invokes the method for the corresponding method and path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="authDetails"></param>
        /// <param name="payload"></param>
        /// <param name="method"></param>
        /// <param name="pathVariable"></param>
        /// <param name="requestParam"></param>
        /// <exception>
        /// Throws an exception when the given endpoint does not exist.
        /// </exception>
        /// <returns>Response as a Response object</returns>
        public Response Invoke(
            Method method, string path, AuthDetails? authDetails, object? payload, 
            string? pathVariable, string? requestParam
        )
        {
            if (mappings[method].ContainsKey(path)) 
                return mappings[method][path].Invoke(authDetails, payload, pathVariable, requestParam);
            throw new EndpointNotFoundException();
        }


        /// <summary>
        /// Wrapper that is used to call the methods that define service endpoints.
        /// </summary>
        public class MethodCaller
        {
            private readonly MethodInfo method;
            private readonly object instance;
            private readonly List<MappingParams> paramInfo;
            private readonly Type? pathVariableType;

            public MethodCaller(MethodInfo method, object instance, List<MappingParams> paramInfo, Type? pathVariableType)
            {
                this.method = method;
                this.instance = instance;
                this.paramInfo = paramInfo;
                this.pathVariableType = pathVariableType;
            }

            /// <summary>
            /// Invokes the method wrapped in the <c>MethodCaller</c>.
            /// </summary>
            /// <param name="authDetails"></param>
            /// <param name="payload"></param>
            /// <param name="pathVariable"></param>
            /// <param name="requestParam"></param>
            /// <returns>Response as a Response object</returns>
            public Response Invoke(AuthDetails? authDetails, object? payload, string? pathVariable, string? requestParam)
            {
                var parameters = new List<object?>();
                foreach (var param in paramInfo)
                {
                    switch (param)
                    {
                        case MappingParams.Auth:
                            parameters.Add(authDetails);
                            break;
                        case MappingParams.Json:
                            parameters.Add(payload is Dictionary<string, object> ? payload : null);
                            break;
                        case MappingParams.Text:
                            parameters.Add(payload is string ? payload : null);
                            break;
                        case MappingParams.PathVariable:
                            // Make generic path parameter instance
                            // See: https://stackoverflow.com/a/43921901/12347616
                            var pathParamGenericType = typeof(PathVariable<>);
                            var constructType = pathParamGenericType.MakeGenericType(pathVariableType);
                            var pathParamObj = Activator.CreateInstance(constructType, pathVariable);
                            parameters.Add(pathParamObj);
                            break;
                        case MappingParams.RequestParam:
                            parameters.Add(new RequestParam(requestParam));
                            break;
                    }
                }
                // Invoke method
                // See: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodbase.invoke?view=netcore-3.1
                return (Response)method.Invoke(instance, parameters.ToArray());
            }

        }

        /// <summary>
        /// Enum is used to declare valid parameters in <c>MethodCaller</c>.
        /// </summary>
        public enum MappingParams
        {
            Auth,
            Text,
            Json,
            PathVariable,
            RequestParam
        }
    }
}