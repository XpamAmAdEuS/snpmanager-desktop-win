using System.Collections.Generic;

namespace Snp.Models;

public class SearchRequestModel
{
    public string SearchText { get; set; } = "";
    public string SortColumn { get; set; } = "id";
    public string SortDirection { get; set; } = "ASC";
    public uint PerPage { get; set; } = 10;
    public uint CurrentPage { get; set; } = 0;
    public List<string> Fields { get; set; } = new (){"id"};
}