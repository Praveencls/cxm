using System;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Elma.SitecoreUtil.Commands
{
    [Serializable]
    public class RibbonCopyFromLanguage : Command
    {
        public override void Execute(CommandContext context)
        {
        }

        public override string GetClick(CommandContext context, string click)
        {
            return string.Empty;
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }
            Item item = context.Items[0];
            if (new[] { TemplateIDs.Template, TemplateIDs.TemplateSection, TemplateIDs.TemplateField }.Contains(item.TemplateID))
            {
                return CommandState.Hidden;
            }

            if (item.Versions.Count > 0)
            {
                return CommandState.Disabled;
            }
            if (item.Appearance.ReadOnly)
            {
                return CommandState.Disabled;
            }
            if (Context.IsAdministrator)
            {
                return CommandState.Enabled;
            }

            if (!item.Access.CanWrite())
            {
                return CommandState.Disabled;
            }
            if (!item.Locking.CanLock() && !item.Locking.HasLock())
            {
                return CommandState.Disabled;
            }
            if (!item.Access.CanWriteLanguage())
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }
    }
}
