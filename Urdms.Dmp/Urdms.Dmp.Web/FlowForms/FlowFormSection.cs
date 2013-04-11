using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Curtin.Framework.Common.Classes;
using Curtin.Framework.Common.Extensions;
using Curtin.Framework.Common.Utils;
using Urdms.Dmp.Web.FlowForms.Helpers;
using Urdms.Dmp.Web.FlowForms.Helpers;

namespace Urdms.Dmp.Web.FlowForms
{
    public class FlowFormSection<TModel> : IDisposable
    {
        #region Private Fields
        private readonly HtmlHelper<TModel> _htmlHelper;
        private readonly bool _nested;
        private readonly bool _omitDl;
        private readonly FlowFormField _parentField;
        private bool HasParent { get { return _parentField != null; } }
        private readonly TextWriter _writer;
        #endregion

        #region Public Methods
        public FlowFormSection(HtmlHelper<TModel> htmlHelper, bool nested, string heading, string id = null, IHtmlString hint = null, string hintClass = null, bool optional = false, IHtmlString instructions = null, bool omitDl = false, FlowFormField parentField = null)
        {
            // Starts a section in a flow form
            // The instance variables allow the form to maintain state for child sections and fields
            //     and to allow the writer to accumulate the output for the section 
            _htmlHelper = htmlHelper;
            _nested = nested;
            _omitDl = omitDl;
            _parentField = parentField;
            _writer = htmlHelper.ViewContext.Writer;

            // Generate the html for the beginning of the section from the class compiled from the Razor template
            var generatedSection = HelperDefinitions.BeginSection(nested, heading, id, hint, hintClass, optional, instructions, omitDl, HasParent);
            _writer.Write(generatedSection.ToHtmlString());

        }

        /// <summary>
        /// Finishes off writing the HTML for the section to the view context writer
        /// </summary>
        public void Dispose()
        {
            // Generate the html for the end of the section from the class compiled from the Razor template
            _writer.Write(HelperDefinitions.EndSection(_omitDl, _nested, HasParent).ToHtmlString());
            if (HasParent)
            {
                _parentField.Dispose();
            }
        }

        /// <summary>
        /// Blank ToString() so an object of this class can be "outputted" in the Razor template.
        /// </summary>
        /// <returns>""</returns>
        public override string ToString()
        {
            return "";
        }

        /// <summary>
        /// Render a basic flow form field with specified HTML for the label and element placeholders.
        /// </summary>
        /// <param name="label">HTML for the label placeholder</param>
        /// <param name="elementHtml">HTML for the element placeholder</param>
        /// <param name="id">If specified the label parameter is wrapped in a &lt;label&gt; with the for attribute having the given id</param>
        /// <returns>A FlowFormField object (has a ToString() that prints "" so can safely have @ in Razor before this call)</returns>
        public FlowFormField Field(string label, IHtmlString elementHtml, string id = null)
        {
            return Field(label, elementHtml, id, false);
        }

        /// <summary>
        /// Render a basic flow form field with specified HTML for the label and element placeholders.
        /// </summary>
        /// <param name="label">HTML for the label placeholder</param>
        /// <param name="elementHtml">HTML for the element placeholder</param>
        /// <param name="id">If specified the label parameter is wrapped in a &lt;label&gt; with the for attribute having the given id</param>
        /// <returns>A FlowFormField object (has a ToString() that prints "" so can safely have @ in Razor before this call)</returns>
        public FlowFormField Field(string label, string elementHtml, string id = null)
        {
            return Field(label, new HtmlString(elementHtml), id, false);
        }

        /// <summary>
        /// Render the start of a basic flow form field with specified HTML for the label and element placeholders.
        /// </summary>
        /// <remarks>
        /// Will render the end of the field after Dispose() is called.<br />
        /// Usually, you would use this within a using block and then use the returned section to output nested fields, e.g.:<br />
        /// using (var ss = s.BeginField("MyLabel", new HtmlString("MyElement")) {<br />
        ///     &#160; &#160; @ss.Field(...)<br />
        /// }
        /// </remarks>
        /// <param name="label">HTML for the label placeholder</param>
        /// <param name="elementHtml">HTML for the element placeholder</param>
        /// <param name="id">If specified the label parameter is wrapped in a &lt;label&gt; with the for attribute having the given id</param>
        /// <param name="sectionId">Adds an id to the &lt;dl&gt; of the nested section</param>
        /// <returns>A FlowFormSection object that will be nested within this field and can be used to create nested fields</returns>
        public FlowFormSection<TModel> BeginField(string label, IHtmlString elementHtml, string id = null, string sectionId = null)
        {
            return new FlowFormSection<TModel>(_htmlHelper, true, null, sectionId, parentField: Field(label, elementHtml, id, true));
        }

