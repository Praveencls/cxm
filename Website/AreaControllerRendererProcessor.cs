using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;

namespace Website
{
    public class AreaControllerRendererProcessor : GetRendererProcessor
    {
        public virtual string TemplateId { get; set; }

        public override void Process(GetRendererArgs args)
        {
            if (args.Result != null)
            {
                return;
            }

            Template renderingTemplate = args.RenderingTemplate;
            if (renderingTemplate == null)
            {
                return;
            }
            if (!renderingTemplate.DescendsFromOrEquals(new ID(TemplateId)))
            {
                return;
            }
            args.Result = this.GetRenderer(args.Rendering, args);
        }

        protected virtual Renderer GetRenderer(Rendering rendering, GetRendererArgs args)
        {
            string action = rendering.RenderingItem.InnerItem.Fields["controller action"].GetValue(true);
            string controller = rendering.RenderingItem.InnerItem.Fields["controller"].GetValue(true);
            string area = rendering.RenderingItem.InnerItem.Fields["area"].GetValue(true);
            string namespaceNames = rendering.RenderingItem.InnerItem.Fields["namespace"].GetValue(true);

            return new AreaControllerRenderer
            {
                Action = action,
                Controller = controller,
                Area = area,
                Namespaces = namespaceNames
            };
        }
    }
}