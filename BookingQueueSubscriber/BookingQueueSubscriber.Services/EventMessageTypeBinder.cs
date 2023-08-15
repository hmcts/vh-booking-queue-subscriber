using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace BookingQueueSubscriber.Services
{
    public class EventMessageTypeBinder : ISerializationBinder
    {
        private readonly Dictionary<string, Type> _typesBySimpleName;
        private readonly Dictionary<Type, string> _simpleNameByType = new Dictionary<Type, string>();

        public EventMessageTypeBinder(Assembly assembly)
        {
            _typesBySimpleName = new Dictionary<string, Type>();

            foreach (var type in assembly.GetTypes().Where(t => t.IsPublic))
            {
                if (_typesBySimpleName.ContainsKey(type.Name))
                {
                    throw new InvalidOperationException(
                        "Cannot user PolymorphicBinder on a namespace where multiple public types have same name.");
                }

                _typesBySimpleName[type.Name] = type;
                _simpleNameByType[type] = type.Name;
            }
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            var typeShortName = typeName.Trim().Split('.').Last();
            var bindToType = _typesBySimpleName.ContainsKey(typeShortName) ? _typesBySimpleName[typeShortName] : null;
            return bindToType;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            if (_simpleNameByType.TryGetValue(serializedType, out var name))
            {
                typeName = name;
                assemblyName = null;
            }
            else
            {
                typeName = null;
                assemblyName = null;
            }
        }
    }
}