        /// <summary>
        /// Render a flow form checkbox field for a Boolean property in the page model.
        /// </summary>
        /// <param name="expression">m => m.BooleanPropertyFromPageModel</param>
        /// <param name="label">The label to add after the checkbox element</param>
        /// <param name="fieldConfiguration">Configuration data for the field</param>
        /// <returns>A FlowFormField object (has a ToString() that prints "" so can safely have @ in Razor before this call)</returns>
        public FlowFormField FieldFor(Expression<Func<TModel, bool>> expression, string label, FieldConfiguration fieldConfiguration = null)
        {

            fieldConfiguration = fieldConfiguration ?? new FieldConfiguration();

            var elementHtml = _htmlHelper.CheckBoxFor(expression, fieldConfiguration.HtmlAttributes).ToHtmlString() +
                _htmlHelper.LabelFor(expression, label);

            string errorHtml;
            var isValid = GetErrors(expression, out errorHtml);

            return new FlowFormField(_writer, false, LabelHtml(expression, fieldConfiguration), elementHtml, errorHtml, isValid, fieldConfiguration.Hint, fieldConfiguration.Tip, fieldConfiguration.HideTip, fieldConfiguration.HintClass, fieldConfiguration.ParentClass, fieldConfiguration.DisplayFieldName);
        }

        /// <summary>
        /// Render the start of a basic flow form field with specified HTML for the label and element placeholders.
        /// </summary>
        /// <remarks>
        /// Will render the end of the field after Dispose() is called.<br />
        /// Usually, you would use this within a using block and then use the returned section to output nested fields, e.g.:<br />
        /// using (var ss = s.BeginField("MyLabel", new HtmlString("MyElement")) {<br />
        ///     &#160; &#160; @ss.Field(...)<br />
        /// }
        /// </remarks>
        /// <param name="expression">m => m.BooleanPropertyFromPageModel</param>
        /// <param name="label">The label to add after the checkbox element</param>
        /// <param name="fieldConfiguration">Configuration data for the field</param>
        /// <param name="sectionId">Adds an id to the &lt;dl&gt; of the nested section</param>
        /// <returns>A FlowFormSection object that will be nested within this field and can be used to create nested fields</returns>
        public FlowFormSection<TModel> BeginFieldFor(Expression<Func<TModel, bool>> expression, string label, FieldConfiguration fieldConfiguration = null, string sectionId = null)
        {
            fieldConfiguration = fieldConfiguration ?? new FieldConfiguration();

            var elementHtml = _htmlHelper.CheckBoxFor(expression, fieldConfiguration.HtmlAttributes).ToHtmlString() +
                _htmlHelper.LabelFor(expression, label);

            string errorHtml;
            var isValid = GetErrors(expression, out errorHtml);

            var field = new FlowFormField(_writer, true, LabelHtml(expression, fieldConfiguration), elementHtml, errorHtml, isValid, fieldConfiguration.Hint, fieldConfiguration.Tip, fieldConfiguration.HideTip, fieldConfiguration.HintClass, fieldConfiguration.ParentClass, fieldConfiguration.DisplayFieldName);
            return new FlowFormSection<TModel>(_htmlHelper, true, null, sectionId, parentField: field);
        }

