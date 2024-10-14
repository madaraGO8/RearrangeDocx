using DocumentFormat.OpenXml.Packaging;
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
    class RearrangeDocx
    {
        public void RearrangeEle(string path, bool isTrue)
        {
            string autoStyleConfig = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\10Packages\New folder\AutostyleConfig.xml";
            //string autoStyleConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutostyleConfig.xml");
            string abc = File.ReadAllText(autoStyleConfig);
            XElement autoStyleContent = XElement.Parse(abc);

            List<string> backMatterList = new List<string> { "appendix", "notes", "note", "endnotes", "endnote", "footnotes", "footnote", "figure" };

            List<string> suppHead = autoStyleContent.Descendants("component").Where(a => a.Attribute("type") != null && (a.Attribute("type").Value == "Referencing")).Descendants("manuscript-headings").
                Where(a => a.Attribute("type").Value == "Supplementaryhead").Descendants("term").Select(term => term.Value.ToLower().Trim()).ToList();

            backMatterList.AddRange(suppHead);

            List<XElement> toMoveRef = new List<XElement>();

            if (File.Exists(path))
            {
                using (WordprocessingDocument wDoc = WordprocessingDocument.Open(path, isTrue))
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
                                toMoveRef.Add(para);
                            }
                            foreach (var para in referencesData)
                            {
                                para.Remove();
                            }
                            wDoc.MainDocumentPart.PutXDocument();
                        }
                    }

                    #region ReloadMS2
                    xDoc = wDoc.MainDocumentPart.GetXDocument();
                    root = xDoc.Root;
                    docxList = root.Descendants(W.p).ToList();
                    #endregion

                    #region AddReferencesAtLast
                    XElement bodyEle = root.Element(W.body);

                    foreach (var refPara in toMoveRef)
                    {
                        bodyEle.Add(refPara);
                    }
                    wDoc.MainDocumentPart.PutXDocument();
                    #endregion
                }
            }
        }
    }
}
