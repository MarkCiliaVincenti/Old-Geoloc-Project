using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFLEET.Web.HtmlHelpers
{
    public static class HtmlExtensions
    {
        static StringBuilder  _sb ;
        public static MvcHtmlString DatePickerDropDowns(this HtmlHelper html,
            string dayName, string monthName, string yearName,
            int? beginYear = null, int? endYear = null,
            int? selectedDay = null, int? selectedMonth = null, int? selectedYear = null, bool localizeLabels = true, bool disabled = false)
        {
            var daysList = new TagBuilder("select");
            daysList.MergeAttribute("style", "width: 70px;height: 35px");
            var monthsList = new TagBuilder("select");
            monthsList.MergeAttribute("style", "width: 120px;height: 35px");
            var yearsList = new TagBuilder("select");
            yearsList.MergeAttribute("style", "width: 90px;height: 35px");

            daysList.Attributes.Add("name", dayName);
            monthsList.Attributes.Add("name", monthName);
            yearsList.Attributes.Add("name", yearName);
            daysList.Attributes.Add("id", dayName);
            monthsList.Attributes.Add("id", monthName);
            yearsList.Attributes.Add("id", yearName);

            daysList.Attributes.Add("class", "date-part");
            monthsList.Attributes.Add("class", "date-part");
            yearsList.Attributes.Add("class", "date-part");

            if (disabled)
            {
                daysList.Attributes.Add("disabled", "disabled");
                monthsList.Attributes.Add("disabled", "disabled");
                yearsList.Attributes.Add("disabled", "disabled");
            }

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            string dayLocale = "Jour";
            string monthLocale = "Mois";
            string yearLocale = "Année";

            days.AppendFormat("<option>{0}</option>", dayLocale);
            for (int i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedDay.HasValue && selectedDay.Value == i) ? " selected=\"selected\"" : null);


            months.AppendFormat("<option>{0}</option>", monthLocale);
            for (int i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                                    i,
                                    (selectedMonth.HasValue && selectedMonth.Value == i) ? " selected=\"selected\"" : null,
                                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }


            years.AppendFormat("<option>{0}</option>", yearLocale);

            if (beginYear == null)
                beginYear = DateTime.UtcNow.Year - 90;
            if (endYear == null)
                endYear = DateTime.UtcNow.Year + 10;

            for (int i = beginYear.Value; i <= endYear.Value; i++)
                years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);

            daysList.InnerHtml = days.ToString();
            monthsList.InnerHtml = months.ToString();
            yearsList.InnerHtml = years.ToString();

            return MvcHtmlString.Create(string.Concat(daysList, monthsList, yearsList));
        }
        public static string FieldNameFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            return html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }
        public static MvcHtmlString Custom_DropdownList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> list)
        {
            //This method in turns calls below overload.
            return Custom_DropdownList(helper, name, list, null);
        }

        public static MvcHtmlString BootstrapModal(this HtmlHelper helper, string modalId, string modalTite,
            string modalBodyId)
        {
            StringBuilder sb = new StringBuilder();
            var openContent = $@"<div id='{modalId}' style='margin: 5px;z-index: 10000'  data-backdrop='false' class='modal fade' role='dialog'>
                <div class='modal-dialog'>

                <!-- Modal content-->
                <div class='modal-content'>";
            var modalHeader = $@"<div class='modal-header'>
                <button type = 'button' class='close' data-dismiss='modal'>&times;</button>
                <h4 class='modal-title'>{modalTite}</h4>
                </div>";
            //here the content will be loaded
            var modalBody = $@" <div class='modal-body'>
                <div id='{modalBodyId}'> </div>
     
                </div>"

                ;
            var modalFooter = $@"<div class='modal-footer'>
                                <button type = 'button' onClick='save()' class='btn btn-info' >Enregistrer</button>
                              <button type = 'button' class='btn btn-default' data-dismiss='modal'>Fermer</button>
                             
                </div>";
            var closeContent = @"</div></div></div>";
            sb.Append(openContent);
            sb.Append(modalHeader);
            sb.Append(modalBody);
            sb.Append(modalFooter);
            sb.Append(closeContent);

            return new MvcHtmlString(sb.ToString());
        }
        public static MvcHtmlString BootstrapModalAngular(this HtmlHelper helper, string modalId, string modalTite,
            string template , string dissablesave = null)
        {
            StringBuilder sb = new StringBuilder();
            var openContent = $@"<div id='{modalId}'  data-backdrop='false' class='modal fade' role='dialog'>
                <div class='modal-dialog'>

                <!-- Modal content-->
                <div class='modal-content'>";
            var modalHeader = $@"<div class='modal-header'>
                <button type = 'button' class='close' data-dismiss='modal'>&times;</button>
                <h4 class='modal-title'>{modalTite}</h4>
                </div>";
            //here the content will be loaded
            var modalBody = $@" <div class='modal-body'>
                <div > </div>
                    {template}
                </div>"

                ;
            var modalFooter = string.IsNullOrEmpty(dissablesave)? $@"<div class='modal-footer'>
                                <button type = 'button' ng-click='save()' class='btn btn-info' >Enregistrer</button>
                              <button type = 'button' class='btn btn-default' data-dismiss='modal'>Fermer</button></div>"
                : $@"<div class='modal-footer'>
                                <button type = 'button' ng-click='save()' ng-disabled='{dissablesave}.$invalid || {dissablesave}.$pristine' class='btn btn-info' >Enregistrer</button>
                               <button type = 'button' class='btn btn-default' data-dismiss='modal'>Fermer</button></div>";
            var closeContent = @"</div></div></div>";
            sb.Append(openContent);
            sb.Append(modalHeader);
            sb.Append(modalBody);
            sb.Append(modalFooter);
            sb.Append(closeContent);

            return new MvcHtmlString(sb.ToString());
        }
        private static StringBuilder RecursiveBuilder(List<Vehicle> direction)
        {
          
            _sb.Append("<li id='"+Guid.Empty+"'>" + "Véhicules"+"</a>");
            if (direction == null)
            {
                _sb.Append("</li>");
                return _sb;
            }
            if (direction.Any())
            {
                _sb.Append("<ul>");

                foreach (var item in direction)
                {
                        _sb.Append(String.Format("<li id='"+item.Id+"' >{0}</li>", item.VehicleName));
                   
                }
                _sb.Append("</ul>");
                _sb.Append("</li>");

                
            }
            else
            {
                _sb.Append("</li>");
            }
            return _sb;
        }
        public static IHtmlString TreeView(this HtmlHelper helper, List<Vehicle> direction)
        {
            _sb = new StringBuilder();
            var sb1 = RecursiveBuilder(direction);
            return new HtmlString( sb1.ToString());
        }
        //This overload is extension method accepts name, list and htmlAttributes as parameters.
        public static MvcHtmlString Custom_DropdownList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> list, object htmlAttributes)
        {
            //Creating a select element using TagBuilder class which will create a dropdown.
            TagBuilder dropdown = new TagBuilder("select");
            //Setting the name and id attribute with name parameter passed to this method.
            dropdown.Attributes.Add("name", name);
            dropdown.Attributes.Add("id", name);

            //Created StringBuilder object to store option data fetched oen by one from list.
            StringBuilder options = new StringBuilder();
            options = options.Append("<option value='" + "" + "'>" + "" + "</option>");
            //Iterated over the IEnumerable list.
            options = list.Aggregate(options, (current, item) => current.AppendFormat(!item.Selected ? "<option value='{0}' >{1}</option>" : "<option value='{0}' selected='{0}' >{1}</option>", item.Value, item.Text));
            //assigned all the options to the dropdown using innerHTML property.
            dropdown.InnerHtml = options.ToString();
            //Assigning the attributes passed as a htmlAttributes object.
            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            //Returning the entire select or dropdown control in HTMLString format.
            return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
        }
        public static IHtmlString DataList<T>(this HtmlHelper helper, IEnumerable<T> items, int columns,
            Func<T, HelperResult> template, int gridColumns = 24)
            where T : class
        {
            if (items == null)
                return new HtmlString("");

            var sb = new StringBuilder();
            sb.Append("<div class='data-list data-list-grid'>");

            int cellIndex = 0;
            string spanClass = String.Format("span{0}", gridColumns / columns);

            foreach (T item in items)
            {
                if (cellIndex == 0)
                    sb.Append("<div class='data-list-row row-fluid'>");

                sb.Append("<div class='{0} data-list-item equalized-column' data-equalized-deep='true'>".FormatInvariant(spanClass));
                sb.Append(template(item).ToHtmlString());
                sb.Append("</div>");

                cellIndex++;

                if (cellIndex == columns)
                {
                    cellIndex = 0;
                    sb.Append("</div>");
                }
            }

            if (cellIndex != 0)
            {
                sb.Append("</div>"); // close .row-fluid
            }

            sb.Append("</div>"); // close .data-list

            return new HtmlString(sb.ToString());
        }

    }
}