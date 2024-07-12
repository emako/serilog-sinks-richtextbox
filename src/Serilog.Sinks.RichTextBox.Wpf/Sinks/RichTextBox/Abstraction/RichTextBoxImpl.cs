#region Copyright 2021-2023 C. Augusto Proiete & Contributors

//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using Serilog.Debugging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Serilog.Sinks.RichTextBox.Abstraction;

public class RichTextBoxImpl : IRichTextBox
{
    public System.Windows.Controls.RichTextBox RichTextBox { get; set; } = null!;

    public RichTextBoxImpl()
    {
    }

    public RichTextBoxImpl(System.Windows.Controls.RichTextBox richTextBox)
    {
        RichTextBox = richTextBox;
    }

    public void Write(string xamlParagraphText)
    {
        if (RichTextBox == null)
        {
            return;
        }

        Paragraph parsedParagraph;

        try
        {
            parsedParagraph = (Paragraph)XamlReader.Parse(xamlParagraphText);
        }
        catch (XamlParseException ex)
        {
            SelfLog.WriteLine($"Error parsing `{xamlParagraphText}` to XAML: {ex.Message}");
            throw;
        }

        var inlines = parsedParagraph.Inlines.ToList();

        var richTextBox = RichTextBox;

        var flowDocument = richTextBox.Document ??= new FlowDocument();

        if (flowDocument.Blocks.LastBlock is not Paragraph paragraph)
        {
            paragraph = new Paragraph();
            flowDocument.Blocks.Add(paragraph);
        }

        paragraph.Inlines.AddRange(inlines);
    }

    public bool CheckAccess()
    {
        return RichTextBox?.CheckAccess() ?? false;
    }

    public Task BeginInvoke(DispatcherPriority priority, Delegate method, object arg)
    {
        return RichTextBox?.Dispatcher.BeginInvoke(priority, method, arg).Task ?? Task.CompletedTask;
    }
}
