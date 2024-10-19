using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace CleanWordFile
{
    class CleanWordFile
    {
        List<XElement> footnoteEndnote = new List<XElement>();
        List<OpenXmlElement> footnoteEndnoteList = new List<OpenXmlElement>();

        #region Start Body Docx
        public void StartBodyDocx(string sourcePath, string targetPath, bool isEditable)
        {
            FetchRemoveAndAppendNotes(sourcePath, targetPath, isEditable);
            CleanDocx(targetPath, isEditable);
            AppendNotesToBody(targetPath, isEditable);
            RemoveSectionBreaks(targetPath);
            RemoveEmptyParagraphs(targetPath, isEditable);
            RecheckBody(targetPath, isEditable);
        }
        #endregion


        #region Finding Start and end of Body
        public void CleanDocx(string newPath, bool isTrue)
        {
            string autoStyleConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutostyleConfig.xml");
            string abc = File.ReadAllText(autoStyleConfig);
            XElement autoStyleContent = XElement.Parse(abc);
            Regex cleanupRegex = new Regex(@"[:\-]+$", RegexOptions.Compiled);

            List<string> backMatterList = new List<string> { "endnotes", "endnote", "footnotes", "footnote" };

            List<string> suppHead = autoStyleContent.Descendants("component").Where(a => a.Attribute("type") != null && (a.Attribute("type").Value == "Referencing")).Descendants("manuscript-headings").
                Where(a => a.Attribute("type").Value == "Supplementaryhead").Descendants("term").Select(term => term.Value.ToLower().Trim()).ToList();

            backMatterList.AddRange(suppHead);

            List<string> frontMatterList = autoStyleContent.Descendants("component").Where(a => a.Attribute("type") != null && (a.Attribute("type").Value == "Metadata")).Descendants("manuscript-headings").
                Where(a => a.Attribute("type").Value == "ArticleCategory" || a.Attribute("type").Value == "Graphical Abstract" || a.Attribute("type").Value == "Abstract" || a.Attribute("type").Value == "Highlight Abstract" ||
                a.Attribute("type").Value == "Keyword" || a.Attribute("type").Value == "JEL" || a.Attribute("type").Value == "Correspondence" || a.Attribute("type").Value == "Subtitle").Descendants("term").
                Select(term => term.Value.ToLower().Trim()).ToList();
            List<string> frontMatterList1 = new List<string> { "highlights" };
            frontMatterList.AddRange(frontMatterList1);

            Regex referencesRegex = new Regex(@"^\b\d? ?-? ?reference\b|^\b\d? ?-? ?references\b|^\bfurther reading\b|^\bliterature\b|^\bliterature cited\b$|^\bworks cited\b$|^\breferences cited\b$|^\breferencias\b$|^\bbibliography\b$|^\bbibliography list\b");
            Regex keywordsRegex = new Regex(@"^\bkeyword group\b|^\bkeyword\b|^\bkeywords\b|^\bkeyterms\b|^\bkey-word\b|^\bkey-words\b|^\bkey word\b|^\bkey words\b");
            bool isKeywordLast = false;
            if (File.Exists(newPath))
            {
                using (WordprocessingDocument wDoc = WordprocessingDocument.Open(newPath, isTrue))
                {
                    var xDoc = wDoc.MainDocumentPart.GetXDocument();
                    XElement root = xDoc.Root;

                    #region DocxListForFrontMatter
                    //List<string> docxList = root.Descendants(W.p).Descendants(W.r).Descendants(W.t).Select(t => t.Value.ToLower().Trim()).Take(50).ToList();
                    List<XElement> docxList = root.Descendants(W.p).ToList();

                    List<string> docxList1 = root.Descendants(W.p).Select(p => string.Concat(p.Descendants(W.t)
                        .Select(t => t.Value)).ToLower().Trim()).Select(text => cleanupRegex.Replace(text, "")).ToList();
                    string lastMatchingHeading = string.Empty;
                    string introHeading = string.Empty;
                    int lastMatchingIndex = -1;
                    int introIndex = -1;
                    for (int i = 0; i < docxList1.Count; i++)
                    {
                        foreach (var heading in frontMatterList)
                        {
                            if (docxList1[i].StartsWith(heading))
                            {
                                lastMatchingHeading = heading;
                                lastMatchingIndex = i;
                                if (keywordsRegex.IsMatch(heading))
                                {
                                    isKeywordLast = true;
                                }
                            }
                        }
                        if (Regex.IsMatch(docxList1[i], @"^\bintroduction\b", RegexOptions.IgnoreCase) && docxList1[i].ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 3)
                        {
                            introIndex = i;
                        }
                    }
                    if (introIndex != -1)
                    {
                        XElement targetParagraph = docxList[introIndex];
                        if (targetParagraph != null)
                        {
                            var allElements = root.Descendants().Where(e => e.Name == W.p || e.Name == W.tbl || e.Name == W.drawing).ToList();
                            int targetIndex = allElements.IndexOf(targetParagraph);
                            if (targetIndex > 0)
                            {
                                for (int i = 0; i < targetIndex; i++)
                                {
                                    allElements[i].Remove();
                                }
                            }
                        }
                    }
                    else if (lastMatchingIndex != -1)
                    {
                        var tempLastMatchingIndex = lastMatchingIndex;

                        if (isKeywordLast && docxList[lastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length != 0 && docxList[tempLastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length >= 3)
                        {
                            tempLastMatchingIndex += 1;
                        }
                        else if (isKeywordLast && docxList[lastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length != 0 && docxList[tempLastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 4)
                        {
                            tempLastMatchingIndex += 2;
                        }
                        else
                        {
                            if (docxList[lastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length != 0 && docxList[lastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length >= 3)
                            {
                                tempLastMatchingIndex = lastMatchingIndex + 1;
                            }
                            else if (docxList[lastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length != 0 && docxList[lastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 4)
                            {
                                tempLastMatchingIndex = lastMatchingIndex + 2;
                            }

                            while (docxList[tempLastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 0 ||
                                    docxList[tempLastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length >= 10)
                            {
                                tempLastMatchingIndex++;
                            }
                        }
                        while (tempLastMatchingIndex < docxList.Count && docxList[tempLastMatchingIndex].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 0)
                        {
                            tempLastMatchingIndex++;
                        }

                        if (tempLastMatchingIndex != 0)
                        {
                            XElement targetParagraph = docxList[tempLastMatchingIndex];
                            if (targetParagraph != null)
                            {
                                var allElements = root.Descendants().Where(e => e.Name == W.p || e.Name == W.tbl || e.Name == W.drawing).ToList();
                                int targetIndex = allElements.IndexOf(targetParagraph);
                                if (targetIndex > 0)
                                {
                                    for (int i = 0; i < targetIndex; i++)
                                    {
                                        allElements[i].Remove();
                                    }
                                }
                            }
                        }
                    }
                    wDoc.MainDocumentPart.PutXDocument();
                    #endregion

                    #region ReloadMS
                    xDoc = wDoc.MainDocumentPart.GetXDocument();
                    root = xDoc.Root;
                    docxList = root.Descendants(W.p).ToList();
                    #endregion

                    #region References
                    try
                    {
                        var references = root.Descendants(W.p)
                        .Where(p => p.Ancestors(W.tc).Count() == 0 && p.Descendants(W.r).Count() <= 3 // Ensure there are 3 or fewer runs
                            && p.Descendants(W.r).Any(r =>
                            {
                                var textEle = r.Descendants(W.t).FirstOrDefault();
                                if (textEle != null)
                                {
                                    string text = textEle.Value.ToLower().Trim();
                                    if (referencesRegex.IsMatch(text) && !text.Contains("literature review") && !text.Contains("literature summary") && r.Parent.Name == W.p && text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 3)
                                    {
                                        return true;
                                    }
                                }
                                return false;
                            })
                        )
                        .FirstOrDefault();

                        var splitReferences = root.Descendants(W.p).Where(p => p.Ancestors(W.tc).Count() == 0 && p.Descendants(W.r).Any(r =>
                        {
                            var combinedText = string.Concat(p.Descendants(W.r).Descendants(W.t).Select(t => t.Value)).ToLower().Trim();
                            if (referencesRegex.IsMatch(combinedText) && !combinedText.Contains("literature review") && !combinedText.Contains("literature summary") && combinedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 3)
                            {
                                return true;
                            }
                            return false;
                        })).FirstOrDefault();

                        if (references != null)
                        {
                            var refNext = references.ElementsAfterSelf();

                            if (refNext != null)
                            {
                                var refInMan = references.ElementsAfterSelf().ToList();
                                if (refInMan != null)
                                {
                                    foreach (var _ref in refInMan)
                                    {
                                        _ref.Remove();
                                    }
                                    references.Remove();
                                }
                            }
                            wDoc.MainDocumentPart.PutXDocument();
                        }
                        else if (splitReferences != null)
                        {
                            var refNext = splitReferences.ElementsAfterSelf();
                            if (refNext != null)
                            {
                                var refInMan = splitReferences.ElementsAfterSelf().ToList();
                                if (refInMan != null)
                                {
                                    foreach (var _ref in refInMan)
                                    {
                                        _ref.Remove();
                                    }
                                    splitReferences.Remove();

                                }
                            }
                            wDoc.MainDocumentPart.PutXDocument();
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                    wDoc.MainDocumentPart.PutXDocument();
                    wDoc.Save();
                }
            }
        }
        #endregion

        #region RemoveFootnotesEndnotesNew
        public void FetchRemoveAndAppendNotes(string sourcePath, string targetPath, bool isEditable)
        {
            try
            {
                if (File.Exists(sourcePath))
        {
                    File.Copy(sourcePath, targetPath, true);

                    using (WordprocessingDocument doc = WordprocessingDocument.Open(targetPath, isEditable))
            {
                try
                {
                            var mainDocPart = doc.MainDocumentPart;

                            #region Fetch and Remove Footnotes
                            foreach (var footnoteReference in mainDocPart.Document.Descendants<FootnoteReference>().ToList())
                    {
                                var footnote = mainDocPart.FootnotesPart?.Footnotes.Elements<Footnote>()
                                           .FirstOrDefault(fn => fn.Id == footnoteReference.Id);

                        if (footnote != null)
                        {
                                    foreach (var paragraph in footnote.Elements<Paragraph>())
                                    {
                                        var clonedParagraph = new Paragraph();
                                        clonedParagraph.Append(paragraph.Elements<Run>().Select(run => run.CloneNode(true)));
                                        // clonedParagraph.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = "Para" });
                                        footnoteEndnoteList.Add(clonedParagraph);
                                    }
                        }
                        footnoteReference.Remove();
                    }
                            #endregion

                            #region Fetch and Remove Endnotes
                            foreach (var endnoteReference in mainDocPart.Document.Descendants<EndnoteReference>().ToList())
                    {
                                var endnote = mainDocPart.EndnotesPart?.Endnotes.Elements<Endnote>()
                                         .FirstOrDefault(en => en.Id == endnoteReference.Id);

                        if (endnote != null)
                        {
                                    foreach (var paragraph in endnote.Elements<Paragraph>())
                                    {
                                        var clonedParagraph = new Paragraph();
                                        clonedParagraph.Append(paragraph.Elements<Run>().Select(run => run.CloneNode(true)));
                                        clonedParagraph.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = "Para" });
                                        footnoteEndnoteList.Add(clonedParagraph);
                                    }
                        }
                        endnoteReference.Remove();
                    }
                            #endregion

                            if (footnoteEndnoteList.Count > 0)
                    {
                                var headingPara = new Paragraph(new Run(new Text("Footnote/Endnote")));
                        var headingProperties = new ParagraphProperties(new ParagraphStyleId() { Val = "Heading1" });
                                headingPara.InsertAt(headingProperties, 0);
                                footnoteEndnoteList.Insert(0, headingPara);
                    }

                            mainDocPart.Document.Save();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }

                    }
                }
            }
            catch (Exception ex) { }
        }
        #endregion

        #region Append FootEndnotes in Body
        public void AppendNotesToBody(string targetPath, bool isEditable)
        {
            try
            {
                if (footnoteEndnoteList.Count == 0)
                    return;

                using (WordprocessingDocument wDoc = WordprocessingDocument.Open(targetPath, isEditable))
                        {
                    var mainPart = wDoc.MainDocumentPart;
                    var body = mainPart.Document.Body;

                    foreach (var note in footnoteEndnoteList)
                    {
                        body.Append(note.CloneNode(true));
                    }
                    footnoteEndnoteList.Clear();
                    mainPart.Document.Save();
                }
                }
            catch (Exception ex) { }
        }
        #endregion

        #region Remove Blank Pages
        public void RemoveEmptyParagraphs(string newPath, bool isTrue)
        {
            try
            {
            using (WordprocessingDocument wDoc = WordprocessingDocument.Open(newPath, isTrue))
            {
                XDocument root = wDoc.MainDocumentPart.GetXDocument();
                List<XElement> docxList = root.Descendants(W.p).ToList();

                if (docxList != null && docxList.Any())
                {
                    int firstNonEmptyIndex = docxList.FindIndex(p => !IsParagraphEmptyOrPageBreak(p));

                    int lastNonEmptyIndex = docxList.FindLastIndex(p => !IsParagraphEmptyOrPageBreak(p));

                    if (firstNonEmptyIndex > 0)
                    {
                        for (int i = 0; i < firstNonEmptyIndex; i++)
                        {
                            if (IsParagraphEmptyOrPageBreak(docxList[i]))
                            {
                                docxList[i].Remove();
                            }
                        }
                    }

                    if (lastNonEmptyIndex < docxList.Count - 1)
                    {
                        for (int i = docxList.Count - 1; i > lastNonEmptyIndex; i--)
                        {
                            if (IsParagraphEmptyOrPageBreak(docxList[i]))
                            {
                                docxList[i].Remove();
                            }
                        }
                    }
                }
                wDoc.MainDocumentPart.PutXDocument();
                wDoc.Save();
            }
            }
            catch (Exception ex) { }
        }
        private bool IsParagraphEmptyOrPageBreak(XElement paragraph)
        {
            try
        {
            bool hasTextContent = paragraph.Descendants(W.t).Any(t => !string.IsNullOrWhiteSpace(t.Value));
            bool hasPageBreak = paragraph.Descendants(W.br)
                                         .Any(br => (string)br.Attribute(W.type) == "page");
            bool hasSectionBreak = paragraph.Descendants(W.sectPr).Any();
            return !hasTextContent && (hasPageBreak || hasSectionBreak);
        }
            catch (Exception ex) { }
            return false;
        }
        #endregion

        #region Remove Section Breaks
        public void RemoveSectionBreaks(string filePath)
        {
            try
            {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, true))
            {
                MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                var paragraphsWithSectionBreaks = mainPart.Document.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>()
                                                    .Where(p => p.Descendants<SectionProperties>().Any())
                                                    .ToList();
                foreach (var paragraph in paragraphsWithSectionBreaks)
                {
                    var sectionProperties = paragraph.Descendants<SectionProperties>().FirstOrDefault();
                    if (sectionProperties != null)
                    {
                        sectionProperties.Remove();
                    }
                }
                var lastSectionProperties = mainPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
                if (lastSectionProperties != null)
                {
                    lastSectionProperties.Remove();
                }
                mainPart.Document.Save();
            }
            }
            catch (Exception ex) { }
        }
        #endregion

        #region Recheck - Body.docx
        private void RecheckBody(string path, bool isEditable)
        {
            List<string> backMatterList = new List<string> { "appendix", "notes", "note", "endnotes", "endnote", "footnotes", "footnote", "figure" };

            string autoStyleConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutostyleConfig.xml");

            string abc = File.ReadAllText(autoStyleConfig);

            XElement autoStyleContent = XElement.Parse(abc);

            List<string> suppHead = autoStyleContent.Descendants("component").Where(a => a.Attribute("type") != null && (a.Attribute("type").Value == "Referencing")).Descendants("manuscript-headings").
                Where(a => a.Attribute("type").Value == "Supplementaryhead").Descendants("term").Select(term => term.Value.ToLower().Trim()).ToList();

            backMatterList.AddRange(suppHead);

            if (File.Exists(path))
            {
                using (WordprocessingDocument wDoc = WordprocessingDocument.Open(path, isEditable))
                {
                    var xDoc = wDoc.MainDocumentPart.GetXDocument();
                    XElement root = xDoc.Root;
                    List<XElement> docxList = root.Descendants(W.p).ToList();

                    //Regex referencesRegex = new Regex(@"\breference\b|\breferences\b|\bfurther reading\b|\bliterature\b|^\bliterature cited\b$|^\bliterature\b|^\bworks cited\b$|^\breferences cited\b$|^\breferencias\b$|^\bbibliography\b$|\bbibliography list\b");
                    Regex referencesRegex = new Regex(@"^\breference\b|^\breferences\b|^\bfurther reading\b|^\bliterature\b|^\bliterature cited\b$|^\bliterature\b|^\bworks cited\b$|^\breferences cited\b$|^\breferencias\b$|^\bbibliography\b$|^\bbibliography list\b", RegexOptions.IgnoreCase);
                    Regex tblFig = new Regex(@"^\bTable\b|^\bTables\b|\bFigures\b|\bFigure\b", RegexOptions.IgnoreCase);

                    var referencesWithIndex = root.Descendants(W.p)
                        .Select((p, index) => new { Paragraph = p, Index = index })
                        .Where(pWithIndex => pWithIndex.Paragraph.Ancestors(W.tc).Count() == 0 &&
                        pWithIndex.Paragraph.Descendants(W.r).Any(r =>
                        {
                            var textEle = r.Descendants(W.t).FirstOrDefault();
                            if (textEle != null)
                            {
                                string text = textEle.Value.ToLower();
                                if (referencesRegex.IsMatch(text) && r.Parent.Name == W.p)
                                {
                                    return true;
                                }
                            }
                            return false;
                        })).FirstOrDefault();

                    var splitReferencesWithIndex = root.Descendants(W.p).Select((p, index) => new { Paragraph = p, Index = index })
                        .Where(pWithIndex =>
                        {
                            var paragraph = pWithIndex.Paragraph;
                            if (paragraph.Ancestors(W.tc).Any()) return false;
                            var combinedText = string.Concat(paragraph.Descendants(W.r).Descendants(W.t).Select(t => t.Value)).ToLower().Trim();
                            return referencesRegex.IsMatch(combinedText) &&
                                    !combinedText.Contains("literature review") &&
                                    !combinedText.Contains("literature summary") &&
                                    combinedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 3;
                        })
                         .FirstOrDefault();

                    if (referencesWithIndex != null)
                    {
                        var referencesElement = referencesWithIndex.Paragraph;
                        var referencesIndex = referencesWithIndex.Index;

                        var nextParas = docxList.Skip(referencesIndex + 1).Select((p, index) => new { Paragraph = p, Index = index + referencesIndex + 1 });
                        var stopIndex = -1;

                        foreach (var para in nextParas)
                        {
                            bool figureAfterRef = para.Paragraph.Descendants(W.drawing).Any();
                            bool headingAfterRef = para.Paragraph.Descendants(W.r).Any(run => run.Descendants(W.b) != null);
                            if (figureAfterRef)
                            {
                                stopIndex = para.Index;
                                break;
                            }
                            else
                            {
                                var text = string.Concat(para.Paragraph.Descendants(W.t).Select(t => t.Value)).Trim();
                                var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (words.Length > 0 && words.Length <= 4)
                                {
                                    if (backMatterList.Any(item => text.Equals(item, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        stopIndex = para.Index;
                                        break;
                                    }
                                    else if (tblFig.IsMatch(text))
                                    {
                                        stopIndex = para.Index;
                                        break;
                                    }
                                    else if (headingAfterRef)
                                    {
                                        stopIndex = para.Index;
                                        break;
                                    }
                                }
                            }

                        }
                        if (stopIndex != -1)
                        {
                            //Fetching the paragraphs between References index and stopIndex - 1
                            var referencesData = docxList
                                .Skip(referencesIndex)
                                .Take(stopIndex - referencesIndex)
                                .ToList();

                            foreach (var para in referencesData)
                            {
                                para.Remove();
                            }
                            wDoc.MainDocumentPart.PutXDocument();
                        }
                    }
                    else if (splitReferencesWithIndex != null)
                    {
                        var referencesElement = splitReferencesWithIndex.Paragraph;
                        var referencesIndex = splitReferencesWithIndex.Index;

                        var nextParas = docxList.Skip(referencesIndex + 1).Select((p, index) => new { Paragraph = p, Index = index + referencesIndex + 1 });
                        var stopIndex = -1;

                        foreach (var para in nextParas)
                        {
                            bool figureAfterRef = para.Paragraph.Descendants(W.drawing).Any();
                            bool headingAfterRef = para.Paragraph.Descendants(W.r).Any(run => run.Descendants(W.b) != null);
                            if (figureAfterRef)
                            {
                                stopIndex = para.Index;
                                break;
                            }
                            else
                            {
                                var text = string.Concat(para.Paragraph.Descendants(W.t).Select(t => t.Value)).Trim();
                                var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (words.Length > 0 && words.Length <= 4)
                                {
                                    if (backMatterList.Any(item => text.Equals(item, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        stopIndex = para.Index;
                                        break;
                                    }
                                    else if (tblFig.IsMatch(text))
                                    {
                                        stopIndex = para.Index;
                                        break;
                                    }
                                    else if (headingAfterRef)
                                    {
                                        stopIndex = para.Index;
                                        break;
                                    }
                                }
                            }

                        }
                        if (stopIndex != -1)
                        {
                            //Fetching the paragraphs between References index and stopIndex - 1
                            var referencesData = docxList
                                .Skip(referencesIndex)
                                .Take(stopIndex - referencesIndex)
                                .ToList();

                            foreach (var para in referencesData)
                            {
                                para.Remove();
                            }
                            wDoc.MainDocumentPart.PutXDocument();
                        }
                    }
                    wDoc.MainDocumentPart.PutXDocument();
                    wDoc.Save();
                }
            }
        }
        #endregion
    }
}


