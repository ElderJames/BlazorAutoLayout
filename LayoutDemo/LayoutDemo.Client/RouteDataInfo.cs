using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace LayoutDemo.Client
{
    public class RouteDataInfo
    {
        public string PageTypeName { get; set; }
        public IReadOnlyDictionary<string, object?> RouteValues { get; set; }

        public RouteDataInfo() { }

      
        public RouteDataInfo(RouteData routeData)
        {
            PageTypeName = routeData.PageType.FullName;
            RouteValues = routeData.RouteValues;
        }

        public RenderFragment RenderBody()
        {
            return builder =>
            {
                var pageType = Type.GetType(PageTypeName);
                if (pageType == null)
                {
                    return;
                }

                builder.OpenComponent(0, pageType);
                foreach (var routeValue in RouteValues)
                {
                    builder.AddAttribute(1, routeValue.Key, routeValue.Value);
                }
                builder.CloseComponent();
            };
        }
    }
}