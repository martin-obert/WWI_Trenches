using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.PathMesh
{
    public class PathData
    {
        public Vector2[] Points;
    }

    public sealed class SvgImporter
    {
        public static PathData ImportPathFromFile(string filePath, string filename)
        {
            var datapath = Application.dataPath;

            var fullPath = Path.Combine(datapath, filePath, filename);

            if (!File.Exists(fullPath))
            {
                Debug.LogError("File in directory " + fullPath + " doesn't exists!");
                return null;
            }

            using (var file = File.Open(fullPath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(svg));

                try
                {
                    var svg = serializer.Deserialize(file) as svg;

                    var commands = svg.g.path.d.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    var points = new List<Vector2>();

                    foreach (var command in commands)
                    {
                        var parts = command.Split(',');

                        if (parts.Length != 2) continue;

                        var vec = new Vector2(float.Parse(parts[0].Replace(".", ",")), float.Parse(parts[1].Replace(".", ",")));

                        points.Add(vec);
                    }

                    return new PathData
                    {
                        Points = points.ToArray()
                    };
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            throw new UnityException("Import failed. Something went wrong!");
        }
    }
    #region Generated
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/svg")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2000/svg", IsNullable = false)]
    public partial class svg
    {

        private svgDefs defsField;

        private namedview namedviewField;

        private svgMetadata metadataField;

        private svgG gField;

        private string widthField;

        private string heightField;

        private string viewBoxField;

        private decimal versionField;

        private string idField;

        private string docnameField;

        private string version1Field;

        /// <remarks/>
        public svgDefs defs
        {
            get
            {
                return this.defsField;
            }
            set
            {
                this.defsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd")]
        public namedview namedview
        {
            get
            {
                return this.namedviewField;
            }
            set
            {
                this.namedviewField = value;
            }
        }

        /// <remarks/>
        public svgMetadata metadata
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }

        /// <remarks/>
        public svgG g
        {
            get
            {
                return this.gField;
            }
            set
            {
                this.gField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string viewBox
        {
            get
            {
                return this.viewBoxField;
            }
            set
            {
                this.viewBoxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd")]
        public string docname
        {
            get
            {
                return this.docnameField;
            }
            set
            {
                this.docnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("version", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public string version1
        {
            get
            {
                return this.version1Field;
            }
            set
            {
                this.version1Field = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/svg")]
    public partial class svgDefs
    {

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd", IsNullable = false)]
    public partial class namedview
    {

        private string idField;

        private string pagecolorField;

        private string bordercolorField;

        private decimal borderopacityField;

        private decimal pageopacityField;

        private byte pageshadowField;

        private decimal zoomField;

        private decimal cxField;

        private decimal cyField;

        private string documentunitsField;

        private string currentlayerField;

        private bool showgridField;

        private ushort windowwidthField;

        private ushort windowheightField;

        private ushort windowxField;

        private ushort windowyField;

        private byte windowmaximizedField;

        private bool objectpathsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pagecolor
        {
            get
            {
                return this.pagecolorField;
            }
            set
            {
                this.pagecolorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bordercolor
        {
            get
            {
                return this.bordercolorField;
            }
            set
            {
                this.bordercolorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal borderopacity
        {
            get
            {
                return this.borderopacityField;
            }
            set
            {
                this.borderopacityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public decimal pageopacity
        {
            get
            {
                return this.pageopacityField;
            }
            set
            {
                this.pageopacityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public byte pageshadow
        {
            get
            {
                return this.pageshadowField;
            }
            set
            {
                this.pageshadowField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public decimal zoom
        {
            get
            {
                return this.zoomField;
            }
            set
            {
                this.zoomField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public decimal cx
        {
            get
            {
                return this.cxField;
            }
            set
            {
                this.cxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public decimal cy
        {
            get
            {
                return this.cyField;
            }
            set
            {
                this.cyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("document-units", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public string documentunits
        {
            get
            {
                return this.documentunitsField;
            }
            set
            {
                this.documentunitsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("current-layer", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public string currentlayer
        {
            get
            {
                return this.currentlayerField;
            }
            set
            {
                this.currentlayerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool showgrid
        {
            get
            {
                return this.showgridField;
            }
            set
            {
                this.showgridField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("window-width", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public ushort windowwidth
        {
            get
            {
                return this.windowwidthField;
            }
            set
            {
                this.windowwidthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("window-height", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public ushort windowheight
        {
            get
            {
                return this.windowheightField;
            }
            set
            {
                this.windowheightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("window-x", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public ushort windowx
        {
            get
            {
                return this.windowxField;
            }
            set
            {
                this.windowxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("window-y", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public ushort windowy
        {
            get
            {
                return this.windowyField;
            }
            set
            {
                this.windowyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("window-maximized", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public byte windowmaximized
        {
            get
            {
                return this.windowmaximizedField;
            }
            set
            {
                this.windowmaximizedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("object-paths", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public bool objectpaths
        {
            get
            {
                return this.objectpathsField;
            }
            set
            {
                this.objectpathsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/svg")]
    public partial class svgMetadata
    {

        private RDF rDFField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public RDF RDF
        {
            get
            {
                return this.rDFField;
            }
            set
            {
                this.rDFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#", IsNullable = false)]
    public partial class RDF
    {

        private Work workField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://creativecommons.org/ns#")]
        public Work Work
        {
            get
            {
                return this.workField;
            }
            set
            {
                this.workField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://creativecommons.org/ns#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://creativecommons.org/ns#", IsNullable = false)]
    public partial class Work
    {

        private string formatField;

        private type typeField;

        private string aboutField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public type type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string about
        {
            get
            {
                return this.aboutField;
            }
            set
            {
                this.aboutField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class type
    {

        private string resourceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string resource
        {
            get
            {
                return this.resourceField;
            }
            set
            {
                this.resourceField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/svg")]
    public partial class svgG
    {

        private svgGRect rectField;

        private svgGPath pathField;

        private string labelField;

        private string groupmodeField;

        private string idField;

        /// <remarks/>
        public svgGRect rect
        {
            get
            {
                return this.rectField;
            }
            set
            {
                this.rectField = value;
            }
        }

        /// <remarks/>
        public svgGPath path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public string label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public string groupmode
        {
            get
            {
                return this.groupmodeField;
            }
            set
            {
                this.groupmodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/svg")]
    public partial class svgGRect
    {

        private string styleField;

        private string idField;

        private decimal widthField;

        private decimal heightField;

        private decimal xField;

        private decimal yField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string style
        {
            get
            {
                return this.styleField;
            }
            set
            {
                this.styleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal x
        {
            get
            {
                return this.xField;
            }
            set
            {
                this.xField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal y
        {
            get
            {
                return this.yField;
            }
            set
            {
                this.yField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/svg")]
    public partial class svgGPath
    {

        private string styleField;

        private string dField;

        private string idField;

        private byte connectorcurvatureField;

        private string nodetypesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string style
        {
            get
            {
                return this.styleField;
            }
            set
            {
                this.styleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string d
        {
            get
            {
                return this.dField;
            }
            set
            {
                this.dField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("connector-curvature", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.inkscape.org/namespaces/inkscape")]
        public byte connectorcurvature
        {
            get
            {
                return this.connectorcurvatureField;
            }
            set
            {
                this.connectorcurvatureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd")]
        public string nodetypes
        {
            get
            {
                return this.nodetypesField;
            }
            set
            {
                this.nodetypesField = value;
            }
        }
    }

    #endregion Generated
}
