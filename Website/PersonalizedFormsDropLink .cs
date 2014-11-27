using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls.Data;
using Sitecore.Web.UI.Sheer;



namespace Website
{
    public class PersonalizedFormsDropLink  : Input
    {
       // EloquaConnector eConnect = new EloquaConnector();

        public PersonalizedFormsDropLink()
        {
            this.Class = "scContentControl";
            base.Activation = true;
        }
        protected virtual string NameStyle
        {
            get
            {
                return "width:150px";
            }
        }
        /// <summary>
        /// Is control vertical
        /// </summary>
        protected virtual bool IsVertical
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="output">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.
        /// </param>
        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            base.SetWidthAndHeightStyle();
            output.Write("<div" + base.ControlAttributes + ">");
            this.RenderChildren(output);
            output.Write("</div>");
        }
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"></see> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (Sitecore.Context.ClientPage.IsEvent)
            {               
                this.LoadValue();
                return;
            }
            this.BuildControl();
        }
        /// <summary>
        /// Parameters the change.
        /// </summary>
        [UsedImplicitly]
        protected void ParameterChange()
        {
            ClientPage clientPage = Sitecore.Context.ClientPage;
            if (clientPage.ClientRequest.Source == StringUtil.GetString(clientPage.ServerProperties[this.ID + "_LastParameterID"]))
            {
                string value = clientPage.ClientRequest.Form[clientPage.ClientRequest.Source];
                if (!string.IsNullOrEmpty(value))
                {                    
                    string value2 = this.BuildParameterKeyValue(string.Empty, string.Empty, true, value);
                    string PrevDropdownId = StringUtil.GetString(clientPage.ServerProperties[this.ID + "_PrevChildID"]);                                     
                    clientPage.ClientResponse.Insert(this.ID, "beforeEnd", value2);
                    clientPage.ClientResponse.Remove("dpDefault");
                    clientPage.ClientResponse.Remove(PrevDropdownId);
                }
                else
                {
                    var emptyDp = CreateEmptyDropDown();
                    clientPage.ClientResponse.Insert(this.ID, "beforeEnd", emptyDp);
                    clientPage.ClientResponse.Remove(StringUtil.GetString(clientPage.ServerProperties[this.ID + "_CurrentChildID"]));
                }
            }
            NameValueCollection nameValueCollection = null;
            System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
            if (page != null)
            {
                nameValueCollection = page.Request.Form;
            }
            if (nameValueCollection == null)
            {
                return;
            }
            if (this.Validate(nameValueCollection))
            {
                clientPage.ClientResponse.SetReturnValue(true);
            }
        }
        /// <summary>
        /// Sets the modified flag.
        /// </summary>
        protected override void SetModified()
        {
            base.SetModified();
            if (base.TrackModified)
            {
                Sitecore.Context.ClientPage.Modified = true;
            }
        }
        /// <summary>
        /// Builds the control.
        /// </summary>
        private void BuildControl()
        {
            string[] myarray = this.Value.Split('|');
            if (myarray.Length > 1)
            {
                this.Controls.Add(new LiteralControl(this.BuildParameterKeyValue(myarray[0], "")));                
                this.Controls.Add(new LiteralControl(this.BuildParameterKeyValue(myarray[1], "", true, myarray[0])));
            }
            else
            {
                this.Controls.Add(new LiteralControl(this.BuildParameterKeyValue(string.Empty, string.Empty)));
                this.Controls.Add(new LiteralControl(CreateEmptyDropDown()));
            }
        }
        /// <summary>
        /// Builds the parameter key value.
        /// </summary>
        /// <param name="key">
        /// The parameter key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The parameter key value.
        /// </returns>
        /// <contract><requires name="key" condition="not null" /><requires name="value" condition="not null" /><ensures condition="not null" /></contract>
        private string BuildParameterKeyValue(string key, string value, bool Isparameter = false, string selectedValue = "")
        {
            Assert.ArgumentNotNull(key.Trim(), "key");
            Assert.ArgumentNotNull(value, "value");
            string uniqueID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID(this.ID + "_Param");
            if (!Isparameter)
            {
                Sitecore.Context.ClientPage.ServerProperties[this.ID + "_LastParameterID"] = uniqueID;
                //Sitecore.Context.ClientPage.ServerProperties[this.ID + "_LastValue"] = GetLatestValue();
            }
            else
            {
                if (Sitecore.Context.ClientPage.ServerProperties[this.ID + "_CurrentChildID"] != null)
                    Sitecore.Context.ClientPage.ServerProperties[this.ID + "_PrevChildID"] = Sitecore.Context.ClientPage.ServerProperties[this.ID + "_CurrentChildID"].ToString();
                Sitecore.Context.ClientPage.ServerProperties[this.ID + "_CurrentChildID"] = uniqueID;

            }
            string clientEvent = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".ParameterChange");
            string text = this.ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string text2 = this.Disabled ? " disabled=\"disabled\"" : string.Empty;
            string arg = this.IsVertical ? "</tr><tr>" : string.Empty;
            string valueHtmlControlValue = Isparameter ?
                this.GetValueHtmlControlValueForSecondDropDown(uniqueID, StringUtil.EscapeQuote(HttpUtility.UrlDecode(key)), clientEvent, selectedValue)
                : this.GetValueHtmlControlValue(uniqueID, StringUtil.EscapeQuote(HttpUtility.UrlDecode(key)), clientEvent);
            return valueHtmlControlValue;
        }
        /// <summary>
        /// Loads the post data.
        /// </summary>
        private void LoadValue()
        {
            if (this.ReadOnly || this.Disabled)
            {
                return;
            }
            System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
            NameValueCollection nameValueCollection;
            if (page != null)
            {
                nameValueCollection = page.Request.Form;
            }
            else
            {
                nameValueCollection = new NameValueCollection();
            }
            UrlString urlString = new UrlString();
            string textValue = string.Empty;
            foreach (string text in nameValueCollection.Keys)
            {
                if (!string.IsNullOrEmpty(text) && text.StartsWith(this.ID + "_Param") && !text.EndsWith("_value"))
                {
                    string text2 = nameValueCollection[text];
                    textValue += text2 + "|";
                }
            }
            
            string text4="";
            if(textValue.Length!=0)
                text4 = textValue.Substring(0, textValue.Length - 1);// urlString.ToString();            
            text4=Regex.Replace(text4, "=", "");
            if (this.Value == text4)
            {
                return;
            }
            this.Value = text4;
            this.SetModified();
        }
        /// <summary>
        /// Validates the specified client page.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns>The result of the validation.</returns>
        private bool Validate(NameValueCollection form)
        {
            Assert.ArgumentNotNull(form, "form");
            foreach (string text in form.Keys)
            {
                if (text != null && text.StartsWith(this.ID + "_Param") && !text.EndsWith("_value"))
                {
                    string text2 = form[text];
                    string text3 = form[text + "_value"];
                    if (string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3))
                    {
                        SheerResponse.Alert(string.Format("The key can not be empty for the value{0}.", text3), new string[0]);
                        SheerResponse.SetReturnValue(false);
                        return false;
                    }
                }
            }
            return true;
        }

        public string ItemID
        {
            get
            {
                return base.GetViewStateString("ItemID");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                base.SetViewStateString("ItemID", value);
            }
        }
        public string FieldName
        {
            get
            {
                return base.GetViewStateString("FieldName");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                base.SetViewStateString("FieldName", value);
            }
        }
        protected string NameStyleValue
        {
            get
            {
                return "width:150px;background-color:lightgrey'";
            }
        }
        public string Source
        {
            get
            {
                return base.GetViewStateString("Source");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                base.SetViewStateString("Source", value);
            }
        }



        protected virtual Item[] GetItems(Item current)
        {
            Assert.ArgumentNotNull(current, "current");
            return LookupSources.GetItems(current, this.Source);
        }

        protected virtual string GetItemHeader(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            string @string = StringUtil.GetString(new string[]
			{
				this.FieldName
			});
            string result;
            if (@string.StartsWith("@"))
            {
                result = item[@string.Substring(1)];
            }
            else
            {
                if (@string.Length > 0)
                {
                    result = item[this.FieldName];
                }
                else
                {
                    result = item.DisplayName;
                }
            }
            return result;
        }

        protected virtual string GetItemValue(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return item.ID.ToString();
        }
        protected virtual bool IsSelected(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return this.Value == item.ID.ToString() || this.Value == item.Paths.LongID;
        }

        protected string GetValueHtmlControlValue(string id, string value, string clientEvent)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter());
            htmlTextWriter.Write(string.Concat(new string[]
			{
                "<div style=\"margin:5px;\" id=\"",
                id,
                "\">",
                "<span style=\"margin:2px;font-weight:bold;font-size:11px;\">Business Form:</span>",
				"<select id=\"",
                id,
                "\" name=\"",
				id,	
			     "\" onchange=\"",                
				clientEvent,
                 "\"",
				this.GetControlAttributes(),
				">"
			}));
            htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(value) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");
            List<DropDownDTO> FormListData = this.ItemList;
            if (FormListData != null)
            {
                foreach (DropDownDTO form in FormListData)
                {
                    bool flag = form.FieldID.ToString().Equals(value);
                    htmlTextWriter.Write(string.Concat(new string[]
				{
					"<option value=\"",
					form.FieldID.ToString(),
					"\"",
					flag ? " selected=\"selected\"" : string.Empty,
					">",
					form.FieldName,
					"</option>"
				}));
                }
            }
            htmlTextWriter.Write("</select></div>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        protected string GetValueHtmlControlValueForSecondDropDown(string id, string value, string clientEvent, string selectedOption)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter());
            htmlTextWriter.Write(string.Concat(new string[]
			{
				"<div style=\"margin:5px;\" id=\"",
                id,
                "\">",
                "<span style=\"margin:2px;font-weight:bold;font-size:11px;\">Form Fields:</span>",
				"<select id=\"",
                id,
                "name=\"",
				id,	
			     "\" onchange=\"",                
				clientEvent,
                 "\"",
				this.GetControlAttributes(),
				">"
			}));
            htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(value) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");
            List<DropDownDTO> fieldsList = SecondItemList(selectedOption);
            if (fieldsList != null)
            {
                foreach (DropDownDTO fields in fieldsList)
                {
                    bool flag = fields.FieldID.ToString().Equals(value);
                    htmlTextWriter.Write(string.Concat(new string[]
				{
					"<option value=\"",
					fields.FieldID.ToString(),
					"\"",
					flag ? " selected=\"selected\"" : string.Empty,
					">",
					fields.FieldName,
					"</option>"
				}));
                }
            }
            htmlTextWriter.Write("</select></div>");
            return htmlTextWriter.InnerWriter.ToString();
        }
        private string CreateEmptyDropDown()
        {
            Assert.ArgumentNotNull(string.Empty, "key");
            Assert.ArgumentNotNull(string.Empty, "value");
            string id = "dpDefault";// Sitecore.Web.UI.HtmlControls.Control.GetUniqueID(this.ID + "_Param");
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter());
            htmlTextWriter.Write(string.Concat(new string[]
			{				
                "<div style=\"margin:5px;\" id=\"",
                id,
                "\">",
                "<span style=\"margin:2px;font-weight:bold;font-size:11px;\">Form Fields</span>",
				"<select name=\"",
				id+"name",	
			     "\"", 
				this.GetControlAttributes(),
				">"
			}));
            htmlTextWriter.Write("<option" + (" selected=\"selected\"") + " value=\"\">&nbsp;</option>");
            htmlTextWriter.Write("</select>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        private List<DropDownDTO> FormList = null;
        public List<DropDownDTO> ItemList
        {

            get
            {                            
                if (FormList == null)
                {
                   // DescribeEntityTypeResult list = eConnect.GetAllEloquaForms();
                    List<DropDownDTO> DropDownDTOList = new List<DropDownDTO>();//list.EntityTypes.Where(x => x != null).Select(CreateFormDropDownDTO).ToList();                    
                    DropDownDTOList.Add(new DropDownDTO { FieldID = "1", FieldName = "One Parent" });
                    DropDownDTOList.Add(new DropDownDTO { FieldID = "2", FieldName = "Two Parent" });
                    FormList = DropDownDTOList;
                }
                return FormList;
            }
            set
            {

            }
        }

        //private DropDownDTO CreateFormDropDownDTO(EntityType et)
        //{
        //    return new DropDownDTO
        //    {
        //        FieldID = et.ID.ToString(),
        //        FieldName = et.Name
        //    };
        //}
        public List<DropDownDTO> SecondItemList(string selectedValue)
        {
            if (!string.IsNullOrEmpty(selectedValue))
            {
                List<DropDownDTO> DropDownDTOList = new List<DropDownDTO>();
               // var list = eConnect.GetEloquaForm(System.Convert.ToInt32(selectedValue));
                if (selectedValue == "1")
                {
                    DropDownDTOList.Add(new DropDownDTO { FieldID = "1", FieldName = "One Child" });
                    DropDownDTOList.Add(new DropDownDTO { FieldID = "2", FieldName = "Two Child" });
                }

                if (selectedValue == "2")
                {
                    DropDownDTOList.Add(new DropDownDTO { FieldID = "3", FieldName = "Three Child" });
                    DropDownDTOList.Add(new DropDownDTO { FieldID = "4", FieldName = "Four Child" });
                }
               // DropDownDTOList = list.Where(x=>x != null).Select(CreateFieldsDropDownList).ToList();
                return DropDownDTOList;
            }
            return null;

        }
        //private DropDownDTO CreateFieldsDropDownList(DynamicEntityFieldDefinition field)
        //{
        //    return new DropDownDTO
        //    {
        //        FieldID=field.InternalName,
        //        FieldName=field.DisplayName
        //    };

        //}       

    }
}