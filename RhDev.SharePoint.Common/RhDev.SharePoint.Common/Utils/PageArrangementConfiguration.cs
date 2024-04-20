using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RhDev.SharePoint.Common.Utils
{
    [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
    [System.Serializable()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.RhDev.cz/projects/nc/sharepoint/page-arrangement")]
    [XmlRoot("RhDev.SharePoint.PageArrangement", Namespace = "http://www.RhDev.cz/projects/nc/sharepoint/page-arrangement", IsNullable = false)]
    public class PageArrangementConfiguration
    {
        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public ArrangementPage[] Pages { get; set; }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.RhDev.cz/projects/nc/sharepoint/page-arrangement")]
        public class ArrangementPage
        {
            [XmlArray("webPartZones")]
            [XmlArrayItem("webPartZone", typeof(WebPartZone), IsNullable = false)]
            public WebPartZone[] WebPartZones { get; set; }

            /// <remarks/>
            [XmlAttribute("url")]
            public string Url { get; set; }

            /// <remarks/>
            [XmlAttribute("create")]
            public bool CreatePage { get; set; }

            /// <remarks/>
            [XmlAttribute("setHomePage")]
            public bool SetHomePage { get; set; }

            /// <remarks/>
            [XmlAttribute("publishingPage")]
            public bool PublishingPage { get; set; }

            /// <remarks/>
            [XmlAttribute("templatePath")]
            public string TemplatePath { get; set; }

            /// <remarks/>
            [XmlAttribute("title")]
            public string Title { get; set; }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.RhDev.cz/projects/nc/sharepoint/page-arrangement")]
        public class WebPartZone
        {
            [XmlArray("webParts")]
            [XmlArrayItem("registerWebPart", typeof(WebPartZoneRegisterWebPart), IsNullable = false)]
            public WebPartZoneRegisterWebPart[] WebParts { get; set; }

            /// <remarks/>
            [XmlAttribute("webPartZoneID")]
            public string WebPartZoneId { get; set; }

            /// <remarks/>
            [XmlAttribute("initAction")]
            public WebPartZoneInitAction InitAction { get; set; }
        }

        public enum WebPartZoneInitAction
        {
            None = 0,
            Clear = 1,
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.RhDev.cz/projects/nc/sharepoint/page-arrangement")]
        public class WebPartZoneRegisterWebPart
        {
            [XmlArray("webPartProperties")]
            [XmlArrayItem("webPartProperty", typeof(WebPartProperty), IsNullable = false)]
            public WebPartProperty[] Properties { get; set; }

            [XmlAttribute("contextCreatorTypeName")]
            public string ContextCreatorTypeName { get; set; }

            [XmlAttribute("typeName")]
            public string TypeName { get; set; }

            [XmlAttribute("webPartZoneIndex")]
            public int WebPartZoneIndex { get; set; }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.RhDev.cz/projects/nc/sharepoint/page-arrangement")]
        public class WebPartProperty
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlText]
            public string Value { get; set; }
        }
    }
}