        public FlowFormField RadioFor(Expression<Func<TModel, bool>> expression, string falseLabel = "No", string trueLabel = "Yes", FieldConfiguration fieldConfiguration = null)
        {
            fieldConfiguration = fieldConfiguration ?? new FieldConfiguration();

            if (fieldConfiguration.As != null & fieldConfiguration.As != ElementType.RadioButtons)
            {
                throw new ApplicationException(string.Format("FieldConfiguration{{ As = {0} }} not valid for field: {1} (which defaults to RadioButton)."
                    , fieldConfiguration.As.Value, expression));
            }

            // var metadata = ModelMetadata.FromLambdaExpression(expression, _htmlHelper.ViewData);
            var value = expression.Compile().Invoke(_htmlHelper.ViewData.Model);
            var elementHtml = HelperDefinitions.BooleanRadio(expression.GetFieldName(), value, falseLabel, trueLabel).ToString();

            elementHtml = (fieldConfiguration.Before ?? "") + elementHtml + (fieldConfiguration.After ?? "");

            return new FlowFormField(_writer, false, LabelHtml(expression, fieldConfiguration), elementHtml, null, true, fieldConfiguration.Hint, fieldConfiguration.Tip, fieldConfiguration.HideTip, fieldConfiguration.HintClass, fieldConfiguration.ParentClass, fieldConfiguration.DisplayFieldName);
        }


        public FlowFormField RadioFor(Expression<Func<TModel, bool?>> expression, string falseLabel = "No", string trueLabel = "Yes", FieldConfiguration fieldConfiguration = null)
        {
            fieldConfiguration = fieldConfiguration ?? new FieldConfiguration();

            if (fieldConfiguration.As != null & fieldConfiguration.As != ElementType.RadioButtons)
            {
                throw new ApplicationException(string.Format("FieldConfiguration{{ As = {0} }} not valid for field: {1} (which defaults to RadioButton)."
                    , fieldConfiguration.As.Value, expression));
            }

            // var metadata = ModelMetadata.FromLambdaExpression(expression, _htmlHelper.ViewData);
            var value = expression.Compile().Invoke(_htmlHelper.ViewData.Model);
            var elementHtml = HelperDefinitions.BooleanRadioNullable(expression.GetFieldName(), value, falseLabel, trueLabel).ToString();

            elementHtml = (fieldConfiguration.Before ?? "") + elementHtml + (fieldConfiguration.After ?? "");

            return new FlowFormField(_writer, false, LabelHtml(expression, fieldConfiguration), elementHtml, null, true, fieldConfiguration.Hint, fieldConfiguration.Tip, fieldConfiguration.HideTip, fieldConfiguration.HintClass, fieldConfiguration.ParentClass, fieldConfiguration.DisplayFieldName);
        }

        /// <summary>
        /// Render a flow form field for a property in the page model.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property in the model that is being rendered</typeparam>
        /// <param name="expression">m => m.PropertyFromPageModel</param>
        /// <param name="fieldConfiguration">Configuration data for the field</param>
        /// <returns>A FlowFormField object (has a ToString() that prints "" so can safely have @ in Razor before this call)</returns>
        public FlowFormField FieldFor<TProperty>(Expression<Func<TModel, TProperty>> expression, FieldConfiguration fieldConfiguration = null)
        {
            return FieldFor(expression, false, fieldConfiguration);
        }

        /// <summary>
        /// Render the start of a basic flow form field for a property in the page model.
        /// </summary>
        /// <remarks>
        /// Will render the end of the field after Dispose() is called.<br />
        /// Usually, you would use this within a using block and then use the returned section to output nested fields, e.g.:<br />
        /// using (var ss = s.BeginFieldFor(m => m.MyField) {<br />
        ///     &#160; &#160; @ss.Field(...)<br />
        /// }
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property in the model that is being rendered</typeparam>
        /// <param name="expression">m => m.PropertyFromPageModel</param>
        /// <param name="fieldConfiguration">Configuration data for the field</param>
        /// <param name="sectionId">Adds an id to the <![CDATA[<dl>]]> of the nested section</param>
        /// <returns>A FlowFormSection object that will be nested within this field and can be used to create nested fields</returns>
        public FlowFormSection<TModel> BeginFieldFor<TProperty>(Expression<Func<TModel, TProperty>> expression, FieldConfiguration fieldConfiguration = null, string sectionId = null)
        {
            return new FlowFormSection<TModel>(_htmlHelper, true, null, sectionId, parentField: FieldFor(expression, true, fieldConfiguration));
        }

