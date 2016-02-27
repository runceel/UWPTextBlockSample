using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ColoredTextBlock
{
    public sealed partial class ColoredTextBlock : UserControl
    {


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text), 
                typeof(string), 
                typeof(ColoredTextBlock), 
                new PropertyMetadata(null, (s, e) =>
                {
                    ((ColoredTextBlock)s).Parse();
                }));

        private void Parse()
        {
            this.TextBlock.Inlines.Clear();
            try
            {
                var doc = XDocument.Parse(this.Text);
                foreach (var node in doc.Root.Nodes())
                {
                    if (node.NodeType == System.Xml.XmlNodeType.Text)
                    {
                        var textNode = (XText)node;
                        this.TextBlock.Inlines.Add(new Run
                        {
                            Text = textNode.Value
                        });
                    }
                    else if (node.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        var elm = (XElement)node;
                        if (elm.Name != "Font")
                        {
                            continue;
                        }

                        var color = elm.Attribute("Color")?.Value ?? "Black";
                        var text = elm.Value;

                        var property = (Color)(typeof(Colors).GetTypeInfo().GetDeclaredProperty(color)?.GetValue(null) ?? Colors.Black);
                        this.TextBlock.Inlines.Add(new Run
                        {
                            Text = text,
                            Foreground = new SolidColorBrush(property)
                        });
                    }
                }
            }
            catch { }
        }

        public ColoredTextBlock()
        {
            this.InitializeComponent();
        }
    }
}
