using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Elma.SitecoreUtil.Commands
{
    [Serializable]
    public class AddVersionRecursiveCommand : Command
    {
        private HashSet<ID> LinkSet;

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items != null && context.Items.Length == 1)
            {
                Context.ClientPage.Start(this, "Run", context.Parameters);
            }
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            string id = args.Parameters["id"];
            Language sourceLang = Language.Parse(args.Parameters["sourceLang"]);
            Language targetLang = Language.Parse(args.Parameters["targetLang"]);
            LinkSet = new HashSet<ID>();

            Item item = Context.ContentDatabase.GetItem(id, targetLang);
            if (item == null)
            {
                return;
            }

            if (Context.IsAdministrator || (item.Access.CanWrite() && (item.Locking.CanLock() || item.Locking.HasLock())))
            {
                if (SheerResponse.CheckModified())
                {
                    AddAllReferences(item);

                    LayoutField layoutField = item.Fields[FieldIDs.LayoutField];
                    if (!string.IsNullOrEmpty(layoutField.Value))
                    {
                        LayoutDefinition layout = LayoutDefinition.Parse(layoutField.Value);

                        foreach (DeviceDefinition device in layout.Devices)
                        {
                            foreach (RenderingDefinition rendering in device.Renderings)
                            {
                                Item datasourceItem = Context.ContentDatabase.GetItem(rendering.Datasource ?? string.Empty, targetLang);
                                if (datasourceItem == null)
                                {
                                    continue;
                                }

                                AddAllReferences(datasourceItem);

                                if (ChildrenGroupingTemplates.Contains(datasourceItem.TemplateName))
                                {
                                    foreach (Item childItem in datasourceItem.Children)
                                    {
                                        AddAllReferences(childItem);
                                    }
                                }
                            }
                        }
                    }

                    foreach (ID linkId in LinkSet)
                    {
                        CopyVersion(linkId, sourceLang, targetLang);
                    }
                }
            }
        }

        private void AddAllReferences(Item item)
        {
            if (LinkSet.Add(item.ID))
            {
                foreach (Item reference in item.Links.GetValidLinks(false).Select(il => il.GetTargetItem()))
                {
                    if (reference.Paths.IsContentItem || reference.Paths.IsMediaItem)
                    {
                        LinkSet.Add(reference.ID);
                    }
                }
            }
        }

        private void CopyVersion(ID id, Language sourceLang, Language targetLang)
        {
            Item source = Context.ContentDatabase.GetItem(id, sourceLang);
            Item target = Context.ContentDatabase.GetItem(id, targetLang);

            if (source == null || target == null || target.Versions.Count > 0)
            {
                return;
            }

            target.Versions.AddVersion();
            target.Editing.BeginEdit();
            foreach (Field field in source.Fields)
            {
                if (!field.Shared)
                {
                    target[field.Name] = source[field.Name];
                }
            }
            target.Editing.EndEdit();
        }

        protected static IEnumerable<string> ChildrenGroupingTemplates
        {
            get
            {
                string rawSetting = Settings.GetSetting("VersionFromLanguage.ChildrenGroupingTemplates", string.Empty);
                return rawSetting.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
            }
        }
    }
}
