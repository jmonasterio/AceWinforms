/*
Copyright (c) 2013, Jorge Monasterio
All rights reserved.


Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of Ajax.org B.V. nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.


THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL AJAX.ORG B.V. BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AceWinforms
{
    public partial class CodeEditor : WebBrowser
    {
        private string _minIeVersion;

        public CodeEditor()
        {
            SetDefaults();

            InitializeComponent();

            //Load();
        }

        /// <summary>
        /// Pick a syntax highlighter from: https://github.com/ajaxorg/ace/tree/master/lib/ace/mode
        /// </summary>
        public string HighlighterMode { get; set; }

        /// <summary>
        /// Pick a theme name from: https://github.com/ajaxorg/ace/tree/master/lib/ace/theme
        /// </summary>
        public string Theme { get; set; }

        private void SetDefaults()
        {
            HighlighterMode = "javascript";
            Theme = "monokai"; 
            MinIeVersion = "10";

        }

        public string MinIeVersion
        {
            get
            {
                return _minIeVersion;
            }
            set
            {
                if (value != "9" && value != "10" && value != "11")
                {
                    // At least IE9 is required
                    throw new Exception("IE 9 is required.");
                }
                _minIeVersion = value;
            } 
        }

        public void Load()
        {
            // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION
            //MessageBox.Show(this, "" + webBrowser1.Version);
            var template = GenerateEditorHtmlFromProps();
            this.DocumentText = template;
        }

        private string _textBeforeHtmlLoaded;
        public string Text
        {
            set
            {
                var editorHtmlElement = GetEditorElement();
                if (editorHtmlElement != null)
                {

                    editorHtmlElement.InnerText = value;
                }
                else
                {
                    _textBeforeHtmlLoaded = value;
                }
            }
        
            get
            {
                var editorHtmlElement = GetEditorElement();
                if (editorHtmlElement != null)
                {

                    return editorHtmlElement.InnerText;
                }
                else
                {
                    return _textBeforeHtmlLoaded;
                }
                
            }
        }

        private HtmlElement GetEditorElement()
        {
            if (this.Document== null) 
            { return null; 
}
            return this.Document.GetElementById("editor");
        }

        private string GenerateEditorHtmlFromProps()
        {
            var template = ReadEmbeddedHtmlEditorTemplate(); //File.ReadAllText("s:/src/acewinforms/editor.html");
            template = ReplaceTemplateField(template, "{{highlighter}}", this.HighlighterMode);
            template = ReplaceTemplateField(template, "{{theme}}", this.Theme);
            template = ReplaceTemplateField(template, "{{minIeVersion}}", this.MinIeVersion);
            template = ReplaceTemplateField(template, "{{editorText}}", this.Text);
            return template;
        }

        private string ReplaceTemplateField(string template, string field, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                template = template.Replace(field, value);
            }
            return template;
        }

      

        private string ReadEmbeddedHtmlEditorTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var imageStream = assembly.GetManifestResourceStream("AceWinforms.html.editor.html"))
            {
                using (var textStreamReader = new StreamReader(imageStream))
                {
                    return textStreamReader.ReadToEnd();
                }

            }

        }
    }
}
