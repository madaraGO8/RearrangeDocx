using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static XmlDataExtraction.DtoRoot;

namespace XmlDataExtraction
{
    class ReadMetaData
    {
        public void ReadMetaDataXml(string path)
        {
            //DtoArticleTitle dtoArticleTitle = new DtoArticleTitle();
            DtoAuthorMetadata dtoAuthorMetadata = new DtoAuthorMetadata();
            DtoAffliations dtoAffliations = new DtoAffliations();
            List<string> Keywords = new List<string>();

            if (File.Exists(path))
            {
                string xmlContent = File.ReadAllText(path);
                XElement art = XElement.Parse(xmlContent);
                var authorAffiliations = art.Descendants("author").ToList();

                #region Article Title
                var articleTitle = art.Descendants("article_title").FirstOrDefault().Value;
                #endregion

                #region Table Count
                var tableCount = art.Descendants("total_tables").FirstOrDefault().Value;
                #endregion

                #region Figure Count
                var figureCount = art.Descendants("total_figures").FirstOrDefault().Value;
                #endregion

                #region Keywords
                var keywords = art.Descendants("content").ToList();
                foreach (var keyword in keywords)
                {
                    var key = keyword.Descendants("attr_type").Where(a => a.Attribute("name")?.Value == "Keywords").Descendants("attribute").Where(b => b.Value == "selected").ToList();
                    if (key.Count == 1)
                    {
                        var nameAttribute = key.FirstOrDefault()?.Attribute("name")?.Value;
                        var namePart = nameAttribute.Split(',');
                        foreach (var _keyword in namePart)
                        {
                            Keywords.Add(_keyword.TrimStart());
                        }

                    }
                    else
                    {
                        foreach (var attrib in key)
                        {
                            var nameAttribute = attrib.Attribute("name")?.Value;
                            Keywords.Add(nameAttribute.TrimStart());
                            //var namePart = nameAttribute.Split(',');
                        }
                    }


                }
                #endregion

                #region Authors
                var authors = art.Descendants("author_list").ToList();
                foreach (var author in authors)
                {
                    var person = author.Element("author");

                    if (person != null)
                    {
                        try { dtoAuthorMetadata.salutation = person.Element("salutation")?.Value.Trim(); } catch { }
                        try { dtoAuthorMetadata.firstname = person.Element("first_name")?.Value.Trim(); } catch { }
                        try { dtoAuthorMetadata.middleName = person.Element("middle_name")?.Value.Trim(); } catch { }
                        try { dtoAuthorMetadata.lastName = person.Element("last_name")?.Value.Trim(); } catch { }
                        try { dtoAuthorMetadata.degree = person.Element("degree")?.Value.Trim(); } catch { }
                        try { dtoAuthorMetadata.email = person.Element("email")?.Value.Trim(); } catch { }
                        try { dtoAuthorMetadata.orcid = person.Element("orcid")?.Value.Trim(); } catch { }
                    }
                }
                #endregion

                #region Affiliations
                foreach (var affiliation in authorAffiliations)
                {
                    var aff = affiliation.Element("affiliation");

                    if (aff != null)
                    {
                        try { dtoAffliations.institute = aff.Element("inst")?.Value.Trim(); } catch { }
                        try { dtoAffliations.department = aff.Element("dept")?.Value.Trim(); } catch { }
                        try { dtoAffliations.addr1 = aff.Element("addr1")?.Value.Trim(); } catch { }
                        try { dtoAffliations.addr2 = aff.Element("addr2")?.Value.Trim(); } catch { }
                        try { dtoAffliations.addr3 = aff.Element("addr3")?.Value.Trim(); } catch { }
                        try { dtoAffliations.city = aff.Element("city")?.Value.Trim(); } catch { }
                        try { dtoAffliations.province = aff.Element("province")?.Value.Trim(); } catch { }
                        try { dtoAffliations.country = aff.Element("country")?.Value.Trim(); } catch { }
                        try { dtoAffliations.postalCode = aff.Element("post_code")?.Value.Trim(); } catch { }
                    }
                }
                #endregion

                #region Abstract
                var abst = art.Descendants("abstract").FirstOrDefault().Value;
                #endregion

                #region Conflict Of Interest
                var conflictOfInt = art.Descendants("configurable_data_fields").ToList();
                if (conflictOfInt != null)
                {
                    var coi = conflictOfInt.Descendants("custom_fields").Where(a => a.Attribute("cd_code")?.Value == "Conflict of Interest").Select(a => a.Attribute("cd_value")?.Value).FirstOrDefault();
                }
                #endregion

                #region Funding Info.
                var fundingInfo = art.Descendants("fundref_information");
                if (fundingInfo != null)
                {
                    var funderPresent = art.Descendants("no_funders").FirstOrDefault().Value;
                    if (funderPresent.ToLower() == "false")
                    {
                        var funder = art.Descendants("preferred_label").FirstOrDefault().Value;
                        var grantId = art.Descendants("grant_number").FirstOrDefault().Value;
                    }
                }
                #endregion

                #region Data Availability
                var dataAvailability = art.Descendants("configurable_data_fields").ToList();
                if (dataAvailability.Count > 0)
                {
                    //var dataAvailStatement = dataAvailability.Descendants("custom_fields").Where(a => a.Attribute("cd_code").Value.Contains("Data Availability Statement")).FirstOrDefault()?.Value;

                    var dataAvailStatement = dataAvailability.Descendants("custom_fields")
                                         .Where(a => a.Attribute("cd_code")?.Value.Contains("Data Availability Statement") == true &&
                                                     a.Attribute("cd_name")?.Value.Contains("Please copy and paste your Data Availability Statement here") == true)
                                         .FirstOrDefault()?.Attribute("cd_value")?.Value;
                }
                #endregion

                #region Submitted Previously
                var subPreviously = art.Descendants("configurable_data_fields").ToList();
                if (subPreviously.Count > 0)
                {
                    var subPrev = subPreviously.Descendants("custom_fields").Where(a => a.Attribute("cd_code")?.Value.Contains("Has manuscript been submitted previously") == true).FirstOrDefault()?.Attribute("cd_value")?.Value;
                }
                #endregion

                #region Ethics 
                var ethicalCompliance = art.Descendants("configurable_data_fields").ToList();
                if (ethicalCompliance.Count > 0)
                {
                    var compliance = ethicalCompliance.Descendants("custom_fields").Where(a => a.Attribute("cd_code")?.Value.Contains("Human Participants") == true)
                        .Where(a => a.Attribute("cd_name")?.Value.ToLower().Contains("if your study involves human participants, have you received the ethics approval of a specific ethics committee, and have you obtained the informed consent to participate in the study from the patients, or in the case of children, their parent or legal guardian?") == true)
                        .Select(a => a.Attribute("cd_value")?.Value).FirstOrDefault();



                    var consent = ethicalCompliance.Descendants("custom_fields").Where(a => a.Attribute("cd_code")?.Value.Contains("Human Participants") == true)
                        .Where(a => a.Attribute("cd_name")?.Value.ToLower().Contains("if your study involves human participants, have you received the ethics approval of a specific ethics committee, and have you obtained the informed consent to participate in the study from the patients, or in the case of children, their parent or legal guardian?") == true)
                        .Select(a => a.Attribute("cd_value_code")?.Value).FirstOrDefault();

                }

                #endregion

                #region Word Count
                var wordCounts = art.Descendants("configurable_data_fields").ToList();
                if (wordCounts.Count > 0)
                {
                    var wordCount = wordCounts.Descendants("custom_fields").Where(a => a.Attribute("cd_code")?.Value.Contains("word count") == true).FirstOrDefault()?.Attribute("cd_value")?.Value;
                }
                #endregion

            }
        }
    }
}
