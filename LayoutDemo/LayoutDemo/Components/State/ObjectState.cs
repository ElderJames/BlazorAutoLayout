using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json;

namespace LayoutDemo.Components.State
{
    [Serializable]
    public class ObjectState
    {
        public Dictionary<string, object> Properties { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }

    public static class ObjectStateHelper
    {
        public static ObjectState GetObjectState<T>(T obj)
        {
            var state = new ObjectState
            {
                Properties = new Dictionary<string, object>(),
                Fields = new Dictionary<string, object>()
            };

            // 获取所有公有属性
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(RenderFragment))
                {
                    continue;
                }

                if (property.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                state.Properties[property.Name] = property.GetValue(obj);
            }

            // 获取所有字段，包括私有字段
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(RenderFragment))
                {
                    continue;
                }

                if (field.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                state.Fields[field.Name] = field.GetValue(obj);
            }

            return state;
        }

        public static void SetObjectState<T>(T obj, ObjectState state)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (state == null) throw new ArgumentNullException(nameof(state));

            // 设置属性的值
            foreach (var propertyPair in state.Properties)
            {
                try
                {
                    var propertyInfo = obj.GetType().GetProperty(propertyPair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (propertyInfo.GetCustomAttribute<NotMappedAttribute>() != null)
                        continue;

                    propertyInfo?.SetValue(obj, ((JsonElement)propertyPair.Value).Deserialize(propertyInfo.PropertyType));
                }
                catch
                {
                    continue;
                }
            }

            // 设置字段的值
            foreach (var fieldPair in state.Fields)
            {
                try
                {
                    var fieldInfo = obj.GetType().GetField(fieldPair.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo.GetCustomAttribute<NotMappedAttribute>() != null)
                        continue;

                    fieldInfo?.SetValue(obj, ((JsonElement)fieldPair.Value).Deserialize(fieldInfo.FieldType));
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}