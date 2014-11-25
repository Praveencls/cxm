using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Website.Models
{
    public class demo
    {
    }

    public class Record
    {
	    [JsonProperty]
	    public string StatusCode {get;set;}
	
	    [JsonProperty]
	    public Result Result {get;set;}
    }

    public class Result
    {
	    [JsonProperty]
	    public int TotalCount {get;set;}
	
	    [JsonProperty]
	    public int ResultCount {get;set;}
	
	    [JsonProperty]
	    public List<JsonItem> Items {get;set;}
    }

    public class JsonItem
    {
	    [JsonProperty]
	    public string Id {get;set;}

        [JsonProperty]
        public string Template { get; set; }
	
	    [JsonProperty]
	    public string SitecoreTemplate {get;set;}
	
	    [JsonProperty]
	    public string DisplayName {get;set;}
	
	    [JsonProperty]
	    public string Path {get;set;}
    }
}