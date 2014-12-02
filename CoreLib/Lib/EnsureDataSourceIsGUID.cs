using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Layouts;
using Sitecore.Data.Fields;

namespace Elma.SitecoreUtil.Events
{
    /// <summary>
    /// Converts a datasource item path to GUID (source: http://www.youtube.com/watch?v=xsahkjcXWYI)
    /// </summary>
    public class EnsureDataSourceIsGUID
    {
        /// <summary>
        /// When an item is saved, ensure any component datasources are saved as GUID not path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnItemSaving(object sender, EventArgs args)
        {
            Item item = Event.ExtractParameter(args, 0) as Item;
            if (item == null)
            {
                return;
            }
            LayoutField layoutField = new LayoutField(item.Fields[Sitecore.FieldIDs.LayoutField]);
            if (!layoutField.InnerField.HasValue)
            {
                return;
            }
            Item existingItem = item.Database.GetItem(
                item.ID,
                item.Language,
                item.Version);
            if (existingItem == null)
            {
                return;
            }
            LayoutField oldLayout = new LayoutField(existingItem.Fields[Sitecore.FieldIDs.LayoutField]);
            if (string.IsNullOrEmpty(layoutField.Value) || layoutField.Value == oldLayout.Value)
            {
                return;
            }
            LayoutDefinition layout = LayoutDefinition.Parse(layoutField.Value);
            for (int i = 0; i < layout.Devices.Count; i++)
            {
                DeviceDefinition device = layout.Devices[i] as DeviceDefinition;
                for (int j = 0; j < device.Renderings.Count; j++)
                {
                    RenderingDefinition rendering = device.Renderings[j] as RenderingDefinition;
                    if (!string.IsNullOrEmpty(rendering.Datasource) && rendering.Datasource.StartsWith("/"))
                    {
                        Item datasourceItem = item.Database.GetItem(rendering.Datasource);
                        if (datasourceItem == null)
                        {
                            Sitecore.Diagnostics.Log.Warn(string.Format("Could not find datasource item at path {0} while saving item {1}", rendering.Datasource, item.ID), this);
                            continue;
                        }
                        rendering.Datasource = datasourceItem.ID.ToString();
                    }
                }
            }
            layoutField.Value = layout.ToXml();
        }
    }
}
