using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Text.Json;

namespace LayoutDemo.Components.State
{
    public class StateService
    {
        public event Action<string, ObjectState> OnStateChanged;

        private Dictionary<string, StatefulComponentBase> componentCache = new();

        private int counter = 0;

        private IFormValueMapper _formValueMapper;

        public StateService(IFormValueMapper formValueMapper)
        {
            _formValueMapper = formValueMapper;
        }

        public int GetIndex()
        {
            Interlocked.Increment(ref counter);
            return counter;
        }

        public void Reset()
        {
            counter = 0;
        }

        public void StateChange(string formName, StatefulComponentBase component)
        {
            componentCache.Add(formName, component);

            ObjectState state = ObjectStateHelper.GetObjectState(component);
            OnStateChanged?.Invoke(formName, state);
        }

        public void Restore(int id, StatefulComponentBase component)
        {
            var type = _formValueMapper.GetType();
            FieldInfo propertyInfo = type.GetField("_formData", BindingFlags.NonPublic | BindingFlags.Instance);

            var _formData = propertyInfo.GetValue(_formValueMapper);

            var formDataType = _formData.GetType();
            var _entriesField = formDataType.GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);

            var _entries = _entriesField.GetValue(_formData) as IReadOnlyDictionary<string, StringValues>;

            if (_entries == null)
            {
                return;
            }

            if (_entries.TryGetValue($"view_state_{id}", out var state))
            {
                var viewState = JsonSerializer.Deserialize<ObjectState>(state);
                ObjectStateHelper.SetObjectState(component, viewState);
            }
        }
    }
}