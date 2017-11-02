using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web;

namespace Linkedin
{
    #region MethodClass
    public class Profile
    {
        public static async Task<LinkedInProfile> GetInfoAsync(string access_token)
        {
            HttpClient client = new HttpClient();
            var responseString2 = await client.GetStringAsync($"https://api.linkedin.com/v1/people/~:(id,email-address,public-profile-url,first-name,maiden-name,location:(name),last-name,headline,picture-url,picture-urls::(original),industry,summary,specialties,positions:(id,title,summary,start-date,end-date,is-current,company:(id,name,type,size,industry,ticker)),educations:(id,school-name,field-of-study,start-date,end-date,degree,activities,notes),associations,interests,num-recommenders,date-of-birth,publications:(id,title,publisher:(name),authors:(id,name),date,url,summary),patents:(id,title,summary,number,status:(id,name),office:(name),inventors:(id,name),date,url),languages:(id,language:(name),proficiency:(level,name)),skills:(id,skill:(name)),certifications:(id,name,authority:(name),number,start-date,end-date),courses:(id,name,number),recommendations-received:(id,recommendation-type,recommendation-text,recommender),honors-awards,three-current-positions,three-past-positions,volunteer)?format=json&oauth2_access_token={access_token}");
            var FullProfileDet = JsonConvert.DeserializeObject<LinkedInProfile>(responseString2);
            return FullProfileDet;
        }
    }
    public class Authenticating
    {
        public static string GetLinkedinAuthorizationURL(string ClientID, string redirect_url, bool r_basicprofile = true, bool r_emailaddress = false, bool rw_company_admin = false, bool w_share = false, string State = null)
        {
            string finalScope = "";
            if (r_basicprofile)
            {
                finalScope += "r_basicprofile,";
            }
            if (r_emailaddress)
            {
                finalScope += "r_emailaddress,";
            }
            if (rw_company_admin)
            {
                finalScope += "rw_company_admin,";
            }
            if (w_share)
            {
                finalScope += "w_share,";
            }
            State = State == null ? Guid.NewGuid().ToString() : State;
            return $"https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={ClientID}&redirect_uri={redirect_url}&state={State}&scope={finalScope}"; ;
        }
        public static async Task<LinkedInAccessTokenResponse> GetAccessTokenAsync(string ClientId, string ClientSecret, string Code, string redirect_uri)
        {
            HttpClient client = new HttpClient();

            var values = new Dictionary<string, string>
            {
                 {"grant_type", "authorization_code" },
                 {"code", Code },
                 {"redirect_uri",redirect_uri },
                 {"client_id",ClientId},
                 { "client_secret",ClientSecret}
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://www.linkedin.com/oauth/v2/accessToken", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var ResponseInClass = JsonConvert.DeserializeObject<LinkedInAccessTokenResponse>(responseString);
            ResponseInClass.ActiveUntil = DateTime.Now.AddSeconds(int.Parse(ResponseInClass.expires_in));
            return ResponseInClass;
        }
        public static async Task<LinkedInCompanies> GetUserCompaniesAsync(string Token)
        {
            HttpClient client = new HttpClient();
            var responseString = await client.GetStringAsync($"https://api.linkedin.com/v1/companies?format=json&is-company-admin=true&oauth2_access_token={Token}");
            var ResponseInClass = JsonConvert.DeserializeObject<LinkedInCompanies>(responseString);
            foreach (var item in ResponseInClass.values)
            {
                var responseString2 = await client.GetStringAsync($"https://api.linkedin.com/v1/companies/{item.id}/is-company-share-enabled?format=json&oauth2_access_token={Token}");
                item.isCompanyShareEnabled = responseString2 == "true";
            }
            return ResponseInClass;
        }
        public static async Task<CompanyInfo> GetCompanyInfoAsync(string pageId, string Token)
        {
            HttpClient client = new HttpClient();
            var responseString = await client.GetStringAsync($"https://api.linkedin.com/v1/companies/{pageId}:(id,name,ticker,founded-year,end-year,num-followers,description,logo-url,square-logo-url,company-type,website-url,industries,status,blog-rss-url,twitter-id,employee-count-range,specialties,locations:(description,is-headquarters,is-active,address:(street1,street2,city,state,postal-code,country-code,region-code),contact-info:(phone1,phone2,fax)))?format=json&oauth2_access_token={Token}");
            var ResponseInClass = JsonConvert.DeserializeObject<CompanyInfo>(responseString);
            return ResponseInClass;
        }
        public static async Task<bool> TokenValidationAsync(string token)
        {
            HttpClient client = new HttpClient();
            try
            {
                var responseString2 = await client.GetStringAsync($"https://api.linkedin.com/v1/people/~:(id,public-profile-url,first-name)?format=json&oauth2_access_token={token}");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
    public class Post
    {
        public static async Task<ResponsePost> UpdateStatusAsync(string accessToken, LinkedInContentPost linkedInPost, bool GetJsonResult = false)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-li-format", "json");
            var content = new StringContent(new JavaScriptSerializer().Serialize(linkedInPost), Encoding.UTF8, "application/json");
            var result = client.PostAsync($"https://api.linkedin.com/v1/people/~/shares?format=json&oauth2_access_token={accessToken}", content).Result;
            var responseString = await result.Content.ReadAsStringAsync();
            if (GetJsonResult)
                return new ResponsePost() { JsonResult = responseString };
            var ResponseInClass = JsonConvert.DeserializeObject<ResponsePost>(responseString);
            return ResponseInClass;
        }
        public static async Task<ResponsePost> UpdateStatusAsync(string accessToken, LinkedInTextPost linkedInPost, bool GetJsonResult = false)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-li-format", "json");
            var content = new StringContent(new JavaScriptSerializer().Serialize(linkedInPost), Encoding.UTF8, "application/json");
            var result = client.PostAsync($"https://api.linkedin.com/v1/people/~/shares?format=json&oauth2_access_token={accessToken}", content).Result;
            var responseString = await result.Content.ReadAsStringAsync();
            if (GetJsonResult)
                return new ResponsePost() { JsonResult = responseString };
            var ResponseInClass = JsonConvert.DeserializeObject<ResponsePost>(responseString);
            return ResponseInClass;
        }
        public static async Task<ResponsePost> UpdateCompanyPageStatusAsync(string accessToken, LinkedInTextPost linkedInPost, string PageId, bool GetJsonResult = false)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-li-format", "json");
            var content = new StringContent(new JavaScriptSerializer().Serialize(linkedInPost), Encoding.UTF8, "application/json");
            var result = client.PostAsync($"https://api.linkedin.com/v1/companies/{PageId}/shares?format=json&oauth2_access_token={accessToken}", content).Result;
            var responseString = await result.Content.ReadAsStringAsync();
            if (GetJsonResult)
                return new ResponsePost() { JsonResult = responseString };
            var ResponseInClass = JsonConvert.DeserializeObject<ResponsePost>(responseString);
            return ResponseInClass;
        }
        public static async Task<ResponsePost> UpdateCompanyPageStatusAsync(string accessToken, LinkedInContentPost linkedInPost, string PageId, bool GetJsonResult = false)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-li-format", "json");
            var content = new StringContent(new JavaScriptSerializer().Serialize(linkedInPost), Encoding.UTF8, "application/json");
            var result = client.PostAsync($"https://api.linkedin.com/v1/companies/{PageId}/shares?format=json&oauth2_access_token={accessToken}", content).Result;
            var responseString = await result.Content.ReadAsStringAsync();
            if (GetJsonResult)
                return new ResponsePost() { JsonResult = responseString };
            var ResponseInClass = JsonConvert.DeserializeObject<ResponsePost>(responseString);
            return ResponseInClass;
        }
        public static async Task<bool> CommentAsCompanyAsync(string Token, string PageId, string UpdateKey, string Comment, bool GetJsonResult = false)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-li-format", "json");
            try
            {
                var content = new StringContent(new JavaScriptSerializer().Serialize(new CommentPost { comment = Comment }), Encoding.UTF8, "application/json");
                var result = client.PostAsync($"https://api.linkedin.com/v1/companies/{PageId}/updates/key={UpdateKey}/update-comments-as-company/?format=json&oauth2_access_token={Token}", content).Result;
                var responseString = await result.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
    public class CompanyUpdate
    {
        public static async Task<PageUpdates> GetCompanyUpdatesAsync(string Token, string PageId, int start, int count, string eventType = null)
        {
            HttpClient client = new HttpClient();
            string responseString = "";
            if (string.IsNullOrEmpty(eventType))
            {
                responseString = await client.GetStringAsync($"https://api.linkedin.com/v1/companies/{PageId}/updates?count={count}&start={start}&format=json&oauth2_access_token={Token}");
            }
            else
            {
                responseString = await client.GetStringAsync($"https://api.linkedin.com/v1/companies/{PageId}/updates?count={count}&start={start}&eventType={eventType}&format=json&oauth2_access_token={Token}");
            }
            var ResponseInClass = JsonConvert.DeserializeObject<PageUpdates>(responseString);
            return ResponseInClass;
        }
        public static async Task<SpecificUpdate> GetSpecificCompanyUpdateAsync(string token, string PageId, string UpdateKey)
        {
            HttpClient client = new HttpClient();
            var responseString = await client.GetStringAsync($"https://api.linkedin.com/v1/companies/{PageId}/updates/key={UpdateKey}?format=json&oauth2_access_token={token}");
            var ResponseInClass = JsonConvert.DeserializeObject<SpecificUpdate>(responseString);
            return ResponseInClass;

        }
        public static async Task<string> GetComapnysFollowersNumberAsync(string Token, string PageId, string geos = null, string companySizes = null, string jobFunc = null, string industries = null, string seniorities = null)
        {
            HttpClient client = new HttpClient();
            var APIAddress = $"https://api.linkedin.com/v1/companies/{PageId}/num-followers?format=json&";
            var tokenfilne = $"oauth2_access_token={Token}";
            if (!string.IsNullOrEmpty(geos))
            {
                APIAddress += $"geos={geos}&";
            }
            if (!string.IsNullOrEmpty(companySizes))
            {
                APIAddress += $"companySizes={companySizes}&";
            }
            if (!string.IsNullOrEmpty(jobFunc))
            {
                APIAddress += $"jobFunc={jobFunc}&";
            }
            if (!string.IsNullOrEmpty(industries))
            {
                APIAddress += $"industries={industries}&";
            }
            if (!string.IsNullOrEmpty(seniorities))
            {
                APIAddress += $"seniorities={seniorities}&";
            }
            APIAddress += tokenfilne;
            var responseString = await client.GetStringAsync(APIAddress);
            return responseString.ToString();
        }
    }
    #endregion

    #region LinkedInDataClass














    /// <summary>
    /// ///////////////////////////////////////////////////////////////
    /// </summary>
    public class Person
    {
        public string firstName { get; set; }
        public string headline { get; set; }
        public string id { get; set; }
        public string lastName { get; set; }
        public string pictureUrl { get; set; }
    }

    public class LikesValue
    {
        public Company company { get; set; }
        public object timestamp { get; set; }
        public Person person { get; set; }
    }

    public class Likes
    {
        public int _total { get; set; }
        public IList<LikesValue> values { get; set; }
    }

    public class CommentsValue
    {
        public string comment { get; set; }
        public object id { get; set; }
        public Person person { get; set; }
        public int sequenceNumber { get; set; }
        public object timestamp { get; set; }
        public Company company { get; set; }
    }

    public class SPFUpdateComments
    {
        public int _total { get; set; }
        public IList<CommentsValue> values { get; set; }
    }

    public class SPFContent
    {
        public string description { get; set; }
        public string eyebrowUrl { get; set; }
        public string shortenedUrl { get; set; }
        public string submittedImageUrl { get; set; }
        public string submittedUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string title { get; set; }
    }
    public class Content
    {
        public string description { get; set; }
        public string submittedImageUrl { get; set; }
        public string submittedUrl { get; set; }
        public string title { get; set; }
    }
    public class Application
    {
        public string name { get; set; }
    }

    public class ServiceProvider
    {
        public string name { get; set; }
    }

    public class Source
    {
        public Application application { get; set; }
        public ServiceProvider serviceProvider { get; set; }
        public string serviceProviderShareId { get; set; }
    }

    public class Visibility
    {
        public string code { get; set; }
    }

    public class SPFShare
    {
        public string comment { get; set; }
        public SPFContent content { get; set; }
        public string id { get; set; }
        public Source source { get; set; }
        public long timestamp { get; set; }
        public Visibility visibility { get; set; }
    }

    public class SPFCompanyStatusUpdate
    {
        public SPFShare share { get; set; }
    }

    public class SpecificUpdate
    {
        public bool isCommentable { get; set; }
        public bool isLikable { get; set; }
        public bool isLiked { get; set; }
        public Likes likes { get; set; }
        public int numLikes { get; set; }
        public long timestamp { get; set; }
        public SPFUpdateComments updateComments { get; set; }
        public UpdateContent updateContent { get; set; }
        public string updateKey { get; set; }
        public string updateType { get; set; }
    }











    public class CommentPost
    {
        public string comment { get; set; }
    }








    public class UpdateComments
    {
        public int _total { get; set; }
    }

    public class CompanyIdName
    {
        public int id { get; set; }
        public string name { get; set; }
    }


    public class ContentPost
    {
        public string description { get; set; }
        public string eyebrowUrl { get; set; }
        public string shortenedUrl { get; set; }
        public string submittedImageUrl { get; set; }
        public string submittedUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string title { get; set; }
    }

    public class Share
    {
        public string comment { get; set; }
        public string id { get; set; }
        public Source source { get; set; }
        public object timestamp { get; set; }
        public Visibility visibility { get; set; }
        public ContentPost content { get; set; }
    }

    public class CompanyStatusUpdate
    {
        public Share share { get; set; }
    }

    public class UpdateContent
    {
        public CompanyIdName company { get; set; }
        public CompanyStatusUpdate companyStatusUpdate { get; set; }
    }

    public class ValuePagePost
    {
        public bool isCommentable { get; set; }
        public bool isLikable { get; set; }
        public bool isLiked { get; set; }
        public int numLikes { get; set; }
        public object timestamp { get; set; }
        public UpdateComments updateComments { get; set; }
        public UpdateContent updateContent { get; set; }
        public string updateKey { get; set; }
        public string updateType { get; set; }
    }

    public class PageUpdates
    {
        public int _total { get; set; }
        public IList<ValuePagePost> values { get; set; }
    }


















    public class CompanyType
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class EmployeeCountRange
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class NameCode
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Industries
    {
        public int _total { get; set; }
        public IList<NameCode> values { get; set; }
    }

    public class Address
    {
        public string city { get; set; }
        public string countryCode { get; set; }
        public string postalCode { get; set; }
        public int regionCode { get; set; }
        public string state { get; set; }
        public string street1 { get; set; }
    }

    public class ContactInfo
    {
    }

    public class AddressContact
    {
        public Address address { get; set; }
        public ContactInfo contactInfo { get; set; }
        public bool isActive { get; set; }
        public bool isHeadquarters { get; set; }
    }

    public class Locations
    {
        public int _total { get; set; }
        public IList<AddressContact> values { get; set; }
    }

    public class CompanyInfo
    {
        public CompanyType companyType { get; set; }
        public string description { get; set; }
        public EmployeeCountRange employeeCountRange { get; set; }
        public int id { get; set; }
        public Industries industries { get; set; }
        public Locations locations { get; set; }
        public string logoUrl { get; set; }
        public string name { get; set; }
        public int numFollowers { get; set; }
        public string squareLogoUrl { get; set; }
        public string websiteUrl { get; set; }
    }
    public class CompanyPage
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool isCompanyShareEnabled { get; set; }
    }