        /// <summary>
        /// Render the start of a flow form section.
        /// </summary>
        /// <remarks>
        /// Will render the end of the section after Dispose is called.<br />
        /// Usually, you would use this within a using block and then use the returned section to output nested fields, e.g.:<br />
        /// using (var ss = s.BeginSection("Section Heading") {<br />
        ///     &#160; &#160; @ss.FieldFor(...)<br />
        /// }
        /// </remarks>
        /// <param name="heading">The heading to give the section</param>
        /// <param name="id">An id to give the section <![CDATA[<fieldset>]]></param>
        /// <param name="hint">A hint to give the section when hovering over or focussing in any fields in that section</param>
        /// <param name="hintClass">A set of classes to give the hint; 'plain' means the hint won't hide with JavaScript, 'wide' means the hint will take up more width</param>
        /// <param name="optional">Set this to true if you want <![CDATA[<em>(optional)</em>]]> appended to the heading</param>
        /// <returns>A FlowFormSection object that can be used to create nested fields</returns>
        public FlowFormSection<TModel> BeginSection(string heading, string id = null, IHtmlString hint = null, string hintClass = null, bool optional = false)
        {
            return new FlowFormSection<TModel>(_htmlHelper, true, heading, id, hint, hintClass, optional);
        }
        #endregion

        #region Private Methods
        private FlowFormField Field(string label, IHtmlString elementHtml, string id = null, bool containsSection = false)
        {
            return new FlowFormField(_writer, containsSection, id == null ? label : _htmlHelper.Label(id, label).ToHtmlString(), elementHtml.ToHtmlString());
        }

