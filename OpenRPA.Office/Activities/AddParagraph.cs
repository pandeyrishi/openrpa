﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace OpenRPA.Office.Activities
{
    [System.ComponentModel.Designer(typeof(AddParagraphDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [System.Drawing.ToolboxBitmap(typeof(ResFinder2), "Resources.toolbox.readexcel.png")]
    public class AddParagraph : CodeActivity
    {
        [System.ComponentModel.Category("Input")]
        public InArgument<string> Filename { get; set; }
        [System.ComponentModel.Category("Input")]
        public InArgument<int> Index { get; set; }
        [System.ComponentModel.Category("Input")]
        public InArgument<string> Text { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var filename = Filename.Get(context);
            filename = Environment.ExpandEnvironmentVariables(filename);
            Application activeObject = null;
            object missing = Type.Missing;
            Document document = null;
            try
            {
                activeObject = (Application)Marshal.GetActiveObject("Word.Application");
                foreach (Document current in activeObject.Documents)
                {
                    if (current.FullName == filename)
                    {
                        document = current;
                        break;
                    }
                }
            }
            catch
            {
                activeObject = null;
            }
            finally
            {
                if(activeObject==null) activeObject = (Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                activeObject.Visible = true;
            }
            object ofilename = filename;
            if(document==null) document = activeObject.Documents.Open(ref ofilename, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
            var p = document.Content.Paragraphs;
            int index = Index.Get(context);
            if (Index.Expression == null || p.Count == index)
            {
                // if (!p.Last.Range.Text.EndsWith("\r") && p.Last.Range.End > 0) p.Last.Range.Text += "\r";
                var p2 = document.Paragraphs.Add();
                p2 = p.Last;
                p2.Range.Text = Text.Get(context);
            }
            else
            {
                if (p.Count < index) throw new Exception("filename only contains " + p.Count + " Paragraphs");

                var p2 = document.Paragraphs.Add(p[index + 1].Range);
                p2 = p[index + 1];
                p2.Range.Text = Text.Get(context) + "\r";
            }
        }
    }
}
