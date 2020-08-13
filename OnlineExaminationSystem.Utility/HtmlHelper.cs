using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OnlineExaminationSystem.Utility
{
    public class HtmlHelper
    {
		public static string RemoveHtmlTag(string content)
		{
			Regex regex = new Regex(@"<(.|\n)+?>");
			content = regex.Replace(content, "");
			content = content.Replace(@"\r\n", "");

			return content;
		}
	}
}
