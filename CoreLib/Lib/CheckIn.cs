using System;
using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Globalization;
using Sitecore;

namespace Elma.SitecoreUtil.Commands
{
    [Serializable]
    public class CheckIn : Command
    {
        // Methods
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                NameValueCollection parameters = new NameValueCollection();
                parameters["id"] = item.ID.ToString();
                parameters["language"] = item.Language.ToString();
                parameters["version"] = item.Version.ToString();
                Context.ClientPage.Start(this, "Run", parameters);
            }
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }
            Item item = context.Items[0];
            string unlockerRole = Sitecore.Configuration.Settings.GetSetting(Constants.SETTING_CHECKIN_ROLE);
            bool userCanUnlock = Context.User.IsInRole(unlockerRole);
            if (Context.IsAdministrator || userCanUnlock)
            {
                if (!item.Locking.IsLocked())
                {
                    return CommandState.Hidden;
                }
                return CommandState.Enabled;
            }
            if (item.Appearance.ReadOnly)
            {
                return CommandState.Disabled;
            }
            if (!item.Access.CanWrite())
            {
                return CommandState.Disabled;
            }
            if (!item.Locking.HasLock())
            {
                return CommandState.Disabled;
            }
            return base.QueryState(context);
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (SheerResponse.CheckModified())
            {
                string itemPath = args.Parameters["id"];
                string name = args.Parameters["language"];
                string str3 = args.Parameters["version"];
                Item item = Client.GetItemNotNull(itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(str3));
                string unlockerRole = Sitecore.Configuration.Settings.GetSetting(Constants.SETTING_CHECKIN_ROLE);
                bool userCanUnlock = Context.User.IsInRole(unlockerRole);
                if (item.Locking.HasLock() || Context.IsAdministrator || userCanUnlock)
                {
                    Log.Audit(this, "Check in: {0}", new string[] { AuditFormatter.FormatItem(item) });
                    item.Editing.BeginEdit();
                    using (new SecurityDisabler())
                    {
                        item.Locking.Unlock();
                    }
                    item.Editing.EndEdit();
                    Context.ClientPage.SendMessage(this, "item:checkedin");
                }
            }
        }
    }
}
