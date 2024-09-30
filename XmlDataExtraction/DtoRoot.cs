using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlDataExtraction
{
    public class DtoRoot
    {
        //public class DtoArticleTitle
        //{
        //    public string articleTile { get; set; }
        //}
        public class DtoAuthorMetadata
        {
            public string salutation { get; set; }
            public string firstname { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string degree { get; set; }
            public string corresponding_author { get; set; } = "no";
            public string email { get; set; }
            public string orcid { get; set; }
        }
        public class DtoAffliations
        {
            public string institute { get; set; }
            public string department { get; set; }
            public string addr1 { get; set; }
            public string addr2 { get; set; }
            public string addr3 { get; set; }
            public string city { get; set; }
            public string province { get; set; }
            public string country { get; set; }
            public string postalCode { get; set; }
        }
        
        //public class DtoXmlAuthorContributions
        //{
        //    public List<DtoAuthors> AuthorDto { get; set; }
        //    public List<DtoAuthor> DtoAuthor { get; set; }
        //    public List<DtoAffliations> AffliationsDto { get; set; }
        //    public List<MetaDtoAffliation> MetaAffliationDto { get; set; }
        //}

        //public class DtoAuthors
        //{
        //    public List<string> AfiliationId { get; set; }
        //    public List<string> AffiliationLabel { get; set; }
        //    public DtoManuscript DtoManuscript { get; set; }
        //    public DtoMetadata dtoMetadata { get; set; }
        //    public string isMatch { get; set; }
        //}

        //public class DtoAuthor
        //{
        //    public DtoManuscriptWord dtoManuscriptWord { get; set; }
        //    public DtoManuscript DtoManuscript { get; set; }
        //    public int Id { get; set; }
        //    public string FirstName { get; set; } = string.Empty;
        //    public string LastName { get; internal set; } = string.Empty;
        //    //public string MiddleName { get; internal set; } = string.Empty;
        //    public string EmailId { get; set; } = string.Empty;
        //    public string[] AffiliationId { get; set; } //aff
        //    public string[] AffLabelId { get; set; }
        //    public string[] Affiliation { get; set; }
        //    public List<string> affiliation_id { get; set; } = new List<string>();
        //    public List<string> affiliation_label { get; set; } = new List<string>();
        //    public string IsCorrespending { get; set; } = "no";
        //    public string Orcid { get; set; } = string.Empty;
        //}


        //public class MetaDtoAffliation
        //{
        //    public string LabelId { get; set; }
        //    public string Id { get; set; }
        //    public string isMatch { get; set; }
        //    //public DtoMetadata dtoMetadataAff { get; set; }
        //    public DtoManuscript dtomanuscriptAff { get; set; }
        //}

        //public class DtoManuscript
        //{
        //    public string fundingInstitute { get; set; }
        //    public string grantID { get; set; }
        //    public string recipient { get; set; }
        //    public string firstname { get; set; }
        //    public string lastName { get; set; }
        //    public string salutation { get; set; }
        //    public string corresponding_author { get; set; }
        //    public string email { get; set; }
        //    public string orcid { get; set; }
        //    public string department { get; set; }
        //    public string institute { get; set; } = string.Empty;
        //    public string addressLine { get; set; } = string.Empty;
        //    public string city { get; set; } = string.Empty;
        //    public string state { get; set; } = string.Empty;
        //    public string country { get; set; } = string.Empty;
        //    public string countryCode { get; set; } = string.Empty;
        //    public string postalCode { get; set; } = string.Empty;
        //    public string Id { get; set; } = string.Empty;
        //    public string Label { get; set; } = string.Empty;
        //}


        //public class DtoManuscriptWord
        //{
        //    public string fundingInstitute { get; set; }
        //    public string grantID { get; set; }
        //    public string recipient { get; set; }
        //    public List<string> affiliation_id { get; set; } = new List<string>();
        //    public List<string> affiliation_label { get; set; } = new List<string>();
        //    public string firstname { get; set; }
        //    public string lastName { get; set; }
        //    public string salutation { get; set; }
        //    public string corresponding_author { get; set; } = "no";
        //    public string email { get; set; }
        //    public string orcid { get; set; }
        //    public string role { get; set; } = string.Empty;
        //    public string degree { get; set; } = string.Empty;
        //    public string suffix { get; set; } = string.Empty;
        //    public string department { get; set; }
        //    public string institute { get; set; } = string.Empty;
        //    public string addressLine { get; set; } = string.Empty;
        //    public string city { get; set; } = string.Empty;
        //    public string state { get; set; } = string.Empty;
        //    public string country { get; set; } = string.Empty;
        //    public string countryCode { get; set; } = string.Empty;
        //    public string postalCode { get; set; }
        //    public string street { get; set; }
        //}
    }
}