    public class LinkedInCompanies
    {
        public int _total { get; set; }
        public IList<CompanyPage> values { get; set; }
    }
    public class LinkedInContentPost
    {
        public string comment { get; set; }
        public Visibility visibility { get; set; }
        public Content content { get; set; }
    }
    public class LinkedInTextPost
    {
        public string comment { get; set; }
        public Visibility visibility { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
    }

    public class PictureUrls
    {
        public int _total { get; set; }
        public IList<string> values { get; set; }
    }

    public class Company
    {
        public int id { get; set; }
        public string industry { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string type { get; set; }
    }

    public class StartDate
    {
        public int month { get; set; }
        public int year { get; set; }
    }

    public class Value
    {
        public Company company { get; set; }
        public int id { get; set; }
        public bool isCurrent { get; set; }
        public StartDate startDate { get; set; }
        public StartDate endDate { get; set; }

        public string summary { get; set; }
        public string title { get; set; }
    }

    public class Positions
    {
        public int _total { get; set; }
        public IList<Value> values { get; set; }
    }

    public class LinkedInProfile
    {
        public string firstName { get; set; }
        public string headline { get; set; }
        public string id { get; set; }
        public string industry { get; set; }
        public string lastName { get; set; }
        public Location location { get; set; }
        public string pictureUrl { get; set; }
        public PictureUrls pictureUrls { get; set; }
        public Positions positions { get; set; }
        public string summary { get; set; }
        public string emailAddress { get; set; }
        public string publicProfileUrl { get; set; }
        public string specialties { get; set; }
        public string volunteer { get; set; }
    }
    public class LinkedInAccessTokenResponse
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public DateTime ActiveUntil { get; set; }
    }
    public class ResponsePost
    {
        public string updateKey { get; set; }
        public string updateUrl { get; set; }
        public string JsonResult { get; set; }
    }

    #endregion
}
