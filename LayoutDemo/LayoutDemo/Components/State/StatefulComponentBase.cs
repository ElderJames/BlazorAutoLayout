using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Text.Json;

namespace LayoutDemo.Components.State
{
    public class StatefulComponentBase : ComponentBase
    {
        [Inject] public StateService State { get; set; }

        protected int Id { get; set; }

        [Inject] private IFormValueMapper FormValueMapper { get; set; }

        [Inject] private IComponentPrerenderer ComponentPrerenderer { get; set; }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (Id <= 0)
            {
                Id = State.GetIndex();
            }

            State.Restore(Id, this);

            return base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender()
        {
            State.StateChange($"view_state_{Id}", this);

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