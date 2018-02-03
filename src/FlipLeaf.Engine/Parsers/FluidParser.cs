using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Fluid.Tags;

namespace FlipLeaf.Parsers
{
    public class FluidParser
    {
        private readonly SiteSettings _settings;
        private readonly RenderContext ctx;

        public FluidParser(SiteSettings settings, RenderContext ctx)
        {
            _settings = settings;
            this.ctx = ctx;
        }

        public string ParseContent(string content, object pageContext, out TemplateContext context)
        {
            context = null;

            if (!ViewTemplate.TryParse(content, out var template))
            {
                throw new ParseException();
            }

            context = new TemplateContext { MemberAccessStrategy = new IgnoreCaseMemberAccessStrategy() };
            context.MemberAccessStrategy.Register<SiteSettings>();
            context.SetValue("page", pageContext);
            context.SetValue("site", this._settings);

            return template.Render(context);
        }

        public string ApplyLayout(string source, TemplateContext context)
        {
            var layoutFile = context.GetValue("page").GetValue("layout", context).ToStringValue();
            if (layoutFile == null)
            {
                return source;
            }

            if (Path.GetExtension(layoutFile) == "")
                layoutFile += ".html";

            var layoutText = File.ReadAllText(Path.Combine(this.ctx.InputDir, this._settings.LayoutFolder, layoutFile));
            context.AmbientValues.Add("Body", source);
            if (!ViewTemplate.TryParse(layoutText, out var layoutTemplate))
            {
                throw new ParseException();
            }

            return layoutTemplate.Render(context);
        }

        class IgnoreCaseMemberAccessStrategy : IMemberAccessStrategy
        {
            private Dictionary<string, IMemberAccessor> _map = new Dictionary<string, IMemberAccessor>(StringComparer.OrdinalIgnoreCase);


            public IMemberAccessor GetAccessor(object obj, string name)
            {
                // Look for specific property map
                if (_map.TryGetValue(Key(obj.GetType(), name), out var getter))
                {
                    return getter;
                }

                // Look for a catch-all getter
                if (_map.TryGetValue(Key(obj.GetType(), "*"), out getter))
                {
                    return getter;
                }

                return null;
            }

            public void Register(Type type, string name, IMemberAccessor getter)
            {
                _map[Key(type, name)] = getter;
            }

            private string Key(Type type, string name) => $"{type.Name}.{name}";
        }

        class ViewTemplate : BaseFluidTemplate<ViewTemplate>
        {
            static ViewTemplate()
            {
                Factory.RegisterTag<RenderBodyTag>("renderbody");
            }
        }

        public class RenderBodyTag : SimpleTag
        {
            public override async Task<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context)
            {
                if (context.AmbientValues.TryGetValue("Body", out var body))
                {
                    await writer.WriteAsync((string)body);
                }
                else
                {
                    throw new ParseException("Could not render body, Layouts can't be evaluated directly.");
                }

                return Completion.Normal;
            }
        }
    }
}