        private FlowFormField FieldFor<TProperty>(Expression<Func<TModel, TProperty>> expression, bool containsSection, FieldConfiguration fieldConfiguration = null)
        {
            /* 
             * Renders a field for a non-boolean property of the model
             */

            fieldConfiguration = fieldConfiguration ?? new FieldConfiguration();

            if (fieldConfiguration.HideIfNull && fieldConfiguration.ReadOnly && GetValue(expression) == null)
            {
                return null;
            }

            var htmlAttrs = Helper.ObjectToDictionary(fieldConfiguration.HtmlAttributes);
            var type = ElementType.Text;
            var grid = default(Grid);
            var selectList = default(MultiSelectList);
            var metadata = ModelMetadata.FromLambdaExpression(expression, _htmlHelper.ViewData);

            // Label
            var labelHtml = LabelHtml(expression, fieldConfiguration);
            if (fieldConfiguration.ReadOnly)
            {
                labelHtml = labelHtml.ReReplace(@"</?label.*?>", "", RegexOptions.IgnoreCase);
            }

            if (fieldConfiguration.ReadOnly && expression.ReturnType.Name != "Grid`1")
            {
                var value = (GetValue(expression) ?? "?");
                var fi = value.GetType().GetField(value.ToString());
                var attributes = fi != null ? (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false) : new DescriptionAttribute[] { };

                var stringValue = (attributes.Length > 0) ? attributes[0].Description : value.ToString();

                return new FlowFormField(_writer, containsSection, labelHtml, (new HtmlString(stringValue)).ToString(), parentClass: fieldConfiguration.ParentClass);
            }

            var validators = metadata.GetValidators(_htmlHelper.ViewContext.Controller.ControllerContext);

            var classes = new ClassBuilder();
            
            if (htmlAttrs.ContainsKey("class"))
            {
                classes.Add((string)htmlAttrs["class"]);
            }

            if (metadata.IsRequired && ( !metadata.ModelType.IsEnum ||  fieldConfiguration.Exclude == null || fieldConfiguration.Exclude.Length == 0 ))
            {
                classes.Add("required");
            }
            else
            {
                labelHtml = labelHtml.Replace("</label>", " <em>(Optional)</em></label>");
            }

            switch (metadata.DataTypeName)
            {
                case "Digits":
                case "CreditCard":
                    classes.Add(metadata.DataTypeName.Replace("-", "").ToLower());
                    break;
                case "IPAddress":
                    classes.Add("ip-address");
                    break;
                case "MultilineText":
                    type = ElementType.TextArea;
                    break;
            }
            switch (metadata.ModelType.Name)
            {
                case "Double":
                    classes.Add("number");
                    break;
                case "Int32":
                    classes.Add("integer");
                    break;
                case "Email":
                    classes.Add("email");
                    break;
                case "Password":
                    type = ElementType.Password;
                    break;
                case "Date":
                    classes.Add("date");
                    break;
                case "HttpPostedFileBase":
                    htmlAttrs["type"] = "file";
                    break;
            }

            if (metadata.ModelType.IsEnum && metadata.ModelType.IsDefined(typeof(FlagsAttribute),false))
            {
                type = ElementType.Select;
                var list = (IEnumerable<object>)EnumInfo.CreateList(metadata.ModelType, fieldConfiguration.Exclude);
                var selectedValues = GetSelectedEnums((Enum) metadata.Model, fieldConfiguration.Exclude).ToList();
                selectList = new MultiSelectList(list, "Value", "Display", selectedValues);
            }
            else if (metadata.ModelType.IsEnum || (
                    metadata.ModelType.IsGenericType
                    && metadata.ModelType.GetGenericTypeDefinition().FullName == "System.Nullable`1"
                    && metadata.ModelType.GetGenericArguments()[0].IsEnum
                ) || (
                    typeof(IEnumerable).IsAssignableFrom(metadata.ModelType)
                    && !typeof(string).IsAssignableFrom(metadata.ModelType)
                    && metadata.ModelType.GetGenericArguments()[0].IsEnum
                )
            )
            {
                type = ElementType.Select;
                var list = (IEnumerable<object>)EnumInfo.CreateList(metadata.ModelType.IsGenericType ? metadata.ModelType.GetGenericArguments()[0] : metadata.ModelType, fieldConfiguration.Exclude);
                var selectedValues = GetSelected(metadata.Model);
                selectList = new MultiSelectList(list, "Value", "Display", selectedValues);
            }

            foreach (var rule in validators.SelectMany(v => v.GetClientValidationRules()))
            {
                switch (rule.ValidationType)
                {
                    case "range":
                        classes.Add(string.Format("range([{0},{1}])", rule.ValidationParameters["min"], rule.ValidationParameters["max"]));
                        break;

                    case "min":
                        classes.Add(string.Format("min({0})", rule.ValidationParameters["min"]));
                        break;

                    case "max":
                        classes.Add(string.Format("max({0})", rule.ValidationParameters["max"]));
                        break;

                    case "regex":
                        classes.Add(string.Format("match(/{0}/)", rule.ValidationParameters["pattern"]));
                        break;

                    case "length":
                    case "maxlength":
                        if (type == ElementType.Text || type == ElementType.Password)
                        {
                            htmlAttrs["maxlength"] = rule.ValidationParameters["max"];
                        }
                        else
                        {
                            classes.Add(string.Format("maxlength({0})", rule.ValidationParameters["max"]));
                        }
                        break;

                    case "minlength":
                        classes.Add(string.Format("minlength({0})", rule.ValidationParameters["min"]));
                        break;

                    case "rangelength":
                        classes.Add(string.Format("rangelength([{0},{1}])", rule.ValidationParameters["min"], rule.ValidationParameters["max"]));
                        break;

                    case "equalto":
                        classes.Add(string.Format("equalTo('#{0}')", rule.ValidationParameters["other"].ToString().Split('.')[1]));
                        labelHtml = labelHtml.Replace(" <em>(Optional)</em>", "");
                        break;

                    case "existsin":
                        type = ElementType.Select;
                        selectList = GetSelectListFromCollection(expression, metadata, rule);
                        break;

                    case "grid":
                        grid = GetGrid(expression, metadata, rule);
                        break;

                    case "notrequired":
                        labelHtml = labelHtml.Replace(" <em>(Optional)</em>", "");
                        break;

                    case "filetype":
                        classes.Add(string.Format("accept('{0}')", rule.ValidationParameters["extension"]));
                        fieldConfiguration.Tip = fieldConfiguration.Tip ?? string.Format("Filetype must be: {0}", rule.ValidationParameters["pretty-extension"]);
                        break;
                }
            }

            if (classes.ToString().Trim() != "")
                htmlAttrs["class"] = classes.ToString();

            if (fieldConfiguration.As.HasValue)
            {
                var validOverride = false;
                var overrideErrorMessage = "";
                switch (type)
                {
                    case ElementType.Select:
                        switch (fieldConfiguration.As.Value)
                        {
                            case ElementType.Checkboxes:
                            case ElementType.RadioButtons:
                            case ElementType.Text:
                                type = fieldConfiguration.As.Value;
                                validOverride = true;
                                break;
                        }
                        break;
                    case ElementType.Text:
                        switch (fieldConfiguration.As.Value)
                        {
                            case ElementType.Checkboxes:
                            case ElementType.RadioButtons:
                            case ElementType.Select:
                                if (fieldConfiguration.PossibleValues != null)
                                {
                                    selectList = fieldConfiguration.PossibleValues;
                                    type = fieldConfiguration.As.Value;
                                    validOverride = true;
                                }
                                else
                                {
                                    overrideErrorMessage = "I was expecting a list.";
                                }
                                break;
                        }
                        break;
                }
                if (!validOverride)
                {
                    throw new ApplicationException(string.Format("FieldConfiguration{{ As = {0} }} not valid for field: {1} (which defaults to {2}). {3}",
                        fieldConfiguration.As.Value, expression, type, overrideErrorMessage));
                }
            }

            var elementHtml = string.Empty;
            var errorHtml = string.Empty;
            var isValid = true;

            if (grid != null)
            {
                elementHtml = RenderGrid(grid, fieldConfiguration.ReadOnly);
                isValid = GetErrors(expression, out errorHtml);
            }
            else
            {
                switch (type)
                {
                    case ElementType.Text:
                        elementHtml = _htmlHelper.TextBoxFor(expression, htmlAttrs).ToHtmlString();
                        break;
                    case ElementType.Password:
                        elementHtml = _htmlHelper.PasswordFor(expression, htmlAttrs).ToHtmlString();
                        break;
                    case ElementType.TextArea:
                        elementHtml = _htmlHelper.TextAreaFor(expression, htmlAttrs).ToHtmlString();
                        break;
                    case ElementType.Select:
                        if (typeof(IEnumerable).IsAssignableFrom(metadata.ModelType) &&
                            !typeof(string).IsAssignableFrom(metadata.ModelType))
                        {
                            elementHtml = _htmlHelper.ListBoxFor(expression, selectList, htmlAttrs).ToHtmlString();
                        }
                        else
                        {
                            elementHtml = _htmlHelper.DropDownListFor(expression, selectList, fieldConfiguration.Label, htmlAttrs).ToHtmlString();
                        }
                        break;
                    case ElementType.Checkboxes:
                    case ElementType.RadioButtons:
                        // TODO: Use HTML Attributes
                        var typeString = type == ElementType.Checkboxes ? "checkbox" : "radio"; 
                        elementHtml += HelperDefinitions.BeginInputList(typeString);
                        elementHtml += string.Join("", selectList.Select(i => HelperDefinitions.InputListItem(typeString, expression.GetFieldName(), i.Value, i.Text, i.Selected).ToString()));
                        elementHtml += HelperDefinitions.EndInputList();
                        break;
                }
                isValid = GetErrors(expression, out errorHtml);
            }

            elementHtml = (fieldConfiguration.Before ?? "") + elementHtml + (fieldConfiguration.After ?? "");

            return new FlowFormField(_writer, containsSection, labelHtml, elementHtml, errorHtml, isValid, fieldConfiguration.Hint, fieldConfiguration.Tip, fieldConfiguration.HideTip, fieldConfiguration.HintClass, fieldConfiguration.ParentClass, fieldConfiguration.DisplayFieldName);
        }

