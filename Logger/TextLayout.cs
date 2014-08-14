using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Logger.Core;
using Logger.Tools;
using Logger.Configuration;
using System.Xml;
using Logger.Layout.Formatter;

namespace Logger.Layout
{
    public class TextLayout : LayoutBase
    {
        public TextLayout()
        {
            LoadConfiguration();
            if (v_configuration.Pattern != null)
                base.formatter = new TextFormatter(v_configuration.Pattern);
            else
                base.formatter = new TextFormatter();
        }

        protected TextLayout(ILayoutConfiguration configuration)
        {

        }

        public override void Format(TextWriter writer, Log log)
        {
            base.formatter.Format(log);
            writer.WriteLine(base.formatter.ToString());
        }

        public override string StartValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Header);
                return sb.ToString().TrimEnd('\n', '\r');
            }
        }

        public override string EndValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Footer);
                return sb.ToString().TrimEnd('\n', '\r');
            }
        }

        /// <summary>
        /// Content type.
        /// </summary>
        public override string ContentType
        {
            get
            {
                return base.ContentType;
            }
            set
            {
                base.ContentType = value;
            }
        }
    }
}
