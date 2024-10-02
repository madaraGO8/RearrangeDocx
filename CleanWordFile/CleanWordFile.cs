using DocumentFormat.OpenXml.Drawing;
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
using System.Xml.Linq;

namespace CleanWordFile
{
    class CleanWordFile
    {
        #region CleanDocx
        public void CleanDocx(string path, string newPath, bool isTrue)
        {
            string autoStyleConfig = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\10Packages\New folder\AutostyleConfig.xml";
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
            if (File.Exists(path))
            {
                File.Copy(path, newPath, true);
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

                    #region DocxListForBackMatter
                    string firstMatchingHeading = string.Empty;
                    int firstMatchingIndex = -1;

                    for (int i = 0; i <= docxList.Count - 1; i++)
                    {
                        foreach (var heading in backMatterList)
                        {
                            if (docxList[i].Value.ToLower().StartsWith(heading) && docxList[i].Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 3)  // Find the first match
                            {
                                firstMatchingHeading = heading;
                                firstMatchingIndex = i;
                            }
                        }
                    }
                    if (firstMatchingIndex != -1)
                    {
                        for (int i = firstMatchingIndex; i < docxList.Count; i++)
                        {
                            docxList[i].Remove();
                        }
                    }
                    wDoc.MainDocumentPart.PutXDocument();
                    #endregion

                    #region ReloadMS2
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
                                var refEndNote = refNext.Descendants(W.endnotePr).FirstOrDefault();
                                var refInMan = references.ElementsAfterSelf().ToList();

                                if (refEndNote != null)
                                {
                                    var xEndnoteDoc = XDocument.Load(wDoc.MainDocumentPart.EndnotesPart.GetStream());
                                    XElement rootEndnote = xEndnoteDoc.Root;
                                    var precedingEle = rootEndnote.Descendants(W.p).ToList();
                                    foreach (var ele in precedingEle)
                                    {
                                        ele.RemoveNodes();
                                    }
                                    references.Remove();
                                    using (var stream = wDoc.MainDocumentPart.EndnotesPart.GetStream(FileMode.Create))
                                    {
                                        xEndnoteDoc.Save(stream);
                                    }
                                    wDoc.MainDocumentPart.PutXDocument();
                                }
                                else if (refInMan != null)
                                {
                                    foreach (var _ref in refInMan)
                                    {
                                        _ref.Remove();
                                    }
                                    references.Remove();

                                    wDoc.MainDocumentPart.PutXDocument();
                                }
                            }
                        }
                        else if (splitReferences != null)
                        {

                            var refNext = splitReferences.ElementsAfterSelf();

                            if (refNext != null)
                            {
                                var refEndNote = refNext.Descendants(W.endnotePr).FirstOrDefault();
                                var refInMan = splitReferences.ElementsAfterSelf().ToList();
                                if (refEndNote != null)
                                {
                                    var xEndnoteDoc = XDocument.Load(wDoc.MainDocumentPart.EndnotesPart.GetStream());
                                    XElement rootEndnote = xEndnoteDoc.Root;
                                    var precedingEle = rootEndnote.Descendants(W.p).ToList();
                                    foreach (var ele in precedingEle)
                                    {
                                        ele.RemoveNodes();
                                    }
                                    splitReferences.Remove();
                                    using (var stream = wDoc.MainDocumentPart.EndnotesPart.GetStream(FileMode.Create))
                                    {
                                        xEndnoteDoc.Save(stream);
                                    }
                                    wDoc.MainDocumentPart.PutXDocument();
                                }
                                else if (refInMan != null)
                                {
                                    foreach (var _ref in refInMan)
                                    {
                                        _ref.Remove();
                                    }
                                    splitReferences.Remove();

                                    wDoc.MainDocumentPart.PutXDocument();
                                }
                            }
                        }
                        wDoc.MainDocumentPart.PutXDocument();
                    }
                    catch (Exception ex) { }
                    #endregion
                    wDoc.Save();
                }
            }
        }
        #endregion

        #region RemoveFootnotesEndnotes
        public void RemoveFootnotesEndnotes(string filePath, bool isTrue)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, isTrue))
            {
                try
                {
                    foreach (var footnoteReference in doc.MainDocumentPart.Document.Descendants<FootnoteReference>().ToList())
                    {
                        footnoteReference.Remove();
                    }

                    foreach (var endnoteReference in doc.MainDocumentPart.Document.Descendants<EndnoteReference>().ToList())
                    {
                        endnoteReference.Remove();
                    }
                    doc.MainDocumentPart.Document.Save();
                }
                catch (Exception ex) { }
            }
        }
        #endregion

        #region Remove Blank Pages
        public void RemoveEmptyParagraphs(string newPath, bool isTrue)
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
        private bool IsParagraphEmptyOrPageBreak(XElement paragraph)
        {
            bool hasTextContent = paragraph.Descendants(W.t).Any(t => !string.IsNullOrWhiteSpace(t.Value));
            bool hasPageBreak = paragraph.Descendants(W.br)
                                         .Any(br => (string)br.Attribute(W.type) == "page");
            bool hasSectionBreak = paragraph.Descendants(W.sectPr).Any();
            return !hasTextContent && (hasPageBreak || hasSectionBreak);
        }

        //private bool IsParagraphEmptyOrPageBreak(XElement paragraph)
        //{
        //    return string.IsNullOrWhiteSpace(paragraph.Value) ||
        //           paragraph.Descendants(W.br).Any(br => (string)br.Attribute(W.type) == "page");
        //}
        #endregion

        public void RemoveSectionBreaks(string filePath)
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
    }
}


