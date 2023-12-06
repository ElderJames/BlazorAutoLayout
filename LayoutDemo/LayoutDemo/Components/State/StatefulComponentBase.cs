using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Text.Json;

namespace LayoutDemo.Components.State
{
    public class StatefulComponentBase : ComponentBase
    {
        [SupplyParameterFromForm(Name = "view_state")]
        public string ViewState { get; set; }

        [Inject] public StateService State { get; set; }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            var viewState = parameters.GetValueOrDefault<string>(nameof(ViewState));
            if (!string.IsNullOrWhiteSpace(viewState))
            {
                var state = JsonSerializer.Deserialize<ObjectState>(viewState);
                ObjectStateHelper.SetObjectState(this, state);
            }

            return base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender()
        {
            //// 获取BaseClass类型的Type对象
            //Type baseType = typeof(ComponentBase);

            //// 使用反射获取私有字段的FieldInfo
            //FieldInfo fieldInfo = baseType.GetField("_renderFragment", BindingFlags.NonPublic | BindingFlags.Instance);

            //if (fieldInfo != null)
            //{
            //    var old = fieldInfo.GetValue(this);
            //    // 使用反射来设置私有字段的值
            //    fieldInfo.SetValue(this, NewRenderFragment((RenderFragment)old));
            //}
            var index = State.GetIndex();
            State.StateChange($"view_state_{index}", ObjectStateHelper.GetObjectState(this));

            return base.ShouldRender();
        }

        private RenderFragment NewRenderFragment(RenderFragment old)
        {
            return builder =>
            {
                builder.AddContent(0, old);

                builder.OpenElement(10, "input");
                builder.AddAttribute(11, "id", "view_state");
                builder.AddAttribute(12, "type", "hidden");
                builder.AddAttribute(13, "value", JsonSerializer.Serialize(ObjectStateHelper.GetObjectState(this)));

                builder.CloseElement();
            };
        }
    }
}