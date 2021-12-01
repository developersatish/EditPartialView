using com.sun.tools.doclets.formats.html.markup;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace PartialEdit
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString DialogFormLink(this HtmlHelper htmlHelper, string linkText, string actionUrl,
            string dialogTitle)
        {
            var builder = new TagBuilder("a");
            builder.SetInnerText(linkText);
            builder.Attributes.Add("href", actionUrl);
            builder.Attributes.Add("data-dialog-title", dialogTitle);
            builder.AddCssClass("dialogLink");

            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString DateTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var routeValueDictionary = new RouteValueDictionary(htmlAttributes)
            {
                {"data-format", "DD/MM/YYYY"},
                {"data-template", "D MMM YYYY"}
            };

            var textBox = htmlHelper.TextBoxFor(expression, "{0:dd/MM/yyyy}", routeValueDictionary);
            return MvcHtmlString.Create(textBox.ToHtmlString());
        }

        public static MvcHtmlString DateTimeTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var routeValueDictionary = new RouteValueDictionary(htmlAttributes)
            {
                {"data-format", "DD/MM/YYYY h:mm a"},
                {"data-template", "D MMM YYYY   hh mm a"}
            };

            var textBox = htmlHelper.TextBoxFor(expression, "{0:dd/MM/yyyy h:mm tt}", routeValueDictionary);
            return MvcHtmlString.Create(textBox.ToHtmlString());
        }

        public static MvcHtmlString Captcha(this HtmlHelper htmlHelper)
        {
            const string honeypotControl =
                "<input id=\"hp_input\" type=\"text\" value=\"\" name=\"hp_input\" tabindex=\"-1\">";

            var tagBuilder = new TagBuilder("div");
            tagBuilder.MergeAttribute("id", "hp-container");
            tagBuilder.MergeAttribute("style", "display: block; position: absolute; left: -9999px");
            tagBuilder.InnerHtml =
                string.Format("<label for=\"hp_input\">Do not enter details into this field</label>{0}", honeypotControl);

            return MvcHtmlString.Create(tagBuilder.ToString());
        }



        public static MvcHtmlString ClickButton(this HtmlHelper htmlHelper, string text, string onClick,
            object htmlAttributes = null, string toolTipText = null)
        {
            var routeValueDictionary = new RouteValueDictionary(htmlAttributes);
            var extraAttributes =
                routeValueDictionary.Select(
                    keyValuePair => string.Format("{0}=\"{1}\"", keyValuePair.Key, keyValuePair.Value)).ToList();

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("<input type=\"button\" value=\"{0}\" onclick=\"{1}\" {2} />", text,
                    onClick, string.Join(" ", extraAttributes)));
            if (!string.IsNullOrWhiteSpace(toolTipText))
            {
                sb.AppendLine(@"<div class=""tooltip hidden"" style=""white-space: nowrap"">");
                sb.AppendLine(string.Format(@"<span class=""tooltiptext"">{0}</span>", toolTipText));
                sb.AppendLine("</div>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static IHtmlString PostWithActionLink(this HtmlHelper htmlHelper, string id, string postUrl,
            string linktext = null, object additionalParams = null, bool button = false, string dialogId = null, string styleClass = null)
        {
            var absolutePath = VirtualPathUtility.ToAbsolute(postUrl);
            var linkId = dialogId ?? string.Format("link_{0}", id);
            var removeJs = additionalParams == null
                ? string.Format("javascript:PostWithAntiForgeryToken('{0}', '{1}');", absolutePath, id)
                : string.Format("javascript:PostWithAntiForgeryToken('{0}', '{1}', '{2}');", absolutePath, id,
                    Json.Encode(additionalParams));

            var onclick = string.Format("setConfirmationSource('{0}')", linkId);

            var tagBuilder = new TagBuilder("a");
            tagBuilder.MergeAttribute("id", linkId);
            tagBuilder.MergeAttribute("onclick", onclick);
            tagBuilder.MergeAttribute("href", removeJs);
            tagBuilder.MergeAttribute("class", "deleteDialog");

            if (button)
                tagBuilder.AddCssClass("button");

            if (!string.IsNullOrEmpty(styleClass))
            {
                tagBuilder.AddCssClass(styleClass);
            }

            tagBuilder.SetInnerText(string.IsNullOrEmpty(linktext) ? "Delete" : linktext);

            return htmlHelper.Raw(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static IHtmlString PostWithActionLink(this HtmlHelper htmlHelper, int id, string postUrl,
            string linktext = null, object additionalParams = null, bool button = false, string dialogId = null, string styleClass = null)
        {
            return PostWithActionLink(htmlHelper, id.ToString(CultureInfo.InvariantCulture), postUrl, linktext,
                additionalParams, button, dialogId, styleClass);
        }

        public static MvcHtmlString HelpLink(this HtmlHelper htmlHelper, string divName,
            string linkText = "More Help...")
        {
            linkText += "<span class='offscreen'>(show dialog)</span>";
            var onClick =
                string.Format(
                    "$({0}).dialog({{open: function(event, ui) {{ openHelp(event); }}, close: function(event, ui) {{ $('#link-{0}').focus();  }} }}); return false;",
                    divName);

            return
                new MvcHtmlString(string.Format("<a id=\"link-{2}\" href=\"#\" onclick=\"{0}\">{1}</a>", onClick,
                    linkText, divName));
        }

        public static MvcHtmlString ExpandHelpLink(this HtmlHelper htmlHelper, string divName,
           string linkText = "More Help...")
        {
            var onClick =
                string.Format(
                    "$({0}).slideToggle(\'slow\')",
                    divName);

            return
                new MvcHtmlString(string.Format("<a id=\"link-{2}\" href=\"#\" onclick=\"{0}\">{1}</a>", onClick,
                    linkText, divName));
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, string hintText = "", string labelText = null)
        {
            var displayName = labelText ?? htmlHelper.DisplayNameFor(expression).ToString();
            var property = ExpressionHelper.GetExpressionText(expression);
            var inputId = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(property);

            TagBuilder hintBuilder = null;

            if (!string.IsNullOrEmpty(hintText))
            {
                hintBuilder = new TagBuilder("span");
                hintBuilder.AddCssClass("format");
                hintBuilder.SetInnerText(hintText);
            }

            var tagBuilder = new TagBuilder("label");
            tagBuilder.Attributes.Add("for", inputId);
            tagBuilder.SetInnerText(displayName);
            tagBuilder.InnerHtml = string.Format("{0}{1}", displayName,
                hintBuilder != null ? hintBuilder.ToString(TagRenderMode.Normal) : string.Empty);

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString MandatoryIndicatorMessage(this HtmlHelper htmlHelper)
        {
            const string imagePath = "~/Content/Images/icon-mandatory.png";
            return
                new MvcHtmlString(
                    string.Format(
                        "<p class=\"mandatory-text\"><img alt=\"Mandatory\" src=\"{0}\">Indicates a mandatory field</p>",
                        UrlHelper.GenerateContentUrl(imagePath, htmlHelper.ViewContext.HttpContext)));
        }

        public static MvcHtmlString MandatoryImage(this HtmlHelper htmlHelper)
        {
            const string imagePath = "~/Content/Images/icon-mandatory.png";

            var tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("alt", "mandatory");
            tagBuilder.Attributes.Add("src", UrlHelper.GenerateContentUrl(imagePath, htmlHelper.ViewContext.HttpContext));

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString MandatoryLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, string hintText = "", bool showAsMandatory = true,
            string inputId = "", string labelText = null)
        {
            var displayName = labelText ?? htmlHelper.DisplayNameFor(expression).ToString();
            var property = ExpressionHelper.GetExpressionText(expression);

            const string imagePath = "~/Content/Images/icon-mandatory.png";

            if (string.IsNullOrEmpty(inputId))
                inputId = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(property);

            var imageBuilder = new TagBuilder("img");
            imageBuilder.Attributes.Add("alt", "mandatory");
            imageBuilder.Attributes.Add("src",
                UrlHelper.GenerateContentUrl(imagePath, htmlHelper.ViewContext.HttpContext));

            TagBuilder hintBuilder = null;

            if (!string.IsNullOrEmpty(hintText))
            {
                hintBuilder = new TagBuilder("span");
                hintBuilder.AddCssClass("format");
                hintBuilder.SetInnerText(hintText);
            }

            var labelBuilder = new TagBuilder("label");

            if (showAsMandatory)
                labelBuilder.AddCssClass("mandatory");

            labelBuilder.Attributes.Add("for", inputId);
            labelBuilder.SetInnerText(displayName);
            labelBuilder.InnerHtml = string.Format("{0}{1}{2}", displayName,
                imageBuilder.ToString(TagRenderMode.SelfClosing),
                hintBuilder != null ? hintBuilder.ToString(TagRenderMode.Normal) : string.Empty);

            return new MvcHtmlString(labelBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ViewState(this HtmlHelper htmlHelper, object model)
        {
            var serializer = DependencyResolver.Current.GetService<ISerializer>();
            return htmlHelper.Hidden("ViewState", serializer.Serialize(model));
        }


        public static MvcHtmlString WizardHiddenText<T>(this HtmlHelper html, T currentStep, T targetStep)
            where T : IComparable
        {
            var isCurrentStep = targetStep.CompareTo(currentStep) == 0;
            return isCurrentStep
                ? new MvcHtmlString("<span class=\"offscreen\"> - Current Step</span>")
                : MvcHtmlString.Empty;
        }


        public static MvcHtmlString NewlinesToBreaks(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return MvcHtmlString.Create(text);

            var lines = text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            var withBreaks = string.Join("<br/>", lines.Select(HttpUtility.HtmlEncode));

            return MvcHtmlString.Create(withBreaks);
        }

        public static string FormatFileSize(this int fileSizeInBytes)
        {
            return fileSizeInBytes > 1000000
                ? Math.Round((float)fileSizeInBytes / 1048576, 2) + " MB"
                : Math.Round((float)fileSizeInBytes / 1024, 1) + " KB";
        }

        public static MvcHtmlString Compare(this HtmlHelper htmlHelper, string seedField, string childField,
            bool truncate = false, bool wrapword = false)
        {
            if (childField == null)
                return null;

            var tagBuilder = new TagBuilder("div");

            if (childField.Equals(seedField, StringComparison.CurrentCultureIgnoreCase))
            {
                tagBuilder.AddCssClass("same");
            }

            if (wrapword)
            {
                tagBuilder.AddCssClass("breakword");
            }

            tagBuilder.SetInnerText(truncate ? htmlHelper.Truncate(childField, 5) : childField);

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString WrapWord(this HtmlHelper htmlHelper, string field)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("breakword");
            tagBuilder.SetInnerText(field);

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static string Truncate(this HtmlHelper htmlHelper, string value, int maxLength = 5)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

        public static MvcHtmlString ReCaptcha(this HtmlHelper htmlHelper)
        {
            string reCaptchaPublicKey = ConfigurationManager.AppSettings["reCaptchaPublicKey"];
            //var result = string.Format("<div class='g-recaptcha' data-sitekey='{0}'></div>", reCaptchaPublicKey);
            //Re-written to provide support for clients that do not have javascript enabled
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<script src=""https://www.google.com/recaptcha/api.js"" async defer></script>");
            sb.AppendLine(string.Format(@"<div class=""g-recaptcha"" data-sitekey=""{0}""></div>", reCaptchaPublicKey));
            sb.AppendLine("<noscript>");
            sb.AppendLine("<div>");
            sb.AppendLine(@"<div style=""width: 302px; height: 422px; position: relative;"">");
            sb.AppendLine(@"<div style=""width: 302px; height: 422px; position: absolute;"">");
            sb.AppendLine(string.Format(@"<iframe src=""https://www.google.com/recaptcha/api/fallback?k={0}""",
                reCaptchaPublicKey));
            sb.AppendLine(@"frameborder=""0"" scrolling=""no""");
            sb.AppendLine(@"style=""width: 302px; height:422px; border-style: none;"">");
            sb.AppendLine("</iframe>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine(@"<div style=""width: 300px; height: 60px; border-style: none;");
            sb.AppendLine("bottom: 12px; left: 25px; margin: 0px; padding: 0px; right: 25px;");
            sb.AppendLine(@"background: #f9f9f9; border: 1px solid #c1c1c1; border-radius: 3px;"">");
            sb.AppendLine(@"<textarea id=""g-recaptcha-response"" name=""g-recaptcha-response""");
            sb.AppendLine(@"class=""g-recaptcha-response""");
            sb.AppendLine(@"style=""width: 250px; height: 40px; border: 1px solid #c1c1c1;");
            sb.AppendLine(@"margin: 10px 25px; padding: 0px; resize: none;"" >");
            sb.AppendLine("</textarea>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</noscript>");

            return MvcHtmlString.Create(sb.ToString());

        }

        public static MvcHtmlString HelpToolTip(this HtmlHelper htmlHelper, string toolTipMessage)
        {
            const string imagePath = "~/Content/Images/icon-help-small.png";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(@"<img src=""{0}"")"" style=""width: 20px; height:20px;"" class=""usitooltip"" />", UrlHelper.GenerateContentUrl(imagePath, htmlHelper.ViewContext.HttpContext)));
            sb.AppendLine(@"<div class=""tooltip hidden"" style=""white-space: nowrap"">");
            sb.AppendLine(string.Format(@"<span class=""tooltiptext"">{0}</span>", toolTipMessage));
            sb.AppendLine("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString BuildTranscriptToolTipText(this HtmlHelper htmlHelper, string outComeText, string outComeDescription)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(@"<span class=""usitooltip"">{0}</span>", outComeText));
            sb.AppendLine(@"<div class=""tooltip hidden"" style=""white-space: nowrap"">");
            sb.AppendLine(string.Format(@"<span class=""tooltiptext"">{0}</span>", outComeDescription));
            sb.AppendLine("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString BuildTranscriptToolTipSmallText(this HtmlHelper htmlHelper, string outComeText, string outComeDescription)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(@"<span class=""usitooltip"">{0}</span>", outComeText));
            sb.AppendLine(@"<div class=""tooltip hidden"" style=""white-space: nowrap"">");
            sb.AppendLine(string.Format(@"<span class=""tooltipsmalltext"">{0}</span>", outComeDescription));
            sb.AppendLine("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }

        //public static bool IsDigitalLogin(this HtmlHelper htmlHelper)
        //{
        //    return ClaimsPrincipal.Current.IsInRole(RoleName.LoginMethodDigitalIdentityRole);
        //}

        public static MvcHtmlString AddressAutoCompleteField(this HtmlHelper htmlHelper, bool withLabel = true,
            string labelText = "", bool showPlaceHolder = true, string placeHolderText = "Type your address...")
        {
            var sb = new StringBuilder();
            if (withLabel)
                sb.AppendLine($@"<label>{labelText}</label>");

            var inputBuilder = new TagBuilder("input");
            //inputBuilder.AddCssClass("ui-autocomplete");
            inputBuilder.MergeAttribute("type", "search");
            inputBuilder.MergeAttribute("name", "typeyouradd");
            inputBuilder.MergeAttribute("id", "typeyouradd");
            if (showPlaceHolder)
                inputBuilder.MergeAttribute("placeholder", placeHolderText);

            sb.AppendLine(inputBuilder.ToString(TagRenderMode.SelfClosing));

            return MvcHtmlString.Create(sb.ToString());

        }

    }
}