        private string LabelHtml<TProperty>(Expression<Func<TModel, TProperty>> expression, FieldConfiguration fieldConfiguration)
        {
            return (
                fieldConfiguration.FieldName != null
                ? _htmlHelper.LabelFor(expression, fieldConfiguration.FieldName)
                : _htmlHelper.LabelFor(expression)
            )
            .ToHtmlString();
        }

        private string RenderGrid(Grid grid, bool readOnly = false)
        {
            return HelperDefinitions.Grid(grid.Id, grid.Choices, grid.Prompts, grid.Values, "%prompt% / %choice%", "Summary", readOnly).ToString();
        }

        private IEnumerable<Enum> GetSelectedEnums(Enum currentValue, Enum[] exclusions)
        {
            var values = Enum.GetValues(currentValue.GetType()).Cast<Enum>().Except(exclusions ?? Enumerable.Empty<Enum>()).ToList();
            foreach (var value in values)
            {
                if (currentValue.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        private bool GetErrors<TProperty>(Expression<Func<TModel, TProperty>> expression, out string errorHtml)
        {
            var state = GetModelState(expression);
            errorHtml = "";
            var isValid = true;
            if (state != null && state.Errors.Count > 0)
            {
                var s = new StringBuilder();
                state.Errors.ForEach(error => s.Append(" " + (error.ErrorMessage.Trim().EndsWith(".") ? error.ErrorMessage.Trim() : error.ErrorMessage.Trim() + ".")));
                errorHtml = HelperDefinitions.ErrorHtml(s.ToString().Trim()).ToHtmlString();
                isValid = false;
            }

            return isValid;
        }

        private Grid GetGrid<TProperty>(Expression<Func<TModel, TProperty>> expression, ModelMetadata metadata, ModelClientValidationRule rule)
        {
            var grid = new Grid();
            var promptsName = rule.ValidationParameters["prompts"].ToString();
            var prompts = GetCollection(promptsName, expression);
            if (prompts == null)
                throw new Exception(String.Format("Model.{0} is null. Unable to make prompts for Model.{1}", promptsName, metadata.DisplayName));
            grid.Prompts = new SelectList(prompts,
                (rule.ValidationParameters["promptsValue"] ?? "Id") as string,
                (rule.ValidationParameters["promptsDisplay"] ?? "Name") as string
            );

            var choicesName = rule.ValidationParameters["choices"].ToString();
            var choices = GetCollection(choicesName, expression);
            if (choices == null)
                throw new Exception(String.Format("Model.{0} is null. Unable to make choices for Model.{1}", choicesName, metadata.DisplayName));
            grid.Choices = new SelectList(choices,
                (rule.ValidationParameters["choicesValue"] ?? "Id") as string,
                (rule.ValidationParameters["choicesDisplay"] ?? "Name") as string
            );

            grid.Id = expression.GetFieldName();
            var attr = ((MemberExpression)expression.Body).Member.Name;

            grid.Type = ElementType.Text;
            grid.Values = GetModel(expression).GetValueOrDefault<Grid<string>>(attr) ?? new Grid<string>();


            return grid;
        }

        protected MultiSelectList GetSelectListFromCollection<TProperty>(Expression<Func<TModel, TProperty>> expression, ModelMetadata metadata, ModelClientValidationRule rule)
        {
            var collectionName = rule.ValidationParameters["collection"].ToString();
            var collection = GetCollection(collectionName, expression);
            if (collection == null)
                throw new Exception(String.Format("Model.{0} is null. Unable to make list for Model.{1}", collectionName, metadata.DisplayName));

            var keyAttr = (rule.ValidationParameters["value"] ?? "Id") as string;
            var valueAttr = (rule.ValidationParameters["display"] ?? "Name") as string;

            // todo: Use blank Value (hard to do because of the dynamic property names above and the Data Binding's use of reflection, might need to use Impromptu lib)

            var selectList = new MultiSelectList(collection, keyAttr, valueAttr, GetSelected(metadata.Model));
            return selectList;
        }

        private IEnumerable GetCollection<TProperty>(string collectionName, Expression<Func<TModel, TProperty>> expression)
        {
            var model = GetModel(expression);
            var collection = model.GetValueOrDefault<IEnumerable>(collectionName);
            if (collection != null)
                return collection;

            return null;
        }

        public object GetModel<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var parentFunc = expression.GetParent();
            var model = _htmlHelper.ViewData.Model;
            return parentFunc != null ? parentFunc(model) : model;
        }

        public object GetValue<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return GetModel(expression).GetValue(((MemberExpression)expression.Body).Member.Name);
        }

        private static IEnumerable GetSelected(object currentValue)
        {
            var iEnumerable = currentValue as IEnumerable;
            if (iEnumerable != null && !(iEnumerable is String))
                return iEnumerable;
            return new[] { currentValue };
        }

        private ModelState GetModelState<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = _htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = default(ModelState);
            _htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state);

            return state;
        }
        #endregion

        class Grid
        {
            public string Id { get; set; }
            public ElementType Type { get; set; }
            public SelectList Choices { get; set; }
            public SelectList Prompts { get; set; }
            public Grid<string> Values { get; set; }
        }
    }

    public class ListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public enum ElementType
    {
        Text, Password, TextArea, Select, Checkboxes, RadioButtons
    }
}